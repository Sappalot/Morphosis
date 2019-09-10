using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sensor {
	protected bool output;
	protected SignalUnitEnum signalUnit;

	public virtual bool GetOutput() {
		return false;
	}

	public virtual void UpdateOutputs(Cell hostCell, int deltaTicks, ulong worldTicks) { }
}
