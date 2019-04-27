using System.Collections;
using UnityEngine;

public class ParticlesCellBleed : MonoBehaviour {

	public ParticleSystem cellBleed;

	public void Prime(Color explosionColor) {
		cellBleed.startColor = explosionColor;
		cellBleed.Play();
		StartCoroutine(RemoveSelf());
	}

	private IEnumerator RemoveSelf() {
		yield return new WaitForSeconds(10);
		Destroy(gameObject);
	}
}
