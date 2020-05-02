using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeSensor : SignalUnit {
	private bool[] output = new bool[6]; // outputs 

	public SizeSensor(SignalUnitEnum signalUnit, Cell hostCell) : base(hostCell) {
		this.hostSignalUnitEnum = signalUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[SignalUnitSlotOutputToIndex(signalUnitSlot)];
	}

	public override void ComputeSignalOutput(int deltaTicks) {
		if (hostSignalUnitEnum == SignalUnitEnum.OriginSizeSensor) {
			if (!hostCell.gene.originSizeSensor.isRooted) {
				return;
			}

			output[0] = hostCell.creature.phenotype.cellCount >= hostCell.creature.CellCountAtCompleteness((hostCell.gene.originSizeSensor as GeneSizeSensor).sizeThreshold); // A
			output[1] = hostCell.creature.phenotype.cellCount < hostCell.creature.CellCountAtCompleteness((hostCell.gene.originSizeSensor as GeneSizeSensor).sizeThreshold); // B
			output[2] = hostCell.creature.canNotGrowMoreTicks == 0; // C
			output[3] = hostCell.creature.canNotGrowMoreTicks > 0; // D
			output[4] = hostCell.creature.canNotGrowMoreTicks * Time.fixedDeltaTime > (hostCell.gene.originSizeSensor as GeneSizeSensor).cantGrowMorePatienseThreshold; // E
			output[5] = hostCell.creature.phenotype.cellCount >= hostCell.creature.CellCountAtCompleteness(hostCell.gene.originEmbryoMaxSizeCompleteness); // F
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