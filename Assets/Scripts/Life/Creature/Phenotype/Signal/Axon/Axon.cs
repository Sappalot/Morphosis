using UnityEngine;

public class Axon : SignalUnit {
	private bool[] outputLate = new bool[6];
	private bool[] outputEarly = new bool[6]; 

	public Axon(SignalUnitEnum signalUnit, Cell hostCell) : base(hostCell) {
		this.signalUnit = signalUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return outputLate[SignalUnitSlotOutputToIndex(signalUnitSlot)];
	}


	public override void UpdateSignalConnections() {
		// TODO: we are here since body has changed and signal nerves need to reconnect
	}

	public override void ComputeSignalOutput(int deltaTicks) {
		if (signalUnit == SignalUnitEnum.Axon) { // redundant check ? 
			outputEarly[0] = selectedProgram == 1; // a
			outputEarly[1] = selectedProgram == 2; // b
			outputEarly[2] = selectedProgram == 3;
			outputEarly[3] = selectedProgram == 4;
			outputEarly[4] = selectedProgram == 0; // relaxed
			outputEarly[5] = false;
		}
	}

	public override void FeedSignal() {
		for (int i = 0; i < 6; i++) {
			outputLate[i] = outputEarly[i];
		}
	}

	public override void Clear() {
		for (int i = 0; i < 6; i++) {
			outputEarly[i] = false;
			outputLate[i] = false;
		}
	}

	public bool isEnabled {
		get {
			return hostCell.gene.axon.axonIsEnabled;
		}
	}

	public float GetPulseValue(int distance) {
		if (isEnabled && selectedProgram > 0) {
			float fromOriginOffset = (hostCell.gene.axon.GetPulse(selectedProgram).axonFromOriginOffset + (hostCell.gene.axon.GetPulse(selectedProgram).axonIsFromOriginPlus180 && hostCell.flipSide == FlipSideEnum.WhiteBlack ? 180f : 0f)) / 360f;
			float fromMeOffest = (hostCell.gene.axon.GetPulse(selectedProgram).axonFromMeOffset * distance) / 360f;
			if (!hostCell.gene.axon.GetPulse(selectedProgram).axonIsReverse) {
				return Mathf.Cos((fromOriginOffset + fromMeOffest + hostCell.creature.phenotype.originCell.originPulseCompleteness) * 2f * Mathf.PI) + hostCell.gene.axon.GetPulse(selectedProgram).axonRelaxContract;
			} else {
				return Mathf.Cos((fromOriginOffset + fromMeOffest - hostCell.creature.phenotype.originCell.originPulseCompleteness) * 2f * Mathf.PI) + hostCell.gene.axon.GetPulse(selectedProgram).axonRelaxContract; // is this really the right way of reversing????!!!!
			}
		} else {
			return 0f; // relax
		}

	}

	public bool IsPulseContracting(int distance) {
		if (selectedProgram > 0) {
			return isEnabled && GetPulseValue(distance) > 0;
		} else {
			return false; // relax
		}
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

	// Load Save
	private AxonData axonData = new AxonData();

	// Save
	public AxonData UpdateData() {
		for (int i = 0; i < 6; i++) {
			axonData.outputEarly[i] = outputEarly[i];
			axonData.outputLate[i] = outputLate[i];
		}

		return axonData;
	}

	// Load
	public void ApplyData(AxonData axonData) {
		for (int i = 0; i < 6; i++) {
			outputEarly[i] = axonData.outputEarly[i];
			outputLate[i] = axonData.outputLate[i];
		}
	}
}
