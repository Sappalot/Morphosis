using UnityEngine;
using UnityEngine.UI;

// TODO Generalize to just be an input panel for all units
public class AxonInputPanel : MonoBehaviour {
	public Image blockButton;
	public Image passButton;
	public Image lockedOverlayImage;
	public Image semiLockedOverlayImage;

	public Image inputButtonImage;
	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	private int column;
	private bool isDirty = false;
	private bool ignoreSliderMoved = false;
	private bool isUsed = false;

	private CellAndGeneAxonComponentPanel motherPanel;
	public GeneAxonInput affectedGeneAxonInput { 
		get {
			if (column == 0) {
				return selectedGene.axon.axonInputLeft;
			} else if (column == 1) {
				return selectedGene.axon.axonInputRight;
			}

			return null;
		}
	}

	public GeneNerve GetGeneNerve() {
		return affectedGeneAxonInput.nerve;
	}

	public void Initialize(PhenoGenoEnum mode, int column, CellAndGeneAxonComponentPanel motherPanel) {
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
		if (!IsUnlocked() || mode == PhenoGenoEnum.Phenotype || affectedGeneAxonInput.lockness == LocknessEnum.Locked || ignoreSliderMoved) {
			return;
		}
		affectedGeneAxonInput.valveMode = SignalValveModeEnum.Block;
		motherPanel.MarkAsNewForge();
		motherPanel.UpdateConnections();
		motherPanel.MakeDirty();
		CellPanel.instance.cellAndGenePanel.arrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.arrowHandler.MakeDirtyConnections();
		MakeDirty();
	}

	public void OnPassClicked() {
		if (!IsUnlocked() || mode == PhenoGenoEnum.Phenotype || affectedGeneAxonInput.lockness == LocknessEnum.Locked || ignoreSliderMoved) {
			return;
		}
		affectedGeneAxonInput.valveMode = SignalValveModeEnum.Pass;
		motherPanel.MarkAsNewForge();
		//motherPanel.UpdateConnections();
		motherPanel.MakeDirty();
		CellPanel.instance.cellAndGenePanel.arrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.arrowHandler.MakeDirtyConnections();
		MakeDirty();
	}

	public void OnSetReferenceClicked() {
		if (isUsed && IsUnlocked() && affectedGeneAxonInput.lockness == LocknessEnum.Unlocked && MouseAction.instance.actionState == MouseActionStateEnum.free && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype && affectedGeneAxonInput.valveMode == SignalValveModeEnum.Pass) {
			MouseAction.instance.actionState = MouseActionStateEnum.selectSignalOutput;
			Debug.Assert(staticAffectedGeneAxonInputPanel == null);
			staticAffectedGeneAxonInputPanel = this;
			motherPanel.MakeDirty();
		}
	}

	public static AxonInputPanel staticAffectedGeneAxonInputPanel;
	public static void TryAnswerSetReference(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot) {
		if (staticAffectedGeneAxonInputPanel == null) {
			return;
		}

		staticAffectedGeneAxonInputPanel.affectedGeneAxonInput.nerve.inputUnit = inputUnit;
		staticAffectedGeneAxonInputPanel.affectedGeneAxonInput.nerve.inputUnitSlot = inputUnitSlot;
		staticAffectedGeneAxonInputPanel.motherPanel.MakeDirty();
		CellPanel.instance.cellAndGenePanel.arrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.arrowHandler.MakeDirtyConnections();
		staticAffectedGeneAxonInputPanel = null;
	}

	private void Update() {
		if (Input.GetKey(KeyCode.Escape)) {
			if (MouseAction.instance.actionState == MouseActionStateEnum.selectSignalOutput) {
				Audio.instance.ActionAbort(1f);
				MouseAction.instance.actionState = MouseActionStateEnum.free;
				staticAffectedGeneAxonInputPanel = null;
				motherPanel.MakeDirty();
			}
		}

		if (!CreatureSelectionPanel.instance.hasSoloSelected) {
			return;
		}

		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Anone Input Panel");
			}
			ignoreSliderMoved = true;

			if (selectedGene == null || affectedGeneAxonInput == null) {
				isDirty = false;
				return;
			}

			blockButton.color = affectedGeneAxonInput.valveMode == SignalValveModeEnum.Block ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
			passButton.color = affectedGeneAxonInput.valveMode == SignalValveModeEnum.Pass ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;

			if (mode == PhenoGenoEnum.Genotype) {
				if (affectedGeneAxonInput.valveMode == SignalValveModeEnum.Block) {
					inputButtonImage.color = ColorScheme.instance.signalUnused;
				} else if (affectedGeneAxonInput.nerve.inputUnit == SignalUnitEnum.Void) {
					inputButtonImage.color = Color.magenta; // should never happen
				} else {
					inputButtonImage.color = ColorScheme.instance.signalOff; // we have a chance of an ON signal
				}
				// Color while choosing output
				if (staticAffectedGeneAxonInputPanel != null && staticAffectedGeneAxonInputPanel.affectedGeneAxonInput == affectedGeneAxonInput) {
					inputButtonImage.color = new Color(0f, 1f, 0f);
				}
				lockedOverlayImage.gameObject.SetActive(affectedGeneAxonInput.lockness == LocknessEnum.Locked);
				semiLockedOverlayImage.gameObject.SetActive(affectedGeneAxonInput.lockness == LocknessEnum.SemiLocked);
			} else {
				if (runtimeOutput == LogicBoxInputEnum.BlockedByValve) {
					inputButtonImage.color = ColorScheme.instance.signalUnused;
				} else if (runtimeOutput == LogicBoxInputEnum.VoidInput) { // should never happen
					inputButtonImage.color = Color.magenta;
				} else if (runtimeOutput == LogicBoxInputEnum.On) {
					inputButtonImage.color = ColorScheme.instance.signalOn;
				} else if (runtimeOutput == LogicBoxInputEnum.Off) {
					inputButtonImage.color = ColorScheme.instance.signalOff;
				} else if (runtimeOutput == LogicBoxInputEnum.Error) {
					inputButtonImage.color = Color.red;
				}
				lockedOverlayImage.gameObject.SetActive(false);
				semiLockedOverlayImage.gameObject.SetActive(false);
			}
			ignoreSliderMoved = false;

			isDirty = false;
		}
	}

	private LogicBoxInputEnum runtimeOutput {
		get {
			if (affectedGeneAxonInput != null) {
				if (affectedGeneAxonInput.valveMode == SignalValveModeEnum.Block) {
					return LogicBoxInputEnum.BlockedByValve;
				} else if (affectedGeneAxonInput.nerve.inputUnit == SignalUnitEnum.Void) {
					return LogicBoxInputEnum.VoidInput;
				} else {
					if (selectedCell != null) {
						return selectedCell.GetOutputFromUnit(affectedGeneAxonInput.nerve.inputUnit, affectedGeneAxonInput.nerve.inputUnitSlot) ? LogicBoxInputEnum.On : LogicBoxInputEnum.Off;
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
