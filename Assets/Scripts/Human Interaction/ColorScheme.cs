using UnityEngine;
using UnityEngine.Serialization;

public class ColorScheme : MonoSingleton<ColorScheme> {
	public Gradient cellGradientEnergy;
	public Gradient cellGradientEffect;
	public Gradient cellGradientLeafExposure;
	public Gradient cellGradientLeafGreenExposure;
	public Gradient cellGradientRamSpeed;
	public Gradient cellCreatureChildCount;
	public Gradient creatureAgeGradient;
	public Gradient creatureAgeTextGradient;

	public Color selectedButtonSymbol;
	public Color notSelectedButtonSymbol;

	public Color selectedButtonBackground;
	public Color notSelectedButtonBackground;

	public Color mouseTextAction;
	public Color mouseTextBussy;

	public Color egg;
	public Color fungal;
	public Color jaw;
	public Color leaf;
	public Color muscle;
	public Color root;
	public Color shell;
	public Gradient shellArmourOpaque;
	public Gradient shellArmourTransparent;
	public Gradient shellArmourOpaqueClear;
	public Gradient shellArmourTransparentClear;
	public Color vein;

	public Gradient creatureFinWake;

	public Color outlineCluster;
	public Color outlineSelected;

	public Color noRelativesArrow;
	public Color noMotherArrow;
	public Color noMotherAttachedArrow;
	public Color motherAttachedArrow;

	public Color creatureLocked;
	public Color creatureUnlocked;

	public Color motherColor;
	public Color fatherColor;

	public Color signalOn;
	public Color signalOff;

	public Color grayedOut;

	public Color ToColor(CellTypeEnum cellType) {
		if (cellType == CellTypeEnum.Egg) {
			return egg;
		} else if (cellType == CellTypeEnum.Fungal) {
			return fungal;
		} else if (cellType == CellTypeEnum.Jaw) {
			return jaw;
		} else if (cellType == CellTypeEnum.Leaf) {
			return leaf;
		} else if (cellType == CellTypeEnum.Muscle) {
			return muscle;
		} else if (cellType == CellTypeEnum.Root) {
			return root;
		} else if (cellType == CellTypeEnum.Shell) {
			return shell;
		} else if (cellType == CellTypeEnum.Vein) {
			return vein;
		}
		return Color.black;
	}
}