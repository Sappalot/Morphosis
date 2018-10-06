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
			} else if (MouseAction.instance.actionState == MouseActionStateEnum.moveCreatures
				|| MouseAction.instance.actionState == MouseActionStateEnum.rotateCreatures) {

				List<Creature> creatures = CreatureSelectionPanel.instance.PlaceHoveringCreatures();
				UpdateCreaturePostPlaced(creatures, true);
			} else if (MouseAction.instance.actionState == MouseActionStateEnum.copyMoveCreatures) {
				List<Creature> creatures;
				if (Input.GetKey(KeyCode.LeftControl)) {
					creatures = CreatureSelectionPanel.instance.PasteHoveringCreatures();
				} else {
					creatures = CreatureSelectionPanel.instance.PlaceHoveringCreatures();
				}
				UpdateCreaturePostPlaced(creatures, true);
			} else if (MouseAction.instance.actionState == MouseActionStateEnum.combineMoveCreatures) {
				List<Creature> creatures;
				if (Input.GetKey(KeyCode.LeftControl)) {
					creatures = CreatureSelectionPanel.instance.PasteHoveringMergling();
				} else {
					creatures = CreatureSelectionPanel.instance.PlaceHoveringCreatures();
				}
				UpdateCreaturePostPlaced(creatures, false);
			}
		}
	}

	private void UpdateCreaturePostPlaced(List<Creature> creatures, bool canFreeze) {
		foreach (Creature c in creatures) {
			if (TerrainPerimeter.instance.IsCompletelyInside(c)) {
				if (c.creation == CreatureCreationEnum.Frozen) {
					c.Defrost();
				} 
			} else if (Freezer.instance.IsCompletelyInside(c) && canFreeze) {
				if (c.creation != CreatureCreationEnum.Frozen) {
					c.Freeze();
				}
			} else {
				World.instance.life.KillCreatureSafe(c, true); // creature placed in the void
			}
		}
	}
}
