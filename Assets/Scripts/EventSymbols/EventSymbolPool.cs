using System.Collections.Generic;
using UnityEngine;


public class EventSymbolPool : MonoSingleton<EventSymbolPool> {
	public EventSymbol creatureDeathEffectPrefab;
	private int serialNumber = 0;
	private Queue<EventSymbol> storedQueues = new Queue<EventSymbol>();

	public override void Init() {

	}

	//Borrow an expand animator
	//TODO: other animators as well
	public EventSymbol Borrow() {
		if (!GlobalSettings.instance.pooling.effects) {
			return Instantiate();
		}

		EventSymbol effect = null;
		EventSymbol poppedEffect = PopEffect();

		if (poppedEffect != null) {
			effect = poppedEffect;
		} else {
			effect = Instantiate();
		}

		effect.transform.parent = transform.parent;
		return effect;
	}

	public void Return(EventSymbol effect) {
		if (!GlobalSettings.instance.pooling.effects) {
			Destroy(effect.gameObject);
			return;
		}

		effect.transform.parent = transform;
		storedQueues.Enqueue(effect);
	}

	private EventSymbol PopEffect() {
		if (storedQueues.Count > 0) {
			EventSymbol effect = storedQueues.Dequeue();
			return effect;
		}
		return null;
	}

	private EventSymbol Instantiate() {
		EventSymbol effect = Instantiate(creatureDeathEffectPrefab, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
		effect.transform.parent = transform;
		effect.name = "Sprite Expand " + serialNumber++;
		return effect;
	}
}
