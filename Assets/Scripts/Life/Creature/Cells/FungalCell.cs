﻿public class FungalCell : Cell {

	public FungalCell() : base() {
		springFrequenzy = 5f;
		springDamping = 11f;
	}

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (GlobalPanel.instance.physicsFungal.isOn) {
			effectConsumptionInternal = GlobalSettings.instance.phenotype.fungalCellEffectCost;
			effectProductionInternal = 0f;
			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectConsumptionInternal = 0f;
			effectProductionInternal = 0f;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Fungal;
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