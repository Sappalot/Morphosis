using System.Collections.Generic;
using UnityEngine;

public static class CellTypeUtil {

	private static Dictionary<CellTypeEnum, Color> cellTypeToColor = new Dictionary<CellTypeEnum, Color>();

	static CellTypeUtil() {
		cellTypeToColor.Add(CellTypeEnum.Egg,       new Color(0.64f, 0.64f,   0.27f,     1f));
		cellTypeToColor.Add(CellTypeEnum.Jaw,       new Color(0.75f,   0.35f,      0.72f,     1f));
		cellTypeToColor.Add(CellTypeEnum.Leaf,      new Color(0f,   1,      0f,     1f));
		cellTypeToColor.Add(CellTypeEnum.Muscle,    new Color(1f,   0.5f,   0f,     1f));
		cellTypeToColor.Add(CellTypeEnum.Vein,      new Color(1f,   0f,     0f,     1f));
	}

	public static Color ToColor(CellTypeEnum cellType) {
		return cellTypeToColor[cellType];
	}
}
