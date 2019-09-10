using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSensor : Sensor {
	
	public float threshold;

	public EffectSensor(SignalUnitEnum outputUnit) {
		this.signalUnit = outputUnit;
	}

	public override bool GetOutput() {
		return output;
	}

	public override void UpdateOutputs(Cell hostCell, int deltaTicks, ulong worldTicks) {
		if (signalUnit == SignalUnitEnum.EffectSensor) {
			output = hostCell.GetEffect(true, true, true, true) >= hostCell.gene.effectSensor.threshold;
		} 
		// Other energy sensor
		
	}
}