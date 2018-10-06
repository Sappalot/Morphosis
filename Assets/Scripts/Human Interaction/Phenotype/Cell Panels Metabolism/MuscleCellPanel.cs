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

		GenePanel.instance.selectedGene.muscleCellIdleWhenAttached = idleWhenAttachedToggle.isOn;
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
					//exposure.text = string.Format("Exposure: {0:F2}%", (CellPanel.instance.selectedCell as LeafCell).lowPassExposure * 100f);
					//exposure.color = Color.black;

					idleWhenAttachedText.color = Color.gray;
					idleWhenAttachedToggle.interactable = false;
				}
			} else if (mode == PhenoGenoEnum.Genotype) {
				//exposure.text = "Exposure : -";
				//exposure.color = Color.gray;

				idleWhenAttachedText.color = Color.black;
				idleWhenAttachedToggle.interactable = isUnlocked();
			}


			if (GenePanel.instance.selectedGene != null) {
				ignoreSliderMoved = true;

				idleWhenAttachedToggle.isOn = GenePanel.instance.selectedGene.muscleCellIdleWhenAttached;

				ignoreSliderMoved = false;
			}

			isDirty = false; 
		}
	}
}
