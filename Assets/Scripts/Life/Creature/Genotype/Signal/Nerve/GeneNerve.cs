// Information on how to connect any of creatures outputs output to an input
public class GeneNerve {
	public SignalUnitEnum outputUnit = SignalUnitEnum.Void; // The output from me "the nerve" The GeneSensor that created this one
	public SignalUnitEnum inputUnit = SignalUnitEnum.Void; // The input to me "the nerve" (Somebodey elses output)
	public SignalUnitSlotEnum inputUnitSlot; // The slot on that (above) unit

	// TODO: let nerv listen to other cells than self (external input)

	// TODO: bool IsAvailableForUnit(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot)

	// TODO: save/load
}