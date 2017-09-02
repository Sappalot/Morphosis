using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

public class CreatureSelectionController : MouseDrag {

	public Camera camera;
	public Transform rectangle;

	private Vector3 dragVector = new Vector3();
	private Vector3 downPositionMouse; //World space
	[HideInInspector]
	public bool isDragging = false;

	public override void OnDraggingStart(int mouseButton) {
		// implement this for start of dragging
		if (mouseButton == 0 && !EventSystem.current.IsPointerOverGameObject()) {
			World.instance.life.ClearSelectionRectangleCells();

			downPositionMouse = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
			rectangle.transform.localScale = new Vector3(0.1f, 0.1f, 0f);
			isDragging = true;
			rectangle.gameObject.SetActive(true);
			//Debug.Log("MouseButton @ " + downPositionMouse);
		}
	}

	public override void OnDragging(int mouseButton) {
		// implement this for dragging
		if (mouseButton == 0 && isDragging) {
			Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
			rectangle.transform.position = (downPositionMouse + mousePosition) / 2f;
			rectangle.transform.localScale = new Vector3(Mathf.Abs(downPositionMouse.x - mousePosition.x), Mathf.Abs(downPositionMouse.y - mousePosition.y), 0f);
;			//Debug.Log("MouseButton" + mouseButton + " Dragging, Position: " + rectangle.transform.position);
		}
	}

	public override void OnDraggingEnd(int mouseButton) {
		// implement this for end of dragging
		if (mouseButton == 0) {
			isDragging = false;

			rectangle.gameObject.SetActive(false);
		}
	}
}