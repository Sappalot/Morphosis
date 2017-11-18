using System.Collections.Generic;
using UnityEngine;

// TODO: Make the souls connencted. That is: one souls mothers child is the same soul
// TODO rename: is this a CreatureRecord
public class Soul {
	public string id = string.Empty;

	// References to be found:
	public CreatureReference creatureReference;
	public SoulReference motherSoulReference = new SoulReference(string.Empty); //has no mother per default
	public List<SoulReference> childSoulReferences = new List<SoulReference>();

	// Relatives

	public Creature creature {
		get {
			Debug.Assert(areAllReferencesUpdated, "Update references first!");
			return creatureReference.creature;
		}
	}

	public bool hasCreature {
		get {
			Debug.Assert(areAllReferencesUpdated, "Update references first!");
			return creature != null;
		}
	}

	public void SetCreatureImmediate(Creature creature) {
		SetCreature(creature.id);
		creatureReference.creature = creature;
	}

	public void SetCreature(string id) {
		Debug.Assert(motherSoulReference.id == string.Empty, "We shouldn't change the id of mother");
		creatureReference.id = id;
	}

	public Soul motherSoul {
		get {
			Debug.Assert(motherSoulReference.isReferenceUpdated, "Update references first!");
			return motherSoulReference.soul;
		}
	}

	//We run into trouble if we set mother here, while we still have this one in lifes list of souls with updated refrernces
	//Call this function from AddMotherSoulImmediateSafe in life only
	public void SetMotherSoulImmediate(Soul motherSoul) {
		SetMotherSoul(motherSoul.id);
		motherSoulReference.soul = motherSoul;
	}

	private void SetMotherSoul(string id) {
		Debug.Assert(motherSoulReference.id == string.Empty, "We shouldn't change the id of mother");
		motherSoulReference.id = id;
	}

	public bool hasMotherSoul {
		get {
			Debug.Assert(motherSoulReference.isReferenceUpdated, "Update references first!");
			return motherSoul != null;
		}
	}

	public Creature mother {
		get {
			if (motherSoulReference.isReferenceUpdated && motherSoulReference.id == string.Empty) {
				return null;
			}
			Debug.Assert(motherSoulReference.isReferenceUpdated, "Update mother reference first!");
			Debug.Assert(motherSoul.areAllReferencesUpdated, "Update mother's references first!");
			return motherSoul.creature;
		}
	}

	public bool hasMother {
		get {
			if (motherSoulReference.isReferenceUpdated && motherSoulReference.id == string.Empty) {
				return false;
			}
			Debug.Assert(motherSoulReference.isReferenceUpdated, "Update references first!");
			Debug.Assert(motherSoul != null && motherSoul.areAllReferencesUpdated, "Update references first!");
			return mother != null;
		}
	}

	//We run into trouble if we add children here, while we still have this one in lifes list of souls with updated refrernces
	//Call this function from AddChildSoulImmediateSafe in life only
	public void AddChildSoulImmediate(Soul childSoul, Vector2i rootMapPosition, int rootBindCardinalIndex, bool isConnected) {
		SoulReference reference = AddChildSoul(childSoul.id, rootMapPosition, rootBindCardinalIndex, isConnected);
		reference.soul = childSoul;
	}

	private SoulReference AddChildSoul(string id, Vector2i rootMapPosition, int rootBindCardinalIndex, bool isConnected) {
		Debug.Assert(childSouls.Find(s => s.id == id) == null, "Creature has allready a child with that id");
		// This soul will be created as the child is created, just fetch teh reference
		SoulReference childSoulReference = new SoulReference(id);
		childSoulReference.childRootMapPosition = rootMapPosition;
		childSoulReference.childRootBindCardinalIndex = rootBindCardinalIndex;
		childSoulReference.isChildConnected = isConnected;
		childSoulReferences.Add(childSoulReference);

		return childSoulReference;
	}

	public List<Creature> children {
		get {
			Debug.Assert(areChildReferencesUpdated, "Update references first!");
			List<Creature> children = new List<Creature>();
			for (int i = 0; i < childSoulReferences.Count; i++) {
				Debug.Assert(childSoulReferences[i].soul.areAllReferencesUpdated, "Update references first!");
				children.Add(childSoulReferences[i].soul.creature);
			}
			return children;
		}
	}

	public List<Soul> childSouls {
		get {
			Debug.Assert(areChildReferencesUpdated, "Update references first!");
			List<Soul> childSouls = new List<Soul>();
			for (int i = 0; i < childSoulReferences.Count; i++) {
				childSouls.Add(childSoulReferences[i].soul);
			}
			return childSouls;
		}
	}

	public int childSoulsCount {
		get {
			Debug.Assert(areChildReferencesUpdated, "Update references first!");
			return childSoulReferences.Count;
		}
	}

	public Soul GetChildSoul(string id) {
		Debug.Assert(areAllReferencesUpdated, "Update references first!");
		return childSouls.Find(c => c.id == id);
	}

