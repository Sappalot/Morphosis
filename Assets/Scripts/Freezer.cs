using System.Collections.Generic;
using UnityEngine;

public class Freezer : MonoSingleton<Freezer> {

	public GameObject legalArea;
	
	//debug
	[HideInInspector]
	public int deletedCellCount = 0;

	private IdGenerator idGenerator = new IdGenerator("f");
	private Dictionary<string, Creature> creatureDictionary = new Dictionary<string, Creature>();
	private List<Creature> creatureList = new List<Creature>();
	private Rect legalRect;

	public string GetUniqueIdStamp() {
		return idGenerator.GetUniqueWorldId();
	}

	public List<Creature> creatures {
		get {
			return creatureList;
		}
	}

	public bool HasCreature(string id) {
		if (id == null) {
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

	public void KillAllCreatures() {
		List<Creature> toKill = new List<Creature>(creatureList);
		foreach (Creature creature in toKill) {
			KillCreatureSafe(creature, false);
		}
		creatureDictionary.Clear();
		creatureList.Clear();
	}

	public void KillCreatureSafe(Creature creature, bool playEffects) {
		//creature.DetatchFromMother(false, playEffects);
		//foreach (Creature child in creature.GetChildrenAlive()) {
		//	child.DetatchFromMother(false, playEffects);
		//}

		if (playEffects && GlobalPanel.instance.graphicsEffectsToggle.isOn) {
			//Animator birth = Instantiate(creatureDeathEffectPrefab, creature.phenotype.originCell.position, Quaternion.Euler(0f, 0f, 0f));
			EffectPlayer.instance.Play(EffectEnum.CreatureDeath, creature.phenotype.originCell.position, 0f, CameraUtils.GetEffectScaleLazy());
		}

		creature.KillAllCells(true); // for the fx :)


		//creatureDeadCount++;

		//TODO: Return root cell to pool
		//This is the only place where ta creature is ultimatly destroyed
		//Are there cells still left on creature?
		Cell[] forgottenCells = creature.phenotype.cellsTransform.GetComponents<Cell>();
		deletedCellCount += forgottenCells.Length;
		Cell[] forgottenGeneCells = creature.genotype.geneCellsTransform.GetComponents<Cell>();
		deletedCellCount += forgottenGeneCells.Length;

		//This is the only place where creature is recycled / destroyed
		creatureDictionary.Remove(creature.id); // Ooops we need to remove it before OnRecycle! OnRecycle will change the id so it wont be found when trying to remove
		creatureList.Remove(creature);

		creature.OnRecycle(); //Not only when using creature pool
		CreaturePool.instance.Recycle(creature);

		PhenotypePanel.instance.MakeDirty(); // Update cell text with fewer cells
		CreatureSelectionPanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();
		GenePanel.instance.MakeDirty();
	}

	public Creature SpawnCreatureCopy(Creature original, ulong bornTick) {
		Creature clone = InstantiateCreature();
		string id = clone.id;
		clone.Clone(original);
		clone.id = id;
		clone.nickname += " (F-Copy)";
		clone.hasPhenotypeCollider = false;
		clone.hasGenotypeCollider = false;
		clone.creation = CreatureCreationEnum.Frozen;
		//Let generation be same as mothers

		clone.bornTick = bornTick;
		return clone;
	}

	private Creature InstantiateCreature() {
		string id = idGenerator.GetUniqueWorldId();
		if (creatureDictionary.ContainsKey(id)) {
			throw new System.Exception("Generated ID was not unique.");
		}

		return InstantiateCreature(id);
	}

	private Creature InstantiateCreature(string id) {
		Creature creature = Morphosis.instance.creaturePool.Borrow();
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
			Cell found = creature.GetCellAt(position);
			if (found != null) {
				return found;
			}
		}
		return null;
	}
	//----

	public bool IsCompletelyInside(Creature creature) {
		return creature.phenotype.IsCompletelyInside(legalRect);
	}

	public bool KillIfOutside(Creature creature) {
		if (!IsCompletelyInside(creature)) {
			World.instance.life.KillCreatureSafe(creature, false);
			return true;
		}
		return false;
	}

	public void Start() {
		legalRect = new Rect(legalArea.transform.position, legalArea.transform.localScale);
	}

	// Load / Save
	private LifeData lifeData = new LifeData();

	public void Load() {

	}

	public void Save() {

	}

	// Save
	private void UpdateData() {

	}

	// Load
	private void ApplyData(WorldData worldData) {

	}

	// ^ Load / Save ^
}