using UnityEngine;
using System.Collections.Generic;
using System;

public class Life : MonoSingleton<Life> {
	public Creature creaturePrefab;

	private IdGenerator idGenerator = new IdGenerator();
	private Dictionary<string, Creature> creatureDictionary = new Dictionary<string, Creature>();
	private List<Creature> creatureList = new List<Creature>(); // All enbodied creatures (the once that we can see and play with)

	//--

	private Dictionary<string, Soul> soulDictionary = new Dictionary<string, Soul>();
	public List<Soul> soulList = new List<Soul>(); //All creature containers, count allways >= number of creatures, since each creature has a container

	public Soul GetSoul(string id) {
		return soulDictionary[id];
	}

	public bool HasSoul(string id) {
		return soulDictionary.ContainsKey(id);
	}

	//--

	public string GetUniqueIdStamp() {
		return idGenerator.GetUniqueId();
	}

	public List<Creature> creatures	{
		get {
			return creatureList;
		}
	}

	public bool HasCreature(string id) {
		return creatureDictionary.ContainsKey(id);
	}

	public Creature GetCreature(string id) {
		return creatureDictionary[id];
	}

	// TODO MOve to creature ?
	public void FertilizeCreature(Cell eggCell) {
		Debug.Assert(eggCell is EggCell, "You are not allowed to fertilize non Egg cell");
		Creature mother = eggCell.creature;

		// Q: What happens when 2 children, attatched to same mother, grows into each other (prio??), A: Let them grow as long as ther is room for each new cell. Probe for room firstm, then build?

		// remove cell at childs root location
		mother.DeleteCell(eggCell); //When deleting egg cell other creatures connected, will come loose since neighbours are updated from mothers cellMap 

		// Spawn child at egg cell location
		Creature child = InstantiateCreature(); // Will create soul as well
		child.GenerateEmbryo(mother.genotype.genome, eggCell.position, eggCell.heading);

		Soul motherSoul = GetSoul(mother.id);
		Soul childSoul = GetSoul(child.id);

		motherSoul.AddChildSoulImmediate(childSoul, eggCell.mapPosition, eggCell.bindCardinalIndex, true);
		childSoul.SetMotherSoulImmediate(motherSoul);

		PhenotypePanel.instance.MakeDirty();
		CreatureSelectionPanel.instance.MakeDirty();
	}

	public void DetatchCreature() {

	}

	public void DeleteAll() {
		foreach (Creature creature in creatureList) {
			Destroy(creature.gameObject);
		}
		creatureDictionary.Clear();
		creatureList.Clear();

		soulDictionary.Clear();
		soulList.Clear();
	}

	public void DeleteCreature(Creature creature) {
		creature.DetatchFromMother();
		foreach(Soul childSoul in creature.childSouls) {
			if (childSoul.hasCreature) {
				childSoul.creature.DetatchFromMother();
			}
		}

		Destroy(creature.gameObject);
		creatureDictionary.Remove(creature.id);
		creatureList.Remove(creature);

		// Note the soul will remain :)
		// Q: will we keep souls forever?
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
		clone.soul = null;

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

		Soul soul = new Soul(id);
		
		soulDictionary.Add(id, soul);
		soulList.Add(soul);

		//creature.soul = soul; //The right Soul will find its way to the creature during update otherwise, Setting it here will cause troubble!
		return creature;
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

	// Load Save

	private LifeData lifeData = new LifeData();

	// Save
	public LifeData UpdateData() {
		lifeData.lastId = idGenerator.number;

		//Creatures
		lifeData.creatureList.Clear();
		lifeData.creatureDictionary.Clear();

		for (int index = 0; index < creatureList.Count; index++) {
			Creature creature = creatureList[index];
			CreatureData data = creature.UpdateData();
			lifeData.creatureList.Add(data);
			lifeData.creatureDictionary.Add(data.id, data);
		}

		//Souls
		lifeData.soulList.Clear();
		lifeData.soulDictionary.Clear();

		for (int index = 0; index < soulList.Count; index++) {
			Soul soul = soulList[index];
			SoulData data = soul.UpdateData();
			lifeData.soulList.Add(data);
			lifeData.soulDictionary.Add(data.id, data);
		}

		return lifeData;
	}

	// Load
	public void ApplyData(LifeData lifeData) {
		idGenerator.number = lifeData.lastId;

		// Create all creatures
		DeleteAll();
		for (int index = 0; index < lifeData.creatureList.Count; index++) {
			CreatureData creatureData = lifeData.creatureList[index];
			Creature creature = InstantiateCreature(creatureData.id); // Creatres soul as
			creature.ApplyData(creatureData);
		}

		// Create all Souls
		soulDictionary.Clear();
		soulList.Clear();
		for (int index = 0; index < lifeData.soulList.Count; index++) {
			SoulData solulData = lifeData.soulList[index];
			Soul newSoul = new Soul(solulData.id);
			newSoul.ApplyData(solulData);
			soulDictionary.Add(newSoul.id, newSoul);
			soulList.Add(newSoul);
		}
	}

	// ^ Load Save ^

	// Update

	public void EvoUpdate() {

		for (int index = 0; index < soulList.Count; index++) {
			soulList[index].UpdateReferences();
		}

		for (int index = 0; index < creatureList.Count; index++) {
			creatureList[index].EvoUpdate();
		}
	}

	public void EvoFixedUpdate(float fixedTime) {
		for (int index = 0; index < creatureList.Count; index++) {
			creatureList[index].EvoFixedUpdate(fixedTime);
		}
	}

	// ^ Update ^
}
