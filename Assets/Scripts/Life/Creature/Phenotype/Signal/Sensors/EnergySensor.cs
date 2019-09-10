using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySensor : Sensor {
	
	public float threshold;

	public EnergySensor(SignalUnitEnum outputUnit) {
		this.signalUnit = outputUnit;
	}

	public override bool GetOutput() {
		return output;
	}

	public override void UpdateOutputs(Cell hostCell, int deltaTicks, ulong worldTicks) {
		if (signalUnit == SignalUnitEnum.WorkEggEnergySensor) {
			output = hostCell.energy >= hostCell.gene.eggCellFertilizeEnergySensor.threshold;
		} else if (signalUnit == SignalUnitEnum.WorkEggFertilizeLogicBox) {
			// TODO: implement
		}
		
	}
}