public class GeneEnergySensor : GeneSensor {
	public GeneEnergySensor(SignalUnitEnum signalUnit, bool isLocked) {
		this.signalUnit = signalUnit;
		this.isLocked = isLocked;
	}

	public int radius = 0; // only me
	public float threshold = 50f; // joules
}
