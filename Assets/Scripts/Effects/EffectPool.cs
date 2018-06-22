using System.Collections.Generic;
using UnityEngine;


public class EffectPool : MonoSingleton<EffectPool> {
	public Effect creatureDeathEffectPrefab;
	private int serialNumber = 0;
	private Queue<Effect> storedQueues = new Queue<Effect>();

	public override void Init() {

	}

	//Borrow an expand animator
	//TODO: other animators as well
	public Effect Borrow() {
		if (!GlobalSettings.instance.pooling.effects) {
			return Instantiate();
		}

		Effect effect = null;
		Effect poppedEffect = PopEffect();

		if (poppedEffect != null) {
			effect = poppedEffect;
		} else {
			effect = Instantiate();
		}

		effect.transform.parent = transform.parent;
		return effect;
	}

	public void Return(Effect effect) {
		if (!GlobalSettings.instance.pooling.effects) {
			Destroy(effect.gameObject);
			return;
		}

		effect.transform.parent = transform;
		storedQueues.Enqueue(effect);
	}

	private Effect PopEffect() {
		if (storedQueues.Count > 0) {
			Effect effect = storedQueues.Dequeue();
			return effect;
		}
		return null;
	}

	private Effect Instantiate() {
		Effect effect = Instantiate(creatureDeathEffectPrefab, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
		effect.transform.parent = transform;
		effect.name = "Sprite Expand " + serialNumber++;
		return effect;
	}
}
