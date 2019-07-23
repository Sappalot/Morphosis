using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Boo.Lang.Runtime;

public class Life : MonoBehaviour {

	private Dictionary<string, Creature> creatureDictionary = new Dictionary<string, Creature>();
	private List<Creature> creatureList = new List<Creature>(); // All enbodied creatures (the once that we can see and play with)

	[HideInInspector]
	public int creatureDeadCount { get; private set; }
	public int creatureDeadByAgeCount { get; private set; } // part of creatureDeadCount
	public int creatureDeadByBreakingCount { get; private set; } // part of creatureDeadCount
	public int creatureDeadByEscapingCount { get; private set; } // part of creatureDeadCount
	public int creatureDeadByEdgeErrorCount { get; private set; } // part of creatureDeadCount

	//debug
	[HideInInspector]
	public int deletedCellCount = 0; // should be safe to remove this guy...

	LowPassCounter creatureBirthsPerSecond = new LowPassCounter(20);
	LowPassCounter creatureDeathsPerSecond = new LowPassCounter(20);

	public Cell GetCellAtPosition(Vector2 position) {
		foreach (Creature creature in creatureList) {
			Cell found = creature.GetCellAtPosition(position);
			if (found != null) {
				return found;
			}
		}
		return null;
	}

	public Cell GetGeneCellAtPosition(Vector2 position, Creature soloSelected = null) {
		if (soloSelected != null) {
			Cell found = soloSelected.GetGeneCellAtPosition(position);
			if (found != null) {
				return found;
			}
		}

		foreach (Creature creature in creatureList) {
			Cell found = creature.GetGeneCellAtPosition(position);
			if (found != null) {
				return found;
			}
		}
		return null;
	}

	public bool IsUsingId(string id) {
		foreach (Creature c in creatureList) {
			if (c.id == id || c.HasRelativeWithId(id)) {
				return true;
			}
		}
		return false;
	}

	public float GetCreatureBirthsPerSecond() {
		return creatureBirthsPerSecond.GetAndStepLowPassCount();
	}

