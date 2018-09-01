public class FungalCell : Cell {

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (GlobalPanel.instance.physicsFungal.isOn) {
			effectDownInternal = GlobalSettings.instance.phenotype.fungalCellEffectCost;
			effectUpInternal = 0f;
			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectDownInternal = 0f;
			effectUpInternal = 0f;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Fungal;
	}
}