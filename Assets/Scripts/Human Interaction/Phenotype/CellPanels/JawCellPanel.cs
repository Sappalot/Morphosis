using UnityEngine;
using UnityEngine.UI;

public class JawCellPanel : MonoSingleton<JawCellPanel> {

	public Text prayCellCount;

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
				prayCellCount.text = "Pray count: " + (CellPanel.instance.selectedCell as JawCell).mouth.prayCount;
			}

			isDirty = false;
		}
	}
}
