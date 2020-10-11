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
			if (selectedGene.type == CellTypeEnum.Egg && motherPanel.signalUnitEnum == SignalUnitEnum.WorkLogicBoxA) {
				return selectedGene.eggCellFertilizeLogic.GetInput(column);
			} else if (motherPanel.signalUnitEnum == SignalUnitEnum.DendritesLogicBox) {
				return selectedGene.dendritesLogicBox.GetInput(column);
			}
			if (motherPanel.signalUnitEnum == SignalUnitEnum.OriginDetatchLogicBox) {
				return selectedGene.originDetatchLogicBox.GetInput(column);
			}
			return null;
		}
	}

	public GeneNerve GetGeneNerve() {
		return affectedGeneLogicBoxInput.geneNerve;
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
		if (!IsUnlocked() || mode == PhenoGenoEnum.Phenotype || affectedGeneLogicBoxInput == null || affectedGeneLogicBoxInput.lockness == LocknessEnum.Locked || ignoreSliderMoved) {
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
		if (!IsUnlocked() || mode == PhenoGenoEnum.Phenotype || affectedGeneLogicBoxInput == null || affectedGeneLogicBoxInput.lockness == LocknessEnum.Locked || ignoreSliderMoved) {
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
			AssignNerveInputPanel.instance.TryStartNerveAssignation(this, affectedGeneLogicBoxInput.geneNerve);
			motherPanel.MakeDirty();
		}
	}

	public void TrySetNerveInputLocally(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot) {
		affectedGeneLogicBoxInput.geneNerve.tailUnitEnum = inputUnit;
		affectedGeneLogicBoxInput.geneNerve.tailUnitSlotEnum = inputUnitSlot;
		affectedGeneLogicBoxInput.geneNerve.nerveVector = null;
	}

	public void TrySetNerveInputExternally(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot, Vector2i nerveVectorLocal) {
		affectedGeneLogicBoxInput.geneNerve.tailUnitEnum = inputUnit;
		affectedGeneLogicBoxInput.geneNerve.tailUnitSlotEnum = inputUnitSlot;
		affectedGeneLogicBoxInput.geneNerve.nerveVector = nerveVectorLocal;
	}

	public void TrySetNerve(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot, SignalUnitEnum outputUnit, SignalUnitSlotEnum outputUnitSlot, Vector2i nerveVector) {
		affectedGeneLogicBoxInput.geneNerve.tailUnitEnum = inputUnit;
		affectedGeneLogicBoxInput.geneNerve.tailUnitSlotEnum = inputUnitSlot;
		affectedGeneLogicBoxInput.geneNerve.headUnitEnum = outputUnit;
		affectedGeneLogicBoxInput.geneNerve.headUnitSlotEnum = outputUnitSlot;
		affectedGeneLogicBoxInput.geneNerve.nerveVector = nerveVector;
	}

	// Used to only display input vectors
	public void ShowNerveInputExternally(Vector2i nerveVectorLocal) {
		affectedGeneLogicBoxInput.geneNerve.tailUnitEnum = SignalUnitEnum.Void;
		affectedGeneLogicBoxInput.geneNerve.nerveVector = nerveVectorLocal;
	}

	private void Update() {
		if (!CreatureSelectionPanel.instance.hasSoloSelected) {
			return;
		}

		if (isDirty) {
			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
				DebugUtil.Log("Update Hibernate Panel");
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
				if (affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Block || !motherPanel.isAnyAffectedSignalUnitsRootedGenotype) {
					inputButtonImage.color = ColorScheme.instance.signalUnused; // blocked or logic box not used (pretty if they are all of if not used)
				} else if (affectedGeneLogicBoxInput.geneNerve.tailUnitEnum == SignalUnitEnum.Void) {
					inputButtonImage.color = Color.red; // should never happen
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

				if (runtimeOutput == LogicBoxInputEnum.BlockedByValve || motherPanel.affectedSignalUnit.rootnessEnum == RootnessEnum.Unrooted) {
					inputButtonImage.color = ColorScheme.instance.signalUnused;
				} else if (motherPanel.affectedSignalUnit.rootnessEnum == RootnessEnum.Rootable) {
					inputButtonImage.color = ColorScheme.instance.signalRootable;
				} else if (runtimeOutput == LogicBoxInputEnum.VoidInput) { // should never happen
					inputButtonImage.color = Color.red;
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
				Nerve inputNerve = ((LogicBox)motherPanel.affectedSignalUnit).inputNerves[column];

				if (affectedGeneLogicBoxInput.valveMode == SignalValveModeEnum.Block) {
					return LogicBoxInputEnum.BlockedByValve;
				} else if (inputNerve.tailSignalUnitEnum == SignalUnitEnum.Void) {
					return LogicBoxInputEnum.VoidInput;
				} else if (inputNerve.tailCell == null) {
					return LogicBoxInputEnum.Off;
				} else if (inputNerve.tailCell.GetOutputFromUnit(inputNerve.tailSignalUnitEnum, inputNerve.tailSignalUnitSlotEnum)) {
					return LogicBoxInputEnum.On;
				} else {
					return LogicBoxInputEnum.Off;
				}
				//return selectedCell.GetOutputFromUnit(affectedGeneLogicBoxInput.geneNerve.tailUnitEnum, affectedGeneLogicBoxInput.geneNerve.tailUnitSlotEnum) ? LogicBoxInputEnum.On : LogicBoxInputEnum.Off;
				
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
