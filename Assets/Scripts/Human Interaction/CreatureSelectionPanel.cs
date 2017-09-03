using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureSelectionPanel : MonoSingleton<CreatureSelectionPanel> {
	public Life life;
	public PhenotypePanel phenotypePanel;

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

	public bool IsSelected(Creature creature) {
		return selection.Contains(creature);
	}

	public override void Init() {
		base.Init();
		selection = new List<Creature>();

		ClearSelection();
	}

	public void ClearSelection() {
		for (int index = 0; index < selection.Count; index++) {
			selection[index].ShowCreatureSelected(false);
			selection[index].ShowCellsSelected(false);
		}
		selection.Clear();
		UpdateGUI();

		UpdateGenotypePanel();
		UpdatePhenotypePanel();
	}

	public void SelectOnly(Creature creature, Cell cell = null) {
		for (int index = 0; index < selection.Count; index++) {
			selection[index].ShowCreatureSelected(false);
			selection[index].ShowCellsSelected(false);
		}
		selection.Clear();

		creature.ShowCreatureSelected(true);
		selection.Add(creature);
		UpdateGUI();
		if (cell != null) {
			if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
				GenePanel.instance.gene = cell.gene;
				UpdateGenotypePanel();
			} else if(CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
				creature.ShowCellSelected(cell, true);
				PhenotypePanel.instance.cell = cell;
				UpdatePhenotypePanel();
			}
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
			selection[index].ShowCellsSelected(false);
		}
		PhenotypePanel.instance.cell = null;

		creature.ShowCreatureSelected(true);
		selection.Add(creature);
		UpdateGUI();

		UpdateGenotypePanel();
		UpdatePhenotypePanel();
	}

	public void RemoveFromSelection(Creature creature) {
		for (int index = 0; index < selection.Count; index++) {
			selection[index].ShowCellsSelected(false);
		}
		PhenotypePanel.instance.cell = null;

		creature.ShowCreatureSelected(false);
		selection.Remove(creature);
		UpdateGUI();

		UpdateGenotypePanel();
		UpdatePhenotypePanel();

		SelectDefaultGeneCell();
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

	//Buttons
	public void OnDeleteClicked() {
		for (int index = 0; index < selection.Count; index++) {
			life.DeleteCreature(selection[index]);
		}
		ClearSelection();
	}
}
