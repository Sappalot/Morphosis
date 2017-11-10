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
	public void FertilizeCreature(Cell eggCell, bool playEffects = false) {
		Debug.Assert(eggCell is EggCell, "You are not allowed to fertilize non Egg cell");

		if (playEffects) {
			Audio.instance.EggCellFertilize();
		}

		Creature mother = eggCell.creature;

		// Q: What happens when 2 children, attatched to same mother, grows into each other (prio??), A: Let them grow as long as ther is room for each new cell. Probe for room firstm, then build?

		// remove cell at childs root location
		mother.DeleteCellButRoot(eggCell); //When deleting egg cell other creatures connected, will come loose since neighbours are updated from mothers cellMap 

		// Spawn child at egg cell location
		Creature child = InstantiateCreature(); // Will create soul as well

		// Let there be evolution, and ther ewas evolution
		//child.GenerateEmbryo(mother.genotype.genome, eggCell.position, eggCell.heading);
		//.GetMutatedClone(0.2f)
		child.GenerateEmbryo(mother.genotype.genome, eggCell.position, eggCell.heading); //Mutation Hack

		Soul motherSoul = GetSoul(mother.id);
		Soul childSoul = GetSoul(child.id);

		motherSoul.AddChildSoulImmediate(childSoul, eggCell.mapPosition, eggCell.bindCardinalIndex, true);
		childSoul.SetMotherSoulImmediate(motherSoul);

		//eggCell.creature.DetatchFromMother(); //HACK detatch when born test

		PhenotypePanel.instance.MakeDirty();
		CreatureSelectionPanel.instance.MakeDirty();
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

	public void DeleteCell(Cell cell) {
		if (cell.isRoot) {
			DeleteCreature(cell.creature);
		} else {
			cell.creature.DeleteCellButRoot(cell, true);
		}
	}

	public void DeleteCreature(Creature creature) {
		Debug.Log("Kill Creature: " + creature.id);
		creature.DetatchFromMother(true);
		foreach(Soul childSoul in creature.childSouls) {
			if (childSoul.hasCreature) {
				childSoul.creature.DetatchFromMother(true);
			}
		}

		creature.DeleteAllCells(); // for the fx :)

		CreatureSelectionPanel.instance.RemoveFromSelection(creature);

		Destroy(creature.gameObject);
		creatureDictionary.Remove(creature.id);
		creatureList.Remove(creature);
		
		PhenotypePanel.instance.MakeDirty(); // Update cell text with fewer cells
		CreatureSelectionPanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();

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
			creatures[index].MakeDirtyGraphics();
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

	// Allways, updated from Update
	// Keeping graphics up to date, creature selection, cell selection, flipArrows, edges
	public void UpdateGraphics() {
		for (int index = 0; index < creatureList.Count; index++) {
			creatureList[index].UpdateGraphics();
		}
	}

	// If (editPhenotype) updated from FixedUpdate
	// If (editGenotype) updated from FixedUpdate
	// Everything that needs to be updated when genome is changed, cells are removed, cells are added, creatures are spawned, creatures die
	public void UpdateStructure() {
		for (int index = 0; index < soulList.Count; index++) {
			soulList[index].UpdateReferences();
		}

		foreach (Creature c in creatureList) {
			c.FetchSoul();
			c.UpdateStructure();
		}
		
	}


	// Phenotype only, updated from FixedUpdate
	// Everything that needs to be updated as the biological clock is ticking, wings, cell tasks, energy
	private List<Creature> killCreatureList = new List<Creature>();

	public void UpdatePhysics(float fixedTime) {
		//kill of weak cells / creatures
		killCreatureList.Clear();
		for (int index = 0; index < creatureList.Count; index++) {
			if (creatureList[index].UpdateKillWeakCells()) {
				killCreatureList.Add(creatureList[index]);
			}
		}
		for (int index = 0; index < killCreatureList.Count; index++) {
			DeleteCreature(killCreatureList[index]);
		}

		//
		for (int index = 0; index < creatureList.Count; index++) {
			creatureList[index].UpdatePhysics(fixedTime);
		}
		
	}

	// ^ Update ^
}
