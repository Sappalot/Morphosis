public class GeneEffectSensor : GeneSignalUnit {
	public GeneEffectSensor(SignalUnitEnum signalUnit, bool isLocked) {
		this.signalUnit = signalUnit;
	}

	public float threshold = 1f; // W
}
