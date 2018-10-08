using UnityEngine;

public class Freezer : MonoSingleton<Freezer> {

	public GameObject legalArea;

	public bool IsCompletelyInside(Creature creature) {
		return creature.phenotype.IsCompletelyInside(legalRect);
	}

	public bool KillIfOutside(Creature creature) {
		if (!IsCompletelyInside(creature)) {
			World.instance.life.KillCreatureSafe(creature, false);
			return true;
		}
		return false;
	}

	private Rect legalRect;
	public void Start() {
		legalRect = new Rect(legalArea.transform.position, legalArea.transform.localScale);
	}

	public void Load() {

	}

	public void Save() {

	}

	// Save
	private void UpdateData() {

	}

	// Load
	private void ApplyData(WorldData worldData) {

	}
}