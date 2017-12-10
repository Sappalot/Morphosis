using UnityEngine;

public class MenuPopup : MonoBehaviour {

	public RectTransform popupPanel;

	public void OnButtonClick() {
		popupPanel.gameObject.SetActive(!popupPanel.gameObject.activeSelf);
	}
}
