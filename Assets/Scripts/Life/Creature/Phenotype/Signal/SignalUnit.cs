// 
public abstract class SignalUnit {
	
	protected SignalUnitEnum signalUnit;

	public virtual bool GetOutput(SignalUnitSlotEnum signalUnitSlot) { return false; }

	public virtual void ComputeSignalOutput(Cell hostCell, int deltaTicks, ulong worldTicks) { }

	public virtual void FeedSignal() { }
}
