public class RootCell : Cell {

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		effectProductionInternalDown = GlobalSettings.instance.phenotype.rootCell.effectProductionDown;
		effectProductionInternalUp = 0f;
		base.UpdateCellWork(deltaTicks, worldTicks);
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Root;
	}
}