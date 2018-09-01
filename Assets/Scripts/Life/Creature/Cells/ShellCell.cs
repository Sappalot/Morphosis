public class ShellCell : Cell {

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (GlobalPanel.instance.physicsShell.isOn) {
			effectDownInternal = GlobalSettings.instance.phenotype.shellCellEffectCost;
			effectUpInternal = 0f;

			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectDownInternal = 0f;
			effectUpInternal = 0f;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Shell;
	}
}