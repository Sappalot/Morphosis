public class JawCell : Cell {

	public JawCellMouth mouth;

	public JawCell() : base() {
		springFrequenzy = 5f;
		springDamping = 11f;
	}

	public override void UpdateMetabolism(int deltaTicks, ulong worldTicks) {
		effectConsumptionInternal = GlobalSettings.instance.phenotype.jawCellEffectCost;

		float weightedPrayCount = 0f;
		foreach (Cell pray in mouth.prays) {
			if (pray.GetCellType() == CellTypeEnum.Jaw) {
				weightedPrayCount += GlobalSettings.instance.phenotype.jawCellEatJawCellFactor;
			} else if (pray.GetCellType() == CellTypeEnum.Shell) {
				weightedPrayCount +=  GlobalSettings.instance.phenotype.jawCellEatShellSellFactor;
			} else {
				weightedPrayCount += 1f;
			}
		}
		effectProduction = weightedPrayCount * GlobalSettings.instance.phenotype.jawCellEatEffect;

		//Hack release pray
		mouth.RemoveNullPrays();

		base.UpdateMetabolism(deltaTicks, worldTicks);
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Jaw;
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
