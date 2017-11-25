﻿public class ShellCell : Cell {

	public ShellCell() : base() {
		springFrequenzy = 5f;
		springDamping = 11f;
	}

	public override void UpdateMetabolism(float deltaTime) {
		effectConsumptionInternal = GlobalSettings.instance.shellCellEffectCost;
		effectProduction = 0f;

		base.UpdateMetabolism(deltaTime);
	}

	public override float effectConsumptionExternal {
		get {
			return predatorCount * GlobalSettings.instance.jawCellEatEffect * GlobalSettings.instance.jawCellEatShellSellFactor;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Shell;
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