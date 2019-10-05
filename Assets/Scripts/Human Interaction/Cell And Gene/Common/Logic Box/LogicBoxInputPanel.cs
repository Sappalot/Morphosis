using UnityEngine;
using UnityEngine.UI;

// TODO Generalize to just be an input panel for all units
public class LogicBoxInputPanel : MonoBehaviour {
	public Image blockButton;
	public Image passButton;

	public Image inputButtonImage;
	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	private int column;
	private bool isDirty = false;
	private bool ignoreSliderMoved = false;
	private bool isUsed = false;

	private LogicBoxPanel motherPanel;
	public GeneLogicBoxInput affectedGeneLogicBoxInput { 
		get {
			if (selectedGene.type == CellTypeEnum.Egg && motherPanel.signalUnit == SignalUnitEnum.WorkLogicBoxA) {
				return selectedGene.eggCellFertilizeLogic.GetInput(column);
			} else if (motherPanel.signalUnit == SignalUnitEnum.Dendrites) {
				return selectedGene.dendrites.GetInput(column);
			}
			return null;
		}
	}

	public GeneNerve GetGeneNerve() {
		return affectedGeneLogicBoxInput.nerve;
	}

	public void Initialize(PhenoGenoEnum mode, int column, LogicBoxPanel motherPanel) {
		this.mode = mode;
		this.motherPanel = motherPanel;
		this.column = column;
		isUsed = true;
	}

	private PhenoGenoEnum GetMode() {
		return mode;
	}

	public void MakeDirty() {
		isDirty = true;
	}

	public void OnBlockClicked() {
		if (mode == PhenoGenoEnum.Phenotype || ignoreSliderMoved) {
			return;
		}
		affectedGeneLogicBoxInput.valveMode = SignalValveModeEnum.Block;
		motherPanel.MarkAsNewForge();
		motherPanel.UpdateConnections();
		motherPanel.MakeDirty();
		CellPanel.instance.cellAndGenePanel.arrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.arrowHandler.MakeDirtyConnections();
		MakeDirty();
	}

	public void OnPassClicked() {
		if (mode == PhenoGenoEnum.Phenotype || ignoreSliderMoved) {
			return;
		}
		affectedGeneLogicBoxInput.valveMode = SignalValveModeEnum.Pass;
		motherPanel.MarkAsNewForge();
		motherPanel.UpdateConnections();
		motherPanel.MakeDirty();
		CellPanel.instance.cellAndGenePanel.arrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.arrowHandler.MakeDirtyConnections();
		MakeDirty();
	}

	public void OnSetReferenceClicked() {
		if (isUsed && MouseAction.instance.actionState == MouseActionStateEnum.free && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype && affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Pass) {
			MouseAction.instance.actionState = MouseActionStateEnum.selectSignalOutput;
			Debug.Assert(staticAffectedGeneLogicBoxInputPanel == null);
			staticAffectedGeneLogicBoxInputPanel = this;
			motherPanel.MakeDirty();
		}
	}

	public static LogicBoxInputPanel staticAffectedGeneLogicBoxInputPanel;
	public static void AnswerSetReference(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot) {
		staticAffectedGeneLogicBoxInputPanel.affectedGeneLogicBoxInput.nerve.inputUnit = inputUnit;
		staticAffectedGeneLogicBoxInputPanel.affectedGeneLogicBoxInput.nerve.inputUnitSlot = inputUnitSlot;
		staticAffectedGeneLogicBoxInputPanel.motherPanel.MakeDirty();
		CellPanel.instance.cellAndGenePanel.arrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.arrowHandler.MakeDirtyConnections();
		staticAffectedGeneLogicBoxInputPanel = null;
	}

	private void Update() {
		if (Input.GetKey(KeyCode.Escape)) {
			if (MouseAction.instance.actionState == MouseActionStateEnum.selectSignalOutput) {
				Audio.instance.ActionAbort(1f);
				MouseAction.instance.actionState = MouseActionStateEnum.free;
				staticAffectedGeneLogicBoxInputPanel = null;
				motherPanel.MakeDirty();
			}
		}

		if (!CreatureSelectionPanel.instance.hasSoloSelected) {
			return;
		}

		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Hibernate Panel");
			}
			ignoreSliderMoved = true;

			if (selectedGene != null && affectedGeneLogicBoxInput != null) {
				blockButton.color = affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Block ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
				passButton.color = affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Pass ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
			}

			if (mode == PhenoGenoEnum.Genotype) {
				if (affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Block) {
					inputButtonImage.color = ColorScheme.instance.grayedOut;
				} else if (affectedGeneLogicBoxInput.nerve.inputUnit == SignalUnitEnum.Void) {
					inputButtonImage.color = Color.black; // signal will allways be OFF (since we are conected to the void)
				} else {
					inputButtonImage.color = ColorScheme.instance.signalOff; // we have a chance of an ON signal
				}
				// Color while choosing output
				if (staticAffectedGeneLogicBoxInputPanel != null && staticAffectedGeneLogicBoxInputPanel.affectedGeneLogicBoxInput == affectedGeneLogicBoxInput) {
					inputButtonImage.color = new Color(0f, 1f, 0f);
				}

			} else {
				if (runtimeOutput == LogicBoxInputEnum.BlockedByValve) {
					inputButtonImage.color = ColorScheme.instance.grayedOut;
				} else if (runtimeOutput == LogicBoxInputEnum.VoidInput) {
					inputButtonImage.color = Color.black;
				} else if (runtimeOutput == LogicBoxInputEnum.On) {
					inputButtonImage.color = ColorScheme.instance.signalOn;
				} else if (runtimeOutput == LogicBoxInputEnum.Off) {
					inputButtonImage.color = ColorScheme.instance.signalOff;
				} else if (runtimeOutput == LogicBoxInputEnum.Error) {
					inputButtonImage.color = Color.red;
				}
			}

			ignoreSliderMoved = false;

			//geneAndGeneCellPanel.MakeDirty();
			isDirty = false;
		}
	}

	private LogicBoxInputEnum runtimeOutput {
		get {
			if (affectedGeneLogicBoxInput != null) {
				if (affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Block) {
					return LogicBoxInputEnum.BlockedByValve;
				} else if (affectedGeneLogicBoxInput.nerve.inputUnit == SignalUnitEnum.Void) {
					return LogicBoxInputEnum.VoidInput;
				} else {
					if (selectedCell != null) {
						return selectedCell.GetOutputFromUnit(affectedGeneLogicBoxInput.nerve.inputUnit, affectedGeneLogicBoxInput.nerve.inputUnitSlot) ? LogicBoxInputEnum.On : LogicBoxInputEnum.Off;
					}
				}
			}
			return LogicBoxInputEnum.Error;
		}
	}

	private Gene selectedGene {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell != null ? CellPanel.instance.selectedCell.gene : null;
			} else {
				return GenePanel.instance.selectedGene;
			}
		}
	}

	private Cell selectedCell {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell;
			} else {
				return null; // there could be many cells selected for the same gene
			}
		}
	}

	public bool IsUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;
	}
}
