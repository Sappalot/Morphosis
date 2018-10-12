using System;
using System.Collections.Generic;

[Serializable]
public class FreezerData {
	//Creatures 
	public Dictionary<string, CreatureData> creatureDictionary = new Dictionary<string, CreatureData>();
	public List<CreatureData> creatureList = new List<CreatureData>();
}