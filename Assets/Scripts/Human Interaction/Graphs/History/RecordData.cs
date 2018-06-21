using System;
using System.Collections.Generic;

[Serializable]
public class RecordData {
	public float fps;
	public float pps;

	public float cellCountTotal;
	public float cellCountEgg;
	public float cellCountFungal;
	public float cellCountJaw;
	public float cellCountLeaf;
	public float cellCountMuscle;
	public float cellCountRoot;
	public float cellCountShell;
	public float cellCountVein;

	public string tag;
	public bool showLine;
}
