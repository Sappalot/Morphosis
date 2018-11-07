﻿using UnityEngine;

public class EggCell : Cell {

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (PhenotypePhysicsPanel.instance.functionEgg.isOn) {
			if (IsIdle()) {
				effectProductionInternalUp = 0f;
				effectProductionInternalDown = GlobalSettings.instance.phenotype.cellIdleEffectCost;
			} else {
				effectProductionInternalUp = 0f;
				effectProductionInternalDown = GlobalSettings.instance.phenotype.eggCellEffectCost;
				if (energyFullness > gene.eggCellFertilizeThreshold) {
					shouldFertilize = true;
				}
			}

			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectProductionInternalUp = 0f;
			effectProductionInternalDown = 0f;
		}
	}

	override public bool IsIdle() {
		return gene.eggCellIdleWhenAttached && creature.IsAttachedToMotherAlive();
	}

	[HideInInspector]
	public bool shouldFertilize = false;

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Egg;
	}
}
