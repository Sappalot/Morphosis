using System.Collections;
using UnityEngine;

public class CellDeath : MonoBehaviour {

	public ParticleSystem particles;
	public AudioSource player;
	public SpriteRenderer filedCircleSprite;

	private void Start() {
		filedCircleSprite.enabled = true;
	}

	public void Prime(Color cellColor, Color explosionColor, float explodeAfterTime = 0f) {
		filedCircleSprite.color = cellColor;
		particles.startColor = cellColor;
		filedCircleSprite.enabled = true;
		StartCoroutine(Explode(explodeAfterTime));
		StartCoroutine(RemoveSelf());
	}

	private IEnumerator Explode(float time) {
		yield return new WaitForSeconds(time);
		particles.Play();
		filedCircleSprite.enabled = false;
	}

	private IEnumerator RemoveSelf() {
		yield return new WaitForSeconds(10);
		Destroy(gameObject);
	}
}
