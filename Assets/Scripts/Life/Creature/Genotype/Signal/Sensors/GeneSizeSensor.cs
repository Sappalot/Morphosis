public class GeneSizeSensor : GeneSignalUnit {
	public GeneSizeSensor(SignalUnitEnum signalUnit) {
		this.signalUnit = signalUnit;
	}

	public float sizeThreshold = 0.5f; // 50% of full size
	public int growthBlockedPatienseThreshold = 10; // seconds of blocked growth

	// Save
	private GeneSizeSensorData geneSizeSensorData = new GeneSizeSensorData();
	public GeneSizeSensorData UpdateData() {
		geneSizeSensorData.sizeThreshold = sizeThreshold;
		geneSizeSensorData.growthBlockedPatienseThreshold = growthBlockedPatienseThreshold;
		return geneSizeSensorData;
	}

	// Load
	public void ApplyData(GeneSizeSensorData geneSizeSensorData) {
		sizeThreshold = geneSizeSensorData.sizeThreshold;
		growthBlockedPatienseThreshold = geneSizeSensorData.growthBlockedPatienseThreshold;
	}
}
