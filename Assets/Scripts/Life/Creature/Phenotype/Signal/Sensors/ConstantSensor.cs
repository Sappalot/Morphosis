using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantSensor : SignalUnit {
	private bool[] output = new bool[6]; // outputs 

	public ConstantSensor(SignalUnitEnum signalUnit, Cell hostCell) : base(hostCell) {
		this.signalUnitEnum = signalUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[SignalUnitSlotOutputToIndex(signalUnitSlot)];
	}

	// TODO: optimize don't update unnessessarily
	// TODO: add random 0/1 at startup. odds controlled by gene 
	public override void ComputeSignalOutput(int deltaTicks) {
		for (int i = 0; i < output.Length; i++) {
			output[i] = i == 1; // only slot 1 is 1 rest is 0
		}
	}

	public override void Clear() {
		for (int i = 0; i < output.Length; i++) {
			output[i] = false;
		}
	}

	// Load Save
	private CommonSensorData constantSensorData = new CommonSensorData();

	// Save
	public CommonSensorData UpdateData() {
		constantSensorData.slotA = output[0];
		constantSensorData.slotB = output[1];
		constantSensorData.slotC = output[2];
		constantSensorData.slotD = output[3];
		constantSensorData.slotE = output[4];
		constantSensorData.slotF = output[5];
		return constantSensorData;
	}

	// Load
	public void ApplyData(CommonSensorData constantSensorData) {
		output[0] = constantSensorData.slotA;
		output[1] = constantSensorData.slotB;
		output[2] = constantSensorData.slotC;
		output[3] = constantSensorData.slotD;
		output[4] = constantSensorData.slotE;
		output[5] = constantSensorData.slotF;
	}
}