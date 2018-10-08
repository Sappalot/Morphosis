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
			} else if ((MouseAction.instance.actionState == MouseActionStateEnum.moveCreatures || MouseAction.instance.actionState == MouseActionStateEnum.rotateCreatures)
				&& CreatureSelectionPanel.instance.CanPlaceMoveCreatures(CreatureSelectionPanel.MoveCreatureType.Move)) {
				List<Creature> creatures = CreatureSelectionPanel.instance.PlaceHoveringCreatures();

			} else if (MouseAction.instance.actionState == MouseActionStateEnum.copyMoveCreatures
				&& CreatureSelectionPanel.instance.CanPlaceMoveCreatures(CreatureSelectionPanel.MoveCreatureType.Copy)) {
				List<Creature> creatures;
				if (Input.GetKey(KeyCode.LeftControl)) {
					creatures = CreatureSelectionPanel.instance.PasteHoveringCreatures();
				} else {
					creatures = CreatureSelectionPanel.instance.PlaceHoveringCreatures();
				}
				
			} else if (MouseAction.instance.actionState == MouseActionStateEnum.combineMoveCreatures
				&& CreatureSelectionPanel.instance.CanPlaceMoveCreatures(CreatureSelectionPanel.MoveCreatureType.Combine)) {
				List<Creature> creatures;
				if (Input.GetKey(KeyCode.LeftControl)) {
					creatures = CreatureSelectionPanel.instance.PasteHoveringMergling();
				} else {
					creatures = CreatureSelectionPanel.instance.PlaceHoveringCreatures();
				}
			}
		}
	}



	//private void UpdateCreaturePostPlaced(List<Creature> creatures, bool canFreeze, bool canDefrost) {
	//	foreach (Creature c in creatures) {
	//		if (TerrainPerimeter.instance.IsCompletelyInside(c)) {
	//			if (c.creation == CreatureCreationEnum.Frozen) {
	//				if (canDefrost) {
	//					c.Defrost();
	//				} else {
	//					World.instance.life.KillCreatureSafe(c, true); // should be killed from freezer instead
	//				}
	//			} 
	//		} else if (Freezer.instance.IsCompletelyInside(c)) {
	//			if (c.creation != CreatureCreationEnum.Frozen) {
	//				if (canFreeze) {
	//					c.Freeze();
	//				} else {
	//					World.instance.life.KillCreatureSafe(c, true);
	//				}
	//			}
	//		} else {
	//			World.instance.life.KillCreatureSafe(c, true); // Where was it picked from?? Kill it there!!
	//		}
	//	}
	//}
}