public class RootCell : Cell {

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		if (PhenotypePhysicsPanel.instance.functionRoot.isOn) {
			effectProductionInternalDown = GlobalSettings.instance.phenotype.rootCellEffectCost;
			effectProductionInternalUp = 0f;

			base.UpdateCellWork(deltaTicks, worldTicks);
		} else {
			effectProductionInternalDown = 0f;
			effectProductionInternalUp = 0f;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Root;
	}
}