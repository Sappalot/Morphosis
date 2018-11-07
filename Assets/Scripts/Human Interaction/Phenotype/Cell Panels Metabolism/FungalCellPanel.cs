using UnityEngine;
using UnityEngine.UI;

public class FungalCellPanel : MetabolismCellPanel {

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			isDirty = false; 
		}
	}
}
