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

	private void Update() {
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
				} else {
					inputButtonImage.color = ColorScheme.instance.signalOff;
				}
			} else {
				if (runtimeOutput == OutputFromInputEnum.BlockedByValve) {
					inputButtonImage.color = ColorScheme.instance.grayedOut;
				} else if (runtimeOutput == OutputFromInputEnum.VoidInput) {
					inputButtonImage.color = ColorScheme.instance.signalOff;
				} else if (runtimeOutput == OutputFromInputEnum.On) {
					inputButtonImage.color = ColorScheme.instance.signalOn;
				} else if (runtimeOutput == OutputFromInputEnum.Off) {
					inputButtonImage.color = ColorScheme.instance.signalOff;
				} else if (runtimeOutput == OutputFromInputEnum.Error) {
					inputButtonImage.color = Color.red;
				}
			}

			ignoreSliderMoved = false;

			isDirty = false;
		}
	}

	private OutputFromInputEnum runtimeOutput {
		get {
			if (affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Block) {
				return OutputFromInputEnum.BlockedByValve;
			} else if (affectedGeneLogicBoxInput.internalInput == SignalUnitEnum.Void) {
				return OutputFromInputEnum.VoidInput;
			} else {
				if (selectedCell != null) {
					return selectedCell.GetOutputFromUnit(affectedGeneLogicBoxInput.internalInput) ? OutputFromInputEnum.On : OutputFromInputEnum.Off;
				}
			}
			return OutputFromInputEnum.Error;
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
