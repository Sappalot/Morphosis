using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlternativeToolController : MouseDrag {
	public new Camera camera;
	public SpringJoint2D spring;
	private Vector3 downPositionMouse; //World space

	public override void OnDraggingStart(int mouseButton) {
		if (mouseButton == 0 && AlternativeToolModePanel.instance.isOn && !EventSystem.current.IsPointerOverGameObject()) {
			downPositionMouse = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
			if (AlternativeToolModePanel.instance.toolMode == AlternativeToolModePanel.RMBToolMode.spring && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
				Cell cell = World.instance.life.GetCellAtPosition(downPositionMouse);
				if (cell != null) {
					spring.connectedBody = cell.theRigidBody;
					spring.anchor = downPositionMouse;
					spring.distance = 0f;
					spring.GetComponent<LineRenderer>().SetPosition(1, downPositionMouse);
					spring.GetComponent<LineRenderer>().SetPosition(0, spring.connectedBody.transform.position);
					spring.GetComponent<LineRenderer>().enabled = true;
				}
			} else if (AlternativeToolModePanel.instance.toolMode == AlternativeToolModePanel.RMBToolMode.spawnEmbryo && TerrainPerimeter.instance.IsInside(camera.ScreenToWorldPoint(Input.mousePosition))) {
				if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype || CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
					Creature spawned = World.instance.life.SpawnCreatureSimple(downPositionMouse, 90f, World.instance.worldTicks);
					EffectsUtils.SpawnAddCreature(spawned);
					HistoryUtil.SpawnAddCreatureEvent(1);
				}
			} else if (AlternativeToolModePanel.instance.toolMode == AlternativeToolModePanel.RMBToolMode.spawnFreak && TerrainPerimeter.instance.IsInside(camera.ScreenToWorldPoint(Input.mousePosition))) {
				if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype || CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
					Creature spawned = World.instance.life.SpawnCreatureFreak(downPositionMouse, 90f, World.instance.worldTicks);
					EffectsUtils.SpawnAddCreature(spawned);
					HistoryUtil.SpawnAddCreatureEvent(1);
				}
			}
		}
	}

	public override void OnDragging(int mouseButton) {
		if (mouseButton == 0) {
			if (AlternativeToolModePanel.instance.isOn && spring.connectedBody != null) {
				Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
				spring.anchor = mousePosition;
				spring.GetComponent<LineRenderer>().SetPosition(1, mousePosition);
				spring.GetComponent<LineRenderer>().SetPosition(0, spring.connectedBody.transform.position);
			}

			if (!AlternativeToolModePanel.instance.isOn) {
				spring.connectedBody = null;
				spring.GetComponent<LineRenderer>().enabled = false;
			}
		}

	}

	public override void OnDraggingEnd(int mouseButton) {
		if (mouseButton == 0 && AlternativeToolModePanel.instance.isOn) {
			spring.connectedBody = null;
			spring.GetComponent<LineRenderer>().enabled = false;
		}
	}
}
