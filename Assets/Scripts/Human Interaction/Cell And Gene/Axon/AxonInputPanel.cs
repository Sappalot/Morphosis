﻿using UnityEngine;
using UnityEngine.UI;

// TODO Generalize to just be an input panel for all units
public class AxonInputPanel : MonoBehaviour, IInputPanel {
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

	private AxonPanel motherPanel;

	public void MakeMotherPanelDirty() {
		motherPanel.MakeDirty();
	}

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

	public void Initialize(PhenoGenoEnum mode, int column, AxonPanel motherPanel) {
		this.mode = mode;
		this.motherPanel = motherPanel;
		this.column = column;
		isUsed = true;
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
		CellPanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.MakeDirty(); // arrows need to be updated
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
		CellPanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.MakeDirty(); // arrows need to be updated
		MakeDirty();
	}

	public void OnSetReferenceClicked() {
		if (isUsed && IsUnlocked() && affectedGeneAxonInput.lockness == LocknessEnum.Unlocked && affectedGeneAxonInput.valveMode == SignalValveModeEnum.Pass) {
			AssignNerveInputPanel.instance.TryStartNerveAssignation(this);
			motherPanel.MakeDirty();
		}
	}

	public void TrySetNerveInputLocally(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot) {
		affectedGeneAxonInput.nerve.inputUnit = inputUnit;
		affectedGeneAxonInput.nerve.inputUnitSlot = inputUnitSlot;
	}

	public void TrySetNerveInputExternally(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot, Vector2i nerveVectorLocal) {

	}

	private void Update() {
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

			blockButton.color = affectedGeneAxonInput.valveMode == SignalValveModeEnum.Block ? ColorScheme.instance.selectedChanged : ColorScheme.instance.notSelectedChanged;
			passButton.color = affectedGeneAxonInput.valveMode == SignalValveModeEnum.Pass ? ColorScheme.instance.selectedChanged : ColorScheme.instance.notSelectedChanged;

			if (mode == PhenoGenoEnum.Genotype) {
				if (affectedGeneAxonInput.valveMode == SignalValveModeEnum.Block || !motherPanel.affectedGeneSensor.isUsedInternal) {
					inputButtonImage.color = ColorScheme.instance.signalUnused;
				} else if (affectedGeneAxonInput.nerve.inputUnit == SignalUnitEnum.Void) {
					inputButtonImage.color = Color.magenta; // should never happen
				} else {
					inputButtonImage.color = ColorScheme.instance.signalOff; // we have a chance of an ON signal
				}
				// Color while choosing output
				if (AssignNerveInputPanel.instance.IsThisThePanelBeingAssigned(this)) {
					inputButtonImage.color = new Color(0f, 1f, 0f);
				}
				lockedOverlayImage.gameObject.SetActive(affectedGeneAxonInput.lockness == LocknessEnum.Locked);
				semiLockedOverlayImage.gameObject.SetActive(affectedGeneAxonInput.lockness == LocknessEnum.SemiLocked);
			} else {
				if (runtimeOutput == LogicBoxInputEnum.BlockedByValve || !motherPanel.affectedGeneSensor.isUsedInternal) {
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
