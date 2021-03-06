﻿using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PhenotypeData {
	public List<CellData> cellDataList = new List<CellData>();
	public bool isCellPatternDirty;

	public int fungalCellTick;
	public int jawCellTick;
	public int leafCellTick;
	public int muscleCellTick;
	public int rootCellTick;
	public int shellCellTick;
	public int veinCellTick;

	public int signalTick;
}