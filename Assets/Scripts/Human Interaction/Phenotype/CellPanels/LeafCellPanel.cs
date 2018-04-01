using UnityEngine;
using UnityEngine.UI;

public class LeafCellPanel : MonoSingleton<LeafCellPanel> {

	public Text exposure;

	private bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (CellPanel.instance.selectedCell != null) {
				exposure.text = string.Format("Expo: {0:F2}%", (CellPanel.instance.selectedCell as LeafCell).lowPassExposure * 100f);

			}

			isDirty = false;
		}
	}
}
