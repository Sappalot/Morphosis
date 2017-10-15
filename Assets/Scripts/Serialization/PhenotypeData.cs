using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PhenotypeData {
	public List<CellData> cellDataList = new List<CellData>();
	public float timeOffset;
	public bool differsFromGenotype;
}