using System;
using System.Collections.Generic;

[Serializable]
public class RecordData {
	public float fps;
	public float pps;
	public float cellCountTotal;
	public float cellCountJaw;
	public float cellCountLeaf;


	public string tag;
	public bool showLine;
	//public Dictionary<RecordEnum, float> entries = new Dictionary<RecordEnum, float>();
	//public String testString;
}
