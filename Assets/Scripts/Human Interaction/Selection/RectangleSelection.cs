using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleSelection : MonoBehaviour {
	public CreatureSelectionController controller;

	private void OnTriggerEnter2D(Collider2D other) {
		if (controller.isDragging) {
			bool firstAdded = other.GetComponent<Cell>().creature.AddSeclectionRectangleCell(other.GetComponent<Cell>());
			if (firstAdded) {
				CreatureSelectionPanel.instance.AddToSelection(other.GetComponent<Cell>().creature);
			}
		}	
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (controller.isDragging) {
			bool lastRemoved = other.GetComponent<Cell>().creature.RemoveSelectionRectangleCell(other.GetComponent<Cell>());
			if (lastRemoved) {
				CreatureSelectionPanel.instance.RemoveFromSelection(other.GetComponent<Cell>().creature);
			}
		}
	}
}
