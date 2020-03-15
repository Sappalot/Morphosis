public class FungalCell : Cell {

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		base.UpdateCellWork(deltaTicks, worldTicks);
		effectProductionInternalDown = GlobalSettings.instance.phenotype.fungalCell.effectProductionDown;
		effectProductionInternalUp = 0f;
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Fungal;
	}
}