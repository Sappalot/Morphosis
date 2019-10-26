using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeSensor : SignalUnit {
	private bool[] output = new bool[6]; // outputs 

	public SizeSensor(SignalUnitEnum outputUnit) {
		this.signalUnit = outputUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[(int)signalUnitSlot];
	}

	public override void ComputeSignalOutput(Cell hostCell, int deltaTicks) {
		if (signalUnit == SignalUnitEnum.OriginSizeSensor) {
			output[0] = hostCell.creature.phenotype.cellCount >= hostCell.creature.CellCountAtCompleteness((hostCell.gene.originSizeSensor as GeneSizeSensor).sizeThreshold); // A
			output[1] = hostCell.creature.phenotype.cellCount < hostCell.creature.CellCountAtCompleteness((hostCell.gene.originSizeSensor as GeneSizeSensor).sizeThreshold); // B
			output[2] = false;
			output[3] = false;
			output[4] = hostCell.creature.growthBlocked * GlobalSettings.instance.quality.growTickPeriod * Time.fixedDeltaTime >= (hostCell.gene.originSizeSensor as GeneSizeSensor).growthBlockedPatienseThreshold; // E
			output[5] = hostCell.creature.phenotype.cellCount >= hostCell.creature.CellCountAtCompleteness(hostCell.gene.embryoMaxSizeCompleteness); // F
		}
	}

	public override void Clear() {
		for (int i = 0; i < output.Length; i++) {
			output[i] = false;
		}
	}

	// Load Save
	private CommonSensorData sizeSensorData = new CommonSensorData();

	// Save
	public CommonSensorData UpdateData() {
		sizeSensorData.slotA = output[0];
		sizeSensorData.slotB = output[1];
		sizeSensorData.slotC = output[2];
		sizeSensorData.slotD = output[3];
		sizeSensorData.slotE = output[4];
		sizeSensorData.slotF = output[5];
		return sizeSensorData;
	}

	// Load
	public void ApplyData(CommonSensorData sizeSensorData) {
		output[0] = sizeSensorData.slotA;
		output[1] = sizeSensorData.slotB;
		output[2] = sizeSensorData.slotC;
		output[3] = sizeSensorData.slotD;
		output[4] = sizeSensorData.slotE;
		output[5] = sizeSensorData.slotF;
	}
}