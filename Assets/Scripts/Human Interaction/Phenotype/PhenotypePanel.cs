﻿using UnityEngine;
using UnityEngine.UI;

public class PhenotypePanel : MonoSingleton<PhenotypePanel> {
	public Text creatureAge;
	public Text creatureCellCount;
	public Text creatureEnergy;
	public CellPanel cellPanel;


	public void OnGrowClicked() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.TryGrow();
		}
	}

	public void OnShrinkClicked() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.TryShrink();
			DirtyMark();
		}
	}

	public void DirtyMark() {
		isDirty = true;
	}

	private bool isDirty = true;
	private void Update() {
		if (isDirty) {
			Debug.Log("Update");
			//Nothing to represent
			Creature solo = CreatureSelectionPanel.instance.soloSelected;
			if (solo == null) {
				creatureAge.text = "Age:";
				creatureCellCount.text = "Cells: ";
				creatureEnergy.text = "Energy:";

				isDirty = false;
				return;
			}

			creatureAge.text = "Age: 100 days";
			creatureCellCount.text = "Cells: " + solo.cellsCount + " (" + solo.cellsTotalCount + ")";
			creatureEnergy.text = "Energy: 100%";

			isDirty = false;
		}
	}
}