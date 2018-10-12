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
				&& CreatureSelectionPanel.instance.CanPlaceMoveCreatures(CreatureSelectionPanel.MoveCreatureType.Move, Input.GetKey(KeyCode.LeftControl))) {
				List<Creature> creatures = CreatureSelectionPanel.instance.PlaceHoveringCreatures();
				// TODO: if moved within freezer save feezer
			} else if (MouseAction.instance.actionState == MouseActionStateEnum.copyMoveCreatures
				&& CreatureSelectionPanel.instance.CanPlaceMoveCreatures(CreatureSelectionPanel.MoveCreatureType.Copy, Input.GetKey(KeyCode.LeftControl))) {
				List<Creature> creatures;
				if (Input.GetKey(KeyCode.LeftControl)) {
					creatures = CreatureSelectionPanel.instance.PasteHoveringCreatures();
				} else {
					creatures = CreatureSelectionPanel.instance.PlaceHoveringCreatures();
					if (creatures.Count == 1) {
						if (TryFreezeCreature(creatures[0])) { // should just be 1 at this point
							Freezer.instance.Save();
						}
						TryDefrostCreature(creatures[0]);
					}
				}
			} else if (MouseAction.instance.actionState == MouseActionStateEnum.combineMoveCreatures
				&& CreatureSelectionPanel.instance.CanPlaceMoveCreatures(CreatureSelectionPanel.MoveCreatureType.Combine, Input.GetKey(KeyCode.LeftControl))) {
				List<Creature> creatures;
				if (Input.GetKey(KeyCode.LeftControl)) {
					creatures = CreatureSelectionPanel.instance.PasteHoveringMergling();
				} else {
					creatures = CreatureSelectionPanel.instance.PlaceHoveringCreatures();
				}
			}
		}
	}

	//TODO move to some util
	static bool TryFreezeCreature(Creature creature) {
		if (Freezer.instance.IsCompletelyInside(creature) && creature.creation != CreatureCreationEnum.Frozen) {
			creature.OnFreeze();
			World.instance.life.RemoveCreature(creature);
			Freezer.instance.AddCreature(creature);
			return true;
		}
		return false;
	}

	static bool TryDefrostCreature(Creature creature) {
		if (TerrainPerimeter.instance.IsCompletelyInside(creature) && creature.creation == CreatureCreationEnum.Frozen) {
			creature.OnDefrost();
			Freezer.instance.RemoveCreature(creature);
			World.instance.life.AddCreature(creature);
			return true;
		}
		return false;
	}

	private void UpdateCreaturePostPlaced(List<Creature> creatures, bool canFreeze, bool canDefrost) {
		foreach (Creature c in creatures) {
			if (TerrainPerimeter.instance.IsCompletelyInside(c)) {
				if (c.creation == CreatureCreationEnum.Frozen) {
					if (canDefrost) {
						c.OnDefrost();
					}
				}
			} else if (Freezer.instance.IsCompletelyInside(c)) {
				if (c.creation != CreatureCreationEnum.Frozen) {
					if (canFreeze) {
						c.OnFreeze();
					}
				}
			} 
		}
	}
}