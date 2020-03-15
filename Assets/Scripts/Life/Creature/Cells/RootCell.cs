public class RootCell : Cell {

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		base.UpdateCellWork(deltaTicks, worldTicks);
		effectProductionInternalDown = GlobalSettings.instance.phenotype.rootCell.effectProductionDown;
		effectProductionInternalUp = 0f;
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Root;
	}
}