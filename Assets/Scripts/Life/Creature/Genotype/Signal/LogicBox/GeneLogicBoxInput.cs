using System.Collections.Generic;

public class GeneLogicBoxInput : GeneLogicBoxPart {

	public SignalValveModeEnum valveMode = SignalValveModeEnum.Pass;
	public GeneNerve nerve = new GeneNerve();

	public SignalUnitEnum input {
		get {
			return nerve.inputUnit;
		}
		set {
			nerve.inputUnit = value;
		}
	}

	public GeneLogicBoxInput(int row, int column) {
		this.row = row;
		leftFlank = GetFlankLeftOfColumn(column);
		rightFlank = GetFlankRightOfColumn(column);
	}

	public int column {
		get {
			return GetColumnRightOfFlank(leftFlank);
		}
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
