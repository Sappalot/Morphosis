using UnityEngine;
using System.Collections.Generic;
using System;

public class Life : MonoBehaviour {
	public Creature creaturePrefab;

	private IdGenerator idGenerator = new IdGenerator();
	private Dictionary<string, Creature> creatureDictionary = new Dictionary<string, Creature>();
	private List<Creature> creatureList = new List<Creature>();

	public List<Creature> creatures	{
		get {
			return creatureList;
		}
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

	public void KillAll() {
		foreach (Creature creature in creatureList) {
			Destroy(creature.gameObject);
		}
		creatureDictionary.Clear();
		creatureList.Clear();
	}

	public void GeneratePhenotypeCells() {
		for (int index = 0; index < creatureList.Count; index++) {
			Creature creature = creatureList[index];
			creature.GeneratePhenotypeCells();
		}
	}

	public void SwitchToPhenotypes() {
		for (int index = 0; index < creatureList.Count; index++) {
			Creature creature = creatureList[index];
			creature.SwitchToPhenotype();
		}
	}

	public void SwitchToGenotypes() {
		for (int index = 0; index < creatureList.Count; index++) {
			Creature creature = creatureList[index];
			creature.SwitchToGenotype();
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



	public Creature SpawnCreatureJellyfish(Vector3 position) {
		Creature creature = InstantiateCreature();
		creature.GenerateJellyfish(position);
		return creature;
	}

	public Creature SpawnCreatureMinimalistic(Vector3 position) {
		Creature creature = InstantiateCreature();
		creature.GenerateMinimalistic(position);
		return creature;
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

	public void DeleteCreature(Creature creature) {
		Destroy(creature.gameObject);

		creatureDictionary.Remove(creature.id);
		creatureList.Remove(creature);
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
        
		KillAll();
		for (int index = 0; index < lifeData.creatureList.Count; index++) {
			CreatureData creatureData = lifeData.creatureList[index];
			Creature creature = InstantiateCreature(creatureData.id);
			creature.ApplyData(creatureData);
		}
	}


}
