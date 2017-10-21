using System.Collections.Generic;
using UnityEngine;

public class Child {
	//Store on disc
	public string id;
	public bool isConnected;
	public Vector2i placentaMapPosition; //map position where mother had cell which gave rise to me

	//Don't store
	public Creature creature;
	public bool isConnectionDirty = true; // true until we hav connected me (using springs) to mother

	public List<Cell> GetNeighboursInMother(CellMap motherCellMap) {
		Debug.Assert(!isReferenceDirty, "Trying to ask for stuff from a dirty Child ");
		return motherCellMap.GetGridNeighbourCell(placentaMapPosition);
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