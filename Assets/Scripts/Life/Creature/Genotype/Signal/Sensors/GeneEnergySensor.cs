public class GeneEnergySensor : GeneSignalUnit {
	public GeneEnergySensor(SignalUnitEnum signalUnit) {
		this.signalUnit = signalUnit;
	}

	public float threshold = 50f; // joules

	// Save
	private GeneEnergySensorData geneEnergySensorData = new GeneEnergySensorData();
	public GeneEnergySensorData UpdateData() {
		geneEnergySensorData.energyThreshold = threshold;
		return geneEnergySensorData;
	}

	// Load
	public void ApplyData(GeneEnergySensorData geneEnergySensorData) {
		threshold = geneEnergySensorData.energyThreshold;
	}
}
