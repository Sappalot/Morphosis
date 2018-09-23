using UnityEngine;

public class EggCell : Cell {

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (PhenotypePhysicsPanel.instance.functionEgg.isOn) {
			effectProductionInternalUp = 0f;
			effectProductionInternalDown = GlobalSettings.instance.phenotype.eggCellEffectCost;

			if (creature.phenotype.originCell.originPulseTick == 0) {
				if (energyFullness > gene.eggCellFertilizeThreshold && (gene.eggCellCanFertilizeWhenAttached || !creature.phenotype.hasPlacentaSpringsToMother) && shouldFertilize == -1) {
					shouldFertilize = 0; // Random.Range(0, 60);
				}
			}

			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectProductionInternalUp = 0f;
			effectProductionInternalDown = 0f;
		}
	}

	[HideInInspector]
	private int m_shouldFertilize = -1;
	public int shouldFertilize {
		get {
			return m_shouldFertilize;
		}
		set {
			m_shouldFertilize = value;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Egg;
	}
}
