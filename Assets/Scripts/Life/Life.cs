using UnityEngine;
using System.Collections.Generic;
using System;

public class Life : MonoBehaviour {
	public Creature creaturePrefab;

	private IdGenerator idGenerator = new IdGenerator();
	private Dictionary<string, Creature> creatureDictionary = new Dictionary<string, Creature>();
	private List<Creature> creatureList = new List<Creature>();

	public string GetUniqueIdStamp() {
		return idGenerator.GetUniqueId();
	}

	public List<Creature> creatures	{
		get {
			return creatureList;
		}
	}

	public Creature GetCreature(string id) {
		return creatureDictionary[id];
	}

	public void EvoUpdate() {
		for (int index = 0; index < creatureList.Count; index++) {
			creatureList[index].EvoUpdate();
		}
	}

	public void EvoFixedUpdate(float fixedTime) {
		for (int index = 0; index < creatureList.Count; index++) {
			creatureList[index].EvoFixedUpdate(fixedTime);
		}
	}

	public void FertilizeCreature(Cell eggCell) {
		Debug.Assert(eggCell is EggCell, "You are not allowed to fertilize non Egg cell");
		Creature mother = eggCell.creature;

		// Q: What happens when 2 children, attatched to same mother, grows into each other (prio??), Let them grow as long as ther is room for each new cell. Probe for room firstm, then build?

		// remove cell at childs root location
		mother.DeleteCell(eggCell); //When deleting egg cell other creatures connected, will come loose since neighbours are updated from mothers cellMap 

		// Spawn child at egg cell location
		Creature child = InstantiateCreature();
		child.GenerateEmbryo(mother.genotype.genome, eggCell.position, eggCell.heading);

		mother.SetChild(child.id, eggCell.mapPosition, eggCell.bindCardinalIndex, true);

		child.SetMother(mother.id, true);

		PhenotypePanel.instance.MakeDirty();
		CreatureSelectionPanel.instance.MakeDirty();
	}

	public void ReleaseCreature(Cell rootCell) {
		Debug.Assert(rootCell is EggCell, "You are not allowed to release child from non Egg cell");
		Creature creature = rootCell.creature;

		// TODO: remove mother's attatchment to child

		// TODO: remove child's childs attatchment to mother

		// TODO: let mother be able to grow back new egg cell

		// TODO: remove occupied mother spaces from child

	}

	public void DeleteAll() {
		foreach (Creature creature in creatureList) {
			Destroy(creature.gameObject);
		}
		creatureDictionary.Clear();
		creatureList.Clear();
	}

	public void DeleteCreature(Creature creature) {
		// Make all other creatures understand that i will be gone now!

		Destroy(creature.gameObject);
		creatureDictionary.Remove(creature.id);
		creatureList.Remove(creature);
	}

	public List<Creature> GetPhenotypesInside(Rect area) {
		List<Creature> insideList = new List<Creature>();
		foreach (Creature c in creatureList) {
			if (c.IsPhenotypeInside(area)) {
				insideList.Add(c);
			}
		}
		return insideList;
	}

	public List<Creature> GetGenotypesInside(Rect area) {
		List<Creature> insideList = new List<Creature>();
		foreach (Creature c in creatureList) {
			if (c.IsGenotypeInside(area)) {
				insideList.Add(c);
			}
		}
		return insideList;
	}

	public Creature SpawnCreatureJellyfish(Vector2 position, float heading) {
		Creature creature = InstantiateCreature();
		creature.GenerateJellyfish(position, heading);
		return creature;
	}

	public Creature SpawnCreatureSimple(Vector3 position, float heading) {
		Creature creature = InstantiateCreature();
		creature.GenerateSimple(position, heading);

		return creature;
	}

	public Creature SpawnCreatureFreak(Vector3 position, float heading) {
		Creature creature = InstantiateCreature();
		creature.GenerateFreak(position, heading);

		return creature;
	}

	public Creature SpawnCreatureMergling(List<Gene[]> genomes, Vector3 position, float heading) {
		Creature creature = InstantiateCreature();
		creature.GenerateMergling(genomes, position, heading);

		creature.hasPhenotypeCollider = false;
		creature.hasGenotypeCollider = false;
		return creature;
	}

	public Creature SpawnCreatureCopy(Creature original) {
		Creature clone = InstantiateCreature();
		string id = clone.id;
		clone.Clone(original);
		clone.id = id;
		clone.nickname += " (Copy)";
		clone.hasPhenotypeCollider = false;
		clone.hasGenotypeCollider = false;
		return clone;
	}

	private Creature InstantiateCreature() {
		string id = idGenerator.GetUniqueId();
		if (creatureDictionary.ContainsKey(id)) {
			throw new System.Exception("Generated ID was not unique.");
		}

		return InstantiateCreature(id);
	}

	private Creature InstantiateCreature(String id) {
		Creature creature = (GameObject.Instantiate(creaturePrefab, Vector3.zero, Quaternion.identity) as Creature);
		creature.transform.parent = this.transform;
		creatureDictionary.Add(id, creature);
		creatureList.Add(creature);
		creature.id = id;
		creature.nickname = "Nick " + id; //dafault
		return creature;
	}

	//data

	private LifeData lifeData = new LifeData();

	public LifeData UpdateData() {
		lifeData.lastId = idGenerator.number;
		lifeData.creatureList.Clear();
		lifeData.creatureDictionary.Clear();

		for (int index = 0; index < creatureList.Count; index++) {
			Creature creature = creatureList[index];
			CreatureData data = creature.UpdateData();
			lifeData.creatureList.Add(data);
			lifeData.creatureDictionary.Add(data.id, data);
		}

		return lifeData;
	}

	public void ApplyData(LifeData lifeData) {
		idGenerator.number = lifeData.lastId;

		DeleteAll();
		for (int index = 0; index < lifeData.creatureList.Count; index++) {
			CreatureData creatureData = lifeData.creatureList[index];
			Creature creature = InstantiateCreature(creatureData.id);
			creature.ApplyData(creatureData);
		}
	}

	public Cell GetCellAt(Vector2 position) {
		foreach (Creature creature in creatureList) {
			Cell found =  creature.GetCellAt(position);
			if (found != null) {
				return found;
			}
		}
		return null;
	}

	//----------------
	public void MakeAllCreaturesDirty() {
		for (int index = 0; index < creatures.Count; index++) {
			creatures[index].MakeDirty();
		}
	}
}
