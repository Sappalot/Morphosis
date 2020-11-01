public class VeinCell : Cell {

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		base.UpdateCellWork(deltaTicks, worldTicks);

		effectProductionInternalDown = GlobalSettings.instance.phenotype.veinCell.effectProductionDown;
		effectProductionInternalUp = 0f;
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Vein;
	}

	public override float Transparency() {
		return GlobalSettings.instance.phenotype.veinCell.transparency;
	}
}