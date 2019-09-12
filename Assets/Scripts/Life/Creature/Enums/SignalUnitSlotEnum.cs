using System;
[Serializable]

// En between cell Panel and Cell.Signal, to know which output we are trying to update in panel, we must know its name (can't have a reference in a good way)

public enum SignalUnitSlotEnum {
	A,
	B,
	C,
	D,
	E,
	F,
	Whatever,
}