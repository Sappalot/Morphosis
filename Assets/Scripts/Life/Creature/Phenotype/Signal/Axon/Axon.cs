using UnityEngine;

public class Axon : SignalUnit {

	public Axon(SignalUnitEnum signalUnit) {
		this.signalUnit = signalUnit;
	}

	public override void Clear() {

	}

	public override void UpdateSignalConnections(Cell hostCell) {
		// TODO: we are here since body has changed and signal nerves need to reconnect
	}

	// get pulse ??
	public static bool GetInputResult(GeneAxonInput input, Cell hostCell) {
		return (input as GeneAxonInput).valveMode == SignalValveModeEnum.Pass && (input as GeneAxonInput).nerve.inputUnit != SignalUnitEnum.Void && hostCell.GetOutputFromUnit((input as GeneAxonInput).nerve.inputUnit, (input as GeneAxonInput).nerve.inputUnitSlot);
	}
}
