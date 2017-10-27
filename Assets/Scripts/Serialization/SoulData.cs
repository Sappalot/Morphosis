using System;
using System.Collections.Generic;

[Serializable]
public class SoulData {
	// me 
	public string id = "no id";

	public CreatureReferenceData creatureReferenceData;
	public SoulReferenceData motherSoulReferenceData = new SoulReferenceData(); //has no mother per default
	public List<SoulReferenceData> childSoulReferencesData = new List<SoulReferenceData>();

}