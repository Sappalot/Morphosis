﻿public class GenePanel : MonoSingleton<GenePanel> {

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