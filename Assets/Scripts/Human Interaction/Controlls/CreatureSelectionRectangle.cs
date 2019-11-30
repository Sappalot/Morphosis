using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CreatureSelectionRectangle : MouseDrag {
	public new Camera camera;
	public Transform rectangle;

	public bool IsIdle {
		get {
			return selectingMode == SelectingMode.idle;
		}
	}

	private Vector3 downPositionMouse; //World space

	private enum SelectingMode {
		idle,
		fresh,
		add,
		subtract,
	}
	private SelectingMode selectingMode;

	private List<Creature> alreadySelected;

	public override void OnDraggingStart(int mouseButton) {
		if (PhenotypePanel.instance.followToggle.isOn) {
			return;
		}


		if (mouseButton == 0 && !EventSystem.current.IsPointerOverGameObject() && MouseAction.instance.actionState == MouseActionStateEnum.free && !GraphPlotter.instance.IsMouseInside() && !AlternativeToolModePanel.instance.isOn) {
			downPositionMouse = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;

			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift)) {
				selectingMode = SelectingMode.subtract;
				alreadySelected = new List<Creature>(CreatureSelectionPanel.instance.selection);
			} else if (Input.GetKey(KeyCode.LeftControl)) {
				return;
			} else if (Input.GetKey(KeyCode.LeftShift)) {
				selectingMode = SelectingMode.add;
				alreadySelected = new List<Creature>(CreatureSelectionPanel.instance.selection);
			} else {
				selectingMode = SelectingMode.fresh;
			}
			
			rectangle.transform.localScale = new Vector3(0.1f, 0.1f, 0f);
			//Debug.Log("MouseButton @ " + downPositionMouse);
		}
	}

	public override void OnDragging(int mouseButton) {
		if (PhenotypePanel.instance.followToggle.isOn) {
			return;
		}

		if (mouseButton == 0 && MouseAction.instance.actionState == MouseActionStateEnum.free && selectingMode != SelectingMode.idle) {

			Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
			Vector3 rectPosition = (downPositionMouse + mousePosition) / 2f;
			Vector2 rectScale = new Vector2(Mathf.Abs(downPositionMouse.x - mousePosition.x), Mathf.Abs(downPositionMouse.y - mousePosition.y));

			Rect area = new Rect(rectPosition, rectScale);
			rectangle.transform.position = new Vector3(area.x, area.y, -20f);
			rectangle.transform.localScale = new Vector3(area.width, area.height, -20f);
			rectangle.gameObject.SetActive(true);

			const float largeThreshold = 1f;
			if (!(rectScale.x > largeThreshold || rectScale.y > largeThreshold)) {
				return;
			}

			List<Creature> inside = null;
			if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
				inside  = World.instance.life.GetPhenotypesInside(area);
			} else if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
				inside  = World.instance.life.GetGenotypesInside(area);
			}

			if (selectingMode == SelectingMode.fresh) {
				CreatureSelectionPanel.instance.Select(inside);

			} else if (selectingMode == SelectingMode.add) {
				List<Creature> sumList = new List<Creature>();
				sumList.AddRange(alreadySelected);
				for (int index = 0; index < inside.Count; index++) {
					Creature creature = inside[index];
					if (!sumList.Contains(creature)) {
						sumList.Add(creature);

					}
				}
				CreatureSelectionPanel.instance.Select(sumList);
			} else if (selectingMode == SelectingMode.subtract) {
				List<Creature> subList = new List<Creature>();
				subList.AddRange(alreadySelected);
				for (int index = 0; index < inside.Count; index++) {
					Creature creature = inside[index];
					if (subList.Contains(creature)) {
						subList.Remove(creature);
					}
				}
				CreatureSelectionPanel.instance.Select(subList);

			}
		}
	}

	public override void OnDraggingEnd(int mouseButton) {
		// implement this for end of dragging
		if (mouseButton == 0) {
			selectingMode = SelectingMode.idle;
			rectangle.gameObject.SetActive(false);
		}
	}
}