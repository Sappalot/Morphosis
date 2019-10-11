using UnityEngine;
using UnityEngine.UI;

public class RootCellPanel : CellAndGeneComponentPanel {

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			footerPanel.SetProductionEffectText(0f, GlobalSettings.instance.phenotype.rootCellEffectCost);

			isDirty = false; 
		}
	}
}
