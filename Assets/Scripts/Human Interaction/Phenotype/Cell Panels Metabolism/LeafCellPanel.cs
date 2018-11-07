using UnityEngine;
using UnityEngine.UI;

public class LeafCellPanel : MetabolismCellPanel {

	public Text exposure;

	public Text idleWhenAttachedText;
	public Toggle idleWhenAttachedToggle;

	public void OnIdleWhenAttachedToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.leafCellIdleWhenAttached = idleWhenAttachedToggle.isOn;
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
					exposure.text = string.Format("Exposure: {0:F2}%", (CellPanel.instance.selectedCell as LeafCell).lowPassExposure * 100f);
					exposure.color = Color.black;

					idleWhenAttachedText.color = ColorScheme.instance.grayedOutGenotype;
					idleWhenAttachedToggle.interactable = false;
				}
			} else if (mode == PhenoGenoEnum.Genotype) {
				exposure.text = "Exposure : -";
				exposure.color = ColorScheme.instance.grayedOutPhenotype;

				idleWhenAttachedText.color = Color.black;
				idleWhenAttachedToggle.interactable = isUnlocked();
			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;

				idleWhenAttachedToggle.isOn = selectedGene.leafCellIdleWhenAttached;

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}
