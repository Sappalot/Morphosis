using System.Collections.Generic;

public class GeneLogicBoxInput : GeneLogicBoxPart {

	public SignalValveModeEnum valveMode = SignalValveModeEnum.Pass;

	//Todo: have univeral reference to input which is possible to save
	// from this universal input, we should be able to retreive reference to input

	public GeneLogicBoxInput(int row, int column) {
		this.row = row;
		leftFlank = GetFlankLeftOfColumn(column);
		rightFlank = GetFlankRightOfColumn(column);
	}

	public override int GetTransmittingInputCount() {
		return isTransmittingSignal ? 1 : 0;
	}

	// Save
	private GeneLogicBoxInputData geneLogicBoxInputData = new GeneLogicBoxInputData();
	public GeneLogicBoxInputData UpdateData() {
		geneLogicBoxInputData.valveMode = valveMode;
		return geneLogicBoxInputData;
	}

	// Load
	public void ApplyData(GeneLogicBoxInputData geneLogicBoxInputData) {
		if (!isLocked) {
			valveMode = geneLogicBoxInputData.valveMode;
		}
	}
}
