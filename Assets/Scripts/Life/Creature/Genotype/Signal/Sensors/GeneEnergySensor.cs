public class GeneEnergySensor : GeneSignalUnit {
	public GeneEnergySensor(SignalUnitEnum signalUnit, bool isLocked) {
		this.signalUnit = signalUnit;
		this.isLocked = isLocked;
	}

	public int radius = 0; // only me
	public float threshold = 50f; // joules
}
