using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RMBToolController : MouseDrag {
	public new Camera camera;
	public SpringJoint2D spring;

	private Vector3 downPositionMouse; //World space

	public override void OnDraggingStart(int mouseButton) {
		// implement this for start of dragging
		if (mouseButton == 1 && !EventSystem.current.IsPointerOverGameObject()) {
			downPositionMouse = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
			if (RMBToolModePanel.instance.toolMode == RMBToolModePanel.RMBToolMode.spring && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
				
				Cell cell = World.instance.life.GetCellAt(downPositionMouse);
				if (cell != null) {
					spring.connectedBody = cell.theRigidBody;
					spring.anchor = downPositionMouse;
					spring.distance = 0f;
					spring.GetComponent<LineRenderer>().SetPosition(1, downPositionMouse);
					spring.GetComponent<LineRenderer>().SetPosition(0, spring.connectedBody.transform.position);
					spring.GetComponent<LineRenderer>().enabled = true;
				}
			} else if (RMBToolModePanel.instance.toolMode == RMBToolModePanel.RMBToolMode.spawnEmbryo) {
				if (GlobalPanel.instance.soundCreatures.isOn) {
					Audio.instance.PlaceCreature(CameraUtils.GetEffectStrengthLazy());
				}
				if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype || CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
					Creature spawned = World.instance.life.SpawnCreatureSimple(downPositionMouse, 90f, World.instance.worldTicks);
					if (!TerrainPerimeter.instance.IsCompletelyInside(spawned)) {
						World.instance.life.KillCreatureSafe(spawned, false);
					}
				}
			} else if (RMBToolModePanel.instance.toolMode == RMBToolModePanel.RMBToolMode.spawnFreak) {
				if (GlobalPanel.instance.soundCreatures.isOn) {
					Audio.instance.PlaceCreature(CameraUtils.GetEffectStrengthLazy());
				}
				if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype || CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
					Creature spawned = World.instance.life.SpawnCreatureFreak(downPositionMouse, 90f, World.instance.worldTicks);
					if (!TerrainPerimeter.instance.IsCompletelyInside(spawned)) {
						World.instance.life.KillCreatureSafe(spawned, false);
					}
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
