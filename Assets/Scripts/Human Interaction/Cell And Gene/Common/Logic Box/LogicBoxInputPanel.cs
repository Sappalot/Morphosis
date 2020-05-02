using UnityEngine;
using UnityEngine.UI;

// TODO: Generalize to just be an input panel for all units, arn't they gonna be all the same?
public class LogicBoxInputPanel : MonoBehaviour, IInputPanel {
	[HideInInspector]
	public bool isGhost;

	public Image blockButton;
	public Image passButton;
	public Image lockedOverlayImage;
	public Image semiLockedOverlayImage;

	public Image inputButtonImage;
	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	private CellAndGenePanel cellAndGenePanel;

	private int column;
	private bool isDirty = false;
	private bool ignoreSliderMoved = false;

	public void MakeMotherPanelDirty() {
		motherPanel.MakeDirty();
	}

	private LogicBoxPanel motherPanel;
	public GeneLogicBoxInput affectedGeneLogicBoxInput { 
		get {
			if (selectedGene.type == CellTypeEnum.Egg && motherPanel.signalUnit == SignalUnitEnum.WorkLogicBoxA) {
				return selectedGene.eggCellFertilizeLogic.GetInput(column);
			} else if (motherPanel.signalUnit == SignalUnitEnum.DendritesLogicBox) {
				return selectedGene.dendritesLogicBox.GetInput(column);
			}
			if (motherPanel.signalUnit == SignalUnitEnum.OriginDetatchLogicBox) {
				return selectedGene.originDetatchLogicBox.GetInput(column);
			}
			return null;
		}
	}

	public GeneNerve GetGeneNerve() {
		return affectedGeneLogicBoxInput.nerve;
	}

	public void Initialize(PhenoGenoEnum mode, int column, LogicBoxPanel motherPanel, CellAndGenePanel cellAndGenePanel) {
		this.mode = mode;
		this.motherPanel = motherPanel;
		this.column = column;
		this.cellAndGenePanel = cellAndGenePanel;
	}

	private PhenoGenoEnum GetMode() {
		return mode;
	}

	public void MakeDirty() {
		isDirty = true;
	}

	public void OnBlockClicked() {
		if (!IsUnlocked() || mode == PhenoGenoEnum.Phenotype || affectedGeneLogicBoxInput.lockness == LocknessEnum.Locked || ignoreSliderMoved) {
			return;
		}
		affectedGeneLogicBoxInput.valveMode = SignalValveModeEnum.Block;
		motherPanel.MarkAsNewForge();
		motherPanel.UpdateConnections();
		motherPanel.MakeDirty();
		CellPanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.MakeDirty(); // arrows need to be updated
		MakeDirty();
	}

	public void OnPassClicked() {
		if (!IsUnlocked() || mode == PhenoGenoEnum.Phenotype || affectedGeneLogicBoxInput.lockness == LocknessEnum.Locked || ignoreSliderMoved) {
			return;
		}
		affectedGeneLogicBoxInput.valveMode = SignalValveModeEnum.Pass;
		motherPanel.MarkAsNewForge();
		motherPanel.UpdateConnections();
		motherPanel.MakeDirty();
		
		CellPanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.MakeDirty(); // arrows need to be updated
		MakeDirty();
	}

	public void OnSetReferenceClicked() {
		if (!isGhost && IsUnlocked() && affectedGeneLogicBoxInput.lockness == LocknessEnum.Unlocked && affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Pass) {
			AssignNerveInputPanel.instance.TryStartNerveAssignation(this, affectedGeneLogicBoxInput.nerve);
			motherPanel.MakeDirty();
		}
	}

	public void TrySetNerveInputLocally(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot) {
		affectedGeneLogicBoxInput.nerve.inputUnit = inputUnit;
		affectedGeneLogicBoxInput.nerve.inputUnitSlot = inputUnitSlot;
		affectedGeneLogicBoxInput.nerve.nerveVector = null;
	}

