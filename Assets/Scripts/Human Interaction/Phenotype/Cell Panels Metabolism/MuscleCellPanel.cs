using UnityEngine;
using UnityEngine.UI;

public class MuscleCellPanel : MetabolismCellPanel {

	//public Text exposure;

	public Text idleWhenAttachedText;
	public Toggle idleWhenAttachedToggle;

	public void OnIdleWhenAttachedToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.muscleCellIdleWhenAttached = idleWhenAttachedToggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (mode == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					idleWhenAttachedText.color = ColorScheme.instance.grayedOutGenotype;
					idleWhenAttachedToggle.interactable = false;
				}
			} else if (mode == PhenoGenoEnum.Genotype) {
				idleWhenAttachedText.color = Color.black;
				idleWhenAttachedToggle.interactable = isUnlocked();
			}


			if (selectedGene != null) {
				ignoreSliderMoved = true;

				idleWhenAttachedToggle.isOn = selectedGene.muscleCellIdleWhenAttached;

				ignoreSliderMoved = false;
			}

			isDirty = false; 
		}
	}
}
