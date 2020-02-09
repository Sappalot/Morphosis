public class VeinCell : Cell {

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		effectProductionInternalDown = GlobalSettings.instance.phenotype.veinCellEffectCost;
		effectProductionInternalUp = 0f;
		base.UpdateCellWork(deltaTicks, worldTicks);
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Vein;
	}
}