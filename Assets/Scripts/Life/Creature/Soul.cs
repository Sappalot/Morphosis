using System.Collections.Generic;

public class Soul {
	//Don't store on disc, because we can't store references
	public Creature creature;

	//Store on disc
	public string id;

	public bool isConnected; // Should be connected (will be connected when isConnectionDirty == false)
	public Vector2i childRootMapPosition; //As seen from mothers frame of reference
	public int childRootBindCardinalIndex; //As seen from mothers frame of reference


	// Relatives
	private List<Soul> originals = new List<Soul>();
	public Soul mother;
	public List<Soul> children = new List<Soul>();

	public Soul(string id) {
		//same as creature
		this.id = id;
	}

	public bool hasMother {
		get {
			return mother != null;
		}
	}

	public bool isMotherDirty {
		get {
			return hasMother && mother.isDirty;
		}
	}

	public bool hasChild {
		get {
			return children.Count > 0;
		}
	}

	public bool isChildDirty {
		get {
			if (!hasChild) {
				return false;
			}
			foreach (Soul c in children) {
				if (c.isDirty) {
					return true;
				}
			}
			return false;
		}
	}

	public Soul GetChildById(string id) {
		return children.Find(c => c.id == id);
	}

	public bool isDirty {
		get {
			return creature == null;
		}
	}

	public void UpdateCreatureRefFromId() {
		if (creature == null) {
			creature = World.instance.life.GetCreature(id);
		}
	}
}