using Boo.Lang.Runtime;
using SerializerFree;
using SerializerFree.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Freezer : MonoSingleton<Freezer> {

	public GameObject legalArea;

	//debug
	[HideInInspector]
	public int deletedCellCount = 0;

	private Dictionary<string, Creature> creatureDictionary = new Dictionary<string, Creature>();
	private List<Creature> creatureList = new List<Creature>();
	private Rect legalRect;

	public void Start() {
		legalRect = new Rect(legalArea.transform.position, legalArea.transform.localScale);
	}

	public List<Creature> creatures {
		get {
			return creatureList;
		}
	}

	public int creatureCount {
		get {
			return creatureList.Count;
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

	public void KillCreatureSafe(Creature creature, bool tryPlayFx) {
		Vector2 position = creature.GetOriginPosition(PhenoGenoEnum.Phenotype);

		creature.KillAllCells(tryPlayFx); // for the fx :)

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
		GeneCellPanel.instance.MakeDirty();

		if (tryPlayFx) {
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

	public Creature SpawnCreatureCopy(Creature original) {
		Creature clone = InstantiateCreature();
		string id = clone.id;
		clone.Clone(original);
		clone.id = id;
		clone.nickname += " (F-Copy)";
		clone.hasPhenotypeCollider = false;
		clone.creation = CreatureCreationEnum.Frozen;
		//Let generation be same as mothers

		clone.bornTick = 0; // We dont care how long it has been frozen, doesn't make any sence when loading together with different world
		return clone;
	}

	private Creature InstantiateCreature() {
		string id = Morphosis.instance.idGenerator.GetUniqueId();
		if (creatureDictionary.ContainsKey(id)) {
			throw new System.Exception("Generated ID was not unique.");
		}

		return InstantiateCreature(id);
	}

	private Creature InstantiateCreature(string id) {
		Creature creature = Morphosis.instance.creaturePool.Borrow();
		creature.gameObject.SetActive(true);
		creatureDictionary.Add(id, creature);
		creatureList.Add(creature);
		creature.id = id;

		creature.nickname = "F-Nick " + id; //dafault

		creature.creation = CreatureCreationEnum.Frozen;
		creature.transform.parent = transform;
		creature.name = creature.sceneGraphName;

		return creature;
	}

	public Cell GetCellAtPosition(Vector2 position) {
		foreach (Creature creature in creatureList) {
			Cell found = creature.GetCellAtPosition(position);
			if (found != null) {
				return found;
			}
		}
		return null;
	}

	public Cell GetGeneCellAtPosition(Vector2 position) {
		foreach (Creature creature in creatureList) {
			Cell found = creature.GetGeneCellAtPosition(position);
			if (found != null) {
				return found;
			}
		}
		return null;
	}

	public bool IsInside(Vector2 position) {
		float top = legalRect.y + legalRect.height / 2f;
		float bottom = legalRect.y - legalRect.height / 2f;
		float left = legalRect.x - legalRect.width / 2f;
		float right = legalRect.x + legalRect.width / 2f;

		if (position.x > right || position.x < left || position.y > top || position.y < bottom) {
			return false;
		}
		return true;
	}

	public bool IsCompletelyInside(Creature creature) {
		if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			return creature.phenotype.IsCompletelyInside(legalRect);
		} else {
			return creature.genotype.IsCompletelyInside(legalRect);
		}
	}

	public bool KillIfOutside(Creature creature) {
		if (!IsCompletelyInside(creature)) {
			World.instance.life.KillCreatureSafe(creature, false);
			return true;
		}
		return false;
	}

	// When freezing
	public void AddCreature(Creature creature) {
		creatureDictionary.Add(creature.id, creature);
		creatureList.Add(creature);

		creature.transform.parent = transform;
		creature.name = creature.sceneGraphName;
	}

	// When defrosting
	public void RemoveCreature(Creature creature) {
		creatureDictionary.Remove(creature.id);
		creatureList.Remove(creature);
	}

	public void UpdateGraphics() {
		for (int index = 0; index < creatureList.Count; index++) {
			creatureList[index].UpdateStructure();
			creatureList[index].UpdateGraphics();
		}
	}

	public void UpdatePhysics() {
		foreach (Creature c in creatureList) {
			c.UpdateStructure();
		}
	}

	public void Load(Action onDone) {
		Load(LoadFreezerData(), onDone);

	}

	public FreezerData LoadFreezerData() {
		string filename = "freezer.txt";

		string path = "F:/Morphosis/";
		if (!File.Exists(path + filename)) {
			Save();
		}
		string serializedString = File.ReadAllText(path + filename);

		return Serializer.Deserialize<FreezerData>(serializedString, new UnityJsonSerializer());
	}

	public void Load(FreezerData freezerData, Action onDone) {
		KillAllCreatures(() => {
			ApplyData(freezerData, () => {
				onDone();
			});
		});
	}



	// Load / Save
	public void Save() {
		UpdateData();

		string path = path = "F:/Morphosis/";
		string freezerToSave = Serializer.Serialize(freezerData, new UnityJsonSerializer());
		if (!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}
		string filename = "freezer.txt";
		File.WriteAllText(path + filename, freezerToSave);
	}

	private FreezerData freezerData = new FreezerData();

	// Save
	private FreezerData UpdateData() {
		//Creatures
		freezerData.creatureList.Clear();
		freezerData.creatureDictionary.Clear();

		for (int index = 0; index < creatureList.Count; index++) {
			Creature creature = creatureList[index];
			if (IsCompletelyInside(creature)) {
				CreatureData data = creature.UpdateData();
				freezerData.creatureList.Add(data);
				freezerData.creatureDictionary.Add(data.id, data);
			}
		}

		return freezerData;
	}

	public void ApplyData(FreezerData freezerData, Action onDone) {
		StartCoroutine(ApplyDataCo(freezerData, () => {
			onDone();
		}));
	}

	public IEnumerator ApplyDataCo(FreezerData freezerData, Action onDone) {
		yield return 0;

		for (int index = 0; index < freezerData.creatureList.Count; index++) {
			CreatureData creatureData = freezerData.creatureList[index];
			creatureData.id = Morphosis.instance.idGenerator.GetUniqueId(); // Freezer ids will allways start from scratch, then moved to range after load when other creatures are loaded
			Creature creature = InstantiateCreature(creatureData.id);
			creature.ApplyData(creatureData);
			creature.OnFreeze();
			ProgressBar.instance.SpawnCreature();
			yield return 0;
		}
		onDone();
	}
	// ^ Load / Save ^
}