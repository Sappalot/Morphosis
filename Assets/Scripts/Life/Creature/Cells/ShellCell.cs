using UnityEngine;

public class ShellCell : Cell {
	public static int armourClassCount = 6;
	public static int transparencyClassCount = 6;

	public enum ShellMaterial {
		Wood,
		Metal,
		Glass,
		Diamond,
	}

	public ShellMaterial material {
		get {
			if (armorClass <= 2) {
				if (transparancyClass <= 2) {
					return ShellMaterial.Wood;
				} else {
					return ShellMaterial.Glass;
				}
			} else {
				if (transparancyClass <= 2) {
					return ShellMaterial.Metal;
				} else {
					return ShellMaterial.Diamond;
				}
			}
		}
	}

	public int armorClass {
		get {
			return gene.shellCellArmorClass;
		}
	}

	public int transparancyClass {
		get {
			return gene.shellCellTransparancyClass; 
		}
	}

	static public float GetNormalizedArmor(int armorClass) {
		return (float)armorClass / (float)(armourClassCount - 1);
	}

	public float normalizedArmor { // from 0 to 1
		get {
			return GetNormalizedArmor(armorClass);
		}
	}

	static public float GetTransparancy(int transparencyClass) {
		return (float)transparencyClass / (float)(transparencyClassCount - 1);
	}

	public override float transparency { //from 0 to 1
		get {
			return GetTransparancy(transparancyClass);
		}
	}

	static public float GetStrength(int armourClass) {
		return GlobalSettings.instance.phenotype.shellCellStrengthAtArmor.Evaluate(GlobalSettings.instance.phenotype.shellCellArmorAtNormalizedArmorClass.Evaluate(GetNormalizedArmor(armourClass)));
	}

	public static float GetEffectCost(float normalizedArmor, float transparency) {
		return GlobalSettings.instance.phenotype.shellCellEffectCostAtArmor.Evaluate(GlobalSettings.instance.phenotype.shellCellArmorAtNormalizedArmorClass.Evaluate(normalizedArmor)) * GlobalSettings.instance.phenotype.shellCellEffectCostMultiplierAtTransparancy.Evaluate(transparency);
	}

	public static float GetEffectCost(int armourClass, int transparencyClass) {
		return GetEffectCost(GetNormalizedArmor(armourClass), GetTransparancy(transparencyClass));
	}

	public float effectCost {
		get {
			return GetEffectCost(normalizedArmor, transparency);
		}
	}

	public static Color GetColor(int armorClass, int transparencyClass) {
		return Color.Lerp(ColorScheme.instance.shellArmourOpaque.Evaluate(GetNormalizedArmor(armorClass)), ColorScheme.instance.shellArmourTransparent.Evaluate(GetNormalizedArmor(armorClass)), GetTransparancy(transparencyClass));
	}

	public static Color GetStrongerColor(int armorClass, int transparencyClass) {
		return Color.Lerp(ColorScheme.instance.shellArmourOpaqueClear.Evaluate(GetNormalizedArmor(armorClass)), ColorScheme.instance.shellArmourTransparentClear.Evaluate(GetNormalizedArmor(armorClass)), GetTransparancy(transparencyClass));
	}

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (PhenotypePhysicsPanel.instance.functionShell.isOn) {
			effectProductionInternalDown = effectCost;//GlobalSettings.instance.phenotype.shellCellEffectCost;
			effectProductionInternalUp = 0f;

			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectProductionInternalDown = 0f;
			effectProductionInternalUp = 0f;
		}
	}

	public override void SetNormalDrag() {
		theRigidBody.drag = GlobalSettings.instance.phenotype.normalShellDrag;
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Shell;
	}

	override public Color GetColor(PhenoGenoEnum phenoGeno) {
		return GetColor(armorClass, transparancyClass);
	}

	public Color GetStrongerColor() {
		return GetStrongerColor(armorClass, transparancyClass);
	}
}