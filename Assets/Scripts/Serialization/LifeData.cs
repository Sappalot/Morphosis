using System;
using System.Collections.Generic;

[Serializable]
public class LifeData {
	public long lastId;

	//Creatures 
	public Dictionary<string, CreatureData> creatureDictionary = new Dictionary<string, CreatureData>();
	public List<CreatureData> creatureList = new List<CreatureData>();
	public int creatureDeadCount;
	public int creatureDeadByAgeCount;
	public int creatureDeadByBreakingCount;
	public int creatureDeadByEscapingCount;
}