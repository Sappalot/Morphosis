using UnityEngine;
using SerializerFree;
using SerializerFree.Serializers;
public class Morphosis : MonoSingleton<Morphosis> {
	public IdGenerator idGenerator = new IdGenerator();
	public new Camera camera;

	// TODO: Move to Morphosis, since they are used in freezer as well
	public CreaturePool creaturePool;
	public CellPool cellPool;
	public GeneCellPool geneCellPool;
	public VeinPool veinPool;
	public EdgePool edgePool;
	public RelationArrows relationArrows;

	private void Start () {
		// TODO: load last world
		// Creature id's will be set from file
		World.instance.Init(); // Just 1 world, lots of work keeping several instances at once

		Restart();
	}

	private void Update() {
		World.instance.UpdateGraphics();
		Freezer.instance.UpdateGraphics();
	}

	private void FixedUpdate() {
		World.instance.UpdatePhysics();
		Freezer.instance.UpdatePhysics();
	}

	public void OnExit() {
		Freezer.instance.Save();
	}

	public void MoveFreezerCreatureIdsToFreeRange() {
		idGenerator.RenameToUniqueIds(Freezer.instance.creatures);
	}

	public void Restart() {
		idGenerator.Restart();
		Freezer.instance.Load();
		World.instance.Restart();
	}

	public void LoadWorld(string filename) {
		Restart();
		World.instance.Load(filename);
		instance.MoveFreezerCreatureIdsToFreeRange();
	}
}