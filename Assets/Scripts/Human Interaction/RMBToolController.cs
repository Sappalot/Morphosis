using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RMBToolController : MouseDrag {
	public new Camera camera;
	public SpringJoint2D spring;

	private Vector3 dragVector = new Vector3();
	private Vector3 downPositionMouse; //World space

	private List<Creature> alreadySelected;

	public override void OnDraggingStart(int mouseButton) {
		// implement this for start of dragging
		if (mouseButton == 1 && CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype && !EventSystem.current.IsPointerOverGameObject()) {
			if (RMBToolModePanel.instance.toolMode == RMBToolModePanel.RMBToolMode.spring) {
				downPositionMouse = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
				Cell cell = World.instance.life.GetCellAt(downPositionMouse);
				Debug.Log("Found: " + cell);
				if (cell != null) {
					spring.connectedBody = cell.GetComponent<Rigidbody2D>();
					spring.anchor = downPositionMouse;
					spring.distance = 0f;
					spring.GetComponent<LineRenderer>().SetPosition(1, downPositionMouse);
					spring.GetComponent<LineRenderer>().SetPosition(0, spring.connectedBody.transform.position);
					spring.GetComponent<LineRenderer>().enabled = true;
				}
			}
		}
	}

	public override void OnDragging(int mouseButton) {
		// implement this for dragging
		if (mouseButton == 1 && spring.connectedBody != null) {
			Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
			spring.anchor = mousePosition;
			spring.GetComponent<LineRenderer>().SetPosition(1, mousePosition);
			spring.GetComponent<LineRenderer>().SetPosition(0, spring.connectedBody.transform.position);
		}
	}

	public override void OnDraggingEnd(int mouseButton) {
		// implement this for end of dragging
		if (mouseButton == 1) {
			spring.connectedBody = null;
			spring.GetComponent<LineRenderer>().enabled = false;
		}
	}
}
