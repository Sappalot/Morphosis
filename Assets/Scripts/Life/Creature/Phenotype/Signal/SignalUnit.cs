// 
public abstract class SignalUnit {
	
	protected SignalUnitEnum signalUnit;

	public virtual void UpdateSignalConnections(Cell hostCell) { }

	public virtual bool GetOutput(SignalUnitSlotEnum signalUnitSlot) { return false; }

	public virtual void ComputeSignalOutput(Cell hostCell, int deltaTicks) { }

	public virtual void FeedSignal() { }

	public abstract void Clear();
}
