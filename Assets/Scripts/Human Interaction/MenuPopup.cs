using UnityEngine;

public class MenuPopup : MonoBehaviour {

	public RectTransform popup1;
	public Transform popup2;

	public void OnButtonClick() {
		if (popup1 != null) {
			popup1.gameObject.SetActive(!popup1.gameObject.activeSelf);
		}
		if (popup2 != null) {
			popup2.gameObject.SetActive(!popup2.gameObject.activeSelf);
		}
	}
}
