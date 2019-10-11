using UnityEngine;
using UnityEngine.UI;

public class FungalCellPanel : CellAndGeneComponentPanel {

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			footerPanel.SetProductionEffectText(0f, GlobalSettings.instance.phenotype.fungalCellEffectCost);

			isDirty = false; 
		}
	}
}
