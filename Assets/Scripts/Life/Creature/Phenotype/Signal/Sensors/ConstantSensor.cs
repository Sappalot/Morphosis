using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantSensor : SignalUnit {
	private bool[] output = new bool[6]; // outputs 

	public ConstantSensor(SignalUnitEnum outputUnit) {
		this.signalUnit = outputUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[(int)signalUnitSlot];
	}

	// TODO: optimize don't update unnessessarily
	// TODO: add random 0/1 at startup. odds controlled by gene 
	public override void ComputeSignalOutput(Cell hostCell, int deltaTicks, ulong worldTicks) {
		for (int i = 0; i < output.Length; i++) {
			output[i] = i == 1; // only slot 1 is 1 rest is 0
		}
	}
}