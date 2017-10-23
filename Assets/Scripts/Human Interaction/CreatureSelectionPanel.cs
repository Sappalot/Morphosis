using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreatureSelectionPanel : MonoSingleton<CreatureSelectionPanel> {
	public Life life;
	public PhenotypePanel phenotypePanel;
	public new Camera camera;
	public LineRenderer lineRenderer;

	public Text selectedCreatureText;
	public Text motherText;
	public Text childrenText;

	public List<Creature> selection { get; private set; }

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

	public override void Init() {
		base.Init();
		selection = new List<Creature>();
		ClearSelection();

		lineRenderer.enabled = false;
	}

	//UI done
	public void ClearSelection() {
		DirtyMarkSelected();
		selection.Clear();

		isDirty = true;
		phenotypePanel.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenePanel.instance.selectedGene = null;

		SetCellAndGeneSelectionToRoot();
	}

	public void SetCellAndGeneSelectionToRoot() {
		selectedCell = null;
		GenePanel.instance.selectedGene = null;
		isDirty = true;
	}

	public void Select(Creature creature, Cell cell = null) {
		DirtyMarkSelected();
		selection.Clear();
		selection.Add(creature);

		selectedCell = cell;
		GenePanel.instance.selectedGene = cell.gene;

		creature.MakeDirty();
		isDirty = true;
		phenotypePanel.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenePanel.instance.MakeDirty();
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

		StoreAllSelectedsState();
	}

	public void AddToSelection(List<Creature> creatures) {
		for (int index = 0; index < creatures.Count; index++) {
			AddToSelection(creatures[index]);
		}
	}

	public void AddToSelection(Creature creature) {
		if (selection.Contains(creature)) {
			return;
		}

		selectedCell = null;
		selection.Add(creature);

		DirtyMarkSelected();
		isDirty = true;
		phenotypePanel.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenePanel.instance.MakeDirty();
	}

	public void RemoveFromSelection(List<Creature> creatures) {
		for (int index = 0; index < creatures.Count; index++) {
			RemoveFromSelection(creatures[index]);
		}
	}

	public void RemoveFromSelection(Creature creature) {
		if (!selection.Contains(creature)) {
			return;
		}
		DirtyMarkSelected();

		selectedCell = null;
		selection.Remove(creature);

		SetCellAndGeneSelectionToRoot();

		isDirty = true;
		phenotypePanel.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenePanel.instance.MakeDirty();
	}

	private void DirtyMarkSelected() {
		for (int index = 0; index < selection.Count; index++) {
			selection[index].MakeDirty();
		}
	}

	public void StoreAllSelectedsState() {
		for (int index = 0; index < selection.Count; index++) {
			selection[index].StoreState();
		}
	}

	//Actions ------------------------------------------------------------------------
	//Separat class?

	// Delete
	public void OnDeleteClicked() {
		for (int index = 0; index < selection.Count; index++) {
			life.DeleteCreature(selection[index]);
		}
		ClearSelection();
	}

	//---- Move, rotate, Copy, merge copy
	public Vector2 MoveCreaturesCenterPhenotype {
		get	{
			Vector2 pow = Vector2.zero;
			foreach (Creature c in moveCreatures) {
				pow += (Vector2)c.phenotype.rootCell.position;
			}
			return pow / selectionCount;
		}
	}

	public Vector2 MoveCreaturesCenterGenotype {
		get	{
			Vector2 pow = Vector2.zero;
			foreach (Creature c in moveCreatures) {
				pow += (Vector2)c.genotype.rootCell.position;
			}
			return pow / selectionCount;
		}
	}

	// Move
	private List<Creature> moveCreatures = new List<Creature>();

	private Dictionary<Creature, Vector2> moveOffset = new Dictionary<Creature, Vector2>();
	public void OnMoveClicked() {
		if (!hasSelection) {
			return;
		}
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
			Creature mergeling = World.instance.life.SpawnCreatureMergling(genomes, Vector2.zero, 90f);
			moveCreatures.Add(mergeling);
		} else if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
			Creature mergeling = World.instance.life.SpawnCreatureMergling(genomes, Vector2.zero, 90f);
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

	private void AddCoppiesToMoveCreature(List<Creature> originals) {
		List<Creature> copies = new List<Creature>();
		Dictionary<string, string> originalToCopy = new Dictionary<string, string>();
		foreach (Creature original in originals) {
			Creature copy = World.instance.life.SpawnCreatureCopy(original);
			moveCreatures.Add(copy);
			copies.Add(copy);
			originalToCopy.Add(original.id, copy.id);
		}

		//Clean up ids can't have same as original and reftrence ids to children mothers must be right too
		//name all creatures with new ids, and map the old ones to the new ones (oldID, newID)
		//Go through all reftrences and update mothers and children id to match the new ones
		foreach (Creature copy in copies) {
			foreach (KeyValuePair<string, string> entry in originalToCopy) {
				copy.TryChangeRelativesId(entry.Key, entry.Value);
			}
		}

		//TODO: remove children


		//Assume we copy whole cluster or nothing at all
		//If we copy parts of a cluster we will need to update the references accodingly 

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
			startCreatureHeading.Add(c, c.genotype.rootCell.heading);
		}

		lineRenderer.GetComponent<LineRenderer>().SetPosition(1, mousePosition);
		lineRenderer.GetComponent<LineRenderer>().SetPosition(0, rotationCenter);
		lineRenderer.enabled = true;

		MouseAction.instance.actionState = MouseActionStateEnum.rotateCreatures;
	}

	public void PlaceHoveringCreatures() {
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
		ReleaseMoveCreatures();

		startCreatureHeading.Clear();
		lineRenderer.enabled = false;
		moveOffset.Clear();

		moveCreatures.Clear();

		OnCombineClicked();
	}

	private void ReleaseMoveCreatures() {
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

	//--------------------------------
	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update CreatureSelectionPanel");
			if (selection.Count == 0) {
				selectedCreatureText.text = "";
				motherText.text = "Mother:";
				childrenText.text = "Offspring:";
			} else if (selection.Count == 1) {
				selectedCreatureText.text = soloSelected.id; // soloSelected.nickname;
				motherText.text = "Mother: " + (soloSelected.hasMother ? (soloSelected.mother.isConnected ? "[" : "") + soloSelected.mother.id + (soloSelected.mother.isConnected ? "]" : "") : "<none>");
				string childrenString = "";
				for (int i = 0; i < soloSelected.children.Count; i++) {
					childrenString += (soloSelected.children[i].isConnected ? "[" : "") + soloSelected.children[i].id + (soloSelected.children[i].isConnected ? "]" : "");
					if (i < soloSelected.children.Count - 1) {
						childrenString += ", ";
					}
				}

				childrenText.text = "Offspring: " + (soloSelected.hasChild ?  childrenString : "<none>");
			} else {
				selectedCreatureText.text = selection.Count + " Creatures";
				motherText.text = "Mother: -"; //TODO : if all have the same mother - write it
				childrenText.text = "Offspring: -"; //TODO: if all have the same chilren - write it
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
					c.phenotype.rootCell.heading = startCreatureHeading[c] + RotateCreaturesAngle;
				} else if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
					c.genotype.rootCell.heading = startCreatureHeading[c] + RotateCreaturesAngle;
				}
			}
		}

		//debug markers
		foreach (Creature c in World.instance.life.creatures) {
			//c.ShowMarkers(IsSelected(c));
			c.ShowMarkers(false);
		}
	}
}