	public Creature GetChild(string id) {
		Debug.Assert(areAllReferencesUpdated, "Update references first!");
		Soul childSoul = childSouls.Find(c => c.id == id);
		if (childSoul != null) {
			Debug.Assert(childSoul.areAllReferencesUpdated, "Update references first!");
			return childSoul.creature;
		}
		return null;
	}

	public bool HasChild(string id) {
		return GetChild(id) != null;
	}

	public bool hasChildSoul {
		get {
			Debug.Assert(areChildReferencesUpdated, "Update references first!");
			return childSoulReferences.Count > 0;
		}
	}

	public bool isConnectedWithMotherSoul {
		get {
			Debug.Assert(motherSoulReference.isReferenceUpdated, "Update references first!");
			//Debug.Assert(motherSoul.areAllReferencesUpdated, "Update mother's references first!");
			return (motherSoulReference.soul == null && motherSoulReference.isReferenceUpdated) || motherSoul.childSoulReferences.Find(c => c.id == id).isChildConnected;
		}
	}

	public bool SetConnectedWithMotherSoul(bool connected) {
		Debug.Assert(areAllReferencesUpdated, "Update references first!");
		Debug.Assert(motherSoul.areAllReferencesUpdated, "Update mother's references first!");
		return motherSoul.childSoulReferences.Find(c => c.id == id).isChildConnected = connected;
	}

	public bool isConnectedWithChildSoul(string childSoulId) {
		Debug.Assert(areAllReferencesUpdated, "Update references first!");
		for (int i = 0; i < childSoulReferences.Count; i++) {
			if (childSoulReferences[i] != null && childSoulReferences[i].id == childSoulId) {
				if (childSoulReferences[i].isChildConnected) {
					return true;
				}
			}
		}
		return false;
		//return childSoulReferences.Find(c => c != null && c.id == childSoulId).isChildConnected; // Didn't work!!
	}

	public bool setConnectedWithChildSoul(string childSoulId, bool connected) {
		Debug.Assert(areAllReferencesUpdated, "Update references first!");
		return childSoulReferences.Find(c => c.id == childSoulId).isChildConnected = connected;
	}

	public Vector2i childSoulRootMapPosition(string childSoulId) {
		Debug.Assert(areAllReferencesUpdated, "Update references first!");
		return childSoulReferences.Find(c => c.id == childSoulId).childRootMapPosition;
	}

	public int childSoulRootBindCardinalIndex(string childSoulId) {
		Debug.Assert(areAllReferencesUpdated, "Update references first!");
		return childSoulReferences.Find(c => c.id == childSoulId).childRootBindCardinalIndex;
	}
	// ^ Relatives ^

	// Update
	private bool areChildReferencesUpdated {
		get {
			for (int i = 0; i < childSoulReferences.Count; i++) {
				if (!childSoulReferences[i].isReferenceUpdated) {
					return false;
				}
			}
			return true;
		}
	}

	public bool areAllReferencesUpdated {
		get {
			if (creatureReference.isCreatureReferenceUpdated && motherSoulReference.isReferenceUpdated && areChildReferencesUpdated) {
				return true;
			}
			return false;
		}
	}

	public bool UpdateReferences() {
		creatureReference.TryGetReference();
		motherSoulReference.TryGetReference();
		for (int i = 0; i < childSoulReferences.Count; i++) {
			childSoulReferences[i].TryGetReference();
		}
		return areAllReferencesUpdated;
	}

	// ^ Update ^
	public Soul(string id) {
		this.id = id;
		creatureReference = new CreatureReference(id);
	}

	//TODO: Store creature Data in this one and create Creature from it

	// Load Save
	private SoulData soulData = new SoulData();

	//Everything is deep cloned even the id. Change this not to have trouble
	public void Clone(Soul original) {
		ApplyData(original.UpdateData());
	}

	// save
	public SoulData UpdateData() {
		soulData.id = id;
		soulData.creatureReferenceData = creatureReference.UpdateData();
		soulData.motherSoulReferenceData = motherSoulReference.UpdateData();
		soulData.childSoulReferencesData.Clear();
		for (int i = 0; i < childSoulReferences.Count; i++) {
			soulData.childSoulReferencesData.Add(childSoulReferences[i].UpdateData());
		}

		return soulData;
	}

	// load
	public void ApplyData(SoulData creatureData) {
		id = creatureData.id;

		creatureReference.ApplyData(creatureData.creatureReferenceData);
		motherSoulReference.ApplyData(creatureData.motherSoulReferenceData);
		childSoulReferences.Clear();
		for (int i = 0; i < creatureData.childSoulReferencesData.Count; i++) {
			SoulReference soulReference = new SoulReference(creatureData.childSoulReferencesData[i].id);
			soulReference.ApplyData(creatureData.childSoulReferencesData[i]);
			childSoulReferences.Add(soulReference);
		}
	}

	// ^ Load / Save ^
}