// 
public abstract class SignalUnit {
	
	protected SignalUnitEnum signalUnit;
	protected Cell hostCell;

	public SignalUnit(Cell hostCell) {
		this.hostCell = hostCell;
	}

	public virtual void UpdateSignalConnections() { }

	public virtual bool GetOutput(SignalUnitSlotEnum signalUnitSlot) { return false; }

	public virtual void ComputeSignalOutput(int deltaTicks) { }

	public virtual void FeedSignal() { }

	public abstract void Clear();
}
