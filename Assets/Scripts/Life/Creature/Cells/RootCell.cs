public class RootCell : Cell {

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (GlobalPanel.instance.physicsRoot.isOn) {
			effectProductionInternalDown = GlobalSettings.instance.phenotype.rootCellEffectCost;
			effectProductionInternalUp = 0f;

			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectProductionInternalDown = 0f;
			effectProductionInternalUp = 0f;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Root;
	}
}