using System.Collections.Generic;
using UnityEngine;


public class EventSymbolPlayer : MonoSingleton<EventSymbolPlayer> {
	public Sprite add;
	public Sprite born;
	public Sprite death;
	public Sprite detatch;

	public void Play(EventSymbolEnum type, Vector2 position, float angle, float scale) {
		EventSymbol effect = EventSymbolPool.instance.Borrow();
		effect.transform.position = position;
		effect.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

		effect.transform.localScale = new Vector3(scale, scale, 0f);
		effect.animator.SetTrigger("expand");
		effect.spriteRenderer.enabled = true;

		Sprite sprite = null;
		if (type == EventSymbolEnum.CreatureAdd) {
			sprite = add;
		} else if (type == EventSymbolEnum.CreatureBorn) {
			sprite = born;
		} else if (type == EventSymbolEnum.CreatureDeath) {
			sprite = death;
		} else if (type == EventSymbolEnum.CreatureDetatch) {
			sprite = detatch;
		}
		effect.spriteRenderer.sprite = sprite; 

		effect.delayedAnimationDelete.StopWhenDone();

	}

	public void Stop(EventSymbol effect) {
		effect.spriteRenderer.enabled = false;
		EventSymbolPool.instance.Return(effect);
	}
}
