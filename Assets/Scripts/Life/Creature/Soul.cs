using System.Collections.Generic;

// TODO: Make the souls connencted. That is: one souls mothers child is the same soul
public class Soul {
	public string id = string.Empty;
	
	//Don't store on disc, because we can't store references
	private Creature m_creature;
	public Creature creature {
		get {
			return m_creature;
		}
		set {
			m_creature = value;
		}
	}
	// Relatives
	public Soul mother;

	public List<Soul> children = new List<Soul>();

	public Soul(string id) {
		this.id = id;
	}

	public void UpdateRefFromId() {
		if (creature == null) {
			creature = World.instance.life.GetCreature(id);
		}
	}

	public bool isConnected; // Should be connected
	public Vector2i childRootMapPosition; //As seen from mothers frame of reference
	public int childRootBindCardinalIndex; //As seen from mothers frame of reference

	public bool hasMother {
		get {
			return mother != null;
		}
	}

	public bool hasChild {
		get {
			return children.Count > 0;
		}
	}

	public Soul GetChildById(string id) {
		return children.Find(c => c.id == id);
	}
}