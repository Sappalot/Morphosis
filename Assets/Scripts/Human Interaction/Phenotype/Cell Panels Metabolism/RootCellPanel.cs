using UnityEngine;
using UnityEngine.UI;

public class RootCellPanel : MetabolismCellPanel {

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			isDirty = false; 
		}
	}
}
