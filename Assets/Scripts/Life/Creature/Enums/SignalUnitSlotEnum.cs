using System;
[Serializable]

// En between cell Panel and Cell.Signal, to know which output we are trying to update in panel, we must know its name (can't have a reference in a good way)

public enum SignalUnitSlotEnum {
	A, //0, output or input
	B, //1
	C,
	D,
	E,
	F,
	processedEarly,
	processedLate,
}