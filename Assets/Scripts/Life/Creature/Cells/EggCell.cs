using UnityEngine;

public class EggCell : Cell {
	public Sensor fertilizeEnergySensor = new EnergySensor(); // locked one

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		if (PhenotypePhysicsPanel.instance.functionEgg.isOn) {
			if (IsHibernating()) {
				effectProductionInternalUp = 0f;
				effectProductionInternalDown = GlobalSettings.instance.phenotype.cellHibernateEffectCost;
			} else {
				effectProductionInternalUp = 0f;
				effectProductionInternalDown = GlobalSettings.instance.phenotype.eggCellEffectCost;
				if (energyFullness > gene.eggCellFertilizeThreshold) {
					shouldFertilize = true;
				}
			}

			base.UpdateCellWork(deltaTicks, worldTicks);
		} else {
			effectProductionInternalUp = 0f;
			effectProductionInternalDown = 0f;
		}
	}

	override public bool IsHibernating() {
		return (gene.eggCellHibernateWhenAttachedToMother && creature.IsAttachedToMotherAlive()) || (gene.eggCellHibernateWhenAttachedToChild && creature.IsAttachedToChildAlive());
	}

	[HideInInspector]
	public bool shouldFertilize = false;

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Egg;
	}
}
