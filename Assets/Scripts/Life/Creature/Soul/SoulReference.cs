using UnityEngine;

public class SoulReference {
	public string id = string.Empty;
	public Soul soul;
	private bool canNotFindSoul = false;

	public SoulReference(string id) {
		this.id = id;
	}

	public void TryGetReference() {
		if(!isReferenceUpdated) {
			if (id != string.Empty) {
				if (World.instance.life.HasSoul(id)) {
					soul = World.instance.life.GetSoul(id);
				} else {
					//Debug.LogError("Soul Reference: Can't find soul in Life");
					canNotFindSoul = true;
				}
			}
		}
	}

	public bool isReferenceUpdated {
		get {
			return id == string.Empty || soul != null || canNotFindSoul; // Is it OK to call soulReference updated when we can not find it
		}
	}

	//---------------- only used for children
	// How mother sees a child:
	public bool isChildConnected; // Should be connected
	public Vector2i childOriginMapPosition; //As seen from mothers frame of reference
	public int childOriginBindCardinalIndex; //As seen from mothers frame of reference

	// Load Save
	private SoulReferenceData soulReferenceData = new SoulReferenceData();

	public SoulReferenceData UpdateData() {
		soulReferenceData.id = id;
		soulReferenceData.isChildConnected = isChildConnected;
		soulReferenceData.childOriginMapPosition = childOriginMapPosition;
		soulReferenceData.childOriginBindCardinalIndex = childOriginBindCardinalIndex;

		return soulReferenceData;
	}

	public void ApplyData(SoulReferenceData soulReferenceData) {
		id = soulReferenceData.id;
		isChildConnected = soulReferenceData.isChildConnected;
		childOriginMapPosition = soulReferenceData.childOriginMapPosition;
		childOriginBindCardinalIndex = soulReferenceData.childOriginBindCardinalIndex;
	}
	// ^ Load Save ^
}