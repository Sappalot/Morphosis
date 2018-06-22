using System.Collections.Generic;
using UnityEngine;


public class EffectPlayer : MonoSingleton<EffectPlayer> {
	public Sprite add;
	public Sprite born;
	public Sprite death;
	public Sprite detatch;

	public void Play(EffectEnum type, Vector2 position, float angle, float scale) {
		Effect effect = EffectPool.instance.Borrow();
		effect.transform.position = position;
		effect.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

		effect.transform.localScale = new Vector3(scale, scale, 0f);
		effect.animator.SetTrigger("expand");
		effect.spriteRenderer.enabled = true;

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
		effect.spriteRenderer.sprite = sprite; 

		effect.delayedAnimationDelete.StopWhenDone();

	}

	public void Stop(Effect effect) {
		effect.spriteRenderer.enabled = false;
		EffectPool.instance.Return(effect);
	}
}
