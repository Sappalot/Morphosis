using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CreatureSelectionController : MouseDrag {
	public Camera camera;
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
			rectangle.gameObject.SetActive(true);
			//Debug.Log("MouseButton @ " + downPositionMouse);
		}
	}

	public override void OnDragging(int mouseButton) {
		// implement this for dragging
		if (mouseButton == 0 && selectingMode != SelectingMode.idle) {
			Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 25;
			Vector3 rectPosition = (downPositionMouse + mousePosition) / 2f;
			Vector2 rectScale = new Vector2(Mathf.Abs(downPositionMouse.x - mousePosition.x), Mathf.Abs(downPositionMouse.y - mousePosition.y));
			
			Rect area = new Rect(rectPosition, rectScale);

			rectangle.transform.position = new Vector3(area.x, area.y, 0f);
			rectangle.transform.localScale = new Vector3(area.width, area.height, 0f);

			List<Creature> inside = null;

			if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
				inside  = World.instance.life.GetPhenotypesInside(area);
			} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
				inside  = World.instance.life.GetGenotypesInside(area);
			}

			if (selectingMode == SelectingMode.fresh) {
				CreatureSelectionPanel.instance.ClearSelection();
				CreatureSelectionPanel.instance.AddToSelection(inside);
			} else if (selectingMode == SelectingMode.add) {
				CreatureSelectionPanel.instance.ClearSelection();
				CreatureSelectionPanel.instance.AddToSelection(inside);
				CreatureSelectionPanel.instance.AddToSelection(alreadySelected);
			} else if (selectingMode == SelectingMode.subtract) {
				List<Creature> rest = new List<Creature>();
				foreach (Creature c in alreadySelected) {
					if (!inside.Contains(c)) {
						rest.Add(c);
					}
				}
				CreatureSelectionPanel.instance.ClearSelection();
				CreatureSelectionPanel.instance.AddToSelection(rest);
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