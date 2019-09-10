using UnityEngine;
using UnityEngine.UI;

public class RootCellPanel : CellComponentPanel {

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			isDirty = false; 
		}
	}
}
