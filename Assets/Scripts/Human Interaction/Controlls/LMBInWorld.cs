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
				Vector2 pickPosition = cameraVirtual.ScreenToWorldPoint(Input.mousePosition);
				Cell cellClicked = Morphosis.instance.GetCellAtPosition(pickPosition);

				if (MouseAction.instance.actionState == MouseActionStateEnum.free) {
					if (Input.GetKey(KeyCode.LeftShift)) {
						return;
					}


					if (cellClicked == null) {
						if (!Input.GetKey(KeyCode.LeftControl)) {
							if (PhenotypePanel.instance.followToggle.isOn && CreatureSelectionPanel.instance.hasSoloSelected) {
								World.instance.cameraController.TurnCameraStraightAtCameraUnlock(); // but don't remove lock
							}

							CreatureSelectionPanel.instance.ClearSelection();
						}
						return;
					}
					Creature creature = cellClicked.creature;

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
						CreatureSelectionPanel.instance.Select(creature, cellClicked);
						GenePanel.instance.cellAndGenePanel.geneNeighboursPanel.MakeDirty();
						GenomePanel.instance.MakeDirty();
						GenomePanel.instance.MakeScrollDirty();
						CreatureSelectionPanel.instance.soloSelected.MakeDirtyGraphics();

						doubleClickCooldown = 0.4f; // for double click
					}

				} else if (MouseAction.instance.actionState == MouseActionStateEnum.selectSignalOutput) {
					// We have been pressing a signal input in the genotype panel and are about to assign an input to this nerve
					if (cellClicked == null) {
						Debug.Log("...in the void");
					} else {
						if (AssignNerveInputPanel.instance.selectedRootCellMapPosition == null) {
							if (AssignNerveInputPanel.instance.TrySetNarrowedGeneCellMapPosition(cellClicked.mapPosition)) {
								Debug.Log("RootGeneCell narrowed down");
							} else {
								Debug.Log("You must select one of the Gene cells containing the selected gene!");
							}
						} else {
							// TODO: show nerves as we move mouse from cell to cell

							// click will select this geneCell as the one we want to listen to
							if (AssignNerveInputPanel.instance.TrySetNerveInputMapPositionExternally(cellClicked.mapPosition)) {
								Debug.Log("(RootGeneCell was allready set) Source geneCell selected");
							} else {
								Debug.Log("You must select an extarnal gene cell as an input source!");
							}
						}
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

		// siece updating if we are in the state to select output inside of gene
		if (!AssignNerveInputPanel.instance.isInAuxternalGene && MouseAction.instance.actionState == MouseActionStateEnum.selectSignalOutput && AssignNerveInputPanel.instance.selectedRootCellMapPosition != null) {
			Vector2 pickPosition = cameraVirtual.ScreenToWorldPoint(Input.mousePosition);
			Cell newCellHover = Morphosis.instance.GetCellAtPosition(pickPosition);
			if (newCellHover != null && newCellHover.creature == CreatureSelectionPanel.instance.soloSelected) {
				// a cell in the creature which we have selected
				if (newCellHover.mapPosition != cellHoverMapPosition) {
					cellHoverMapPosition = newCellHover.mapPosition;
					
					AssignNerveInputPanel.instance.ShowNerveInputMapPositionExternally(newCellHover.mapPosition);
				}
			} else {
				AssignNerveInputPanel.instance.ShowNerveInputMapPositionExternally(null);
			}
		}
	}

	private Vector2i cellHoverMapPosition;


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