public class Mother {
	public string id;
	public Creature creature;
	public bool isConnected;

	public bool isReferenceDirty {
		get {
			return creature == null;
		}
	}

	public Mother(string id) {
		this.id = id;
	}

	public void UpdateCreatureFromId() {
		if (creature == null) {
			creature = World.instance.life.GetCreature(id);
		}
	}
}