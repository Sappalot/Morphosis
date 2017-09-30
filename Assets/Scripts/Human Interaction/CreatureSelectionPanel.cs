using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreatureSelectionPanel : MonoSingleton<CreatureSelectionPanel> {
	public Life life;
	public PhenotypePanel phenotypePanel;
	public Camera camera;
	public LineRenderer lineRenderer; 

	public Text selectedCreatureText;

	public List<Creature> selection { get; private set; }

	public Creature soloSelected {
		get {
			if (selection.Count != 1)
				return null;
			return selection[0];
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

	public void ClearSelection() {
		for (int index = 0; index < selection.Count; index++) {
			selection[index].ShowSelected(false);
			selection[index].ShowCellsAndGeneCellsSelected(false);
		}
		selection.Clear();
		UpdateGUI();

		UpdateGenotypePanel();
		UpdatePhenotypePanel();
	}

	public void Select(Creature creature, Cell cell = null) {
		for (int index = 0; index < selection.Count; index++) {
			selection[index].ShowSelected(false);
			selection[index].ShowCellsAndGeneCellsSelected(false);
		}
		selection.Clear();

		creature.ShowSelected(true);
		selection.Add(creature);
		SelectedCellAndGene(cell);
		creature.StoreState();
		UpdateGUI();
	}

	public void Select(List<Creature> creatures) {
		List<Creature> allCreatures = life.creatures;
		for (int index = 0; index < allCreatures.Count; index++) {
			Creature creature = allCreatures[index];
			if (creatures.Contains(creature)) {
				creature.ShowSelected(true);
				creature.StoreState();
			} else {
				creature.ShowSelected(false);
				creature.ShowCellsAndGeneCellsSelected(false);
			}
			if (creatures.Count > 1) {
				creature.ShowCellsAndGeneCellsSelected(false);
			}
		}

		selection.Clear();
		selection.AddRange(creatures);
		SelectedCellAndGene(null);

		UpdateGUI();
		UpdateGenotypePanel();
		UpdatePhenotypePanel();
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

		for (int index = 0; index < selection.Count; index++) {
			selection[index].ShowCellsAndGeneCellsSelected(false);
		}
		PhenotypePanel.instance.cell = null;

		creature.ShowSelected(true);
		selection.Add(creature);
		creature.StoreState();
		SelectedCellAndGene(null);
		UpdateGUI();

		UpdateGenotypePanel();
		UpdatePhenotypePanel();
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
		for (int index = 0; index < selection.Count; index++) {
			selection[index].ShowCellsAndGeneCellsSelected(false);
		}
		PhenotypePanel.instance.cell = null;

		creature.ShowSelected(false);
		selection.Remove(creature);
		SelectedCellAndGene(null);
		UpdateGUI();

		UpdateGenotypePanel();
		UpdatePhenotypePanel();

		//SelectDefaultGeneCell();
	}

	private void SelectedCellAndGene(Cell cell) {
		if (cell == null) {
			if (soloSelected == null) {
				return;
			}
			cell = soloSelected.phenotype.rootCell;
		}

		if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
			GenePanel.instance.gene = cell.gene;
			UpdateGenotypePanel();
		} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
			soloSelected.ShowCellSelected(cell, true);
			PhenotypePanel.instance.cell = cell;
			UpdatePhenotypePanel();
		}
	}

	private void SelectDefaultGeneCell() {
		Creature creature = CreatureSelectionPanel.instance.soloSelected;
		if (creature != null) {
			GenePanel.instance.gene = creature.genotype.geneCellList[0].gene;
			GenotypePanel.instance.genotype = creature.genotype;
		}
	}

	private void UpdateGenotypePanel() {
		if (selection.Count == 1) {
			GenotypePanel.instance.genotype = soloSelected.genotype;
		} else {
			GenotypePanel.instance.genotype = null;
		}
	}

	private void UpdatePhenotypePanel() {
		PhenotypePanel.instance.UpdateRepresentation();
	}

	private void UpdateGUI() {
		if (selection.Count == 0) {
			//gameObject.SetActive(false);
			selectedCreatureText.text = "";
			//phenotypePanel.gameObject.SetActive(false);
		} else if (selection.Count == 1) {
			//gameObject.SetActive(true);
			selectedCreatureText.text = soloSelected.nickname;
			//phenotypePanel.gameObject.SetActive(true);
		} else {
			//gameObject.SetActive(true);
			selectedCreatureText.text = selection.Count + " Creatures";
			//phenotypePanel.gameObject.SetActive(false);
		}
	}

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

	//Actions ------------------------------------------------------------------------
	//Separat class?


	// Delete
	public void OnDeleteClicked() {
		for (int index = 0; index < selection.Count; index++) {
			life.DeleteCreature(selection[index]);
		}
		ClearSelection();
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
			genomes.Add(source.genotype.genes);
		}

		if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
			Creature mergeling = World.instance.life.SpawnCreatureMergling(genomes, Vector2.zero, 90f, PhenoGenoEnum.Phenotype);
			moveCreatures.Add(mergeling);
		} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
			Creature mergeling = World.instance.life.SpawnCreatureMergling(genomes, Vector2.zero, 90f, PhenoGenoEnum.Genotype);
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
		foreach (Creature original in originals) {
			if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
				Creature copy = World.instance.life.SpawnCreatureCopy(original, PhenoGenoEnum.Phenotype);
				moveCreatures.Add(copy);
			} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
				Creature copy = World.instance.life.SpawnCreatureCopy(original, PhenoGenoEnum.Genotype);
				moveCreatures.Add(copy);
			}
		}
	}

	public void StartMoveCreatures() {
		Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
		if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
			foreach (Creature c in moveCreatures) {
				c.Grab(PhenoGenoEnum.Phenotype);
			}

			//Offset
			foreach (Creature c in moveCreatures) {
				moveOffset.Add(c, (Vector2)c.transform.position - MoveCreaturesCenterPhenotype);
			}
		} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
			foreach (Creature c in moveCreatures) {
				c.Grab(PhenoGenoEnum.Genotype);
			}
			foreach (Creature c in moveCreatures) {
				moveOffset.Add(c, (Vector2)c.transform.position - MoveCreaturesCenterGenotype);
			}
		}
	}

	private void Update() {

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

				if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
					c.phenotype.rootCell.heading = startCreatureHeading[c] + RotateCreaturesAngle;
				} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
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
		if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
			foreach (Creature c in moveCreatures) {
				c.Grab(PhenoGenoEnum.Phenotype);
			}

			foreach (Creature c in moveCreatures) {
				moveOffset.Add(c, (Vector2)c.transform.position - MoveCreaturesCenterPhenotype);
			}
			zeroRotationVector = (Vector2)mousePosition - MoveCreaturesCenterPhenotype;
			rotationCenter = MoveCreaturesCenterPhenotype;
		} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
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
		if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
			foreach (Creature c in moveCreatures) {
				c.Release(PhenoGenoEnum.Phenotype);
			}
		} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
			foreach (Creature c in moveCreatures) {
				c.Release(PhenoGenoEnum.Genotype);
			}
		}
	}
}