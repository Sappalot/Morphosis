using UnityEngine;
using UnityEngine.UI;

// TODO Generalize to just be an input panel for all units
public class LogicBoxInputPanel : MonoBehaviour {
	public Image motherBlockBackground;
	public Image motherPassBackground;

	public Image inputButtonImage;
	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	private bool isDirty = false;
	private bool ignoreSliderMoved = false;
	private LogicBoxPanel motherPanel;
	public GeneLogicBoxInput affectedGeneLogicBoxInput;

	public void Initialize(PhenoGenoEnum mode, LogicBoxPanel motherPanel) {
		this.mode = mode;
		this.motherPanel = motherPanel;
	}

	private PhenoGenoEnum GetMode() {
		return mode;
	}

	public void MakeDirty() {
		isDirty = true;
	}

	public void OnMotherBlockClicked() {
		if (mode == PhenoGenoEnum.Phenotype || ignoreSliderMoved) {
			return;
		}
		affectedGeneLogicBoxInput.valveMode = SignalValveModeEnum.Block;
		motherPanel.MarkAsNewForge();
		motherPanel.UpdateConnections();
		motherPanel.MakeDirty();
		MakeDirty();
	}

	public void OnMotherPassClicked() {
		if (mode == PhenoGenoEnum.Phenotype || ignoreSliderMoved) {
			return;
		}
		affectedGeneLogicBoxInput.valveMode = SignalValveModeEnum.Pass;
		motherPanel.MarkAsNewForge();
		motherPanel.UpdateConnections();
		motherPanel.MakeDirty();
		MakeDirty();
	}

	public void OnSetReferenceClicked() {
		if (MouseAction.instance.actionState == MouseActionStateEnum.free && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			MouseAction.instance.actionState = MouseActionStateEnum.selectSignalOutput;
			staticAffectedGeneLogicBoxInput = affectedGeneLogicBoxInput;
		}
	}

	public static GeneLogicBoxInput staticAffectedGeneLogicBoxInput;
	public static void AnswerSetReference(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot) {
		staticAffectedGeneLogicBoxInput.nerve.inputUnit = inputUnit;
		staticAffectedGeneLogicBoxInput.nerve.inputUnitSlot = inputUnitSlot;
		staticAffectedGeneLogicBoxInput = null;
	}

	private void Update() {
		if (Input.GetKey(KeyCode.Escape)) {
			if (MouseAction.instance.actionState == MouseActionStateEnum.selectGene) {
				Audio.instance.ActionAbort(1f);

				MouseAction.instance.actionState = MouseActionStateEnum.free;
			}
		}

		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Hibernate Panel");
			}
			ignoreSliderMoved = true;

			if (selectedGene != null) {
				motherBlockBackground.color = affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Block ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
				motherPassBackground.color = affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Pass ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
			}

			if (mode == PhenoGenoEnum.Genotype) {
				if (affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Block) {
					inputButtonImage.color = ColorScheme.instance.grayedOut;
				} else if (affectedGeneLogicBoxInput.nerve.inputUnit == SignalUnitEnum.Void) {
					inputButtonImage.color = Color.black; // signal will allways be OFF (since we are conected to the void)
				} else {
					inputButtonImage.color = ColorScheme.instance.signalOff; // we have a chance of an ON signal
				}
				if (staticAffectedGeneLogicBoxInput != null) {
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

			isDirty = false;
		}
	}

	private LogicBoxInputEnum runtimeOutput {
		get {
			if (affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Block) {
				return LogicBoxInputEnum.BlockedByValve;
			} else if (affectedGeneLogicBoxInput.nerve.inputUnit == SignalUnitEnum.Void) {
				return LogicBoxInputEnum.VoidInput;
			} else {
				if (selectedCell != null) {
					return selectedCell.GetOutputFromUnit(affectedGeneLogicBoxInput.nerve.inputUnit, SignalUnitSlotEnum.Whatever) ? LogicBoxInputEnum.On : LogicBoxInputEnum.Off;
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
				return GeneCellPanel.instance.selectedGene;
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
