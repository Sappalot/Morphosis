﻿public abstract class SignalUnit {
	protected bool[] output = new bool[6];
	protected SignalUnitEnum signalUnit;

	public virtual bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return false;
	}

	public virtual void UpdateOutputs(Cell hostCell, int deltaTicks, ulong worldTicks) { }
}
