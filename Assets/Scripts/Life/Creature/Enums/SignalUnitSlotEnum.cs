using System;


// En between cell Panel and Cell.Signal, to know which output we are trying to update in panel, we must know its name (can't have a reference in a good way)
[Serializable]
public enum SignalUnitSlotEnum {
	inputA,
	inputB,
	inputC,
	inputD,
	inputE,
	inputF,

	outputEarlyA,
	outputLateA,

	outputEarlyB,
	outputLateB,

	outputEarlyC,
	outputLateC,

	outputEarlyD,
	outputLateD,

	outputEarlyE,
	outputLateE,

	outputEarlyF,
	outputLateF,
}