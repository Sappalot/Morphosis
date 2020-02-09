public class FungalCell : Cell {

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		effectProductionInternalDown = GlobalSettings.instance.phenotype.fungalCellEffectCost;
		effectProductionInternalUp = 0f; // GlobalSettings.instance.phenotype.fungalCellEffect[6 - neighbourCountAll] * GlobalSettings.instance.phenotype.leafCellSunEffectFactorAtBodySize.Evaluate(creature.cellCount);
		base.UpdateCellWork(deltaTicks, worldTicks);
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Fungal;
	}
}