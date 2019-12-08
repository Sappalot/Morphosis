using System.Collections.Generic;
using UnityEngine;


public class EventSymbolPool : MonoSingleton<EventSymbolPool> {
	public EventSymbol creatureDeathEffectPrefab;
	private int serialNumber = 0;
	private Queue<EventSymbol> storedQueue = new Queue<EventSymbol>();

	public override void Init() {
	}

	public int storedCount {
		get {
			return storedQueue.Count;
		}
	}

	//We are expecting to gett all of these back if all edges were recycled
	private int m_loanedCount;
	public int loanedCount {
		get {
			return m_loanedCount;
		}
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
		m_loanedCount++;
		return effect;
	}

	public void Return(EventSymbol effect) {
		if (!GlobalSettings.instance.pooling.effects) {
			Destroy(effect.gameObject);
			return;
		}

		effect.transform.parent = transform;
		storedQueue.Enqueue(effect);
		m_loanedCount--;
	}

	private EventSymbol PopEffect() {
		if (storedQueue.Count > 0) {
			EventSymbol effect = storedQueue.Dequeue();
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
