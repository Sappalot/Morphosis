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

	public override void Clear() {
		for (int i = 0; i < output.Length; i++) {
			output[i] = false;
		}
	}

	// Load Save
	private CommonSensorData sensorData = new CommonSensorData();

	// Save
	public CommonSensorData UpdateData() {
		sensorData.slotA = output[0];
		sensorData.slotB = output[1];
		sensorData.slotC = output[2];
		sensorData.slotD = output[3];
		sensorData.slotE = output[4];
		sensorData.slotF = output[5];
		return sensorData;
	}

	// Load
	public void ApplyData(CommonSensorData sensorData) {
		output[0] = sensorData.slotA;
		output[1] = sensorData.slotB;
		output[2] = sensorData.slotC;
		output[3] = sensorData.slotD;
		output[4] = sensorData.slotE;
		output[5] = sensorData.slotF;
	}
}