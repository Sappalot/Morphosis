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

	public Creature selectedCreature {
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
			selection[index].ShowCreatureSelected(false);
			selection[index].ShowCellsAndGeneCellsSelected(false);
		}
		selection.Clear();
		UpdateGUI();

		UpdateGenotypePanel();
		UpdatePhenotypePanel();
	}

	public void Select(Creature creature, Cell cell = null) {
		for (int index = 0; index < selection.Count; index++) {
			selection[index].ShowCreatureSelected(false);
			selection[index].ShowCellsAndGeneCellsSelected(false);
		}
		selection.Clear();

		creature.ShowCreatureSelected(true);
		selection.Add(creature);
		SelectedCellAndGene(cell);
		UpdateGUI();
	}

	public void Select(List<Creature> creatures) {
		List<Creature> allCreatures = life.creatures;
		for (int index = 0; index < allCreatures.Count; index++) {
			Creature creature = allCreatures[index];
			if (creatures.Contains(creature)) {
				creature.ShowCreatureSelected(true);
			} else {
				creature.ShowCreatureSelected(false);
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

	private void SelectedCellAndGene(Cell cell) {
		if (cell == null) {
			if (selectedCreature == null) {
				return;
			}
			cell = selectedCreature.phenotype.rootCell;
		}

		if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
			GenePanel.instance.gene = cell.gene;
			UpdateGenotypePanel();
		} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
			selectedCreature.ShowCellSelected(cell, true);
			PhenotypePanel.instance.cell = cell;
			UpdatePhenotypePanel();
		}
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

		creature.ShowCreatureSelected(true);
		selection.Add(creature);
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

		creature.ShowCreatureSelected(false);
		selection.Remove(creature);
		SelectedCellAndGene(null);
		UpdateGUI();

		UpdateGenotypePanel();
		UpdatePhenotypePanel();

		//SelectDefaultGeneCell();
	}

	private void SelectDefaultGeneCell() {
		Creature creature = CreatureSelectionPanel.instance.selectedCreature;
		if (creature != null) {
			GenePanel.instance.gene = creature.genotype.geneCellList[0].gene;
			GenotypePanel.instance.genotype = creature.genotype;
		}
	}

	private void UpdateGenotypePanel() {
		if (selection.Count == 1) {
			GenotypePanel.instance.genotype = selectedCreature.genotype;
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
			selectedCreatureText.text = selectedCreature.nickname;
			//phenotypePanel.gameObject.SetActive(true);
		} else {
			//gameObject.SetActive(true);
			selectedCreatureText.text = selection.Count + " Creatures";
			//phenotypePanel.gameObject.SetActive(false);
		}
	}

	public Vector2 SelectionPointOfWeightPhenotype {
		get	{
			Vector2 pow = Vector2.zero;
			foreach (Creature c in selection) {
				pow += (Vector2)c.phenotype.rootCell.position;
			}
			return pow / selectionCount;
		}
	}

	public Vector2 SelectionPointOfWeightGenotype {
		get	{
			Vector2 pow = Vector2.zero;
			foreach (Creature c in selection) {
				pow += (Vector2)c.genotype.rootCell.position;
			}
			return pow / selectionCount;
		}
	}

	//Actions


	// Delete
	public void OnDeleteClicked() {
		for (int index = 0; index < selection.Count; index++) {
			life.DeleteCreature(selection[index]);
		}
		ClearSelection();
	}

	// Move
	private Dictionary<Creature, Vector2> moveOffset = new Dictionary<Creature, Vector2>();
	public void OnMoveClicked() {
		if (!hasSelection) {
			return;
		}
		Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
		if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
			foreach (Creature c in selection) {
				c.Grab(PhenotypeGenotypeEnum.Phenotype);
			}

			//Offset
			foreach (Creature c in selection) {
				moveOffset.Add(c, (Vector2)c.transform.position - SelectionPointOfWeightPhenotype);
			}
		} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
			foreach (Creature c in selection) {
				c.Grab(PhenotypeGenotypeEnum.Genotype);
			}
			foreach (Creature c in selection) {
				moveOffset.Add(c, (Vector2)c.transform.position - SelectionPointOfWeightGenotype);
			}
		}

		MouseAction.instance.actionState = MouseActionStateEnum.moveCreatures;
	}

	//Rotate
	private Vector2 taraRotationVector;
	private Vector2 rotationCenter;
	private Dictionary<Creature, float> startCreatureHeading = new Dictionary<Creature, float>();

	public void OnRotateClicked() {
		if (!hasSelection) {
			return;
		}

		Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
		if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
			foreach (Creature c in selection) {
				c.Grab(PhenotypeGenotypeEnum.Phenotype);
			}

			foreach (Creature c in selection) {
				moveOffset.Add(c, (Vector2)c.transform.position - SelectionPointOfWeightPhenotype);
			}
			taraRotationVector = (Vector2)mousePosition - SelectionPointOfWeightPhenotype;
			rotationCenter = SelectionPointOfWeightPhenotype;
		} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
			foreach (Creature c in selection) {
				c.Grab(PhenotypeGenotypeEnum.Genotype);
			}
			foreach (Creature c in selection) {
				moveOffset.Add(c, (Vector2)c.transform.position - SelectionPointOfWeightGenotype);
			}

			taraRotationVector = (Vector2)mousePosition - SelectionPointOfWeightGenotype;
			rotationCenter = SelectionPointOfWeightGenotype;

			foreach (Creature c in selection) {
				startCreatureHeading.Add(c, c.genotype.rootCell.heading);
			}
		}
		lineRenderer.GetComponent<LineRenderer>().SetPosition(1, mousePosition);
		lineRenderer.GetComponent<LineRenderer>().SetPosition(0, rotationCenter);
		lineRenderer.enabled = true;

		MouseAction.instance.actionState = MouseActionStateEnum.rotateCreatures;
	}

	float angle;

	private void Update() {
		//Keys
		if (Input.GetKeyDown(KeyCode.M)) {
			OnMoveClicked();
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			OnRotateClicked();
		}
		if (Input.GetKeyDown(KeyCode.Delete)) {
			OnDeleteClicked();
		}

		Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;

		//move
		if (MouseAction.instance.actionState == MouseActionStateEnum.moveCreatures) {
			foreach (Creature c in selection) {
				c.transform.position = (Vector2)mousePosition + moveOffset[c];
			}
		}

		//rotate
		if (MouseAction.instance.actionState == MouseActionStateEnum.rotateCreatures) {
			lineRenderer.GetComponent<LineRenderer>().SetPosition(1, mousePosition);
			lineRenderer.GetComponent<LineRenderer>().SetPosition(0, rotationCenter);

			angle = Vector2.SignedAngle(taraRotationVector, (Vector2)mousePosition - rotationCenter);

			foreach (Creature c in selection) {
				//Rotate around main center
				Vector3 turnedVector = Quaternion.Euler(0, 0, angle) * moveOffset[c];
				c.transform.position = (Vector2)rotationCenter + (Vector2)turnedVector;

				//rotate around own center
				c.transform.localRotation = Quaternion.Euler(0, 0, angle);

				if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
					c.genotype.rootCell.heading = startCreatureHeading[c] + angle;
				}
			}
		}

		//debug markers
		foreach (Creature c in World.instance.life.creatures) {
			c.ShowMarkers(IsSelected(c));
		}
	}

	public void PlaceHoveringCreatures() {
		if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
			foreach (Creature c in selection) {
				c.Release(PhenotypeGenotypeEnum.Phenotype);
			}
		} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
			foreach (Creature c in selection) {
				c.Release(PhenotypeGenotypeEnum.Genotype);
			}
		}

		//rotate
		lineRenderer.enabled = false;
		startCreatureHeading.Clear();

		//Offset
		moveOffset.Clear();

		MouseAction.instance.actionState = MouseActionStateEnum.free;
	}
}