using System;
using System.Collections.Generic;

[Serializable]
public class LifeData {
	public long lastId;
	public Dictionary<string, CreatureData> creatureDictionary = new Dictionary<string, CreatureData>();
	public List<CreatureData> creatureList = new List<CreatureData>();
	//TODO: store Soul data, let soul carry creature data
}