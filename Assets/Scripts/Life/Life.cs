using UnityEngine;
using System.Collections.Generic;
using System;

public class Life : MonoBehaviour {
	public CellPool cellPool;
	public GeneCellPool geneCellPool;
	public VeinPool veinPool;
	public EdgePool edgePool;

	public Creature creaturePrefab;
	public Animator creatureDeathEffectPrefab;
	public Animator creatureBirthEffectPrefab;
	public Animator creatureAddEffectPrefab;

	private IdGenerator idGenerator = new IdGenerator();
	private Dictionary<string, Creature> creatureDictionary = new Dictionary<string, Creature>();
	private List<Creature> creatureList = new List<Creature>(); // All enbodied creatures (the once that we can see and play with)

	private Dictionary<string, Soul> soulDictionary = new Dictionary<string, Soul>();
	private List<Soul> soulListUnupdated = new List<Soul>(); //All creature containers, count allways >= number of creatures, since each creature has a container
	private List<Soul> soulListUpdated = new List<Soul>();
	public int soulsLostCount { get; private set; }
	public int soulsDeadButUsedCount { get; private set; }
	[HideInInspector]
	public int sterileKilledCount;

	//debug
	public int deletedCellCount = 0;

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
	public void FertilizeCreature(Cell eggCell, bool playEffects, ulong worldTicks, bool wasForced) {
		Debug.Assert(eggCell is EggCell, "You are not allowed to fertilize non Egg cell");

		if (playEffects && GlobalPanel.instance.effectsPlaySound.isOn && CameraUtils.IsObservedLazy(eggCell.position, GlobalSettings.instance.orthoMaxHorizonFx)) {
			Audio.instance.EggCellFertilize(CameraUtils.GetEffectStrengthLazy());
		}

		if (playEffects) {
			Animator birth = Instantiate(creatureBirthEffectPrefab, eggCell.position, Quaternion.Euler(0f, 0f, 0f));
		}

		Creature mother = eggCell.creature;

		// remove cell at childs origin location
		float eggEnergy = eggCell.energy;

		//Do we want to have a cool down for egg or not?
		mother.KillCell(eggCell, false, worldTicks); //When deleting egg cell other creatures connected, will come loose since neighbours are updated from mothers cellMap 

		// Spawn child at egg cell location
		Creature child = InstantiateCreature(); // Will create soul as well
		child.bornTick = worldTicks; //The time of birth is the time when the egg is fertilized
		child.creation = CreatureCreationEnum.Born;
		child.generation = mother.generation + 1;

		// Let there be evolution, and there was evolution
		//child.GenerateEmbryo(mother.genotype.genome, eggCell.position, eggCell.heading);
		//.GetMutatedClone(0.2f)
		child.GenerateEmbryo(mother.genotype.GetMutatedClone(GlobalSettings.instance.mutation.masterMutationStrength), eggCell.position, eggCell.heading); //Mutation Hack
		child.phenotype.originCell.energy = eggEnergy;
		child.phenotype.originCell.originDetatchMode = eggCell.eggCellDetatchMode;
		child.phenotype.originCell.originDetatchSizeThreshold = eggCell.eggCellDetatchSizeThreshold;
		child.phenotype.originCell.originDetatchEnergyThreshold = eggCell.eggCellDetatchEnergyThreshold; // form mothers eggCell to childs origin

		Soul motherSoul = GetSoul(mother.id);
		Soul childSoul = GetSoul(child.id);

		AddChildSoulImmediateSafe(motherSoul, childSoul, eggCell.mapPosition, eggCell.bindCardinalIndex, true);
		SetMotherSoulImmediateSafe(childSoul, motherSoul);

		PhenotypePanel.instance.MakeDirty();
		CreatureSelectionPanel.instance.MakeDirty();

		//Sometimes child origin is placed with spring too far from mother's placent this update might fix this problem
		UpdateStructure();

		if (wasForced) {
			CreatureSelectionPanel.instance.Select(mother, mother.phenotype.originCell);
		}
		CreatureSelectionPanel.instance.UpdateSelectionCluster();
	}

