public class RootCell : Cell {

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (GlobalPanel.instance.physicsRoot.isOn) {
			effectDownInternal = GlobalSettings.instance.phenotype.rootCellEffectCost;
			effectUpInternal = 0f;

			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectDownInternal = 0f;
			effectUpInternal = 0f;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Root;
	}
}