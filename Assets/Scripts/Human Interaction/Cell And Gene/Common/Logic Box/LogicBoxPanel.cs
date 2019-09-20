using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicBoxPanel : CellAndGeneSignalUnitPanel {
	public Image outputImage;

	private static Vector2 rowSize = new Vector2(270f, 40f);
	public static float cellWidth = rowSize.x * (1f / 5f);
	public static float cellHeight = 40;

	[HideInInspector]
	public string outputText; 

	public Text outputLabel;
	public LogicBoxGatePanel gateTemplate;

	private LogicBoxGatePanel gateRow0;
	private LogicBoxGatePanel[] gatesRow1 = new LogicBoxGatePanel[GeneLogicBox.maxGatesPerRow];
	private LogicBoxGatePanel[] gatesRow2 = new LogicBoxGatePanel[GeneLogicBox.maxGatesPerRow];
	public LogicBoxInputPanel[] inputRow3 = new LogicBoxInputPanel[GeneLogicBox.maxGatesPerRow];

	public GeneLogicBox affectedGeneLogicBox {
		get {
			return selectedGene.eggCellFertilizeLogic;
		}
	}

	public Vector3 gateGridOrigo {
		get {
			return gateTemplate.transform.position;
		}
	}

	public void UpdateConnections() {
		affectedGeneLogicBox.UpdateConnections();
	}

	override public void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit ) {
		base.Initialize(mode, signalUnit);

		gateRow0 = GameObject.Instantiate(gateTemplate, transform);
		gateRow0.transform.position = gateTemplate.transform.position + Vector3.right * 0f * cellWidth + Vector3.down * 0f * cellHeight;
		gateRow0.transform.SetAsFirstSibling();
		gateRow0.Initialize(mode, 0, 0, this);

		// create small gate pool
		for (int row = 1; row < GeneLogicBox.rowCount; row++) {
			for (int index = 0; index < GeneLogicBox.maxGatesPerRow; index++) {
				LogicBoxGatePanel gate = GameObject.Instantiate(gateTemplate, transform);
				gate.GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth, cellHeight);
				gate.transform.position = gateTemplate.transform.position + Vector3.right * index * cellWidth + Vector3.down * row * cellHeight;
				gate.transform.SetAsFirstSibling();
				gate.Initialize(mode, row, index, this);
				gate.gameObject.SetActive(true);

				if (row == 1) {
					gatesRow1[index] = gate;
				} else {
					gatesRow2[index] = gate;
				}
			}
		}
		gateTemplate.gameObject.SetActive(false);

		// Initialize input boxes
		for (int column = 0; column < GeneLogicBox.columnCount; column++) {
			inputRow3[column].Initialize(mode, column, this);
		}

		MakeDirty();

		
	}

	public void MarkAsNewForge() {
		CreatureSelectionPanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
	}

	public void OnClickedAddGateRow1() {
		if (mode == PhenoGenoEnum.Genotype && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome && affectedGeneLogicBox.TryCreateGate(1, LogicOperatorEnum.And)) {
			UpdateConnections();
			MarkAsNewForge();
			MakeDirty();
		}
	}

	public void OnClickedAddGateRow2() {
		if (mode == PhenoGenoEnum.Genotype && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome && affectedGeneLogicBox.TryCreateGate(2, LogicOperatorEnum.And)) {
			UpdateConnections();
			MarkAsNewForge();
			MakeDirty();
		}
	}

	public List<GeneLogicBoxInput> GetAllGeneGeneLogicBoxInputs() {
		List<GeneLogicBoxInput> arrows = new List<GeneLogicBoxInput>();
		for (int i = 0; i < inputRow3.Length; i++) {
			arrows.Add(inputRow3[i].affectedGeneLogicBoxInput);
		}
		return arrows;
	}

	private LogicBoxInputEnum RuntimeLogicBoxInputAfterValve(int inputColumn) {
		if (inputRow3[inputColumn].affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Block) {
			return LogicBoxInputEnum.BlockedByValve;
		} else if (inputRow3[inputColumn].affectedGeneLogicBoxInput.nerve.inputUnit == SignalUnitEnum.Void) {
			return LogicBoxInputEnum.VoidInput;
		} else {
			if (selectedCell != null) {
				return selectedCell.GetOutputFromUnit(inputRow3[inputColumn].affectedGeneLogicBoxInput.nerve.inputUnit, SignalUnitSlotEnum.Whatever) ? LogicBoxInputEnum.On : LogicBoxInputEnum.Off;
			}
		}
		return LogicBoxInputEnum.Error;
	}

	public void OnClickedOutputButton() {
		if (MouseAction.instance.actionState == MouseActionStateEnum.selectSignalOutput && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			LogicBoxInputPanel.AnswerSetReference(affectedGeneLogicBox.signalUnit, SignalUnitSlotEnum.Whatever); // Whatever... there is just one output slot
			MouseAction.instance.actionState = MouseActionStateEnum.free;
		}
	}

	public Gene selectedGene {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell != null ? CellPanel.instance.selectedCell.gene : null;
			} else {
				return GenePanel.instance.selectedGene;
			}
		}
	}

	public Cell selectedCell {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell;
			} else {
				return null; // there could be many cells selected for the same gene
			}
		}
	}

	private void Update() {
		if (dirt) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Signal logic box");
			}

			gateRow0.MakeDirty();
			for (int i = 0; i < gatesRow1.Length; i++) {
				gatesRow1[i].MakeDirty();
			}
			for (int i = 0; i < gatesRow2.Length; i++) {
				gatesRow2[i].MakeDirty();
			}
			for (int i = 0; i < inputRow3.Length; i++) {
				inputRow3[i].MakeDirty();
			}

			if (mode == PhenoGenoEnum.Phenotype && CellPanel.instance.selectedCell != null) {
				outputImage.color = selectedCell.GetOutputFromUnit(affectedGeneLogicBox.signalUnit, SignalUnitSlotEnum.Whatever) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
			}

			outputLabel.text = outputText;
			dirt = false;
		}
	}
}
