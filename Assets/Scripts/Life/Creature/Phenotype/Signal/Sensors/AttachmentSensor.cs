﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentSensor : SignalUnit {
	private bool[] output = new bool[6]; // outputs 

	public AttachmentSensor(SignalUnitEnum outputUnit) {
		this.signalUnit = outputUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[(int)signalUnitSlot];
	}

	public override void ComputeSignalOutput(Cell hostCell, int deltaTicks) {
		// There are no settings on this sensor, so it doesn't matter which sensor this is, it should be reporting the same info
		output[0] = hostCell.creature.IsAttachedToMotherAlive();
		output[1] = !hostCell.creature.IsAttachedToMotherAlive();
		output[2] = hostCell.creature.IsAttachedToChildAlive();
		output[3] = !hostCell.creature.IsAttachedToChildAlive();
		output[4] = false; // TODO: several children
		output[5] = false; // TODO: several children
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
	public void ApplyData(CommonSensorData energySensorData) {
		output[0] = energySensorData.slotA;
		output[1] = energySensorData.slotB;
		output[2] = energySensorData.slotC;
		output[3] = energySensorData.slotD;
		output[4] = energySensorData.slotE;
		output[5] = energySensorData.slotF;
	}
}