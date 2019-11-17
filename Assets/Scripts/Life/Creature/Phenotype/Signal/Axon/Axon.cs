using UnityEngine;

public class Axon : SignalUnit {

	public Axon(SignalUnitEnum signalUnit, Cell hostCell) : base(hostCell) {
		this.signalUnit = signalUnit;
	}

	public override void Clear() {

	}

	public override void UpdateSignalConnections() {
		// TODO: we are here since body has changed and signal nerves need to reconnect
	}

	public override void ComputeSignalOutput(int deltaTicks) {
		//int selectedCombination = SelectedCombination(hostCell.gene.axon.axonInputLeft, );


	}

	public bool isEnabled {
		get {
			return hostCell.gene.axon.axonIsEnabled;
		}
	}

	public float GetPulseValue(int distance) {
		if (isEnabled && selectedCombination > 0) {
			float fromOriginOffset = (hostCell.gene.axon.axonFromOriginOffset + (hostCell.gene.axon.axonIsFromOriginPlus180 && hostCell.flipSide == FlipSideEnum.WhiteBlack ? 180f : 0f)) / 360f;
			float fromMeOffest = (hostCell.gene.axon.axonFromMeOffset * distance) / 360f;
			if (!hostCell.gene.axon.axonIsReverse) {
				return Mathf.Cos((fromOriginOffset + fromMeOffest + hostCell.creature.phenotype.originCell.originPulseCompleteness) * 2f * Mathf.PI) + hostCell.gene.axon.axonRelaxContract;
			} else {
				return Mathf.Cos((fromOriginOffset + fromMeOffest - hostCell.creature.phenotype.originCell.originPulseCompleteness) * 2f * Mathf.PI) + hostCell.gene.axon.axonRelaxContract; // is this really the right way of reversing????!!!!
			}
		} else {
			return 0f;
		}

	}

	public bool IsPulseContracting(int distance) {
		if (selectedCombination > 0) {
			return isEnabled && GetPulseValue(distance) > 0;
		} else {
			return false;
		}
		

		// haxor test with sensor
		//if (signal.effectSensor.isOutputOn) {
		//	return false; // Leelax if have enough effect
		//} else {
		//	return isAxonEnabled && GetAxonPulseValue(distance) > 0;
		//}

	}

	public int selectedProgram {
		get {
			int combination = selectedCombination;
			if (combination == 3) {
				return hostCell.gene.axon.pulseProgram3;
			} else if (combination == 2) {
				return hostCell.gene.axon.pulseProgram2;
			} else if (combination == 1) {
				return hostCell.gene.axon.pulseProgram1;
			} else if (combination == 0) {
				return hostCell.gene.axon.pulseProgram0;
			}
			return -1;
		}
	}

	public int selectedCombination {
		get {
			IGeneInput inputLeft = hostCell.gene.axon.axonInputLeft;
			IGeneInput inputRight = hostCell.gene.axon.axonInputRight;

			if (HasSignalAtCombination(3)) {
				return 3;
			} else if (HasSignalAtCombination(2)) {
				return 2;
			} else if (HasSignalAtCombination(1)) {
				return 1;
			} else if (HasSignalAtCombination(0)) {
				return 0;
			}

			return -1;
		}
	}

	public bool HasSignalAtCombination(int combination) {
		IGeneInput inputLeft = hostCell.gene.axon.axonInputLeft;
		IGeneInput inputRight = hostCell.gene.axon.axonInputRight;

		bool left = HasSignalPostInputValve(inputLeft);
		bool right = HasSignalPostInputValve(inputRight);

		if (combination == 3) {
			return left && right;
		} else if (combination == 2) {
			return left && !right;
		} else if (combination == 1) {
			return !left && right;
		} else if (combination == 0) {
			return !left && !right;
		}

		return false;
	}

	// get pulse ??
	public bool HasSignalPostInputValve(IGeneInput input) {
		return (input as IGeneInput).valveMode == SignalValveModeEnum.Pass && (input as IGeneInput).nerve.inputUnit != SignalUnitEnum.Void && hostCell.GetOutputFromUnit((input as IGeneInput).nerve.inputUnit, (input as IGeneInput).nerve.inputUnitSlot);
	}
}
