using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureSelectionPanel : MonoSingleton<CreatureSelectionPanel> {
	public Life life;
	public PhenotypePanel phenotypePanel;
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

	public List<Creature> selection { get; private set; }
	public List<Creature> selectionCluster { get; private set; }

	private bool isDirty = true;

	public void MakeDirty() {
		isDirty = true;
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

	public bool IsSelected(Creature creature) {
		return selection.Contains(creature);
	}

	public bool IsSelectedCluster(Creature creature) {
		return selectionCluster.Contains(creature);
	}

	public override void Init() {
		base.Init();
		selection = new List<Creature>();
		selectionCluster = new List<Creature>();
		ClearSelection();

		lineRenderer.enabled = false;
	}

	//UI done
	public void ClearSelection() {
		DirtyMarkSelection();
		selection.Clear();

		isDirty = true;
		phenotypePanel.MakeDirty();
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
		isDirty = true;
	}

	public void Select(Creature creature, Cell cell = null) {
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
		isDirty = true;
		phenotypePanel.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenePanel.instance.MakeDirty();

		UpdateSelectionCluster();
	}

	public void Select(List<Creature> creatures) {
		List<Creature> allCreatures = life.creatures;

		ClearSelection();
		selection.AddRange(creatures);

		life.MakeAllCreaturesDirty();
		isDirty = true;
		phenotypePanel.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenePanel.instance.MakeDirty();
		LockedUnlockedPanel.instance.MakeDirty();

		StoreAllSelectedsState();

		UpdateSelectionCluster();
	}

	public void AddToSelection(List<Creature> creatures) {
		for (int index = 0; index < creatures.Count; index++) {
			AddToSelection(creatures[index]);
		}

		UpdateSelectionCluster();
	}

	public void AddToSelection(Creature creature) {
		if (selection.Contains(creature)) {
			return;
		}

		selectedCell = null;
		selection.Add(creature);

		DirtyMarkSelection();
		isDirty = true;
		phenotypePanel.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenePanel.instance.MakeDirty();
		LockedUnlockedPanel.instance.MakeDirty();

		UpdateSelectionCluster();
	}

	public void RemoveFromSelection(List<Creature> creatures) {
		for (int index = 0; index < creatures.Count; index++) {
			RemoveFromSelection(creatures[index]);
		}

		UpdateSelectionCluster();
	}

	public void RemoveFromSelection(Creature creature) {
		if (!selection.Contains(creature)) {
			return;
		}
		DirtyMarkSelection();

		selectedCell = null;
		selection.Remove(creature);

		SetCellAndGeneSelectionToOrigin();

		isDirty = true;
		phenotypePanel.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenePanel.instance.MakeDirty();
		LockedUnlockedPanel.instance.MakeDirty();

		Life.instance.UpdateStructure();
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

	public void StoreAllSelectedsState() {
		for (int index = 0; index < selection.Count; index++) {
			selection[index].StoreState();
		}
	}

	//Actions ------------------------------------------------------------------------
	//Separat class?

	// Select
	public void OnSelectMotherClicked() {
		if (hasSoloSelected && soloSelected.hasMother && soloSelected.mother != null) {
			Select(soloSelected.mother);
		}
	}

	public void OnSelectChildrenClicked() {
		if (hasSoloSelected && soloSelected.hasChildSoul) {
			List<Creature> select = new List<Creature>();
			foreach(Creature child in soloSelected.children) {
				select.Add(child);
			}
			Select(select);
		}
	}
	// ^ Select ^

	// Delete
	public void OnDeleteClicked() {
		for (int index = 0; index < selection.Count; index++) {
			life.KillCreatureSafe(selection[index]);
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
			if (creature.soul.motherSoulReference.id != string.Empty && !selection.Contains(creature.mother)) {
				creature.DetatchFromMother(false, true);
			}

			//children
			foreach (Creature child in creature.children) {
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
		if (!hasSelection) {
			return;
		}

		DetatchAllUnselectedRelatives();

		moveCreatures.AddRange(selection);
		StartMoveCreatures();
		MouseAction.instance.actionState = MouseActionStateEnum.moveCreatures;
	}

	// Combine
	public void OnCombineClicked() {
		if (!hasSelection) {
			return;
		}
		List<Gene[]> genomes = new List<Gene[]>();
		foreach (Creature source in selection) {
			genomes.Add(source.genotype.genome);
		}

		if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype) {
			Creature mergeling = Life.instance.SpawnCreatureMergling(genomes, Vector2.zero, 90f, World.instance.worldTicks);
			moveCreatures.Add(mergeling);
		} else if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
			Creature mergeling = Life.instance.SpawnCreatureMergling(genomes, Vector2.zero, 90f, World.instance.worldTicks);
			moveCreatures.Add(mergeling);
		}

		StartMoveCreatures();
		MouseAction.instance.actionState = MouseActionStateEnum.combineMoveCreatures;
	}

	// Copy
	public void OnCopyClicked() {
		if (!hasSelection) {
			return;
		}
		AddCoppiesToMoveCreature(selection);
		StartMoveCreatures();
		MouseAction.instance.actionState = MouseActionStateEnum.copyMoveCreatures;
	}

	private void AddCoppiesToMoveCreature(List<Creature> originalCreatures) {
		List<Creature> copies = new List<Creature>();
		Dictionary<string, string> originalToCopy = new Dictionary<string, string>();
		Dictionary<string, string> copyToOriginal = new Dictionary<string, string>();
		foreach (Creature originalCreature in originalCreatures) {
			Creature copy = Life.instance.SpawnCreatureCopy(originalCreature, World.instance.worldTicks); // will instantiate souls as well
			moveCreatures.Add(copy);
			copies.Add(copy);
			originalToCopy.Add(originalCreature.id, copy.id);
			copyToOriginal.Add(copy.id, originalCreature.id);
		}

		foreach (Creature copy in copies) {
			Soul soulCopy = Life.instance.GetSoul(copy.id);
			Soul soulOriginal = Life.instance.GetSoul(copyToOriginal[copy.id]);

			// me
			//soulCopy.id = copy.id; //not really needed
			//soulCopy.creatureReference.id = copy.id;
			soulCopy.SetCreatureImmediate(copy);

			// mother
			if (originalCreatures.Find(c => c.id == soulOriginal.motherSoulReference.id)) {
				//my mother is among the creatures which was coppied
				string copyId = originalToCopy[soulOriginal.motherSoulReference.id];

				//soulCopy.SetMotherSoul(copyId);
				Life.instance.SetMotherSoulImmediateSafe(soulCopy, Life.instance.GetSoul(copyId));
				//soulCopy.SetMotherSoulImmediate(Life.instance.GetSoul(copyId));
			}

			//children
			for (int i = 0; i < soulOriginal.childSoulsCount; i++) {
				SoulReference childReference = soulOriginal.childSoulReferences[i];
				if (originalCreatures.Find(c => c.id == childReference.id)) {
					string copyId = originalToCopy[childReference.id];
					Life.instance.AddChildSoulImmediateSafe(soulCopy, Life.instance.GetSoul(copyId), childReference.childOriginMapPosition, childReference.childOriginBindCardinalIndex, childReference.isChildConnected);
				}
			}
		}
	}

	public void StartMoveCreatures() {
		Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
		if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype) {

			foreach (Creature c in moveCreatures) {
				c.Grab(PhenoGenoEnum.Phenotype);
				//c.isDirty = false;
			}

			//Offset
			foreach (Creature c in moveCreatures) {
				moveOffset.Add(c, (Vector2)c.transform.position - MoveCreaturesCenterPhenotype);
			}
		} else if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
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

	public void OnRotateClicked() {
		if (!hasSelection) {
			return;
		}

		DetatchAllUnselectedRelatives();

		moveCreatures.AddRange(selection);

		Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
		if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype) {
			foreach (Creature c in moveCreatures) {
				c.Grab(PhenoGenoEnum.Phenotype);
			}

			foreach (Creature c in moveCreatures) {
				moveOffset.Add(c, (Vector2)c.transform.position - MoveCreaturesCenterPhenotype);
			}
			zeroRotationVector = (Vector2)mousePosition - MoveCreaturesCenterPhenotype;
			rotationCenter = MoveCreaturesCenterPhenotype;
		} else if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
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
	}

	public void PlaceHoveringCreatures() {
		if (GlobalPanel.instance.effectsPlaySound.isOn) {
			Audio.instance.PlaceCreature(CameraUtils.GetEffectStrengthLazy());
		}
		ReleaseMoveCreatures();
		startCreatureHeading.Clear();

		ClearSelection();
		AddToSelection(moveCreatures);
		moveCreatures.Clear();

		lineRenderer.enabled = false;
		moveOffset.Clear();

		MouseAction.instance.actionState = MouseActionStateEnum.free;
	}

	public void PasteHoveringCreatures() {
		if (GlobalPanel.instance.effectsPlaySound.isOn) {
			Audio.instance.PlaceCreature(CameraUtils.GetEffectStrengthLazy());
		}
		List<Creature> continueMoveCopy = new List<Creature>();
		continueMoveCopy.AddRange(moveCreatures);

		ReleaseMoveCreatures();
		startCreatureHeading.Clear();

		lineRenderer.enabled = false;
		moveOffset.Clear();

		moveCreatures.Clear();
		AddCoppiesToMoveCreature(continueMoveCopy);
		StartMoveCreatures();
		MouseAction.instance.actionState = MouseActionStateEnum.copyMoveCreatures;
	}

	public void PasteHoveringMergling() {
		if (GlobalPanel.instance.effectsPlaySound.isOn) {
			Audio.instance.PlaceCreature(CameraUtils.GetEffectStrengthLazy());
		}
		ReleaseMoveCreatures();

		startCreatureHeading.Clear();
		lineRenderer.enabled = false;
		moveOffset.Clear();

		moveCreatures.Clear();

		OnCombineClicked();
	}

	private void ReleaseMoveCreatures() {
		if (GlobalPanel.instance.effectsPlaySound.isOn) {
			Audio.instance.PlaceCreature(CameraUtils.GetEffectStrengthLazy());
		}
		if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype) {
			foreach (Creature c in moveCreatures) {
				c.Release(PhenoGenoEnum.Phenotype);
			}
		} else if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
			foreach (Creature c in moveCreatures) {
				c.Release(PhenoGenoEnum.Genotype);
			}
		}
	}

	public void RemoveNullCreaturesFromSelection() {
		List<Creature> keepers = new List<Creature>();
		foreach (Creature enbodied in selection) {
			if (enbodied != null) {
				keepers.Add(enbodied);
			}
		}
		selection.Clear();
		selection.AddRange(keepers);
	}

	//--------------------------------
	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update CreatureSelectionPanel");

			RemoveNullCreaturesFromSelection();

			if (selection.Count == 0) {
				selectedCreatureText.text = "";
				creatureCreatedText.text = "";

				//right side
				moveButtonText.color =     Color.gray;
				rotateButtonText.color =   Color.gray;
				deleteButtonText.color =   Color.gray;
				cloneButtonText.color =    Color.gray;
				combineButtonText.color =  Color.gray;
				// ^ right side ^
				// left side
				spiecesButtonText.color =  Color.gray;

				motherButtonText.color =   Color.gray;
				motherButton.gameObject.SetActive(true);

				fatherButtonText.color =   Color.gray;
				fatherButton.gameObject.SetActive(true);

				childrenButtonText.color = Color.gray;
				childrenButton.gameObject.SetActive(true);
				// ^ left side ^


			} else if (selection.Count == 1) {
				selectedCreatureText.text = soloSelected.id; // soloSelected.nickname;
				//motherText.text = "Mother: " + (soloSelected.hasMotherSoul ? (soloSelected.soul.isConnectedWithMotherSoul ? "[" : "") + soloSelected.motherSoul.id + (soloSelected.soul.isConnectedWithMotherSoul ? "]" : "") : "<none>");
				string childrenString = "" + soloSelected.childSouls.Count + " ";

				creatureCreatedText.text = soloSelected.creation.ToString() + (soloSelected.creation != CreatureCreationEnum.Forged ? ", Generation: " + soloSelected.generation : "");
				//right side
				moveButtonText.color = Color.black;
				rotateButtonText.color = Color.black;
				deleteButtonText.color = Color.black;
				cloneButtonText.color = Color.black;
				combineButtonText.color = Color.black;
				// ^ right side ^
				// left side
				spiecesButtonText.color = Color.black;

				motherButtonText.color = Color.red;
				motherButton.gameObject.SetActive(soloSelected.hasMotherSoul); //show even if mother is dead

				fatherButtonText.color = Color.red;
				fatherButton.gameObject.SetActive(false);

				childrenButtonText.color = Color.red;
				if (soloSelected.hasChildSoul) {
					childrenButton.gameObject.SetActive(true); //show even if mother is dead
					if (soloSelected.childSoulCount == 1) {
						childrenButtonText.text = "1 Child";
					} else {
						childrenButtonText.text = soloSelected.childSoulCount + " Children";
					}
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
				spiecesButtonText.color = Color.gray;

				motherButtonText.color = Color.gray;
				motherButton.gameObject.SetActive(true);

				fatherButtonText.color = Color.gray;
				fatherButton.gameObject.SetActive(true);

				childrenButtonText.color = Color.gray;
				childrenButton.gameObject.SetActive(true);
				// ^ left side ^
			}

			isDirty = false;
		}

		//Keys
		if (Input.GetKeyDown(KeyCode.M)) {
			OnMoveClicked();
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			OnRotateClicked();
		}
		if (Input.GetKeyDown(KeyCode.C)) {
			OnCopyClicked();
		}
		if (Input.GetKeyDown(KeyCode.Delete)) {
			OnDeleteClicked();
		}
		if (Input.GetKeyDown(KeyCode.B)) {
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

				if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype) {
					c.phenotype.originCell.heading = startCreatureHeading[c] + RotateCreaturesAngle;
				} else if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
					c.genotype.originCell.heading = startCreatureHeading[c] + RotateCreaturesAngle;
				}
			}
		}

		//debug markers
		foreach (Creature c in Life.instance.creatures) {
			//c.ShowMarkers(IsSelected(c));
			c.ShowMarkers(false);
		}
	}
}