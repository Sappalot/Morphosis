using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignNerveInputPanel : MonoSingleton<AssignNerveInputPanel> {
	public GameObject hidable;

	public enum state {
		standby,
		started, // waiting for: (internally: set arrow tail | (extrernally: set "arrow tail" at geneCell or set "arrow head" at gene cell))

	}

	public bool isInAuxternalGene {
		get {
			return GeneAuxiliaryPanel.instance.gameObject.activeInHierarchy;
		}
	}

	private bool isDirty = true;

	//stached nerve, needed when aborting assignment
	private SignalUnitEnum stachedNerveInputUnit;
	private SignalUnitSlotEnum stachedNerveInputUnitSlot;
	private SignalUnitEnum stachedNerveOutputUnit;
	private SignalUnitSlotEnum stachedNerveOutputUnitSlot;
	private Vector2i stachedNerveNerveVector;

	private void StachGeneNerve(GeneNerve geneNerve) {
		stachedNerveInputUnit = geneNerve.tailUnitEnum;
		stachedNerveInputUnitSlot = geneNerve.tailUnitSlotEnum;
		stachedNerveOutputUnit = geneNerve.headUnitEnum;
		stachedNerveOutputUnitSlot = geneNerve.headUnitSlotEnum;
		stachedNerveNerveVector = geneNerve.nerveVector;
	}

	public void MakeDirty() {
		isDirty = true;
	}

	public bool IsThisThePanelBeingAssigned(IInputPanel inputPanel) {
		return affectedPanel != null && affectedPanel == inputPanel;
	}

	// Only way in
	// After we clicked a legal "nerve output" / "arrow head" / "signal unit input" (on either part of axon panel OR part of logic box panel)
	public void TryStartNerveAssignation(IInputPanel affectedPanel, GeneNerve geneNerve) {
		if (MouseAction.instance.actionState == MouseActionStateEnum.free && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			hidable.SetActive(true);

			StachGeneNerve(geneNerve);

			this.affectedPanel = affectedPanel;
			MouseAction.instance.actionState = MouseActionStateEnum.selectSignalOutput;
		}
	}

	private IInputPanel affectedPanel;

	// After we clicked a legal othr  "nerve input" / "arrow tail" / "signal output OR sensor output"
	public void TrySetNerveInput(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot) {
		if (affectedPanel == null) {
			return;
		}
		if (isInAuxternalGene) {
			// we have been pushing a button in an external gene panel
			DebugUtil.Log("Setting externally");
			affectedPanel.TrySetNerveInputExternally(inputUnit, inputUnitSlot, nerveVectorToSet);

		} else {
			// we have been pushing a button in the same gene panel, we are local
			DebugUtil.Log("Setting locally");
			affectedPanel.TrySetNerveInputLocally(inputUnit, inputUnitSlot);
		}
		CellPanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
		GenePanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
		OnLeave();
	}

	// External...
	[HideInInspector]
	private Vector2i m_narrowedGeneCellMapPosition;
	public Vector2i narrowedGeneCellMapPosition {
		get {
			return m_narrowedGeneCellMapPosition;
		} set {
			m_narrowedGeneCellMapPosition = value;
		}
	}

	// After we click one of the gene cells which has selected gene 'in it'
	public bool TrySetNarrowedGeneCellMapPosition(Vector2i cellPosition) {
		foreach (Cell c in selectedGeneCells) {
			if (c.mapPosition == cellPosition) {
				narrowedGeneCellMapPosition = cellPosition;
				return true;
			}
		}
		return false;
	}

	// the cells with the selected gene in it
	public List<Cell> selectedGeneCells {
		get {
			Genotype genotype = CreatureSelectionPanel.instance.soloSelected.genotype;
			Gene gene = GenePanel.instance.selectedGene;

			return genotype.GetGeneCellsWithGene(gene);
		}

	}

	public int selectedGeneCellsCount {
		get {
			return selectedGeneCells.Count;
		}
	}

	// The cell to which the nerve is sending its signal "nerve arrow head"
	public Vector2i selectedRootCellMapPosition {
		get {
			if (narrowedGeneCellMapPosition != null) {
				return narrowedGeneCellMapPosition; // we have many but have allready narrowed doen to one
			} else if (selectedGeneCellsCount == 1) {
				return selectedGeneCells[0].mapPosition; // the one and only (no need to specify)
			}

			return null;
		}
	}

	public bool ShowNerveInputMapPositionExternally(Vector2i targetGeneCellMapPosition) {
		if (affectedPanel == null) {
			return false;
		}

		Genotype genotype = CreatureSelectionPanel.instance.soloSelected.genotype;

		if (targetGeneCellMapPosition == null || genotype.GetCellAtMapPosition(targetGeneCellMapPosition) == null || selectedRootCellMapPosition == null || genotype.GetCellAtMapPosition(selectedRootCellMapPosition) == null) {
			affectedPanel.ShowNerveInputExternally(null);
			return true;
		}


		if (selectedRootCellMapPosition != null && targetGeneCellMapPosition != selectedRootCellMapPosition) {
			//DebugUtil.Log("Target position: " + targetGeneCell.mapPosition);
			//DebugUtil.Log("Root position: " + selectedRootCell.mapPosition);
			int distance = CellMap.ManhexanDistance(targetGeneCellMapPosition, selectedRootCellMapPosition);
			//DebugUtil.Log("Manhexan distance: " + distance);

			if (distance <= 5) {
				// move vector to origin (tail at origo)
				Vector2i nerveVector = CellMap.HexagonalMinus(targetGeneCellMapPosition, selectedRootCellMapPosition);

				// imagine rotateing gene cell so that it is pointing (arrow pointing to) north (cardinal index 2)
				// the vector needs to be rotated the same amount to end up in local space
				// ....so let's do just that
				int rootDirection = genotype.GetCellAtMapPosition(selectedRootCellMapPosition).bindCardinalIndex;
				int turnToLocalAngle = 0;
				if (rootDirection == 0) { // ne
					turnToLocalAngle = 1; // +60
				} else if (rootDirection == 1) { // n
					turnToLocalAngle = 0; // just fine
				} else if (rootDirection == 2) { // nw
					turnToLocalAngle = 5; // +300
				} else if (rootDirection == 3) { // sw
					turnToLocalAngle = 4; // +240
				} else if (rootDirection == 4) { // s
					turnToLocalAngle = 3; // +180
				} else if (rootDirection == 5) { // se
					turnToLocalAngle = 2; // +120
				}
				//DebugUtil.Log("Vector before rotate: " + nerveVector.ToString());
				nerveVector = CellMap.HexagonalRotate(nerveVector, turnToLocalAngle);

				// flip vector horizontally only if cell flip side is (white|black) 
				if (genotype.GetCellAtMapPosition(selectedRootCellMapPosition).flipSide == FlipSideEnum.WhiteBlack) {
					nerveVector = CellMap.HexagonalFlip(nerveVector);
				}

				//DebugUtil.Log("Final Vector: " + nerveVector.ToString());

				// nerveVector is now in creature space
				CellPanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
				GenePanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
	
				affectedPanel.ShowNerveInputExternally(nerveVector);

				return true;
			} else {
				DebugUtil.Log("Too long nerve, max distance = 5");
			}

		}
		return false;
	}

	private Vector2i nerveVectorToSet;

	// After we click the external gene cell to which we want our nerve to listen "nerve arrow tail"
	public bool TrySetNerveInputMapPositionExternally(Vector2i targetGeneCellMapPosition) {
		if (affectedPanel == null) {
			return false;
		}

		Genotype genotype = CreatureSelectionPanel.instance.soloSelected.genotype;
		Phenotype phenotype = CreatureSelectionPanel.instance.soloSelected.phenotype;

		if (targetGeneCellMapPosition == null || genotype.GetCellAtMapPosition(targetGeneCellMapPosition) == null || selectedRootCellMapPosition == null || genotype.GetCellAtMapPosition(selectedRootCellMapPosition) == null) {
			affectedPanel.ShowNerveInputExternally(null);
			return true;
		}

		
		if (selectedRootCellMapPosition != null && targetGeneCellMapPosition != selectedRootCellMapPosition ) {
			//DebugUtil.Log("Target position: " + targetGeneCell.mapPosition);
			//DebugUtil.Log("Root position: " + selectedRootCell.mapPosition);
			int distance = CellMap.ManhexanDistance(targetGeneCellMapPosition, selectedRootCellMapPosition);
			//DebugUtil.Log("Manhexan distance: " + distance);

			if (distance <= 5) {
				// move vector to origin (tail at origo)
				Vector2i nerveVector = CellMap.HexagonalMinus(targetGeneCellMapPosition, selectedRootCellMapPosition);

				// imagine rotateing gene cell so that it is pointing (arrow pointing to) north (cardinal index 2)
				// the vector needs to be rotated the same amount to end up in local space
				// ....so let's do just that
				int rootDirection = genotype.GetCellAtMapPosition(selectedRootCellMapPosition).bindCardinalIndex;
				int turnToLocalAngle = 0;
				if (rootDirection == 0) { // ne
					turnToLocalAngle = 1; // +60
				} else if (rootDirection == 1) { // n
					turnToLocalAngle = 0; // just fine
				} else if (rootDirection == 2) { // nw
					turnToLocalAngle = 5; // +300
				} else if (rootDirection == 3) { // sw
					turnToLocalAngle = 4; // +240
				} else if (rootDirection == 4) { // s
					turnToLocalAngle = 3; // +180
				} else if (rootDirection == 5) { // se
					turnToLocalAngle = 2; // +120
				}
				//DebugUtil.Log("Vector before rotate: " + nerveVector.ToString());
				nerveVector = CellMap.HexagonalRotate(nerveVector, turnToLocalAngle);

				// flip vector horizontally only if cell flip side is (white|black) 
				if (genotype.GetCellAtMapPosition(selectedRootCellMapPosition).flipSide == FlipSideEnum.WhiteBlack) {
					nerveVector = CellMap.HexagonalFlip(nerveVector);
				}

				nerveVectorToSet = nerveVector;

				//DebugUtil.Log("Final Vector: " + nerveVector.ToString());

				// nerveVector is now in creature space
				CellPanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
				GenePanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();

				//affectedPanel.TrySetNerveInputExternally(SignalUnitEnum.Axon, SignalUnitSlotEnum.outputLateA, nerveVector);
				//OnLeave();
				GeneAuxiliaryPanel.instance.gameObject.SetActive(true);
				GeneAuxiliaryPanel.instance.viewedGene = genotype.GetCellAtMapPosition(targetGeneCellMapPosition).gene;
				GeneAuxiliaryPanel.instance.viewedCell = phenotype.cellMap.GetCell(targetGeneCellMapPosition); // We need this one in order to show build priority properly (not super important but better showing it right than wrong)

				return true;
			} else {
				DebugUtil.Log("Too long nerve, max distance = 5");
			}

		}
		return false;
	}
	// ^External^

	// Abort assignation regardless of state and put nerve back in the state it was before we started assignation
	private void TryAbortAssignation() {
		if (MouseAction.instance.actionState == MouseActionStateEnum.selectSignalOutput && affectedPanel != null) {
			Audio.instance.ActionAbort(1f);
			affectedPanel.TrySetNerve(stachedNerveInputUnit, stachedNerveInputUnitSlot, stachedNerveOutputUnit, stachedNerveOutputUnitSlot, stachedNerveNerveVector);
			OnLeave();
		}
	}

	private void OnLeave() {
		MouseAction.instance.actionState = MouseActionStateEnum.free;
		//affectedPanel.MakeMotherPanelDirty();
		narrowedGeneCellMapPosition = null;
		affectedPanel = null;
		hidable.SetActive(false);
		GeneAuxiliaryPanel.instance.gameObject.SetActive(false);
		nerveVectorToSet = null;

		//CreatureSelectionPanel.instance.soloSelected.genotype.MakeGeneCellPatternDirty();
		GenePanel.instance.cellAndGenePanel.MakeDirty(); // arrows need to be updated
		GenePanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
	}

	public void OnAbortButtonClicked() {
		TryAbortAssignation();
	}

	private void Update() {
		if (Input.GetKey(KeyCode.Escape)) {
			TryAbortAssignation();
		}
		if (isDirty) {
			isDirty = false;
		}
	}
}
