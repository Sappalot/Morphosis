using UnityEngine;
using UnityEngine.UI;

public class GeneCellPanel : MonoSingleton<GeneCellPanel> {

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
	}

	private void Update() {
		if (isDirty) {
			

			isDirty = false;
		}
	}
}