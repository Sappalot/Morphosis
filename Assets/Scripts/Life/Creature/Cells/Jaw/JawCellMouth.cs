using UnityEngine;

public class JawCellMouth : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D other) {
		JawCell predatorCell = transform.parent.GetComponent<JawCell>();
		predatorCell.TriggerEnter(other);
	}

	void OnTriggerExit2D(Collider2D other) {
		JawCell predatorCell = transform.parent.GetComponent<JawCell>();
		predatorCell.TriggerExit(other);
	}
}
