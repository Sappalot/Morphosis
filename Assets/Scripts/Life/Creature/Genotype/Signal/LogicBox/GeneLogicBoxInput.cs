public class GeneLogicBoxInput : GeneLogicBoxPart {

	// TODO make it so that nerve input can't be changed if locked
	public GeneNerve nerve = new GeneNerve();

	private SignalValveModeEnum m_valveMode;
	public SignalValveModeEnum valveMode { 
		get {
			return m_valveMode;
		}
		set {
			if (!isLocked) {
				m_valveMode = value;
			}
		}
	}

	public GeneLogicBoxInput(int row, int column, SignalUnitEnum signalUnit) {
		this.row = row;
		leftFlank = GetFlankLeftOfColumn(column);
		rightFlank = GetFlankRightOfColumn(column);
		nerve.outputUnit = signalUnit;
		nerve.outputUnitSlot = (SignalUnitSlotEnum)column;
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
		geneLogicBoxInputData.geneNerveData = nerve.UpdateData();
		return geneLogicBoxInputData;
	}

	// Load
	public void ApplyData(GeneLogicBoxInputData geneLogicBoxInputData) {
		valveMode = geneLogicBoxInputData.valveMode;
		nerve.ApplyData(geneLogicBoxInputData.geneNerveData); 
	}
}
