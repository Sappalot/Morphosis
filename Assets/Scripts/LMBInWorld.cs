using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LMBInWorld : MonoBehaviour {

	//Bevare of LMB Mouse Drag in selectionController
	private void OnMouseDown() {
		if (Input.GetKey("mouse 0") && !EventSystem.current.IsPointerOverGameObject()) {
			if (MouseAction.instance.actionState == MouseActionStateEnum.free) { 
				if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) {
					return;
				}
				CreatureSelectionPanel.instance.ClearSelection();
			} else if (MouseAction.instance.actionState == MouseActionStateEnum.moveCreatures || MouseAction.instance.actionState == MouseActionStateEnum.rotateCreatures) {
				CreatureSelectionPanel.instance.PlaceHoveringCreatures();
			} else if (MouseAction.instance.actionState == MouseActionStateEnum.copyMoveCreatures) {
				if (Input.GetKey(KeyCode.LeftControl)) {
					CreatureSelectionPanel.instance.PasteHoveringCreatures();
				} else {
					CreatureSelectionPanel.instance.PlaceHoveringCreatures();
				}
			}
		}
	}
}
