using UnityEngine;

public class Ball : MonoBehaviour {
	private void Start() {
		transform.GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
	}
}
