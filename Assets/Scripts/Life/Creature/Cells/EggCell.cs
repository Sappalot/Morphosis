
using UnityEngine;

public class EggCell : Cell {
	public EggCell() : base() {
		springFrequenzy = 5f;
		springDamping = 11f;
		m_shouldFertilize = -1;
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

	public override void UpdateMetabolism(int deltaTicks, ulong worldTicks) {
		effectConsumptionInternal = GlobalSettings.instance.phenotype.eggCellEffectCost;
		effectProduction = 0f;

		base.UpdateMetabolism(deltaTicks, worldTicks);

		if (energy > eggCellFertilizeThreshold && shouldFertilize == -1) {
			shouldFertilize = Random.Range(0, 60);
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
