using UnityEngine;

public class CreatureReference {
	public string id = string.Empty;
	public Creature creature;
	public bool giveUpLooking = false;

	public CreatureReference(string id) {
		this.id = id;
	}

	public void TryGetReference() {
		if (!isCreatureReferenceUpdated) {
			if (Life.instance.HasCreature(id)) {
				creature = Life.instance.GetCreature(id);
				//creature.soul = soul;
			} else {
				Debug.Log("Can't find creature");
				giveUpLooking = true;
			}
		}
	}

	public bool isCreatureReferenceUpdated {
		get {
			return creature != null || giveUpLooking;
		}
	}

	// Load Save

	private CreatureReferenceData creatureReferenceData = new CreatureReferenceData();

	public CreatureReferenceData UpdateData() {
		creatureReferenceData.id = id;
		return creatureReferenceData;
	}

	public void ApplyData(CreatureReferenceData creatureReferenceData) {
		id = creatureReferenceData.id;
	}

	// ^Load Save ^
}