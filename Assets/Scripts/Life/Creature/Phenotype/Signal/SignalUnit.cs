public abstract class SignalUnit {
	protected bool output;
	protected SignalUnitEnum signalUnit;

	public virtual bool GetOutput() {
		return false;
	}

	public virtual void UpdateOutputs(Cell hostCell, int deltaTicks, ulong worldTicks) { }
}