	public float GetCreatureDeathsPerSecond() {
		return creatureDeathsPerSecond.GetAndStepLowPassCount();
	}

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
			count += c.phenotype.GetCellOfTypeCount(type);
		}
		return count;
	}

	public int GetShellCellOfMaterialAliveCount(ShellCell.ShellMaterial material) {

		int count = 0;
		foreach (Creature c in creatures) {
			count += c.phenotype.GetShellCellOfMaterialCount(material);
		}
		return count;
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
	public void FertilizeCreature(Cell eggCell, bool tryPlayFx, ulong worldTicks, bool wasForced) {
		Debug.Assert(eggCell is EggCell, "You are not allowed to fertilize non Egg cell");

		if (tryPlayFx) {
			bool hasAudio; float audioVolume; bool hasParticles; bool hasMarker;
			SpatialUtil.GetFxGrade(eggCell.position, true, out hasAudio, out audioVolume, out hasParticles, out hasMarker);

			if (hasAudio) {
				Audio.instance.CreatureBirth(audioVolume);
			}
			if (hasMarker) {
				EventSymbolPlayer.instance.Play(EventSymbolEnum.CreatureBorn, eggCell.position, 0f, SpatialUtil.GetMarkerScale());
			}
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
		child.phenotype.originCell.originPulseTick = mother.phenotype.originCell.originPulseTick; // should be 0 for both allready...

		ChildData childData = new ChildData();
		childData.id = child.id; // a creature carying a child with an id that can not be found in life ==> child concidered dead to mother
		childData.isConnectedToMother = true; // Connected from the start
		childData.originMapPosition = eggCell.mapPosition; //As seen from mothers frame of reference
		childData.originBindCardinalIndex = eggCell.bindCardinalIndex; //As seen from mothers frame of reference
		mother.AddChildReference(childData);

		child.SetMotherReference(mother.id);

		PhenotypePanel.instance.MakeDirty();
		CreatureSelectionPanel.instance.MakeDirty();

		//Sometimes child origin is placed with spring too far from mother's placenta this update might fix this problem
		UpdateStructure();

		if (wasForced) {
			CreatureSelectionPanel.instance.Select(mother, mother.phenotype.originCell);
		}
		CreatureSelectionPanel.instance.UpdateSelectionCluster();
	}

	public void KillCreatureByBreaking(Creature creature, bool playFx) {
		KillCreatureSafe(creature, true);
		World.instance.AddHistoryEvent(new HistoryEvent("b", false, Color.red));
		creatureDeadByBreakingCount++;
	}

	public void KillCreatureByEscaping(Creature creature, bool playFx) {
		KillCreatureSafe(creature, playFx);
		creatureDeadByEscapingCount++;
		World.instance.AddHistoryEvent(new HistoryEvent("r", false, Color.red));
	}

	public void Restart(Action onDone) {
		KillAllCreatures(() => {
			creatureDeadCount = 0;
			creatureDeadByAgeCount = 0;
			creatureDeadByBreakingCount = 0;
			creatureDeadByEscapingCount = 0;
			creatureDeadByEdgeErrorCount = 0;

			creatureBirthsPerSecond.Clear();
			creatureBirthsPerSecond.Clear();

			onDone();
		});
	}

	public void KillAllCreatures(Action onDone) {
		StartCoroutine(KillAllCreatureCo(() => {
			onDone();
		}));
	}

	public IEnumerator KillAllCreatureCo(Action onDone) {
		List<Creature> toKill = new List<Creature>(creatureList);
		foreach (Creature creature in toKill) {
			KillCreatureSafe(creature, false);
			ProgressBar.instance.KillCreature();
			yield return 0;
		}
		creatureDictionary.Clear();
		creatureList.Clear();

		onDone();
	}

	//When pressing delete, use effects
	public void KillCellSafe(Cell cell, ulong worldTicks) {
		if (cell.isOrigin) {
			KillCreatureSafe(cell.creature, true);
		} else {
			cell.creature.KillCell(cell, true, worldTicks);
		}
	}

	//This is the only way, where the world creature GO is deleted
	public void KillCreatureSafe(Creature creature, bool tryPlayFX) {
		Vector2 position = creature.GetOriginPosition(PhenoGenoEnum.Phenotype);

		creature.DetatchFromMother(false, tryPlayFX);
		foreach (Creature child in creature.GetChildrenAlive()) {
			child.DetatchFromMother(false, tryPlayFX);
		}

		creature.KillAllCells(tryPlayFX); // for the fx :)
		creatureDeadCount++;

		//TODO: Return root cell to pool
		//This is the only place where ta creature is ultimatly destroyed
		//Are there cells still left on creature?
		Cell[] forgottenCells = creature.phenotype.cellsTransform.GetComponents<Cell>();
		deletedCellCount += forgottenCells.Length;
		Cell[] forgottenGeneCells = creature.genotype.geneCellsTransform.GetComponents<Cell>();
		deletedCellCount += forgottenGeneCells.Length;

		RemoveCreature(creature); // Ooops we need to remove it before OnRecycle! OnRecycle will change the id so it wont be found when trying to remove

		//This is the only place where creature is recycled / destroyed
		creature.OnRecycle(); //Not only when using creature pool
		CreaturePool.instance.Recycle(creature);

		PhenotypePanel.instance.MakeDirty(); // Update cell text with fewer cells
		CreatureSelectionPanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();
		GeneCellPanel.instance.MakeDirty();

		creatureDeathsPerSecond.IncreaseCounter();

		if (tryPlayFX) {
			bool hasAudio; float audioVolume; bool hasParticles; bool hasMarker;
			SpatialUtil.GetFxGrade(position, true, out hasAudio, out audioVolume, out hasParticles, out hasMarker);
			if (hasAudio) {
				Audio.instance.CreatureDeath(audioVolume);
			}
			if (hasMarker) {
				EventSymbolPlayer.instance.Play(EventSymbolEnum.CreatureDeath, position, 0f, SpatialUtil.GetMarkerScale());
			}
		}
	}

	// When leaving to freezer
	public void RemoveCreature(Creature creature) {
		creatureDictionary.Remove(creature.id); // Ooops we need to remove it before OnRecycle! OnRecycle will change the id so it wont be found when trying to remove
		creatureList.Remove(creature);
	}

	// When defrosting
	public void AddCreature(Creature creature) {
		creatureDictionary.Add(creature.id, creature);
		creatureList.Add(creature);

		creature.transform.parent = transform;
		creature.name = creature.sceneGraphName;
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

		World.instance.AddHistoryEvent(new HistoryEvent("+", false, Color.gray));
		return creature;
	}

	public Creature SpawnCreatureFreak(Vector3 position, float heading, ulong bornTick) {
		Creature creature = InstantiateCreature();
		creature.bornTick = bornTick;
		creature.GenerateFreak(position, heading);
		creature.creation = CreatureCreationEnum.Forged;
		creature.generation = 1;

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
		return creature;
	}

	// A deep copy with a unique id (different than clone that is) 
	public Creature SpawnCreatureCopy(Creature original, ulong bornTick) {
		Creature clone = InstantiateCreature();
		string id = clone.id;
		clone.Clone(original);
		clone.id = id;
		clone.nickname += " (Copy)";
		clone.hasPhenotypeCollider = false;
		clone.creation = CreatureCreationEnum.Cloned;
		//Let generation be same as mothers

		clone.bornTick = bornTick;
		return clone;
	}

	private void SpawnAddEffect(Vector2 position) {
		if (GlobalPanel.instance.graphicsEffectsToggle.isOn) {
			EventSymbolPlayer.instance.Play(EventSymbolEnum.CreatureAdd, position, 0f, SpatialUtil.GetMarkerScale());
		}
	}

	private void SpawnBirthEffect(Vector2 position) {
		if (GlobalPanel.instance.graphicsEffectsToggle.isOn) {
			EventSymbolPlayer.instance.Play(EventSymbolEnum.CreatureBorn, position, 0f, SpatialUtil.GetMarkerScale());
		}
	}

	private Creature InstantiateCreature() {
		string id = Morphosis.instance.idGenerator.GetUniqueId();
		if (creatureDictionary.ContainsKey(id)) {
			throw new System.Exception("Generated ID was not unique.");
		}

		creatureBirthsPerSecond.IncreaseCounter();

		return InstantiateCreature(id);
	}

	private Creature InstantiateCreature(string id) {
		Creature creature = Morphosis.instance.creaturePool.Borrow();
		creature.gameObject.SetActive(true);

		creatureDictionary.Add(id, creature);
		creatureList.Add(creature);
		creature.id = id;
		creature.nickname = "Nick " + id; //dafault

		creature.transform.parent = transform;
		creature.name = creature.sceneGraphName;

		return creature;
	}

	public void Restart() {
		
	}

	//----------------
	public void MakeAllCreaturesDirty() {
		for (int index = 0; index < creatures.Count; index++) {
			creatures[index].MakeDirtyGraphics();
		}
	}

	// Update

	// Allways, updated from Update
	// Keeping graphics up to date, creature selection, cell selection, flipArrows, edges
	public void UpdateGraphics() {
		for (int index = 0; index < creatureList.Count; index++) {
			creatureList[index].UpdateGraphics();
		}

		Morphosis.instance.relationArrows.creature = CreatureSelectionPanel.instance.soloSelected;
		Morphosis.instance.relationArrows.UpdateGraphics();
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
	private int killOldCreaturesTicks;

	// Phenotype only, updated from FixedUpdate
	// Everything that needs to be updated as the biological clock is ticking, wings, cell tasks, energy
	private List<Creature> killCreatureList = new List<Creature>();

	public void UpdatePhysics(ulong worldTicks) {

		//Ticks 
		phenotypePanelTicks++;
		if (phenotypePanelTicks >= GlobalSettings.instance.quality.phenotypePanelTickPeriod) {
			phenotypePanelTicks = 0;
		}

		killOldCreaturesTicks++;
		if (killOldCreaturesTicks >= GlobalSettings.instance.quality.killOldCreaturesTickPeriod) {
			killOldCreaturesTicks = 0;
		}

		// ^ Ticks ^
		for (int index = 0; index < creatureList.Count; index++) {
			creatureList[index].UpdatePhysics(worldTicks);
		}

		for (int index = 0; index < creatureList.Count; index++) {
			creatureList[index].UpdateFertilize(worldTicks);
		}

		killCreatureList.Clear();



		//TODO: dont do it every tick
		for (int index = 0; index < creatureList.Count; index++) {
			if (creatureList[index].phenotype.isAlive) {
				if (creatureList[index].UpdateKillWeakCells(worldTicks)) {
					killCreatureList.Add(creatureList[index]);
				}
			} else {
				if (creatureList[index].phenotype.hasError) {
					World.instance.AddHistoryEvent(new HistoryEvent("e", false, Color.red));
					creatureDeadByEdgeErrorCount++;
				}
				killCreatureList.Add(creatureList[index]);
			}
		}

		if (PhenotypePhysicsPanel.instance.killOld.isOn) {
			if (killOldCreaturesTicks == 0) {
				for (int index = 0; index < creatureList.Count; index++) {
					if (creatureList[index].GetAge(worldTicks) > GlobalSettings.instance.phenotype.maxAge) {
						killCreatureList.Add(creatureList[index]);
						creatureDeadByAgeCount++;
					}
				}
			}
		}

		for (int index = 0; index < killCreatureList.Count; index++) {
			KillCreatureSafe(killCreatureList[index], true);
		}

		if (phenotypePanelTicks == 0) {
			if (CreatureSelectionPanel.instance.hasSoloSelected) {
				CreatureSelectionPanel.instance.MakeDirty();
				PhenotypePanel.instance.MakeDirty();
				CellPanel.instance.MakeDirty();
			}
		}
	}
	// ^ Update ^

	// Load Save
	private LifeData lifeData = new LifeData();

	// Save
	public LifeData UpdateData() {
		//UpdateSoulReferences();

		lifeData.lastId = Morphosis.instance.idGenerator.serialNumber;

		//Creatures
		lifeData.creatureList.Clear();
		lifeData.creatureDictionary.Clear();
		lifeData.creatureDeadCount =           creatureDeadCount;
		lifeData.creatureDeadByAgeCount =      creatureDeadByAgeCount;
		lifeData.creatureDeadByBreakingCount = creatureDeadByBreakingCount;
		lifeData.creatureDeadByEscapingCount = creatureDeadByEscapingCount;
		lifeData.creatureDeadByEdgeErrorCount = creatureDeadByEdgeErrorCount;

		for (int index = 0; index < creatureList.Count; index++) {
			Creature creature = creatureList[index];
			CreatureData data = creature.UpdateData();
			lifeData.creatureList.Add(data);
			lifeData.creatureDictionary.Add(data.id, data);
		}

		return lifeData;
	}

	// Load
	public void ApplyData(LifeData lifeData, Action onDone) {
		StartCoroutine(ApplyDataCo(lifeData, onDone));
	}

	public IEnumerator ApplyDataCo(LifeData lifeData, Action onDone) {
		Morphosis.instance.idGenerator.serialNumber = lifeData.lastId;

		// Create all creatures
		for (int index = 0; index < lifeData.creatureList.Count; index++) {
			CreatureData creatureData = lifeData.creatureList[index];
			Creature creature = InstantiateCreature(creatureData.id);
			creature.ApplyData(creatureData);
			ProgressBar.instance.SpawnCreature();
			yield return 0;
		}
		creatureDeadCount =           lifeData.creatureDeadCount;
		creatureDeadByAgeCount =      lifeData.creatureDeadByAgeCount;
		creatureDeadByBreakingCount = lifeData.creatureDeadByBreakingCount;
		creatureDeadByEscapingCount = lifeData.creatureDeadByEscapingCount;
		creatureDeadByEdgeErrorCount = lifeData.creatureDeadByEdgeErrorCount;
		onDone();
	}

	// ^ Load Save ^
}