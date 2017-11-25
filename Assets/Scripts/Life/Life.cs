using UnityEngine;
using System.Collections.Generic;
using System;

public class Life : MonoSingleton<Life> {
	public Creature creaturePrefab;

	private IdGenerator idGenerator = new IdGenerator();
	private Dictionary<string, Creature> creatureDictionary = new Dictionary<string, Creature>();
	private List<Creature> creatureList = new List<Creature>(); // All enbodied creatures (the once that we can see and play with)

	private Dictionary<string, Soul> soulDictionary = new Dictionary<string, Soul>();
	private List<Soul> soulListUnupdated = new List<Soul>(); //All creature containers, count allways >= number of creatures, since each creature has a container
	private List<Soul> soulListUpdated = new List<Soul>();
	public int soulsLostCount { get; private set; }
	public int soulsDeadButUsedCount { get; private set; }

	public int soulUpdatedCount {
		get {
			return soulListUpdated.Count;
		}
	}

	public int soulUnupdatedCount {
		get {
			return soulListUnupdated.Count;
		}
	}

	public int creatureCount {
		get {
			return creatureList.Count;
		}
	}

	public void KillSoulIfUnneeded(Soul soul) {
		if (!IsAnyOfMyCloseRelativesAlive(soul)) {
			KillSoul(soul);
			soulsLostCount++;
		}
	}

	private bool IsAnyOfMyCloseRelativesAlive(Soul soul) {
		//is living mother refering to me
		if (HasCreature(soul.motherSoulReference.id)) {
			return true;
		}

		//child living child refering to me
		foreach (SoulReference child in soul.childSoulReferences) {
			if (HasCreature(child.id)) {
				return true;
			}
		}

		return false;
	}

	private void KillSoul(Soul soul) {
		//remove all references to me
		soul.OnKill();

		if (soulDictionary.ContainsValue(soul)) {
			soulDictionary.Remove(soul.id);
		}
		if (soulListUnupdated.Contains(soul)) {
			soulListUnupdated.Remove(soul);
		}
		if (soulListUpdated.Contains(soul)) {
			soulListUpdated.Remove(soul);
		}

		//Note soul sill remains in other souls refering to it
	}

	public void UpdateSoulReferences() {
		List<Soul> moveSouls = new List<Soul>();
		moveSouls.Clear();
		for (int index = 0; index < soulListUnupdated.Count; index++) {
			if (soulListUnupdated[index].UpdateReferences()) {
				moveSouls.Add(soulListUnupdated[index]);
			}
		}
		foreach (Soul s in moveSouls) {
			MoveToUpdated(s);
		}
	}

	private void MoveToUpdated(Soul soul) {
		
		if (soulListUpdated.Contains(soul)) {
			//Debug.Log("Ooops! Trying to move soul: " + soul.id + " to soulListUpdated when it's allready there!");
		} else {
			soulListUnupdated.Remove(soul);
			soulListUpdated.Add(soul);
		}
	}

	private void MoveToUnupdated(Soul soul) {
		
		if (soulListUnupdated.Contains(soul)) {
			//Debug.Log("Ooops! Trying to move soul: " + soul.id + " to soulListUnupdated when it's allready there!");
		} else {
			soulListUpdated.Remove(soul);
			soulListUnupdated.Add(soul);
		}
	}

	public void SetMotherSoulImmediateSafe(Soul childSoul, Soul motherSoul) {
		childSoul.SetMotherSoulImmediate(motherSoul);
		MoveToUnupdated(motherSoul);
		MoveToUnupdated(childSoul);
	}

	public void AddChildSoulImmediateSafe(Soul motherSoul, Soul childSoul, Vector2i originMapPosition, int originBindCardinalIndex, bool isConnected) {
		motherSoul.AddChildSoulImmediate(childSoul, originMapPosition, originBindCardinalIndex, isConnected);
		MoveToUnupdated(motherSoul);
		MoveToUnupdated(childSoul);
	}

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
	public void FertilizeCreature(Cell eggCell, bool playEffects, float? fixedTime) {
		Debug.Assert(eggCell is EggCell, "You are not allowed to fertilize non Egg cell");

		if (playEffects) {
			Audio.instance.EggCellFertilize();
		}

		Creature mother = eggCell.creature;

		// Q: What happens when 2 children, attatched to same mother, grows into each other (prio??), A: Let them grow as long as ther is room for each new cell. Probe for room firstm, then build?

		// remove cell at childs origin location
		float eggEnergy = eggCell.energy;
		mother.KillCell(eggCell, false, fixedTime); //When deleting egg cell other creatures connected, will come loose since neighbours are updated from mothers cellMap 

		// Spawn child at egg cell location
		Creature child = InstantiateCreature(); // Will create soul as well

		// Let there be evolution, and ther ewas evolution
		//child.GenerateEmbryo(mother.genotype.genome, eggCell.position, eggCell.heading);
		//.GetMutatedClone(0.2f)
		child.GenerateEmbryo(mother.genotype.GetMutatedClone(0.1f), eggCell.position, eggCell.heading); //Mutation Hack
		child.phenotype.originCell.energy = eggEnergy;

		Soul motherSoul = GetSoul(mother.id);
		Soul childSoul = GetSoul(child.id);

		AddChildSoulImmediateSafe(motherSoul, childSoul, eggCell.mapPosition, eggCell.bindCardinalIndex, true);
		SetMotherSoulImmediateSafe(childSoul, motherSoul);

		PhenotypePanel.instance.MakeDirty();
		CreatureSelectionPanel.instance.MakeDirty();
	}

