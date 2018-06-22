using UnityEngine;
using System.Collections.Generic;
using System;

public class Life : MonoBehaviour {
	public CreaturePool creaturePool;
	public CellPool cellPool;
	public GeneCellPool geneCellPool;
	public VeinPool veinPool;
	public EdgePool edgePool;

	public Creature creaturePrefab;

	private IdGenerator idGenerator = new IdGenerator();
	private Dictionary<string, Creature> creatureDictionary = new Dictionary<string, Creature>();
	private List<Creature> creatureList = new List<Creature>(); // All enbodied creatures (the once that we can see and play with)

	[HideInInspector]
	public int sterileKilledCount;

	public int creatureDeadCount { get; private set; }

	//debug
	[HideInInspector]
	public int deletedCellCount = 0;

	public int creatureAliveCount {
		get {
			return creatureList.Count;
		}
	}

	public int cellAliveCount {
		get {
			int count = 0;
			foreach (Creature c in creatures) {
				count += c.cellCount;
			}
			return count;
		}
	}

	public int GetCellAliveCount(CellTypeEnum type) {
		int count = 0;
		foreach (Creature c in creatures) {
			count += c.phenotype.GetCellCount(type);
		}
		return count;
	}

	public string GetUniqueIdStamp() {
		return idGenerator.GetUniqueId();
	}

	public List<Creature> creatures	{
		get {
			return creatureList;
		}
	}

	public bool HasCreature(string id) {
		if (id == null) { // MAkes it possible to load files from the soul era
			return false;
		}
		return creatureDictionary.ContainsKey(id);
	}

	public Creature GetCreature(string id) {
		if (HasCreature(id)) {
			return creatureDictionary[id];
		}
		return null;
	}

