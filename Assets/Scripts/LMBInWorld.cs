using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LMBInWorld : MonoBehaviour {
	public new Camera camera;

	//Bevare of LMB Mouse Drag in selectionController
	private void OnMouseDown() {
		if (Input.GetKey("mouse 0") && !EventSystem.current.IsPointerOverGameObject()) {
			if (MouseAction.instance.actionState == MouseActionStateEnum.free) { 
				if (Input.GetKey(KeyCode.LeftShift)) {
					return;
				}
				Vector2 pickPosition = camera.ScreenToWorldPoint(Input.mousePosition);
				Cell cell = Morphosis.instance.GetCellAtPosition(pickPosition);

				if (cell == null) {
					if (!Input.GetKey(KeyCode.LeftControl)) {
						CreatureSelectionPanel.instance.ClearSelection();
					}
					return;
				}
				Creature creature = cell.creature;

				if (Input.GetKey(KeyCode.LeftControl)) {
					if (creature.creation == CreatureCreationEnum.Frozen ||
						(CreatureSelectionPanel.instance.hasSelection && CreatureSelectionPanel.instance.GetSelectionTemperatureState() == CreatureSelectionPanel.TemperatureState.Frozen)) {
						return;
					}
					if (CreatureSelectionPanel.instance.IsSelected(creature)) {
						CreatureSelectionPanel.instance.RemoveFromSelection(creature);
					} else {
						CreatureSelectionPanel.instance.AddToSelection(creature);
					}
				} else {
					CreatureSelectionPanel.instance.Select(creature, cell);
					GeneNeighboursPanel.instance.MakeDirty();
					GenomePanel.instance.MakeDirty();
					GenomePanel.instance.MakeScrollDirty();
					CreatureSelectionPanel.instance.soloSelected.MakeDirtyGraphics();
				}

			} else if ((MouseAction.instance.actionState == MouseActionStateEnum.moveCreatures || MouseAction.instance.actionState == MouseActionStateEnum.rotateCreatures)
				&& CreatureSelectionPanel.instance.CanPlaceMoveCreatures(CreatureSelectionPanel.MoveCreatureType.Move, Input.GetKey(KeyCode.LeftControl))) {
				List<Creature> creatures = CreatureSelectionPanel.instance.PlaceHoveringCreatures();

			} else if (MouseAction.instance.actionState == MouseActionStateEnum.copyMoveCreatures
				&& CreatureSelectionPanel.instance.CanPlaceMoveCreatures(CreatureSelectionPanel.MoveCreatureType.Copy, Input.GetKey(KeyCode.LeftControl))) {
				List<Creature> creatures;
				if (Input.GetKey(KeyCode.LeftControl)) {
					creatures = CreatureSelectionPanel.instance.PasteHoveringCreatures();
				} else {
					creatures = CreatureSelectionPanel.instance.PlaceHoveringCreatures();
					if (creatures.Count == 1) {
						TryFreezeCreature(creatures[0]);
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
			World.instance.life.RemoveCreature(creature);
			Freezer.instance.AddCreature(creature);
			creature.OnFreeze();
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
}