public class FungalCell : Cell {

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (PhenotypePhysicsPanel.instance.functionFungal.isOn) {
			effectProductionInternalDown = GlobalSettings.instance.phenotype.fungalCellEffectCost;
			effectProductionInternalUp = 0f; // GlobalSettings.instance.phenotype.fungalCellEffect[6 - neighbourCountAll] * GlobalSettings.instance.phenotype.leafCellSunEffectFactorAtBodySize.Evaluate(creature.cellCount);

			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectProductionInternalDown = 0f;
			effectProductionInternalUp = 0f;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Fungal;
	}
}