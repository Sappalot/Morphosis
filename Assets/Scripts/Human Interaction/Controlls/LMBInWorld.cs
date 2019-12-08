using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LMBInWorld : MonoBehaviour {
	public Camera cameraVirtual;

	private bool mouseDown0;

	private float doubleClickCooldown;

	//Bevare of LMB Mouse Drag in selectionController
	private void Update() {

		// Haxor double click :D
		doubleClickCooldown -= Time.unscaledDeltaTime;
		if (Input.GetMouseButtonDown(0) && doubleClickCooldown > 0f) {
			ViewSelectedCreaturePanel.instance.OnPressedViewAllSelectedCreatures();
			doubleClickCooldown = 0f;
			return;
		}
		// just after double click
		if (doubleClickCooldown > -0.05f) {
			return;
		}

		if (Input.GetKey("mouse 0") && !mouseDown0) {
			mouseDown0 = true;


			if (!EventSystem.current.IsPointerOverGameObject() && !GraphPlotter.instance.IsMouseInside() && !AlternativeToolModePanel.instance.isOn) {
				if (MouseAction.instance.actionState == MouseActionStateEnum.free) {
					if (Input.GetKey(KeyCode.LeftShift)) {
						return;
					}
					Vector2 pickPosition = cameraVirtual.ScreenToWorldPoint(Input.mousePosition);
					Cell cell = Morphosis.instance.GetCellAtPosition(pickPosition);

					if (cell == null) {
						if (!Input.GetKey(KeyCode.LeftControl)) {
							if (PhenotypePanel.instance.followToggle.isOn && CreatureSelectionPanel.instance.hasSoloSelected) {
								World.instance.cameraController.TurnCameraStraightAtCameraUnlock(); // but don't remove lock
							}

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
							//if (!PhenotypePanel.instance.followToggle.isOn) {
							CreatureSelectionPanel.instance.RemoveFromSelection(creature, true);
							//}
						} else {
							//if (!PhenotypePanel.instance.followToggle.isOn) {
							CreatureSelectionPanel.instance.AddToSelection(creature, true);
							//}

						}
					} else {
						CreatureSelectionPanel.instance.Select(creature, cell);
						GenePanel.instance.cellAndGenePanel.geneNeighboursPanel.MakeDirty();
						GenomePanel.instance.MakeDirty();
						GenomePanel.instance.MakeScrollDirty();
						CreatureSelectionPanel.instance.soloSelected.MakeDirtyGraphics();

						doubleClickCooldown = 0.4f; // for double click
					}

				} else if ((MouseAction.instance.actionState == MouseActionStateEnum.moveCreatures || MouseAction.instance.actionState == MouseActionStateEnum.rotateCreatures)
					&& CreatureSelectionPanel.instance.CanPlaceMoveCreatures(CreatureSelectionPanel.MoveCreatureType.Move, Input.GetKey(KeyCode.LeftControl))) {
					
					Audio.instance.CreaturePlace(1f);
					CreatureSelectionPanel.instance.PlaceHoveringCreatures();

				} else if (MouseAction.instance.actionState == MouseActionStateEnum.copyMoveCreatures
					&& CreatureSelectionPanel.instance.CanPlaceMoveCreatures(CreatureSelectionPanel.MoveCreatureType.Copy, Input.GetKey(KeyCode.LeftControl))) {
					List<Creature> creatures;
					if (Input.GetKey(KeyCode.LeftControl)) {
						creatures = CreatureSelectionPanel.instance.PasteHoveringCreatures();
						EffectsUtils.SpawnAddCreatures(creatures);
						HistoryUtil.SpawnAddCreatureEvent(creatures.Count);
					} else {
						creatures = CreatureSelectionPanel.instance.PlaceHoveringCreatures();
						EffectsUtils.SpawnAddCreatures(creatures);

						if (creatures.Count == 1) {
							TryFreezeCreature(creatures[0]);
							TryDefrostCreature(creatures[0]);
						}
						foreach (Creature c in creatures) {
							World.instance.AddHistoryEvent(new HistoryEvent("+", false, c.creation == CreatureCreationEnum.Frozen ? Color.blue : Color.gray));
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
					EffectsUtils.SpawnAddCreatures(creatures);
					HistoryUtil.SpawnAddCreatureEvent(creatures.Count);
				}
			}
		}
		if (!Input.GetKey("mouse 0")) {
			mouseDown0 = false;
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