	public void TrySetNerveInputExternally(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot, Vector2i nerveVectorLocal) {
		affectedGeneLogicBoxInput.nerve.inputUnit = inputUnit;
		affectedGeneLogicBoxInput.nerve.inputUnitSlot = inputUnitSlot;
		affectedGeneLogicBoxInput.nerve.nerveVector = nerveVectorLocal;
	}

	public void TrySetNerve(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot, SignalUnitEnum outputUnit, SignalUnitSlotEnum outputUnitSlot, Vector2i nerveVector) {
		affectedGeneLogicBoxInput.nerve.inputUnit = inputUnit;
		affectedGeneLogicBoxInput.nerve.inputUnitSlot = inputUnitSlot;
		affectedGeneLogicBoxInput.nerve.outputUnit = outputUnit;
		affectedGeneLogicBoxInput.nerve.outputUnitSlot = outputUnitSlot;
		affectedGeneLogicBoxInput.nerve.nerveVector = nerveVector;
	}

	// Used to only display input vectors
	public void ShowNerveInputExternally(Vector2i nerveVectorLocal) {
		affectedGeneLogicBoxInput.nerve.inputUnit = SignalUnitEnum.Void;
		affectedGeneLogicBoxInput.nerve.nerveVector = nerveVectorLocal;
	}

	private void Update() {
		if (!CreatureSelectionPanel.instance.hasSoloSelected) {
			return;
		}

		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Hibernate Panel");
			}
			ignoreSliderMoved = true;

			// ...ghost ...
			
			if (isGhost) {
				blockButton.color = ColorScheme.instance.grayedOut;
				passButton.color = ColorScheme.instance.grayedOut;
				inputButtonImage.color = ColorScheme.instance.signalGhost;
				lockedOverlayImage.gameObject.SetActive(false);
				semiLockedOverlayImage.gameObject.SetActive(false);
				return;
			}

			// ^ ghost ^ 

			if (selectedGene == null || affectedGeneLogicBoxInput == null) {
				isDirty = false;
				return;
			}

			blockButton.color = affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Block ? ColorScheme.instance.selectedChanged : ColorScheme.instance.notSelectedChanged;
			passButton.color = affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Pass ? ColorScheme.instance.selectedChanged : ColorScheme.instance.notSelectedChanged;

			if (mode == PhenoGenoEnum.Genotype) {
				if (affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Block || !motherPanel.affectedGeneLogicBox.isRooted) {
					inputButtonImage.color = ColorScheme.instance.signalUnused; // blocked or logic box not used (pretty if they are all of if not used)
				} else if (affectedGeneLogicBoxInput.nerve.inputUnit == SignalUnitEnum.Void) {
					inputButtonImage.color = Color.magenta; // should never happen
				} else {
					inputButtonImage.color = ColorScheme.instance.signalOff; // we have a chance of an ON signal
				}
				// Color while choosing output
				if (AssignNerveInputPanel.instance.IsThisThePanelBeingAssigned(this)) {
					inputButtonImage.color = new Color(0f, 1f, 0f);
				}
				lockedOverlayImage.gameObject.SetActive(affectedGeneLogicBoxInput.lockness == LocknessEnum.Locked);
				semiLockedOverlayImage.gameObject.SetActive(affectedGeneLogicBoxInput.lockness == LocknessEnum.SemiLocked);
			} else {
				if (runtimeOutput == LogicBoxInputEnum.BlockedByValve || !motherPanel.affectedGeneLogicBox.isRooted) {
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
				return cellAndGenePanel.cell != null ? cellAndGenePanel.gene : null;
			} else {
				return cellAndGenePanel.gene;
			}
		}
	}

	private Cell selectedCell {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return cellAndGenePanel.cell;
			} else {
				return null; // there could be many cells selected for the same gene
			}
		}
	}

	public bool IsUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome && !cellAndGenePanel.isAuxiliary;
	}
}
