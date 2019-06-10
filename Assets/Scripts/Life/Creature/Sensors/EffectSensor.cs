using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSensor : Sensor {
	public bool isOutputOn { get; private set; }

	public override SensorTypeEnum GetSensorType() {
		return SensorTypeEnum.Effect;
	}

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		isOutputOn = cell.GetEffect(true, true, true, true) > 0f;
	}
}