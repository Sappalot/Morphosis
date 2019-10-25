using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeSensor : SignalUnit {
	private bool[] output = new bool[6]; // outputs 

	public float threshold;

	public SizeSensor(SignalUnitEnum outputUnit) {
		this.signalUnit = outputUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[(int)signalUnitSlot];
	}

	public override void ComputeSignalOutput(Cell hostCell, int deltaTicks) {
		if (signalUnit == SignalUnitEnum.OriginSizeSensor) {
			for (int i = 0; i < output.Length; i++) {
				if (i == 5) {
					output[5] = hostCell.creature.phenotype.cellCount >= hostCell.creature.CompletenessCellCount((hostCell.gene.originSizeSensor as GeneSizeSensor).sizeThreshold); // F
				} else {
					output[i] = false;
				}
			}
		}
	}

	public override void Clear() {
		for (int i = 0; i < output.Length; i++) {
			output[i] = false;
		}
	}

	// Load Save
	private SizeSensorData sizeSensorData = new SizeSensorData();

	// Save
	public SizeSensorData UpdateData() {
		sizeSensorData.slotA = output[0];
		sizeSensorData.slotB = output[1];
		sizeSensorData.slotC = output[2];
		sizeSensorData.slotD = output[3];
		sizeSensorData.slotE = output[4];
		sizeSensorData.slotF = output[5];
		return sizeSensorData;
	}

	// Load
	public void ApplyData(SizeSensorData sizeSensorData) {
		output[0] = sizeSensorData.slotA;
		output[1] = sizeSensorData.slotB;
		output[2] = sizeSensorData.slotC;
		output[3] = sizeSensorData.slotD;
		output[4] = sizeSensorData.slotE;
		output[5] = sizeSensorData.slotF;
	}
}