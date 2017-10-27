using UnityEngine;

public class SoulReference {
	public string id = string.Empty;
	public Soul soul;
	public bool giveUpLooking = false;

	public SoulReference(string id) {
		this.id = id;
	}

	public void TryGetReference() {
		if(!isReferenceUpdated) {
			if (id != string.Empty) {
				if (Life.instance.HasSoul(id)) {
					soul = Life.instance.GetSoul(id);
				} else {
					Debug.Log("Can't find soul");
					giveUpLooking = true;
				}
			}
		}
	}

	public bool isReferenceUpdated {
		get {
			return id == string.Empty || soul != null || giveUpLooking;
		}
	}

	//---------------- only used for children
	// How mother sees a child:
	public bool isChildConnected; // Should be connected
	public Vector2i childRootMapPosition; //As seen from mothers frame of reference
	public int childRootBindCardinalIndex; //As seen from mothers frame of reference

	// Load Save
	private SoulReferenceData soulReferenceData = new SoulReferenceData();

	public SoulReferenceData UpdateData() {
		soulReferenceData.id = id;
		soulReferenceData.isChildConnected = isChildConnected;
		soulReferenceData.childRootMapPosition = childRootMapPosition;
		soulReferenceData.childRootBindCardinalIndex = childRootBindCardinalIndex;

		return soulReferenceData;
	}

	public void ApplyData(SoulReferenceData soulReferenceData) {
		id = soulReferenceData.id;
		isChildConnected = soulReferenceData.isChildConnected;
		childRootMapPosition = soulReferenceData.childRootMapPosition;
		childRootBindCardinalIndex = soulReferenceData.childRootBindCardinalIndex;
	}
	// ^ Load Save ^
}