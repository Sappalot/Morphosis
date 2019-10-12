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

	public override void ComputeSignalOutput(Cell hostCell, int deltaTicks) {
		if (signalUnit == SignalUnitEnum.WorkSensorA && hostCell.GetCellType() == CellTypeEnum.Egg) {
			for (int i = 0; i < output.Length; i++) {
				output[i] = hostCell.energy >= (hostCell.gene.eggCellFertilizeEnergySensor as GeneEnergySensor).threshold;
			}
			
		} else if (signalUnit == SignalUnitEnum.EnergySensor) {
			for (int i = 0; i < output.Length; i++) {
				output[i] = hostCell.energy >= (hostCell.gene.energySensor as GeneEnergySensor).threshold;
			}
		}
		// Other energy sensor
		
	}

	// Load Save
	private EnergySensorData energySensorData = new EnergySensorData();

	// Save
	public EnergySensorData UpdateData() {
		energySensorData.slotA = output[0];
		energySensorData.slotB = output[1];
		energySensorData.slotC = output[2];
		energySensorData.slotD = output[3];
		energySensorData.slotE = output[4];
		energySensorData.slotF = output[5];
		return energySensorData;
	}

	// Load
	public void ApplyData(EnergySensorData energySensorData) {
		output[0] = energySensorData.slotA;
		output[1] = energySensorData.slotB;
		output[2] = energySensorData.slotC;
		output[3] = energySensorData.slotD;
		output[4] = energySensorData.slotE;
		output[5] = energySensorData.slotF;
	}
}