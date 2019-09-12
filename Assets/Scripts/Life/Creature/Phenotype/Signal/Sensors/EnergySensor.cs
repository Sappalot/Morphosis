using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySensor : SignalUnit {
	
	public float threshold;

	public EnergySensor(SignalUnitEnum outputUnit) {
		this.signalUnit = outputUnit;
	}

	public override bool GetOutput() {
		return output;
	}

	public override void UpdateOutputs(Cell hostCell, int deltaTicks, ulong worldTicks) {
		if (signalUnit == SignalUnitEnum.WorkSensorA) {
			output = hostCell.energy >= hostCell.gene.eggCellFertilizeEnergySensor.threshold;
		} 
		// Other energy sensor
		
	}
}