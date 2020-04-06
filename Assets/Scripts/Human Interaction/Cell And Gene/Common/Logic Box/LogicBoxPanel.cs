using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicBoxPanel : SignalUnitPanel {
	public Image outputImageLate;
	public Image outputImageEarly;
	public Transform bodyPanel;

	private static Vector2 rowSize = new Vector2(625f, 40f);
	public static float cellWidth = rowSize.x * (1f / GeneLogicBox.columnCount);
	public static float cellHeight = 40;

	[HideInInspector]
	public string outputText;

	public Text outputLabel;
	public LogicBoxGatePanel gateTemplate;

	public RectTransform lockedCellTemplate;

	private LogicBoxGatePanel gateRow0;
	private LogicBoxGatePanel[] gatesRow1 = new LogicBoxGatePanel[GeneLogicBox.maxGatesPerRow];
	private LogicBoxGatePanel[] gatesRow2 = new LogicBoxGatePanel[GeneLogicBox.maxGatesPerRow];
	public LogicBoxInputPanel[] inputRow3 = new LogicBoxInputPanel[GeneLogicBox.maxGatesPerRow];

	public RectTransform[,] lockedCells = new RectTransform[GeneLogicBox.rowCount - 1, GeneLogicBox.columnCount];

	private void Awake() {
		gateTemplate.gameObject.SetActive(false);
	}

	public GeneLogicBox affectedGeneLogicBox {
		get {
			if (selectedGene == null) {
				return null;
			}
			if (selectedGene.type == CellTypeEnum.Egg && signalUnit == SignalUnitEnum.WorkLogicBoxA) {
				return selectedGene.eggCellFertilizeLogic;
			} else if (signalUnit == SignalUnitEnum.DendritesLogicBox) {
				return selectedGene.dendritesLogicBox;
			} else if (signalUnit == SignalUnitEnum.OriginDetatchLogicBox) {
				return selectedGene.originDetatchLogicBox;
			}


			return null;
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

	override public void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit) {
		base.Initialize(mode, signalUnit);
		gateTemplate.gameObject.SetActive(true);
		gateRow0 = GameObject.Instantiate(gateTemplate, bodyPanel.transform);
		gateRow0.transform.position = gateTemplate.transform.position + Vector3.right * 0f * cellWidth + Vector3.down * 0f * cellHeight;
		gateRow0.transform.SetAsFirstSibling();
		gateRow0.Initialize(mode, 0, 0, this);

		// create small gate pool
		for (int row = 1; row < GeneLogicBox.rowCount; row++) {
			for (int index = 0; index < GeneLogicBox.maxGatesPerRow; index++) {
				LogicBoxGatePanel gate = GameObject.Instantiate(gateTemplate, bodyPanel.transform);
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

		// Create locked overlays
		for (int row = 1; row < GeneLogicBox.rowCount; row++) {
			for (int column = 0; column < GeneLogicBox.columnCount; column++) {
				RectTransform lockedCell = GameObject.Instantiate(lockedCellTemplate, bodyPanel.transform);
				lockedCell.GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth, cellHeight);
				lockedCell.transform.position = gateTemplate.transform.position + Vector3.right * column * cellWidth + Vector3.down * row * cellHeight;
				lockedCell.transform.SetAsLastSibling();
				lockedCell.gameObject.SetActive(false);

				lockedCells[row - 1, column] = lockedCell;
			}
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

	public override List<IGeneInput> GetAllGeneInputs() {
		List<IGeneInput> arrows = new List<IGeneInput>();
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
				return selectedCell.GetOutputFromUnit(inputRow3[inputColumn].affectedGeneLogicBoxInput.nerve.inputUnit, inputRow3[inputColumn].affectedGeneLogicBoxInput.nerve.inputUnitSlot) ? LogicBoxInputEnum.On : LogicBoxInputEnum.Off;
			}
		}
		return LogicBoxInputEnum.Error;
	}

	public void OnClickedOutputButton() { // The arrow, processed late
		if (MouseAction.instance.actionState == MouseActionStateEnum.selectSignalOutput && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			if (affectedGeneLogicBox != null) {
				LogicBoxInputPanel.TryAnswerSetReference(affectedGeneLogicBox.signalUnit, SignalUnitSlotEnum.outputLateA);
				AxonInputPanel.TryAnswerSetReference(affectedGeneLogicBox.signalUnit, SignalUnitSlotEnum.outputLateA);
				MouseAction.instance.actionState = MouseActionStateEnum.free;

				
			} else {
				Debug.Log("Can't connect a nerve to a ghost output");
				// TODO: play error audio
			}
		}
	}

	private void Update() {
		if (isDirty) {
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
				if (affectedGeneLogicBox != null) {
					outputImageLate.color = selectedCell.GetOutputFromUnit(affectedGeneLogicBox.signalUnit, SignalUnitSlotEnum.outputLateA) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
					outputImageEarly.color = selectedCell.GetOutputFromUnit(affectedGeneLogicBox.signalUnit, SignalUnitSlotEnum.outputEarlyA) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				}
			} else {
				outputImageLate.color = ColorScheme.instance.signalOff;
				outputImageEarly.color = ColorScheme.instance.signalOff;
			}

			// locked cells
			if (affectedGeneLogicBox != null && (mode == PhenoGenoEnum.Phenotype && selectedCell != null || mode == PhenoGenoEnum.Genotype && selectedGene != null)) {
				for (int row = 1; row < GeneLogicBox.rowCount; row++) {
					for (int column = 0; column < GeneLogicBox.columnCount; column++) {
						lockedCells[row - 1, column].gameObject.SetActive(mode == PhenoGenoEnum.Genotype && affectedGeneLogicBox.IsCellOccupiedByLock(row, column));
					}
				}
			}

			outputLabel.text = outputText;
			if (affectedGeneLogicBox != null && !affectedGeneLogicBox.isUsedInternal) {
				outputLabel.text = "UNUSED";
			}
			isDirty = false;
		}
	}
}
