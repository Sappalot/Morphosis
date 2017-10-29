using System.Collections;
using UnityEngine;

public class CellDeath : MonoBehaviour {

	public ParticleSystem particles;
	public AudioSource player;

	public void Prime(Color explosionColor) {
		particles.startColor = explosionColor;
		particles.Play();
		StartCoroutine(RemoveSelf());
	}

	private IEnumerator RemoveSelf() {
		yield return new WaitForSeconds(2);
		Destroy(gameObject);
	}
}
