using System.Collections.Generic;
using UnityEngine;

public class Record {
	float fps;
	float pps;

	float health;
	float cellCountTotal;
	float cellCountEgg;
	float cellCountFungal;
	float cellCountJaw;
	float cellCountLeaf;
	float cellCountMuscle;
	float cellCountRoot;
	float cellCountShell;
	float cellCountShellWood;
	float cellCountShellMetal;
	float cellCountShellGlass;
	float cellCountShellDiamond;
	float cellCountVein;

	float creatureCount;
	float creatureBirthsPerSecond;
	float creatureDeathsPerSecond;

	

	public string tagText = null;
	public bool tagShowLine = false;
	public float tagRed = 1f;
	public float tagGreen = 1f;
	public float tagBlue = 1f;

	public Color color {
		get {
			return new Color(tagRed, tagGreen, tagBlue, 1f);
		}
	}

	public void Clear() {
		fps =             0f;
		pps =             0f;
		health =      0f;

		cellCountTotal =  0f;
		cellCountEgg =    0f;
		cellCountFungal = 0f;
		cellCountJaw =    0f;
		cellCountLeaf =   0f;
		cellCountMuscle = 0f;
		cellCountRoot =   0f;
		cellCountShell =  0f;
		cellCountShellWood =    0f;
		cellCountShellMetal =   0f;
		cellCountShellGlass =   0f;
		cellCountShellDiamond = 0f;
		cellCountVein =   0f;

		creatureCount =           0f;
		creatureBirthsPerSecond = 0f;
		creatureDeathsPerSecond = 0f;

		tagText =         "";
		tagShowLine =     false;
		tagRed =          1f;
		tagGreen =        1f;
		tagBlue =         1f;
	}

	public bool HasTag() {
		return tagText != null && tagText != "";
	}

	public void SetTagText(string text, Color color, bool drawLine) {
		tagText =     text;
		tagShowLine = drawLine;
		tagRed =      color.r;
		tagGreen =    color.g;
		tagBlue =     color.b;
	}

	public float Get(RecordEnum type) {
		if (type == RecordEnum.fps) {
			return fps;
		}
		if (type == RecordEnum.pps) {
			return pps;
		}
		if (type == RecordEnum.health) {
			return health;
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
		if (type == RecordEnum.cellCountShellWood) {
			return cellCountShellWood;
		}
		if (type == RecordEnum.cellCountShellMetal) {
			return cellCountShellMetal;
		}
		if (type == RecordEnum.cellCountShellGlass) {
			return cellCountShellGlass;
		}
		if (type == RecordEnum.cellCountShellDiamond) {
			return cellCountShellDiamond;
		}
		if (type == RecordEnum.cellCountVein) {
			return cellCountVein;
		}

		if (type == RecordEnum.creatureCount) {
			return creatureCount;
		}
		if (type == RecordEnum.creatureBirthsPerSecond) {
			return creatureBirthsPerSecond;
		}
		if (type == RecordEnum.creatureDeathsPerSecond) {
			return creatureDeathsPerSecond;
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
		if (type == RecordEnum.health) {
			health = value;
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
		if (type == RecordEnum.cellCountShellWood) {
			cellCountShellWood = value;
		}
		if (type == RecordEnum.cellCountShellMetal) {
			cellCountShellMetal = value;
		}
		if (type == RecordEnum.cellCountShellGlass) {
			cellCountShellGlass = value;
		}
		if (type == RecordEnum.cellCountShellDiamond) {
			cellCountShellDiamond = value;
		}
		if (type == RecordEnum.cellCountVein) {
			cellCountVein = value;
		}

		if (type == RecordEnum.creatureCount) {
			creatureCount = value;
		}
		if (type == RecordEnum.creatureBirthsPerSecond) {
			creatureBirthsPerSecond = value;
		}
		if (type == RecordEnum.creatureDeathsPerSecond) {
			creatureDeathsPerSecond = value;
		}
	}

	private RecordData recordData = new RecordData();

	// Save
	public RecordData UpdateData() {
		recordData.fps =             fps;
		recordData.pps =             pps;
		recordData.health =      health;

		recordData.cellCountTotal =  cellCountTotal;
		recordData.cellCountEgg =    cellCountEgg;
		recordData.cellCountFungal = cellCountFungal;
		recordData.cellCountJaw =    cellCountJaw;
		recordData.cellCountLeaf =   cellCountLeaf;
		recordData.cellCountMuscle = cellCountMuscle;
		recordData.cellCountRoot =   cellCountRoot;
		recordData.cellCountShell =  cellCountShell;
		recordData.cellCountShellWood    = cellCountShellWood;
		recordData.cellCountShellMetal   = cellCountShellMetal;
		recordData.cellCountShellGlass   = cellCountShellGlass;
		recordData.cellCountShellDiamond = cellCountShellDiamond;
		recordData.cellCountVein =   cellCountVein;

		recordData.creatureCount =           creatureCount;
		recordData.creatureBirthsPerSecond = creatureBirthsPerSecond;
		recordData.creatureDeathsPerSecond = creatureDeathsPerSecond;

		recordData.tagText =         tagText;
		recordData.showLine =        tagShowLine;
		recordData.tagRed =          tagRed;
		recordData.tagGreen =        tagGreen;
		recordData.tagBlue =         tagBlue;

		return recordData;
	}

	// Load
	public void ApplyData(RecordData recordData) {
		fps =             recordData.fps;
		pps =             recordData.pps;
		health =      recordData.health;

		cellCountTotal =        recordData.cellCountTotal;
		cellCountEgg =          recordData.cellCountEgg;
		cellCountFungal =       recordData.cellCountFungal;
		cellCountJaw =          recordData.cellCountJaw;
		cellCountLeaf =         recordData.cellCountLeaf;
		cellCountMuscle =       recordData.cellCountMuscle;
		cellCountRoot =         recordData.cellCountRoot;
		cellCountShell =        recordData.cellCountShell;
		cellCountShellWood =    recordData.cellCountShellWood;
		cellCountShellMetal =   recordData.cellCountShellMetal;
		cellCountShellGlass =   recordData.cellCountShellGlass;
		cellCountShellDiamond = recordData.cellCountShellDiamond;
		cellCountVein =         recordData.cellCountVein;

		creatureCount =           recordData.creatureCount;
		creatureBirthsPerSecond = recordData.creatureBirthsPerSecond;
		creatureDeathsPerSecond = recordData.creatureDeathsPerSecond;

		tagText =         recordData.tagText;
		tagShowLine =     recordData.showLine;
		tagRed =          recordData.tagRed;
		tagGreen =        recordData.tagGreen;
		tagBlue =         recordData.tagBlue;
	}
}