using System;
using System.Collections.Generic;

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
	public int canNotGrowMoreTicks;
	public bool detatch;

	public MotherData motherData = null;
	public List<ChildData> childDataList = new List<ChildData>();
}