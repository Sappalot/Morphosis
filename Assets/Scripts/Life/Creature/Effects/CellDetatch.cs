using System.Collections;
using UnityEngine;

public class CellDetatch : MonoBehaviour {

	public ParticleSystem blood;

	public void Prime(Color explosionColor) {
		blood.startColor = explosionColor;


		blood.Play();


		StartCoroutine(RemoveSelf());
	}

	private IEnumerator RemoveSelf() {
		yield return new WaitForSeconds(10);
		Destroy(gameObject);
	}
}
