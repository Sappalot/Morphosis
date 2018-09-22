public class ShellCell : Cell {

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (PhenotypePhysicsPanel.instance.functionShell.isOn) {
			effectProductionInternalDown = GlobalSettings.instance.phenotype.shellCellEffectCost;
			effectProductionInternalUp = 0f;

			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectProductionInternalDown = 0f;
			effectProductionInternalUp = 0f;
		}
	}

	public override void SetNormalDrag() {
		theRigidBody.drag = GlobalSettings.instance.phenotype.normalShellDrag;
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Shell;
	}
}