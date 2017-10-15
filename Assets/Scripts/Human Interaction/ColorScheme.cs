using UnityEngine;

public class ColorScheme : MonoSingleton<ColorScheme> {
	public Color selectedButton;
	public Color notSelectedButton;
	public Color mouseText;

	public Color egg;
	public Color jaw;
	public Color leaf;
	public Color muscle;
	public Color vein;

	public Color ToColor(CellTypeEnum cellType) {
		if (cellType == CellTypeEnum.Egg) {
			return egg;
		} else if (cellType == CellTypeEnum.Jaw) {
			return jaw;
		} else if (cellType == CellTypeEnum.Leaf) {
			return leaf;
		} else if (cellType == CellTypeEnum.Muscle) {
			return muscle;
		} else if (cellType == CellTypeEnum.Vein) {
			return vein;
		}
		return Color.black;
	}
}