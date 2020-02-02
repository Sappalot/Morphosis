using UnityEngine;

public class JawCellMouth : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		JawCell predatorCell = transform.parent.GetComponent<JawCell>();
		predatorCell.TriggerEnter(other);
	}

	void OnTriggerExit(Collider other) {
		JawCell predatorCell = transform.parent.GetComponent<JawCell>();
		predatorCell.TriggerExit(other);
	}
}
