using System.Collections;
using UnityEngine;

public class DelayedAnimationDelete : MonoBehaviour {
	public float linger = 0f;

	public void StopWhenDone() {
		StartCoroutine(Stop());
	}

	private IEnumerator Stop() {
		yield return new WaitForSecondsRealtime(this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + linger);
		EffectPlayer.instance.Stop(GetComponent<Animator>());
	}
}
