using UnityEngine;
using UnityEngine.UI;

public class FungalCellPanel : ComponentPanel {

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			componentFooterPanel.SetProductionEffectText(0f, GlobalSettings.instance.phenotype.fungalCell.effectProductionDown);

			isDirty = false; 
		}
	}
}
