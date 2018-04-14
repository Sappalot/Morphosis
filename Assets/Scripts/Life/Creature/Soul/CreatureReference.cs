using UnityEngine;

public class CreatureReference {
	public string id = string.Empty;
	public Creature creature;
	public bool giveUpLooking = false;

	private bool hadReference = false; // We might had it and lost it as creature dies

	public CreatureReference(string id) {
		this.id = id;
	}

	public void TryGetReference() {
		if (!isCreatureReferenceUpdated) {
			if (World.instance.life.HasCreature(id)) {
				creature = World.instance.life.GetCreature(id);
				hadReference = true;
			} else {
				//Debug.Log("Can't find creature");
				giveUpLooking = true;
			}
		}
	}

	public bool isCreatureReferenceUpdated {
		get {
			return creature != null || giveUpLooking || hadReference;
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