using System.Collections.Generic;
using UnityEngine;


public class EffectPlayer : MonoSingleton<EffectPlayer> {
	public Sprite add;
	public Sprite born;
	public Sprite death;
	public Sprite detatch;

	public void Play(EffectEnum type, Vector2 position, float angle = 0f) {
		Animator animator = EffectPool.instance.Borrow();
		animator.transform.position = position;
		animator.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		animator.SetTrigger("expand");
		animator.GetComponent<SpriteRenderer>().enabled = true;

		Sprite sprite = null;
		if (type == EffectEnum.CreatureAdd) {
			sprite = add;
		} else if (type == EffectEnum.CreatureBorn) {
			sprite = born;
		} else if (type == EffectEnum.CreatureDeath) {
			sprite = death;
		} else if (type == EffectEnum.CreatureDetatch) {
			sprite = detatch;
		}
		animator.GetComponent<SpriteRenderer>().sprite = sprite;

		animator.GetComponent<DelayedAnimationDelete>().StopWhenDone();

	}

	public void Stop(Animator animator) {
		animator.GetComponent<SpriteRenderer>().enabled = false;
		EffectPool.instance.Return(animator);
	}
}
