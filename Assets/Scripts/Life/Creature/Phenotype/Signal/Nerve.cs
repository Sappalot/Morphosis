public class Nerve {
	public NerveStatusEnum nerveStatusEnum;

	public Cell headCell;
	public SignalUnitEnum headSignalUnitEnum;
	public SignalUnitSlotEnum headSignalUnitSlotEnum;

	public Cell tailCell;
	public SignalUnitEnum tailSignalUnitEnum;
	public SignalUnitSlotEnum tailSignalUnitSlotEnum;

	public Vector2i nerveVector; // vector in cell space, from head (0, 0) to tail (x, y)

	public Nerve() {
		headCell = null;
		headSignalUnitEnum = SignalUnitEnum.Void;
		headSignalUnitSlotEnum = SignalUnitSlotEnum.inputA;

		tailCell = null;
		tailSignalUnitEnum = SignalUnitEnum.Void;
		tailSignalUnitSlotEnum = SignalUnitSlotEnum.inputA;
	}

	public Nerve(Nerve other) {
		headCell = other.headCell;
		headSignalUnitEnum = other.headSignalUnitEnum;
		headSignalUnitSlotEnum = other.headSignalUnitSlotEnum;

		tailCell = other.tailCell;
		tailSignalUnitEnum = other.tailSignalUnitEnum;
		tailSignalUnitSlotEnum = other.tailSignalUnitSlotEnum;
	}
}
