using System;
using System.Collections.Generic;

[Serializable]
public class RecordData {
	public float fps;
	public float pps;
	public float health;

	public float cellCountTotal;
	public float cellCountEgg;
	public float cellCountFungal;
	public float cellCountJaw;
	public float cellCountLeaf;
	public float cellCountMuscle;
	public float cellCountRoot;
	public float cellCountShell;
	public float cellCountShellWood;
	public float cellCountShellMetal;
	public float cellCountShellGlass;
	public float cellCountShellDiamond;
	public float cellCountVein;

	public float creatureCount;
	public float creatureBirthsPerSecond;
	public float creatureDeathsPerSecond;

	public string tagText;
	public bool showLine;
	public float tagRed;
	public float tagGreen;
	public float tagBlue;
}
