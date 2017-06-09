using System.Collections.Generic;
using UnityEngine;

public static class CellTypeUtil {

    private static Dictionary<CellTypeEnum, Color> cellTypeToColor = new Dictionary<CellTypeEnum, Color>();

    static CellTypeUtil() {
        cellTypeToColor.Add(CellTypeEnum.Leaf,      new Color(0f,   1,      0f,     1f));
        cellTypeToColor.Add(CellTypeEnum.Mouth,     new Color(1f,   0,      1f,     1f));
        cellTypeToColor.Add(CellTypeEnum.Muscle,    new Color(1f,   0.5f,   0f,     1f));
        cellTypeToColor.Add(CellTypeEnum.Vein,      new Color(1f,   0f,     0f,     1f));
    }

    public static Color ToColor(CellTypeEnum cellType) {
        return cellTypeToColor[cellType];
    }
}
