using System.Collections.Generic;
using UnityEngine;


public class EffectPool : MonoSingleton<EffectPool> {
	public Animator creatureDeathEffectPrefab;
	private int serialNumber = 0;
	private Queue<Animator> storedQueues = new Queue<Animator>();

	public override void Init() {

	}

	//Borrow an expand animator
	//TODO: other animators as well
	public Animator Borrow() {
		if (!GlobalSettings.instance.pooling.effects) {
			return Instantiate();
		}

		Animator animator = null;
		Animator poppedAnimator = PopAnimator();

		if (poppedAnimator != null) {
			animator = poppedAnimator;
		} else {
			animator = Instantiate();
		}

		animator.transform.parent = transform.parent;
		return animator;
	}

	public void Return(Animator animator) {
		if (!GlobalSettings.instance.pooling.effects) {
			Destroy(animator.gameObject);
			return;
		}

		animator.transform.parent = transform;
		storedQueues.Enqueue(animator);
	}

	private Animator PopAnimator() {
		if (storedQueues.Count > 0) {
			Animator animator = storedQueues.Dequeue();
			return animator;
		}
		return null;
	}

	private Animator Instantiate() {
		Animator animator = Instantiate(creatureDeathEffectPrefab, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
		animator.transform.parent = transform;
		animator.name = "Sprite Expand " + serialNumber++;
		return animator;
	}
}
