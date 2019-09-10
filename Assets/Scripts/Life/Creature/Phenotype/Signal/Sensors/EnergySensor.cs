using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySensor : Sensor {
	public bool isOutputOn { get; private set; }
	public float threshold;

	public override SensorTypeEnum GetSensorType() {
		return SensorTypeEnum.Energy;
	}

	public override void UpdateOutputs(int deltaTicks, ulong worldTicks) {
		isOutputOn = cell.GetEffect(true, true, true, true) >= threshold;
	}
}