using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureSelectionPanel : MonoSingleton<CreatureSelectionPanel> {

	//--
	public CameraController cameraController;

	public GameObject showHideRoot;

	public new Camera camera;
	public LineRenderer lineRenderer;

	public Text selectedCreatureText;
	public Text creatureCreatedText;

	//right side
	public Text moveButtonText;
	public Text rotateButtonText;
	public Text deleteButtonText;
	public Text cloneButtonText;
	public Text combineButtonText;
	// ^ right side ^

	// left side
	public Text spiecesButtonText;

	public Button motherButton;
	public Text motherButtonText;

	public Button fatherButton;
	public Text fatherButtonText;

	public Button childrenButton;
	public Text childrenButtonText;
	// ^ left side ^

	public Text selectPreviousGroupLabel;

	[HideInInspector]
	public List<Creature> selection = new List<Creature>();
	[HideInInspector]
	public List<string> selectionPreviousIds = new List<string>();
	[HideInInspector]
	public List<Creature> selectionCluster = new List<Creature>();

	public bool isDirty = true;

	public enum TemperatureState {
		Defrosted,
		Frozen,
		Mixed,
		Error,
	}

	public bool hasSoloSelectedThatCanChangeGenome {
		get {
			return hasSoloSelected && soloSelected.allowedToChangeGenome;
		}
	}

	public TemperatureState GetSelectionTemperatureState() {
		return GetTemperatureState(selection);
	}

	static TemperatureState GetTemperatureState(List<Creature> creatures) {
		int defrosted = 0;
		int frozen = 0;
		foreach (Creature c in creatures) {
			if (c.creation != CreatureCreationEnum.Frozen) {
				defrosted++;
			}
			if (c.creation == CreatureCreationEnum.Frozen) {
				frozen++;
			}
		}
		if (frozen > 0 && defrosted > 0) {
			return TemperatureState.Mixed;
		} else if (frozen > 0 && defrosted == 0) {
			return TemperatureState.Frozen;
		} else if (defrosted > 0 && frozen == 0) {
			return TemperatureState.Defrosted;
		}
		return TemperatureState.Error;
	}

	public void MakeDirty() {
		isDirty = true;
		ViewSelectedCreaturePanel.instance.MakeDirty();
	}

	public Creature soloSelected {
		get {
			if (selection.Count != 1)
				return null;
			return selection[0];
		}
	}

	public bool hasSoloSelected {
		get {
			return selection.Count == 1;
		}
	}

	public bool hasFrozenSoloSelected {
		get {
			return selection.Count == 1 && GetTemperatureState(selection) == TemperatureState.Frozen;
		}
	}

	public Cell selectedCell {
		get {
			return CellPanel.instance.selectedCell;
		}
		set {
			CellPanel.instance.selectedCell = value;
		}
	}

	public Gene selectedGene {
		get {
			return GenePanel.instance.selectedGene;
		}
		set {
			GenePanel.instance.selectedGene = value;
		}
	}

	public int selectionCount {
		get {
			return selection.Count;
		}
	}

	public bool hasSelection { 
		get {
			return selection.Count > 0;
		}
	}

	public bool hasDefrostedSelection {
		get {
			return selection.Count > 0 && GetTemperatureState(selection) == TemperatureState.Defrosted;
		}
	}

	public bool IsSelected(Creature creature) {
		return selection.Contains(creature);
	}

	public bool IsSelectedCluster(Creature creature) {
		return selectionCluster.Contains(creature);
	}

	public override void Init() {
		base.Init();
	}

	//UI done
	public void ClearSelection() {
		StoreSelectionToPrevious();

		DirtyMarkSelection();
		selection.Clear();

		MakeDirty();
		PhenotypePanel.instance.MakeDirty();
		GenotypePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenePanel.instance.selectedGene = null;
		LockedUnlockedPanel.instance.MakeDirty();

		SetCellAndGeneSelectionToOrigin();

		ClearSelectionCluster();
	}

	public void ClearSelectionCluster() {
		DirtyMarkSelectionCluster();
		selectionCluster.Clear();
	}

	public void SetCellAndGeneSelectionToOrigin() {
		selectedCell = null;
		GenePanel.instance.selectedGene = null;
		MakeDirty();
	}

	private void StoreSelectionToPrevious() {
		selectionPreviousIds.Clear();
		foreach (Creature c in selection) {
			selectionPreviousIds.Add(c.id);
		}
	}

	private void UpdatePreviousSelectionWhatIsLeft() {
		List<Creature> whatIsLeft = new List<Creature>();
		foreach (string id in selectionPreviousIds) {
			Creature stillThere = World.instance.life.GetCreature(id);
			if (stillThere != null && stillThere.phenotype.isAlive) {
				whatIsLeft.Add(stillThere);
			}
		}
		selectionPreviousIds.Clear();
		foreach (Creature c in whatIsLeft) {
			selectionPreviousIds.Add(c.id);
		}
	}

	private void RestoreSelectionFromPrevious() {
		UpdatePreviousSelectionWhatIsLeft();
		List<Creature> whatIsLeft = new List<Creature>();
		foreach (string id in selectionPreviousIds) {
			whatIsLeft.Add(World.instance.life.GetCreature(id));
		}

		if (whatIsLeft.Count > 0) {
			Select(whatIsLeft);
		}
	}

	public void OnPressedRestorePreviousSelection() {
		RestoreSelectionFromPrevious();
	}

	public void Select(Creature creature, Cell cell = null) {
		StoreSelectionToPrevious();

		if (cell == null) {
			cell = creature.phenotype.originCell;
		}
		DirtyMarkSelection();
		selection.Clear();
		selection.Add(creature);

		selectedCell = cell;

		GenePanel.instance.selectedGene = cell.gene;
		LockedUnlockedPanel.instance.MakeDirty();

		creature.MakeDirtyGraphics();
		MakeDirty();
		PhenotypePanel.instance.MakeDirty();
		GenotypePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenePanel.instance.cellAndGenePanel.geneNeighboursPanel.MakeDirty();
		
		GenePanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();

		UpdateSelectionCluster();
	}

	public void Select(List<Creature> creatures) {
		StoreSelectionToPrevious();

		List<Creature> allCreatures = World.instance.life.creatures;

		ClearSelection();
		selection.AddRange(creatures);

		World.instance.life.MakeAllCreaturesDirty();
		MakeDirty();
		PhenotypePanel.instance.MakeDirty();
		GenotypePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenePanel.instance.cellAndGenePanel.geneNeighboursPanel.MakeDirty();
		LockedUnlockedPanel.instance.MakeDirty();

		GenePanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();

		UpdateSelectionCluster();
	}

	public void AddToSelection(List<Creature> creatures) {
		StoreSelectionToPrevious();
		for (int index = 0; index < creatures.Count; index++) {
			AddToSelection(creatures[index], false);
		}

		UpdateSelectionCluster();
	}

	public void AddToSelection(Creature creature, bool storePreviousSelection) {
		if (storePreviousSelection) {
			StoreSelectionToPrevious();
		}

		if (selection.Contains(creature)) {
			return;
		}

		selectedCell = null;
		selection.Add(creature);

		DirtyMarkSelection();
		MakeDirty();
		PhenotypePanel.instance.MakeDirty();
		GenotypePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenePanel.instance.cellAndGenePanel.geneNeighboursPanel.MakeDirty();
		LockedUnlockedPanel.instance.MakeDirty();

		UpdateSelectionCluster();
	}

	public void RemoveFromSelection(List<Creature> creatures) {
		StoreSelectionToPrevious();
		for (int index = 0; index < creatures.Count; index++) {
			RemoveFromSelection(creatures[index], false);
		}

		UpdateSelectionCluster();
	}

	public void RemoveFromSelection(Creature creature, bool storePreviousSelection) {
		if (storePreviousSelection) {
			StoreSelectionToPrevious();
		}

		if (!selection.Contains(creature)) {
			return;
		}
		DirtyMarkSelection();

		selectedCell = null;
		selection.Remove(creature);

		SetCellAndGeneSelectionToOrigin();

		MakeDirty();
		PhenotypePanel.instance.MakeDirty();
		GenotypePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenePanel.instance.cellAndGenePanel.geneNeighboursPanel.MakeDirty();
		LockedUnlockedPanel.instance.MakeDirty();

		World.instance.life.UpdateStructure();
		UpdateSelectionCluster();
	}

	public void UpdateSelectionCluster() {
		ClearSelectionCluster();
		for (int indexSelected = 0; indexSelected < selection.Count; indexSelected++) {
			Creature selectedCreature = selection[indexSelected];
			List<Creature> creaturesInCluster = selectedCreature.creaturesInCluster;
			for (int indexCluster = 0; indexCluster < creaturesInCluster.Count; indexCluster++) {
				Creature clusterCreature = creaturesInCluster[indexCluster];
				if (!selectionCluster.Contains(clusterCreature)) {
					selectionCluster.Add(clusterCreature);
				}
			}
		}
		DirtyMarkSelectionCluster();
	}

	private void DirtyMarkSelection() {
		for (int index = 0; index < selection.Count; index++) {
			selection[index].MakeDirtyGraphics();
		}
	}

	private void DirtyMarkSelectionCluster() {
		for (int index = 0; index < selectionCluster.Count; index++) {
			selectionCluster[index].MakeDirtyGraphics();
		}
	}

	//Actions ------------------------------------------------------------------------
	//Separat class?

	// Select
	public void OnSelectMotherClicked() {
		if (isInterferredBySomeAction) { return; }

		if (hasSoloSelected && soloSelected.HasMotherAlive() && soloSelected.GetMotherAlive() != null) {
			if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && PhenotypePanel.instance.followToggle.isOn) {
				World.instance.cameraController.TurnCameraStraightAtCameraUnlock();
			}

			Select(soloSelected.GetMotherAlive());
		}
	}

	public void OnSelectFatherClicked() {
		if (isInterferredBySomeAction) { return; }
		// TODO
	}

	public void OnSelectChildrenClicked() {
		if (isInterferredBySomeAction) { return; }

		if (hasSoloSelected && soloSelected.HasChildrenAlive()) {
			if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && PhenotypePanel.instance.followToggle.isOn) {
				World.instance.cameraController.TurnCameraStraightAtCameraUnlock();
			}

			List<Creature> select = new List<Creature>();
			foreach(Creature child in soloSelected.GetChildrenAlive()) {
				select.Add(child);
			}
			Select(select);
		}
	}
	// ^ Select ^

	// Delete
	public void OnDeleteClicked() {
		if (isInterferredBySomeAction) {
			return;
		}

		for (int index = 0; index < selection.Count; index++) {
			if (selection[index].creation == CreatureCreationEnum.Frozen) {
				Freezer.instance.KillCreatureSafe(selection[index], true);
				World.instance.AddHistoryEvent(new HistoryEvent("x", false, Color.blue));
			} else {
				World.instance.life.KillCreatureSafe(selection[index], true);
				World.instance.AddHistoryEvent(new HistoryEvent("x", false, Color.gray));
			}
		}
		ClearSelection();
	}

	//---- Move, rotate, Copy, merge copy
	public Vector2 MoveCreaturesCenterPhenotype {
		get	{
			Vector2 pow = Vector2.zero;
			foreach (Creature c in moveCreatures) {
				pow += (Vector2)c.phenotype.originCell.position;
			}
			return pow / selectionCount;
		}
	}

	public Vector2 MoveCreaturesCenterGenotype {
		get	{
			Vector2 pow = Vector2.zero;
			foreach (Creature c in moveCreatures) {
				pow += (Vector2)c.genotype.originCell.position;
			}
			return pow / selectionCount;
		}
	}

	private void DetatchAllUnselectedRelatives() {
		List<Creature> attachedUnselectedChildren = new List<Creature>();
		foreach (Creature creature in selection) {
			//mother
			if (creature.HasMotherAlive() && !selection.Contains(creature.GetMotherAlive())) {
				creature.DetatchFromMother(false, true);
			}

			//children
			foreach (Creature child in creature.GetChildrenAlive()) {
				if (!selection.Contains(child) && child != null) {
					child.DetatchFromMother(false, true);
				}
			}
		}
	}

	// Move
	private List<Creature> moveCreatures = new List<Creature>();

	private Dictionary<Creature, Vector2> moveOffset = new Dictionary<Creature, Vector2>();
	public void OnMoveClicked() {
		if (!hasSelection || isInterferredBySomeAction) {
			return;
		}

		World.instance.cameraController.TryUnlockCamera();

		DetatchAllUnselectedRelatives();
		StoreCreatures(selection);

		moveCreatures.AddRange(selection);
		StartMoveCreatures();
		MouseAction.instance.actionState = MouseActionStateEnum.moveCreatures;

		MakeDirty();
		PhenotypePanel.instance.MakeDirty();
		GenotypePanel.instance.MakeDirty();
	}

	// Rotate
	public void OnRotateClicked() {
		if (!hasSelection || isInterferredBySomeAction) {
			return;
		}

		World.instance.cameraController.TryUnlockCamera();

		DetatchAllUnselectedRelatives();
		StoreCreatures(selection);

		moveCreatures.AddRange(selection);

		Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
		if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			foreach (Creature c in moveCreatures) {
				c.Grab(PhenoGenoEnum.Phenotype);
			}

			foreach (Creature c in moveCreatures) {
				moveOffset.Add(c, (Vector2)c.transform.position - MoveCreaturesCenterPhenotype);
			}
			zeroRotationVector = (Vector2)mousePosition - MoveCreaturesCenterPhenotype;
			rotationCenter = MoveCreaturesCenterPhenotype;
		} else if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			foreach (Creature c in moveCreatures) {
				c.Grab(PhenoGenoEnum.Genotype);
			}
			foreach (Creature c in moveCreatures) {
				moveOffset.Add(c, (Vector2)c.transform.position - MoveCreaturesCenterGenotype);
			}

			zeroRotationVector = (Vector2)mousePosition - MoveCreaturesCenterGenotype;
			rotationCenter = MoveCreaturesCenterGenotype;
		}
		foreach (Creature c in moveCreatures) {
			startCreatureHeading.Add(c, c.genotype.originCell.heading);
		}

		lineRenderer.GetComponent<LineRenderer>().SetPosition(1, mousePosition);
		lineRenderer.GetComponent<LineRenderer>().SetPosition(0, rotationCenter);
		lineRenderer.enabled = true;

		MouseAction.instance.actionState = MouseActionStateEnum.rotateCreatures;

		MakeDirty();
		PhenotypePanel.instance.MakeDirty();
		GenotypePanel.instance.MakeDirty();
	}

	// Combine
	public void OnCombineClicked() {
		if (!hasSelection || selectionCount == 1 || isInterferredBySomeAction) {
			return;
		}

		World.instance.cameraController.TryUnlockCamera();

		Combine();
	}

	private void Combine() {
		List<Gene[]> genomes = new List<Gene[]>();
		foreach (Creature source in selection) {
			genomes.Add(source.genotype.genome);
		}

		if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			Creature mergeling = World.instance.life.SpawnCreatureMergling(genomes, Vector2.zero, 90f, World.instance.worldTicks);
			moveCreatures.Add(mergeling);
		} else if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			Creature mergeling = World.instance.life.SpawnCreatureMergling(genomes, Vector2.zero, 90f, World.instance.worldTicks);
			moveCreatures.Add(mergeling);
		}

		foreach (Creature c in moveCreatures) {
			c.UpdateStructure();
		}
		foreach (Creature c in moveCreatures) {
			c.phenotype.UpdateSpringLengths();
		}

		StartMoveCreatures();
		MouseAction.instance.actionState = MouseActionStateEnum.combineMoveCreatures;

		MakeDirty();
		PhenotypePanel.instance.MakeDirty();
		GenotypePanel.instance.MakeDirty();
	}

	// Copy
	private int creaturesInOriginalSelectionCount;
	public void OnCopyClicked() {
		if (!hasSelection || isInterferredBySomeAction) {
			return;
		}

		World.instance.cameraController.TryUnlockCamera();

		creaturesInOriginalSelectionCount = selection.Count;
		TemperatureState temperatureState = GetTemperatureState(selection);
		if (hasDefrostedSelection) {
			AddDefrostedCoppiesToMoveCreature(selection);
		} else if (hasFrozenSoloSelected) {
			Creature copy = Freezer.instance.SpawnCreatureCopy(soloSelected);
			moveCreatures.Add(copy);
		} else {
			return; // mixed or error
		}

		//in order to grab cells (make them not have any colliders we need them spawned at this point. lets force it)
		foreach (Creature c in moveCreatures) {
			c.UpdateStructure();
		}
		foreach (Creature c in moveCreatures) {
			c.phenotype.UpdateSpringLengths();
		}

		StartMoveCreatures();
		MouseAction.instance.actionState = MouseActionStateEnum.copyMoveCreatures;

		MakeDirty();
		PhenotypePanel.instance.MakeDirty();
		GenotypePanel.instance.MakeDirty();
	}

	private void AddDefrostedCoppiesToMoveCreature(List<Creature> originalCreatureList) {
		List<Creature> copies = new List<Creature>();
		Dictionary<string, string> originalToCopy = new Dictionary<string, string>();
		Dictionary<string, string> copyToOriginal = new Dictionary<string, string>();

		foreach (Creature originalCreature in originalCreatureList) {
			Creature copy = World.instance.life.SpawnCreatureCopy(originalCreature, World.instance.worldTicks); // Add new creature to life creature list
			moveCreatures.Add(copy);
			copies.Add(copy);
			originalToCopy.Add(originalCreature.id, copy.id);
			copyToOriginal.Add(copy.id, originalCreature.id);
		}

		foreach (Creature copy in copies) {
			Creature creatureCopy = World.instance.life.GetCreature(copy.id);
			Creature creatureOriginal = World.instance.life.GetCreature(copyToOriginal[copy.id]);
			// We use this one for freezer as well, and was not finding original in life of course, so let's look in the freezer if it wasn't there
			// Why did we have a problem with this first autumn 2019? it was made and has been working for a year or so .... strange!!
			// This seem to patch up the problem
			if (creatureOriginal == null) {
				creatureOriginal = Freezer.instance.GetCreature(copyToOriginal[copy.id]);
			}

			creatureCopy.ClearMotherAndChildrenReferences();

			// mother
			if (originalCreatureList.Find(c => c.id == creatureOriginal.GetMotherIdDeadOrAlive())) {
				//my mother is among the creatures which was coppied
				string copyId = originalToCopy[creatureOriginal.GetMotherIdDeadOrAlive()];
				creatureCopy.SetMotherReference(copyId);
			}

			//children
			List<string> creatureOriginalChildIdList = creatureOriginal.GetChildrenIdDeadOrAlive();

			// Narly code below !!
			for (int i = 0; i < creatureOriginal.ChildrenCountDeadOrAlive(); i++) {
				//For each child of the current original

				//SoulReference childReference = soulOriginal.childSoulReferences[i];
				string creatureOriginalsChildId = creatureOriginalChildIdList[i];

				if (originalCreatureList.Find(c => c.id == creatureOriginalsChildId)) { //childReference.id
					//Child was one of the copied creatures, so we need to make a child reference for the copy as well

					ChildData childData = new ChildData();

					// Our copy child reference is the same as the original, but poits at another creature child
					childData.id =                      originalToCopy[creatureOriginalsChildId];
					childData.isConnectedToMother =     creatureOriginal.IsAttachedToChildAlive(creatureOriginalsChildId);
					childData.originMapPosition =       creatureOriginal.ChildOriginMapPosition(creatureOriginalsChildId); //As seen from mothers frame of reference
					childData.originBindCardinalIndex = creatureOriginal.ChildOriginBindCardinalIndex(creatureOriginalsChildId);
					creatureCopy.AddChildReference(childData);
				}
			}
			creatureCopy.phenotype.connectionsDiffersFromCells = true;
		}
	}

	public void StartMoveCreatures() {
		Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
		if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {

			foreach (Creature c in moveCreatures) {
				c.Grab(PhenoGenoEnum.Phenotype);
				//c.isDirty = false;
			}

			//Offset
			foreach (Creature c in moveCreatures) {
				moveOffset.Add(c, (Vector2)c.transform.position - MoveCreaturesCenterPhenotype);
			}
		} else if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			foreach (Creature c in moveCreatures) {
				c.Grab(PhenoGenoEnum.Genotype);
			}
			foreach (Creature c in moveCreatures) {
				moveOffset.Add(c, (Vector2)c.transform.position - MoveCreaturesCenterGenotype);
			}
		}
	}

	//Rotate
	private Vector2 zeroRotationVector;
	private Vector2 rotationCenter;
	private float RotateCreaturesAngle;
	private Dictionary<Creature, float> startCreatureHeading = new Dictionary<Creature, float>();

	public List<Creature> PlaceHoveringCreatures() { //final creature
		List<Creature> placedCreatures = new List<Creature>();
		placedCreatures.AddRange(moveCreatures);

		ReleaseMoveCreatures();
		startCreatureHeading.Clear();

		ClearSelection();
		AddToSelection(moveCreatures);
		moveCreatures.Clear();

		lineRenderer.enabled = false;
		moveOffset.Clear();

		MouseAction.instance.actionState = MouseActionStateEnum.free;

		ClearStoredCreatures();

		CellPanel.instance.MakeDirty();
		GenePanel.instance.MakeDirty();

		return placedCreatures;
	}

	public List<Creature> PasteHoveringCreatures() {
		List<Creature> continueMoveCopy = new List<Creature>();
		continueMoveCopy.AddRange(moveCreatures);

		ReleaseMoveCreatures();
		startCreatureHeading.Clear();

		lineRenderer.enabled = false;
		moveOffset.Clear();

		moveCreatures.Clear();

		if (creaturesInOriginalSelectionCount == selectionCount) {
			AddDefrostedCoppiesToMoveCreature(continueMoveCopy);

			foreach (Creature c in moveCreatures) {
				c.UpdateStructure();
			}
			foreach (Creature c in moveCreatures) {
				c.phenotype.UpdateSpringLengths();
			}

			StartMoveCreatures();
			MouseAction.instance.actionState = MouseActionStateEnum.copyMoveCreatures;
		} else {
			// We have placed a capy and now all our originals are dead (we were at phenotype->play mode)
			MouseAction.instance.actionState = MouseActionStateEnum.free;
			Audio.instance.ActionAbort(1f);
		}

		return continueMoveCopy;
	}

	public List<Creature> PasteHoveringMergling() {
		List<Creature> merglings = new List<Creature>();
		merglings.AddRange(moveCreatures);

		ReleaseMoveCreatures();
		startCreatureHeading.Clear();

		lineRenderer.enabled = false;
		moveOffset.Clear();

		moveCreatures.Clear();

		if (hasSelection) {
			Combine();
		} else {
			// We have placed a mergeling and now all our originals are dead (we were at phenotype->play mode)
			MouseAction.instance.actionState = MouseActionStateEnum.free;
			Audio.instance.ActionAbort(1f);
		}

		return merglings;
	}

	public enum MoveCreatureType {
		Move,
		Copy,
		Combine,
	}

	public bool CanPlaceMoveCreatures(MoveCreatureType type, bool pressingLeftControl) {
		int frozenCount = 0;
		int worldCount = 0;
		int insideWorldCount = 0;
		int insideFreezerCount = 0;

		foreach (Creature c in moveCreatures) {
			if (c.creation == CreatureCreationEnum.Frozen) {
				frozenCount++;
			} else {
				worldCount++;
			}
			if (TerrainPerimeter.instance.IsCompletelyInside(c)) {
				insideWorldCount++;
			}
			if (Freezer.instance.IsCompletelyInside(c)) {
				insideFreezerCount++;
			}
		}

		if (type == MoveCreatureType.Move) {
			// world ==> world
			if (frozenCount == 0 && worldCount > 0 && worldCount == insideWorldCount && insideFreezerCount == 0) {
				return true;
			}
			// freezer ==> freezer
			if (worldCount == 0 && frozenCount == 1 && insideFreezerCount == 1 && insideWorldCount == 0) {
				return true;
			}
		} else if (type == MoveCreatureType.Copy) {
			// world ==> world
			if (frozenCount == 0 && worldCount > 0 && worldCount == insideWorldCount && insideFreezerCount == 0) {
				return true;
			}
			// world ==> freezer
			if (frozenCount == 0 && worldCount == 1 && insideFreezerCount == 1 && insideWorldCount == 0 && !pressingLeftControl) {
				return true;
			}
			// freezer ==> world
			if (worldCount == 0 && frozenCount == 1 && insideFreezerCount == 0 && insideWorldCount == 1 && !pressingLeftControl) {
				return true;
			}
			// freezer ==> freezer
			if (worldCount == 0 && frozenCount == 1 && insideFreezerCount == 1 && insideWorldCount == 0 && !pressingLeftControl) {
				return true;
			}
		} else if (type == MoveCreatureType.Combine) {
			// world ==> world
			if (frozenCount == 0 && worldCount >= 1 && worldCount == insideWorldCount && insideFreezerCount == 0) {
				return true;
			}
		}
		return false;
	}

	private void ReleaseMoveCreatures() {
		foreach (Creature c in moveCreatures) {
			c.Release(CreatureEditModePanel.instance.mode);
		}
	}

	private void SpawnAddEffect(Vector2 position) {
		if (MouseAction.instance.actionState == MouseActionStateEnum.combineMoveCreatures || MouseAction.instance.actionState == MouseActionStateEnum.copyMoveCreatures) {
			EventSymbolPlayer.instance.Play(EventSymbolEnum.CreatureAdd, position, 0f, SpatialUtil.MarkerScale());
		}
	}

	public void RemoveDeletedAndRecycledFromSelection() {
		List<Creature> keepers = new List<Creature>();
		foreach (Creature enbodied in selection) {
			if (enbodied != null && enbodied.phenotype.isAlive) {
				keepers.Add(enbodied);
			}
		}
		selection.Clear();
		selection.AddRange(keepers);
	}

	private Dictionary<Creature, CreatureData> storedCreatures = new Dictionary<Creature, CreatureData>();
	private void StoreCreatures(List<Creature> creatures) {
		foreach (Creature c in creatures) {
			storedCreatures.Add(c, c.UpdateData());
		}
	}

	private void RestoreCreatures() {
		foreach (KeyValuePair<Creature, CreatureData> pair in storedCreatures) {
			pair.Key.ApplyData(pair.Value);
			pair.Key.OnLoaded();
		}
	}

	private void ClearStoredCreatures() {
		storedCreatures.Clear();
	}

	private void Update() {
		if (World.instance.life == null) {
			return;
		}

		// Abort copy / breed
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (MouseAction.instance.actionState == MouseActionStateEnum.copyMoveCreatures || MouseAction.instance.actionState == MouseActionStateEnum.combineMoveCreatures) {
				Audio.instance.ActionAbort(1f);

				List<Creature> killList = new List<Creature>(moveCreatures);
				ReleaseMoveCreatures();
				startCreatureHeading.Clear();
				moveCreatures.Clear();
				lineRenderer.enabled = false;
				moveOffset.Clear();

				foreach (Creature c in killList) {
					if (c.creation ==CreatureCreationEnum.Frozen) {
						Freezer.instance.KillCreatureSafe(c, false);
					} else {
						World.instance.life.KillCreatureSafe(c, false);
					}
					
				}
				
				MouseAction.instance.actionState = MouseActionStateEnum.free;

				MakeDirty();
				PhenotypePanel.instance.MakeDirty();
				GenotypePanel.instance.MakeDirty();
			}

			// Abort move / rotate
			if (MouseAction.instance.actionState == MouseActionStateEnum.moveCreatures || MouseAction.instance.actionState == MouseActionStateEnum.rotateCreatures) {
				Audio.instance.ActionAbort(1f);

				RestoreCreatures();
				ClearStoredCreatures();
				ReleaseMoveCreatures();
				startCreatureHeading.Clear();
				moveCreatures.Clear();
				moveOffset.Clear();
				lineRenderer.enabled = false;

				MouseAction.instance.actionState = MouseActionStateEnum.free;

				MakeDirty();
				PhenotypePanel.instance.MakeDirty();
				GenotypePanel.instance.MakeDirty();
			}
		} else if (Input.GetKeyDown(KeyCode.Backspace)) {
			OnPressedRestorePreviousSelection();
		}

		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CreatureSelectionPanel");
			}

			StartCoroutine(UpdateIsVisible());

			RemoveDeletedAndRecycledFromSelection();

			UpdatePreviousSelectionWhatIsLeft();
			if (selectionPreviousIds.Count == 0) {
				selectPreviousGroupLabel.color = ColorScheme.instance.grayedOut;
				selectPreviousGroupLabel.text = "Select previous creatures";
			} else {
				selectPreviousGroupLabel.color = ColorScheme.instance.normalText;
				if (selectionPreviousIds.Count == 1) {
					selectPreviousGroupLabel.text = "Select previous creature";
				} else {
					List<string> ids = new List<string>();
					foreach (Creature c in selection) {
						ids.Add(c.id);
					}
					if (IsEqual(selectionPreviousIds, ids)) {
						selectPreviousGroupLabel.color = ColorScheme.instance.grayedOut;
						selectPreviousGroupLabel.text = "Select previous creatures";
					} else {
						selectPreviousGroupLabel.color = ColorScheme.instance.normalText;
						selectPreviousGroupLabel.text = "Select previous " + selectionPreviousIds.Count + " creatures";
					}
					
				}
			}

			if (selection.Count == 0) {
				selectedCreatureText.text = "";
				creatureCreatedText.text = "";

				//right side
				moveButtonText.color = ColorScheme.instance.grayedOut;
				rotateButtonText.color = ColorScheme.instance.grayedOut;
				deleteButtonText.color = ColorScheme.instance.grayedOut;
				cloneButtonText.color = ColorScheme.instance.grayedOut;
				combineButtonText.color = ColorScheme.instance.grayedOut;
				// ^ right side ^
				// left side
				spiecesButtonText.color = ColorScheme.instance.grayedOut;

				motherButtonText.color = ColorScheme.instance.grayedOut;
				motherButton.gameObject.SetActive(true);
				childrenButtonText.text = "Mother";

				fatherButtonText.color = ColorScheme.instance.grayedOut;
				fatherButton.gameObject.SetActive(true);
				childrenButtonText.text = "Father";

				childrenButtonText.color = ColorScheme.instance.grayedOut;
				childrenButton.gameObject.SetActive(true);
				childrenButtonText.text = "Children";
				// ^ left side ^


			} else if (selection.Count == 1) {
				selectedCreatureText.text = soloSelected.id; // soloSelected.nickname;
				//motherText.text = "Mother: " + (soloSelected.hasMotherSoul ? (soloSelected.soul.isConnectedWithMotherSoul ? "[" : "") + soloSelected.motherSoul.id + (soloSelected.soul.isConnectedWithMotherSoul ? "]" : "") : "<none>");

				creatureCreatedText.text = soloSelected.creation.ToString() + (soloSelected.creation != CreatureCreationEnum.Frozen ? ", Generation: " + soloSelected.generation : "");
				

				//right side
				moveButtonText.color = Color.black;
				rotateButtonText.color = Color.black;
				deleteButtonText.color = Color.black;
				cloneButtonText.color = Color.black;
				combineButtonText.color = Color.grey;
				// ^ right side ^
				// left side
				spiecesButtonText.color = Color.black;


				motherButtonText.color = ColorScheme.instance.mother;
				if (soloSelected.HasMotherDeadOrAlive()) {
					motherButton.gameObject.SetActive(true); //show even if mother is dead
					motherButtonText.text = soloSelected.HasMotherAlive() ? "Mother" : "(Mother)";
				} else {
					motherButton.gameObject.SetActive(false);
				}

				//TODO: father
				fatherButtonText.color = ColorScheme.instance.father;
				fatherButton.gameObject.SetActive(false);

				childrenButtonText.color = Color.black;
				if (soloSelected.HasChildrenDeadOrAlive()) {
					childrenButton.gameObject.SetActive(true); //show even if mother is dead
					int alive = soloSelected.ChildrenCountAlive();
					int dead = soloSelected.ChildrenCountDeadOrAlive() - alive;
					childrenButtonText.text = "Cn: " + (alive > 0 ? alive.ToString() : "") + (alive > 0 && dead > 0 ? "+" : "") + (dead > 0 ? "(" + dead.ToString() + ")" : "");
				} else {
					childrenButton.gameObject.SetActive(false);
				}
				// ^ left side ^

			} else {
				selectedCreatureText.text = selection.Count + " Creatures";
				creatureCreatedText.text = "-";
				

				//right side
				moveButtonText.color = Color.black;
				rotateButtonText.color = Color.black;
				deleteButtonText.color = Color.black;
				cloneButtonText.color = Color.black;
				combineButtonText.color = Color.black;
				// ^ right side ^
				// left side
				spiecesButtonText.color = ColorScheme.instance.grayedOut;

				motherButtonText.color = ColorScheme.instance.grayedOut;
				motherButton.gameObject.SetActive(true);

				fatherButtonText.color = ColorScheme.instance.grayedOut;
				fatherButton.gameObject.SetActive(true);

				childrenButtonText.color = ColorScheme.instance.grayedOut;
				childrenButton.gameObject.SetActive(true);
				// ^ left side ^
			}

			isDirty = false;
		}

		//Keys
		if (Input.GetKeyDown(KeyCode.M) && !GlobalPanel.instance.isWritingHistoryNote) {
			OnMoveClicked();
		}
		if (Input.GetKeyDown(KeyCode.R) && !GlobalPanel.instance.isWritingHistoryNote) {
			OnRotateClicked();
		}
		if (Input.GetKeyDown(KeyCode.C) && !GlobalPanel.instance.isWritingHistoryNote) {
			OnCopyClicked();
		}
		if (Input.GetKeyDown(KeyCode.Delete) && !GlobalPanel.instance.isWritingHistoryNote) {
			OnDeleteClicked();
		}
		if (Input.GetKeyDown(KeyCode.B) && !GlobalPanel.instance.isWritingHistoryNote) {
			OnCombineClicked();
		}

		Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;

		//move
		if (MouseAction.instance.actionState == MouseActionStateEnum.moveCreatures ||
			MouseAction.instance.actionState == MouseActionStateEnum.copyMoveCreatures ||
			MouseAction.instance.actionState == MouseActionStateEnum.combineMoveCreatures) {

			foreach (Creature c in moveCreatures) {
				c.transform.position = (Vector2)mousePosition + moveOffset[c];
			}
		}

		//rotate
		if (MouseAction.instance.actionState == MouseActionStateEnum.rotateCreatures) {
			lineRenderer.GetComponent<LineRenderer>().SetPosition(1, mousePosition);
			lineRenderer.GetComponent<LineRenderer>().SetPosition(0, rotationCenter);

			RotateCreaturesAngle = Vector2.SignedAngle(zeroRotationVector, (Vector2)mousePosition - rotationCenter);

			foreach (Creature c in moveCreatures) {
				//Rotate around main center
				Vector3 turnedVector = Quaternion.Euler(0, 0, RotateCreaturesAngle) * moveOffset[c];
				c.transform.position = (Vector2)rotationCenter + (Vector2)turnedVector;

				//rotate around own center
				c.transform.localRotation = Quaternion.Euler(0, 0, RotateCreaturesAngle);

				if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
					c.phenotype.originCell.heading = startCreatureHeading[c] + RotateCreaturesAngle;
				} else if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
					c.genotype.originCell.heading = startCreatureHeading[c] + RotateCreaturesAngle;
				}
			}
		}

		//debug markers
		foreach (Creature c in World.instance.life.creatures) {
			//c.ShowMarkers(IsSelected(c));
			c.ShowMarkers(false);
		}

		foreach (Creature c in Freezer.instance.creatures) {
			//c.ShowMarkers(IsSelected(c));
			c.ShowMarkers(false);
		}
	}

	private bool IsEqual(List<string> collection1, List<string> collection2) {
		if (collection1.Count != collection2.Count)
			return false; // the collections are not equal

		foreach (string item in collection1) {
			if (!collection2.Contains(item))
				return false; // the collections are not equal
		}

		foreach (string item in collection2) {
			if (!collection1.Contains(item))
				return false; // the collections are not equal
		}

		return true; // the collections are equal
	}

	private IEnumerator UpdateIsVisible() {
		bool show = hasSelection && MouseAction.instance.actionState == MouseActionStateEnum.free && !AlternativeToolModePanel.instance.isOn;
		yield return 0;
		showHideRoot.SetActive(show);
	}

	private bool isInterferredBySomeAction {
		get {
			return MouseAction.instance.actionState != MouseActionStateEnum.free || !World.instance.creatureSelectionController.IsIdle || AlternativeToolModePanel.instance.isOn;
		}
	}
}