// Information on how to connect any of creatures outputs output to an input
public class GeneNerve {
	public SignalUnitEnum outputUnit = SignalUnitEnum.Void; // The output from me "the nerve" The GeneSensor that created this one
	public SignalUnitSlotEnum outputUnitSlot; // The slot on that (above) unit
	public SignalUnitEnum inputUnit = SignalUnitEnum.Void; // The input to me "the nerve" (Somebodey elses output)
	public SignalUnitSlotEnum inputUnitSlot; // The slot on that (above) unit


	// Save
	private GeneNerveData geneNerveData = new GeneNerveData();
	public GeneNerveData UpdateData() {
		geneNerveData.outputUnit = outputUnit;
		geneNerveData.outputUnitSlot = outputUnitSlot;
		geneNerveData.inputUnit = inputUnit;
		geneNerveData.inputUnitSlot = inputUnitSlot;
		return geneNerveData;
	}

	//Load
	public void ApplyData(GeneNerveData geneNerveData) {
		outputUnit = geneNerveData.outputUnit;
		outputUnitSlot = geneNerveData.outputUnitSlot;
		inputUnit = geneNerveData.inputUnit;
		inputUnitSlot = geneNerveData.inputUnitSlot;
	}
}