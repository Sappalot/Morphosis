using System.Collections;
using UnityEngine;

public class CellDeath : MonoBehaviour {

	public ParticleSystem particles;
	public AudioSource player;

	public void Prime(Color cellColor, Color explosionColor) {
		particles.startColor = cellColor;
		particles.Play();
		StartCoroutine(RemoveSelf());
	}

	private IEnumerator RemoveSelf() {
		yield return new WaitForSeconds(2);
		Destroy(gameObject);
	}
}
