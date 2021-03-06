﻿using UnityEngine;

public class ShellCell : Cell {


	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Shell;
	}

	public override Color GetColor(PhenoGenoEnum phenoGeno) {
		return ColorScheme.instance.ToColor(GetCellType());
		//return Color.Lerp(ColorScheme.instance.shellArmourOpaque.Evaluate(GetNormalizedArmor(armorClass)), ColorScheme.instance.shellArmourTransparent.Evaluate(GetNormalizedArmor(armorClass)), GetTransparancy(transparencyClass));
	}

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		base.UpdateCellWork(deltaTicks, worldTicks);
		effectProductionInternalDown = GlobalSettings.instance.phenotype.shellCell.effectProductionDown;
		effectProductionInternalUp = 0f;
	}

	public override float Transparency() {
		return GlobalSettings.instance.phenotype.shellCell.transparency;
	}

	// old transparent shell below

	//public static int armourClassCount = 6;
	//public static int transparencyClassCount = 6;

	//public enum ShellMaterial {
	//	Wood,
	//	Metal,
	//	Glass,
	//	Diamond,
	//}

	//public ShellMaterial material {
	//	get {
	//		if (armorClass <= 2) {
	//			if (transparancyClass <= 2) {
	//				return ShellMaterial.Wood;
	//			} else {
	//				return ShellMaterial.Glass;
	//			}
	//		} else {
	//			if (transparancyClass <= 2) {
	//				return ShellMaterial.Metal;
	//			} else {
	//				return ShellMaterial.Diamond;
	//			}
	//		}
	//	}
	//}

	//public int armorClass {
	//	get {
	//		return gene.shellCellArmorClass;
	//	}
	//}

	//public int transparancyClass {
	//	get {
	//		return gene.shellCellTransparancyClass; 
	//	}
	//}

	//static public float GetNormalizedArmor(int armorClass) {
	//	return (float)armorClass / (float)(armourClassCount - 1);
	//}

	//public float normalizedArmor { // from 0 to 1
	//	get {
	//		return GetNormalizedArmor(armorClass);
	//	}
	//}

	//static public float GetTransparancy(int transparencyClass) {
	//	return (float)transparencyClass / (float)(transparencyClassCount - 1);
	//}

	//public override float transparency { //from 0 to 1
	//	get {
	//		return GetTransparancy(transparancyClass);
	//	}
	//}

	//static public float GetStrength(int armourClass) {
	//	return GlobalSettings.instance.phenotype.shellCellStrengthAtArmor.Evaluate(GetNormalizedArmor(armourClass));
	//}

	//public static float GetEffectCost(float normalizedArmor, float transparency) {
	//	return GlobalSettings.instance.phenotype.shellCellEffectCostAtArmor.Evaluate(normalizedArmor) * GlobalSettings.instance.phenotype.shellCellEffectCostMultiplierAtTransparancy.Evaluate(transparency);
	//}

	//public static float GetEffectCost(int armourClass, int transparencyClass) {
	//	return GetEffectCost(GetNormalizedArmor(armourClass), GetTransparancy(transparencyClass));
	//}

	//public float effectCost {
	//	get {
	//		return GetEffectCost(normalizedArmor, transparency);
	//	}
	//}

	//public static Color GetStrongerColor(int armorClass, int transparencyClass) {
	//	return Color.Lerp(ColorScheme.instance.shellArmourOpaqueClear.Evaluate(GetNormalizedArmor(armorClass)), ColorScheme.instance.shellArmourTransparentClear.Evaluate(GetNormalizedArmor(armorClass)), GetTransparancy(transparencyClass));
	//}

	//public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
	//	effectProductionInternalDown = effectCost;//GlobalSettings.instance.phenotype.shellCellEffectCost;
	//	effectProductionInternalUp = 0f;
	//	base.UpdateCellWork(deltaTicks, worldTicks);
	//}

	//public override void SetNormalDrag() {
	//	theRigidBody.drag = GlobalSettings.instance.phenotype.normalShellDrag;
	//}

	//override public Color GetColor(PhenoGenoEnum phenoGeno) {
	//	return GetColor(armorClass, transparancyClass);
	//}
}