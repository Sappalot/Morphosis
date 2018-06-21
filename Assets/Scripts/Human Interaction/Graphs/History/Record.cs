using System.Collections.Generic;
using UnityEngine;

public class Record {
	float fps;
	float pps;

	float cellCountTotal;
	float cellCountEgg;
	float cellCountFungal;
	float cellCountJaw;
	float cellCountLeaf;
	float cellCountMuscle;
	float cellCountRoot;
	float cellCountShell;
	float cellCountVein;

	public string tag = null;
	public bool showLine = false;

	public void Clear() {
		fps =             0f;
		pps =             0f;
		cellCountTotal =  0f;
		cellCountEgg =    0f;
		cellCountFungal = 0f;
		cellCountJaw =    0f;
		cellCountLeaf =   0f;
		cellCountMuscle = 0f;
		cellCountRoot =   0f;
		cellCountShell =  0f;
		cellCountVein =   0f;
		tag = "";
	}

	public bool HasTag() {
		return tag != null && tag != "";
	}

	public void SetTagText(string text, bool drawLine) {
		tag = text;
		showLine = drawLine;
	}

	public float Get(RecordEnum type) {
		if (type == RecordEnum.fps) {
			return fps;
		}
		if (type == RecordEnum.pps) {
			return pps;
		}
		if (type == RecordEnum.cellCountTotal) {
			return cellCountTotal;
		}
		if (type == RecordEnum.cellCountEgg) {
			return cellCountEgg;
		}
		if (type == RecordEnum.cellCountFungal) {
			return cellCountFungal;
		}
		if (type == RecordEnum.cellCountJaw) {
			return cellCountJaw;
		}
		if (type == RecordEnum.cellCountLeaf) {
			return cellCountLeaf;
		}
		if (type == RecordEnum.cellCountMuscle) {
			return cellCountMuscle;
		}
		if (type == RecordEnum.cellCountRoot) {
			return cellCountRoot;
		}
		if (type == RecordEnum.cellCountShell) {
			return cellCountShell;
		}
		if (type == RecordEnum.cellCountVein) {
			return cellCountVein;
		}
		return 0f;
	}

	public void Set(RecordEnum type, float value) {
		if (type == RecordEnum.fps) {
			fps = value;
		}
		if (type == RecordEnum.pps) {
			pps = value;
		}

		if (type == RecordEnum.cellCountTotal) {
			cellCountTotal = value;
		}
		if (type == RecordEnum.cellCountEgg) {
			cellCountEgg = value;
		}
		if (type == RecordEnum.cellCountFungal) {
			cellCountFungal = value;
		}
		if (type == RecordEnum.cellCountJaw) {
			cellCountJaw = value;
		}
		if (type == RecordEnum.cellCountLeaf) {
			cellCountLeaf = value;
		}
		if (type == RecordEnum.cellCountMuscle) {
			cellCountMuscle = value;
		}
		if (type == RecordEnum.cellCountRoot) {
			cellCountRoot = value;
		}
		if (type == RecordEnum.cellCountShell) {
			cellCountShell = value;
		}
		if (type == RecordEnum.cellCountVein) {
			cellCountVein = value;
		}
	}

	private RecordData recordData = new RecordData();

	// Save
	public RecordData UpdateData() {
		recordData.fps =             fps;
		recordData.pps =             pps;

		recordData.cellCountTotal =  cellCountTotal;
		recordData.cellCountEgg =    cellCountEgg;
		recordData.cellCountFungal = cellCountFungal;
		recordData.cellCountJaw =    cellCountJaw;
		recordData.cellCountLeaf =   cellCountLeaf;
		recordData.cellCountMuscle = cellCountMuscle;
		recordData.cellCountRoot =   cellCountRoot;
		recordData.cellCountShell =  cellCountShell;
		recordData.cellCountVein =   cellCountVein;

		recordData.tag =             tag;
		recordData.showLine =        showLine;

		return recordData;
	}

	// Load
	public void ApplyData(RecordData recordData) {
		fps =             recordData.fps;
		pps =             recordData.pps;

		cellCountTotal =  recordData.cellCountTotal;
		cellCountEgg =    recordData.cellCountEgg;
		cellCountFungal = recordData.cellCountFungal;
		cellCountJaw =    recordData.cellCountJaw;
		cellCountLeaf =   recordData.cellCountLeaf;
		cellCountMuscle = recordData.cellCountMuscle;
		cellCountRoot =   recordData.cellCountRoot;
		cellCountShell =  recordData.cellCountShell;
		cellCountVein =   recordData.cellCountVein;

		tag =             recordData.tag;
		showLine =        recordData.showLine;
	}
}