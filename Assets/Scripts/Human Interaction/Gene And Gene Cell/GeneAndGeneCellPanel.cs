using UnityEngine;

// Will this one ever be nessesary??
public class GeneAndGeneCellPanel : MonoBehaviour {

	private bool isDirty;

	public void MakeDirty() {
		isDirty = true;
	}

	// Update is called once per frame
	void Update() {
		if (isDirty) {

			Debug.Log("Updating arrows");

			isDirty = false;
		}
	}
}
