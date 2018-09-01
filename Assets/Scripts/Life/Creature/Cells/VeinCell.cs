public class VeinCell : Cell {

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (GlobalPanel.instance.physicsVein.isOn) {
			effectDownInternal = GlobalSettings.instance.phenotype.veinCellEffectCost;
			effectUpInternal = 0f;

			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectDownInternal = 0f;
			effectUpInternal = 0f;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Vein;
	}
}