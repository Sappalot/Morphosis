using System;
using System.Collections.Generic;

[Serializable]
public class LifeData {
	public long lastId;

	//Creatures 
	public Dictionary<string, CreatureData> creatureDictionary = new Dictionary<string, CreatureData>();
	public List<CreatureData> creatureList = new List<CreatureData>();

	//Souls
	public Dictionary<string, SoulData> soulDictionary = new Dictionary<string, SoulData>();
	public List<SoulData> soulList = new List<SoulData>();
	public int soulsLostCount;
}