	// TODO MOve to creature ?
	public void FertilizeCreature(Cell eggCell, bool playEffects, ulong worldTicks, bool wasForced) {
		Debug.Assert(eggCell is EggCell, "You are not allowed to fertilize non Egg cell");

		if (playEffects && GlobalPanel.instance.soundCreatures.isOn && CameraUtils.IsObservedLazy(eggCell.position, GlobalSettings.instance.orthoMaxHorizonFx)) {
			Audio.instance.EggCellFertilize(CameraUtils.GetEffectStrengthLazy());
		}

		if (playEffects && GlobalPanel.instance.graphicsEffects.isOn) {
			EffectPlayer.instance.Play(EffectEnum.CreatureBorn, eggCell.position, 0f, CameraUtils.GetEffectScaleLazy());
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

		ChildData childData = new ChildData();
		childData.id = child.id; // a creature carying a child with an id that can not be found in life ==> child concidered dead to mother
		childData.isConnectedToMother = true; // Connected from the start
		childData.originMapPosition = eggCell.mapPosition; //As seen from mothers frame of reference
		childData.originBindCardinalIndex = eggCell.bindCardinalIndex; //As seen from mothers frame of reference
		mother.AddChild(childData);

		child.SetMother(mother.id);

		PhenotypePanel.instance.MakeDirty();
		CreatureSelectionPanel.instance.MakeDirty();

		//Sometimes child origin is placed with spring too far from mother's placent this update might fix this problem
		UpdateStructure();

		if (wasForced) {
			CreatureSelectionPanel.instance.Select(mother, mother.phenotype.originCell);
		}
		CreatureSelectionPanel.instance.UpdateSelectionCluster();
	}

	public void KillAllCreatures() {
		List<Creature> toKill = new List<Creature>(creatureList);
		foreach (Creature creature in toKill) {
			KillCreatureSafe(creature, false);
		}
		creatureDictionary.Clear();
		creatureList.Clear();
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
		foreach (Creature child in creature.GetChildren()) {
			child.DetatchFromMother(false, true);
		}

		if (playEffects && GlobalPanel.instance.graphicsEffects.isOn) {
			//Animator birth = Instantiate(creatureDeathEffectPrefab, creature.phenotype.originCell.position, Quaternion.Euler(0f, 0f, 0f));
			EffectPlayer.instance.Play(EffectEnum.CreatureDeath, creature.phenotype.originCell.position, 0f, CameraUtils.GetEffectScaleLazy());
		}

		creature.KillAllCells(true); // for the fx :)

		creature.OnRecycle(); //Not only when using creature pool
		creatureDeadCount++;

		//TODO: Return root cell to pool
		//This is the only place where ta creature is ultimatly destroyed
		//Are there cells still left on creature?
		Cell[] forgottenCells = creature.phenotype.cellsTransform.GetComponents<Cell>();
		deletedCellCount += forgottenCells.Length;
		Cell[] forgottenGeneCells = creature.genotype.geneCellsTransform.GetComponents<Cell>();
		deletedCellCount += forgottenGeneCells.Length;

		

		//This is the only place where creature is recycled / destroyed
		CreaturePool.instance.Recycle(creature);
		creatureDictionary.Remove(creature.id);
		creatureList.Remove(creature);

		PhenotypePanel.instance.MakeDirty(); // Update cell text with fewer cells
		CreatureSelectionPanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();
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
		World.instance.AddHistoryEvent(new HistoryEvent("+", false, Color.gray));
		return creature;
	}

	public Creature SpawnCreatureFreak(Vector3 position, float heading, ulong bornTick) {
		Creature creature = InstantiateCreature();
		creature.bornTick = bornTick;
		creature.GenerateFreak(position, heading);
		creature.creation = CreatureCreationEnum.Forged;
		creature.generation = 1;

		SpawnAddEffect(position);
		World.instance.AddHistoryEvent(new HistoryEvent("+", false, Color.gray));
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
		clone.creation = CreatureCreationEnum.Cloned;
		//Let generation be same as mothers

		clone.bornTick = bornTick;
		return clone;
	}

	private void SpawnAddEffect(Vector2 position) {
		if (GlobalPanel.instance.graphicsEffects.isOn) {
			EffectPlayer.instance.Play(EffectEnum.CreatureAdd, position, 0f, CameraUtils.GetEffectScaleLazy());
		}
	}

	private void SpawnBirthEffect(Vector2 position) {
		if (GlobalPanel.instance.graphicsEffects.isOn) {
			EffectPlayer.instance.Play(EffectEnum.CreatureBorn, position, 0f, CameraUtils.GetEffectScaleLazy());
		}
	}

	private Creature InstantiateCreature() {
		string id = idGenerator.GetUniqueId();
		if (creatureDictionary.ContainsKey(id)) {
			throw new System.Exception("Generated ID was not unique.");
		}

		return InstantiateCreature(id);
	}

	private Creature InstantiateCreature(String id) {
		//Creature creature = (Instantiate(creaturePrefab, Vector3.zero, Quaternion.identity) as Creature); //TODO: borrow from pool instead
		Creature creature = creaturePool.Borrow();
		creature.gameObject.SetActive(true);
		creature.name = "Creature " + id;
		
		creature.transform.parent = this.transform;
		creatureDictionary.Add(id, creature);
		creatureList.Add(creature);
		creature.id = id;
		creature.nickname = "Nick " + id; //dafault

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
		//UpdateSoulReferences();

		lifeData.lastId = idGenerator.number;

		//Creatures
		lifeData.creatureList.Clear();
		lifeData.creatureDictionary.Clear();
		lifeData.creatureDeadCount = creatureDeadCount;
		lifeData.sterileKilledCount = sterileKilledCount;

		for (int index = 0; index < creatureList.Count; index++) {
			Creature creature = creatureList[index];
			CreatureData data = creature.UpdateData();
			lifeData.creatureList.Add(data);
			lifeData.creatureDictionary.Add(data.id, data);
		}

		return lifeData;
	}

	// Load
	public void ApplyData(LifeData lifeData) {
		idGenerator.number = lifeData.lastId;

		// Create all creatures
		KillAllCreatures();
		for (int index = 0; index < lifeData.creatureList.Count; index++) {
			CreatureData creatureData = lifeData.creatureList[index];
			Creature creature = InstantiateCreature(creatureData.id); // Creatres soul as
			creature.ApplyData(creatureData);
		}
		creatureDeadCount = lifeData.creatureDeadCount;
		sterileKilledCount = lifeData.sterileKilledCount;

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
		foreach (Creature c in creatureList) {
			c.UpdateStructure();
		}
	}

	private int phenotypePanelTicks;
	private int killSterileCreaturesTicks;

	// Phenotype only, updated from FixedUpdate
	// Everything that needs to be updated as the biological clock is ticking, wings, cell tasks, energy
	private List<Creature> killCreatureList = new List<Creature>();

	public void UpdatePhysics(ulong worldTicks) {

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

		if (GlobalPanel.instance.physicsKillSterile.isOn) {
			if (killSterileCreaturesTicks == 0) {
				int sterileKilled = 0;
				for (int index = 0; index < creatureList.Count; index++) {
					if (creatureList[index].GetAge(worldTicks) > GlobalSettings.instance.phenotype.maxAgeAsChildless && !creatureList[index].HasChildrenIncDead()) {
						killCreatureList.Add(creatureList[index]);
						sterileKilled++;
					}
				}
				sterileKilledCount += sterileKilled;
				if (sterileKilled > 0) {
					World.instance.AddHistoryEvent(new HistoryEvent("SK: " + sterileKilled, false, Color.red));
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

	// ^ Update ^
}
