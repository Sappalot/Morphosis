using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySensor : SignalUnit {
	private bool[] output = new bool[6]; // outputs 

	public float threshold;

	public EnergySensor(SignalUnitEnum outputUnit) {
		this.signalUnit = outputUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[(int)signalUnitSlot];
	}

	public override void UpdateOutputs(Cell hostCell, int deltaTicks, ulong worldTicks) {
		if (signalUnit == SignalUnitEnum.WorkSensorA && hostCell.GetCellType() == CellTypeEnum.Egg) {
			for (int i = 0; i < output.Length; i++) {
				output[i] = hostCell.energy >= (hostCell.gene.eggCellFertilizeEnergySensor as GeneEnergySensor).threshold;

				if (i == 3) {
					output[i] = false;
				}
			}
			
		} 
		// Other energy sensor
		
	}
}