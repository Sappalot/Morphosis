using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sensor {
	[HideInInspector]
	protected Cell cell;

	abstract public SensorTypeEnum GetSensorType();

	public virtual void Init(Cell cell) {
		this.cell = cell;
	}

	public virtual void UpdateOutputs(int deltaTicks, ulong worldTicks) { }
}
