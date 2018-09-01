using UnityEngine;

public class EggCell : Cell {

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (GlobalPanel.instance.physicsEgg.isOn) {
			effectUpInternal = 0f;
			effectDownInternal = GlobalSettings.instance.phenotype.eggCellEffectCost;

			if (energyFullness > eggCellFertilizeThreshold && (eggCellCanFertilizeWhenAttached || !creature.phenotype.hasPlacentaSpringsToMother) && shouldFertilize == -1) {
				shouldFertilize = 0; // Random.Range(0, 60);
			}

			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectUpInternal = 0f;
			effectDownInternal = 0f;
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
