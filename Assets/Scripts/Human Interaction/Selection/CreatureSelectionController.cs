using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CreatureSelectionController : MouseDrag {
	public new Camera camera;
	public Transform rectangle;

	private Vector3 dragVector = new Vector3();
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
		// implement this for start of dragging
		
		if (mouseButton == 0 && !EventSystem.current.IsPointerOverGameObject()) {
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

			downPositionMouse = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
			rectangle.transform.localScale = new Vector3(0.1f, 0.1f, 0f);
			//Debug.Log("MouseButton @ " + downPositionMouse);
		}
	}

	private List<Creature> oldSelection = new List<Creature>();

	private bool AreSelectionsSame(List<Creature> s1, List<Creature> s2) {
		if (s1.Count == s2.Count) {
			foreach (Creature c in s1) {
				if (!s2.Contains(c)) {
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private List<Creature> ToAdd(List<Creature> oldSelection, List<Creature> newSelection) {
		List<Creature> extra = new List<Creature>();
		foreach (Creature c in newSelection) {
			if (!oldSelection.Contains(c)) {
				extra.Add(c);
			}
		}
		return extra;
	}

	private List<Creature> ToRemove(List<Creature> oldSelection, List<Creature> newSelection) {
		List<Creature> remove = new List<Creature>();
		foreach (Creature c in oldSelection) {
			if (!newSelection.Contains(c)) {
				remove.Add(c);
			}
		}
		return remove;
	}

	public override void OnDragging(int mouseButton) {
		// implement this for dragging
		if (mouseButton == 0 && selectingMode != SelectingMode.idle) {
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
			if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
				inside  = World.instance.life.GetPhenotypesInside(area);
			} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
				inside  = World.instance.life.GetGenotypesInside(area);
			}

			if (selectingMode == SelectingMode.fresh) {
				CreatureSelectionPanel.instance.Select(inside);
			} else if (selectingMode == SelectingMode.add) {
				List<Creature> sumList = new List<Creature>();
				sumList.AddRange(inside);
				sumList.AddRange(alreadySelected);
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