using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSensor : SignalUnit {
	
	public float threshold;

	public EffectSensor(SignalUnitEnum outputUnit) {
		this.signalUnit = outputUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[(int)signalUnitSlot];
	}

	public override void UpdateOutputs(Cell hostCell, int deltaTicks, ulong worldTicks) {
		if (signalUnit == SignalUnitEnum.EffectSensor) {
			for (int i = 0; i < output.Length; i++) {
				output[i] = hostCell.GetEffect(true, true, true, true) >= hostCell.gene.effectSensor.threshold;

				if (i == 3) {
					output[i] = false;
				}
			}
		} 
		// Other energy sensor
		
	}
}