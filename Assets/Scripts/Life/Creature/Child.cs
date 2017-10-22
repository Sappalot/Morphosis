using System.Collections.Generic;
using UnityEngine;

public class Child {
	//Store on disc
	public string id;
	public bool isConnected; // Shild should be connected (will be connected when isConnectionDirty == false)
	public Vector2i rootMapPosition; //In mothers frame of reference
	public int rootBindCardinalIndex; //In mothers frame of reference

	//Don't store
	public Creature creature;
	public bool isConnectionDirty = true; // true until we have connected me (using springs) to mother

	public List<Cell> GetNeighboursInMother(CellMap motherCellMap) {
		Debug.Assert(!isReferenceDirty, "Trying to ask for stuff from a dirty Child ");
		return motherCellMap.GetGridNeighbourCell(rootMapPosition);
	}

	public bool isReferenceDirty {
		get {
			return creature == null;
		}
	}

	public Child(string id) {
		this.id = id;
	}

	public void UpdateCreatureFromId() {
		if (creature == null) {
			creature = World.instance.life.GetCreature(id);
		}
	}
}