	public void KillAllCreaturesAndSouls() {
		List<Creature> toKill = new List<Creature>(creatureList);
		foreach (Creature creature in toKill) {
			KillCreatureSafe(creature, false);
		}
		creatureDictionary.Clear();
		creatureList.Clear();

		soulDictionary.Clear();
		soulListUnupdated.Clear();
		soulListUpdated.Clear();

		soulsLostCount = 0;
		soulsDeadButUsedCount = 0;
	}

	//When pressing delete, use effects
	public void KillCellSafe(Cell cell, ulong worldTicks) {
		if (cell.isOrigin) {
			KillCreatureSafe(cell.creature, true);
		} else {
			cell.creature.KillCell(cell, true, worldTicks);
		}
	}

	//This is the only way, where the creature GO is deleted
	public void KillCreatureSafe(Creature creature, bool playEffects) {
		
		creature.DetatchFromMother(false, true);
		foreach(Soul childSoul in creature.childSouls) {
			if (childSoul.creatureReference.creature != null) {
				childSoul.creatureReference.creature.DetatchFromMother(false, true);
			}
		}

		if (playEffects) {
			Animator birth = Instantiate(creatureDeathEffectPrefab, creature.phenotype.originCell.position, Quaternion.Euler(0f, 0f, 0f));
		}

		creature.KillAllCells(true); // for the fx :)

		creature.OnRecycle(); //Not only when using creature pool

		//TODO: Return root cell to pool
		//This is the only place where ta creature is ultimatly destroyed
		//Are there cells still left on creature?
		Cell[] forgottenCells = creature.phenotype.cellsTransform.GetComponents<Cell>();
		deletedCellCount += forgottenCells.Length;
		Cell[] forgottenGeneCells = creature.genotype.geneCellsTransform.GetComponents<Cell>();
		deletedCellCount += forgottenGeneCells.Length;

		Destroy(creature.gameObject); //TODO: return it to pool instead
		//CreaturePool.instance.Recycle(creature);
		creatureDictionary.Remove(creature.id);
		creatureList.Remove(creature);

		RemoveUnusedSouls();


		PhenotypePanel.instance.MakeDirty(); // Update cell text with fewer cells
		CreatureSelectionPanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();
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
			if (c.IsPhenotypePartlyInside(area)) {
				insideList.Add(c);
			}
		}
		return insideList;
	}

	public List<Creature> GetGenotypesInside(Rect area) {
		List<Creature> insideList = new List<Creature>();
		foreach (Creature c in creatureList) {
			if (c.IsGenotypePartlyInside(area)) {
				insideList.Add(c);
			}
		}
		return insideList;
	}

	public Creature SpawnCreatureJellyfish(Vector2 position, float heading, ulong bornTick) {
		Creature creature = InstantiateCreature();
		creature.bornTick = bornTick;
		creature.GenerateJellyfish(position, heading);
		creature.creation = CreatureCreationEnum.Forged;
		creature.generation = 1;

		SpawnAddEffect(position);

		return creature;
	}

	public Creature SpawnCreatureSimple(Vector3 position, float heading, ulong bornTick) {
		Creature creature = InstantiateCreature();
		creature.bornTick = bornTick;
		creature.GenerateSimple(position, heading);
		creature.creation = CreatureCreationEnum.Forged;
		creature.generation = 1;

		SpawnAddEffect(position);

		return creature;
	}

	public Creature SpawnCreatureFreak(Vector3 position, float heading, ulong bornTick) {
		Creature creature = InstantiateCreature();
		creature.bornTick = bornTick;
		creature.GenerateFreak(position, heading);
		creature.creation = CreatureCreationEnum.Forged;
		creature.generation = 1;

		SpawnAddEffect(position);

		return creature;
	}

	public Creature SpawnCreatureMergling(List<Gene[]> genomes, Vector3 position, float heading, ulong bornTick) {
		Creature creature = InstantiateCreature();
		creature.bornTick = bornTick;
		creature.GenerateMergling(genomes, position, heading);
		creature.creation = CreatureCreationEnum.Forged;
		creature.generation = 1;

		creature.hasPhenotypeCollider = false;
		creature.hasGenotypeCollider = false;

		return creature;
	}

	public Creature SpawnCreatureCopy(Creature original, ulong bornTick) {
		Creature clone = InstantiateCreature();
		string id = clone.id;
		clone.Clone(original);
		clone.id = id;
		clone.nickname += " (Copy)";
		clone.hasPhenotypeCollider = false;
		clone.hasGenotypeCollider = false;
		clone.soul = null;
		clone.creation = CreatureCreationEnum.Cloned;
		//Let generation be same as mothers

		clone.bornTick = bornTick;

		return clone;
	}

	private void SpawnAddEffect(Vector2 position) {
		Animator birth = Instantiate(creatureAddEffectPrefab, position, Quaternion.Euler(0f, 0f, 0f));
	}

	private void SpawnBirthEffect(Vector2 position) {
		Animator birth = Instantiate(creatureBirthEffectPrefab, position, Quaternion.Euler(0f, 0f, 0f));
	}

	private Creature InstantiateCreature() {
		string id = idGenerator.GetUniqueId();
		if (creatureDictionary.ContainsKey(id)) {
			throw new System.Exception("Generated ID was not unique.");
		}

		return InstantiateCreature(id);
	}

	private Creature InstantiateCreature(String id) {
		Creature creature = (Instantiate(creaturePrefab, Vector3.zero, Quaternion.identity) as Creature); //TODO: borrow from pool instead
		creature.name = "Creature " + id;
		//Creature creature = CreaturePool.instance.Borrow();
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
		lifeData.sterileKilledCount = sterileKilledCount;

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
		sterileKilledCount = lifeData.sterileKilledCount;

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

	private int phenotypePanelTicks;
	private int killSterileCreaturesTicks;

	// Phenotype only, updated from FixedUpdate
	// Everything that needs to be updated as the biological clock is ticking, wings, cell tasks, energy
	private List<Creature> killCreatureList = new List<Creature>();

	public void UpdatePhysics(ulong worldTicks) {
		//kill of weak cells / creatures

		//Ticks 
		phenotypePanelTicks++;
		if (phenotypePanelTicks >= GlobalSettings.instance.quality.phenotypePanelTickPeriod) {
			phenotypePanelTicks = 0;
		}

		killSterileCreaturesTicks++;
		if (killSterileCreaturesTicks >= GlobalSettings.instance.quality.killSterileCreaturesTickPeriod) {
			killSterileCreaturesTicks = 0;
		}

		// ^ Ticks ^
		for (int index = 0; index < creatureList.Count; index++) {
			creatureList[index].UpdatePhysics(worldTicks);
		}

		killCreatureList.Clear();

		//TODO: dont do it every tick
		for (int index = 0; index < creatureList.Count; index++) {
			if (creatureList[index].phenotype.isAlive) {
				if (creatureList[index].UpdateKillWeakCells(worldTicks)) {
					killCreatureList.Add(creatureList[index]);
				}
			} else {
				killCreatureList.Add(creatureList[index]);
			}
		}
		
		if (killSterileCreaturesTicks == 0) {
			for (int index = 0; index < creatureList.Count; index++) {
				if (creatureList[index].GetAge(worldTicks) > GlobalSettings.instance.phenotype.maxAgeAsChildless && creatureList[index].childSoulCount == 0) {
					killCreatureList.Add(creatureList[index]);
					sterileKilledCount++;
				}
			}
		}

		for (int index = 0; index < killCreatureList.Count; index++) {
			KillCreatureSafe(killCreatureList[index], true);
		}
		
		if (phenotypePanelTicks == 0) {
			if (CreatureSelectionPanel.instance.hasSoloSelected) {
				PhenotypePanel.instance.MakeDirty();
				CellPanel.instance.MakeDirty();
			}
		}
	}

	public void OnDestroy() {
		//Destroy(cellPool.gameObject);
		//Destroy(geneCellPool.gameObject);
		//Destroy(veinPool.gameObject);
		//Destroy(edgePool.gameObject);
	}

	// ^ Update ^
}
