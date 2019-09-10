public class VeinCell : Cell {

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		if (PhenotypePhysicsPanel.instance.functionVein.isOn) {
			effectProductionInternalDown = GlobalSettings.instance.phenotype.veinCellEffectCost;
			effectProductionInternalUp = 0f;

			base.UpdateCellWork(deltaTicks, worldTicks);
		} else {
			effectProductionInternalDown = 0f;
			effectProductionInternalUp = 0f;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Vein;
	}
}