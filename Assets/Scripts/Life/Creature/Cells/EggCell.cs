using UnityEngine;

public class EggCell : Cell {
	public EggCell() : base() {
		springFrequenzy = 5f;
		springDamping = 11f;
		m_shouldFertilize = -1;
	}

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		effectProductionInternal = 0f;
		effectConsumptionInternal = GlobalSettings.instance.phenotype.eggCellEffectCost;

		if (energy > eggCellFertilizeThreshold && (eggCellCanFertilizeWhenAttached || !creature.phenotype.isAttachedToMother) && shouldFertilize == -1) {
			shouldFertilize = Random.Range(0, 60);
		}

		base.UpdateCellFunction(deltaTicks, worldTicks);
	}

	[HideInInspector]
	private int m_shouldFertilize;
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

	public override void UpdateSpringFrequenzy() {
		base.UpdateSpringFrequenzy();

		if (HasOwnNeighbourCell(CardinalEnum.north)) {
			northSpring.frequency = (this.springFrequenzy + northNeighbour.cell.springFrequenzy) / 2f;
			northSpring.dampingRatio = (this.springDamping + northNeighbour.cell.springDamping) / 2f;
		}

		if (HasOwnNeighbourCell(CardinalEnum.southWest)) {
			southWestSpring.frequency = (this.springFrequenzy + southWestNeighbour.cell.springFrequenzy) / 2f;
			southWestSpring.dampingRatio = (this.springDamping + southWestNeighbour.cell.springDamping) / 2f;
		}

		if (HasOwnNeighbourCell(CardinalEnum.southEast)) {
			southEastSpring.frequency = (this.springFrequenzy + southEastNeighbour.cell.springFrequenzy) / 2f;
			southEastSpring.dampingRatio = (this.springDamping + southEastNeighbour.cell.springDamping) / 2f;
		}
	}
}
