using System.Collections;
using UnityEngine;

public class DelayedAnimationDelete : MonoBehaviour {
	public float linger = 0f;

	void Start() {
		StartCoroutine(Die());
	}

	private IEnumerator Die() {
		yield return new WaitForSecondsRealtime(this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + linger);
		Destroy(gameObject);
	}
}
