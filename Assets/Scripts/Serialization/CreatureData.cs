using System;
using UnityEngine;

[Serializable]
public class CreatureData {
	public string id = "no id";
	public string nickname = "no name";
	public CreatureCreationEnum creation = CreatureCreationEnum.Forged;
	public int generation = 1;

	public ulong bornTick;
	public ulong deadTick;

	public GenotypeData genotypeData = new GenotypeData();
	public PhenotypeData phenotypeData = new PhenotypeData();

	public int growTicks;
	public int detatchTicks;

}