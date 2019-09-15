public class GeneEffectSensor : GeneSignalUnit {
	public GeneEffectSensor(SignalUnitEnum signalUnit, bool isLocked) {
		this.signalUnit = signalUnit;
		this.isLocked = isLocked;
	}

	public float threshold = 1f; // W
}
