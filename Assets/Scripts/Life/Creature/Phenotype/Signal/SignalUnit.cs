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

	public int SignalUnitSlotOutputToIndex(SignalUnitSlotEnum slot) {
		if (slot == SignalUnitSlotEnum.outputLateA) {
			return 0;
		} else if (slot == SignalUnitSlotEnum.outputLateB) {
			return 1;
		} else if (slot == SignalUnitSlotEnum.outputLateC) {
			return 2;
		} else if (slot == SignalUnitSlotEnum.outputLateD) {
			return 3;
		} else if (slot == SignalUnitSlotEnum.outputLateE) {
			return 4;
		} else if (slot == SignalUnitSlotEnum.outputLateF) {
			return 5;
		}
		return -1;
	}
}