	public void KillAllCreaturesAndSouls() {
		foreach (Creature creature in creatureList) {
			Destroy(creature.gameObject);
		}
		creatureDictionary.Clear();
		creatureList.Clear();

		soulDictionary.Clear();
		soulListUnupdated.Clear();
		soulListUpdated.Clear();

		soulsLostCount = 0;
		soulsDeadButUsedCount = 0;
	}

	public void KillCellSafe(Cell cell) {
		if (cell.isOrigin) {
			KillCreatureSafe(cell.creature);
		} else {
			cell.creature.KillCell(cell, true, null);
		}
	}

	//This is the only way, where the creature GO is deleted
	public void KillCreatureSafe(Creature creature) {
		creature.DetatchFromMother(true);
		foreach(Soul childSoul in creature.childSouls) {
			if (childSoul.creatureReference.creature != null) {
				childSoul.creatureReference.creature.DetatchFromMother(true);
			}
		}

		creature.KillAllCells(); // for the fx :)

		Destroy(creature.gameObject);
		creatureDictionary.Remove(creature.id);
		creatureList.Remove(creature);

		////remove my soul, if unused
		//KillSoulIfUnneeded(GetSoul(creature.soul.id));

		////remove mother's soul, if unused
		//if (creature.soul.motherSoulReference.id != string.Empty && HasSoul(creature.soul.motherSoulReference.id)) {
		//	KillSoulIfUnneeded(GetSoul(creature.soul.motherSoulReference.id));
		//}

		//remove children's souls if unused

		RemoveUnusedSouls();

		PhenotypePanel.instance.MakeDirty(); // Update cell text with fewer cells
		CreatureSelectionPanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();

		// Note the soul will remain :)
		// Q: will we keep souls forever? This will cause a really slow application over night
	}

	List<Soul> unusedSoulsToKill = new List<Soul>();
	private void RemoveUnusedSouls() {
		UpdateSoulReferences();

		unusedSoulsToKill.Clear();
		soulsDeadButUsedCount = 0;
		foreach (Soul soul in soulListUpdated) {
			if (!HasCreature(soul.id)) {
				if (!IsAnyOfMyCloseRelativesAlive(soul)) {
					unusedSoulsToKill.Add(soul);
				} else {
					soulsDeadButUsedCount++;
				}
			} 
		}
		foreach (Soul soul in unusedSoulsToKill) {
			KillSoulIfUnneeded(soul);
		}		
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
		soulListUnupdated.Add(soul);

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
		UpdateSoulReferences();

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
		lifeData.soulsLostCount = soulsLostCount;


		for (int index = 0; index < soulListUpdated.Count; index++) {
			Soul soul = soulListUpdated[index];
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
		KillAllCreaturesAndSouls();
		for (int index = 0; index < lifeData.creatureList.Count; index++) {
			CreatureData creatureData = lifeData.creatureList[index];
			Creature creature = InstantiateCreature(creatureData.id); // Creatres soul as
			creature.ApplyData(creatureData);
		}

		// Create all Souls
		soulDictionary.Clear();
		soulListUnupdated.Clear();
		soulListUpdated.Clear();

		for (int index = 0; index < lifeData.soulList.Count; index++) {
			SoulData solulData = lifeData.soulList[index];
			Soul newSoul = new Soul(solulData.id);
			newSoul.ApplyData(solulData);
			soulDictionary.Add(newSoul.id, newSoul);
			soulListUnupdated.Add(newSoul);
		}
		soulsLostCount = lifeData.soulsLostCount;

		RemoveUnusedSouls();
		UpdateSoulReferences();
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
		UpdateSoulReferences();

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
			if (creatureList[index].UpdateKillWeakCells(fixedTime) || !creatureList[index].phenotype.isAlive) {
				killCreatureList.Add(creatureList[index]);
			}
		}
		for (int index = 0; index < killCreatureList.Count; index++) {
			KillCreatureSafe(killCreatureList[index]);
		}

		//
		for (int index = 0; index < creatureList.Count; index++) {
			creatureList[index].UpdatePhysics(fixedTime);
		}
		
	}

	// ^ Update ^
}
