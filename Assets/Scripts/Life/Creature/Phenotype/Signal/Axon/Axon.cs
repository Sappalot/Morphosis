using System.Collections.Generic;
using UnityEngine;

public class Axon : SignalUnit {
	private bool[] outputLate = new bool[6];
	private bool[] outputEarly = new bool[6];

	private Nerve[] inputNerves = new Nerve[2]; // left, right

	public Axon(SignalUnitEnum signalUnitEnum, Cell hostCell) : base(hostCell) {
		this.signalUnitEnum = signalUnitEnum;
		inputNerves[0] = new Nerve();
		inputNerves[1] = new Nerve();
	}

	public override void UpdateInputNervesGenotype(Genotype genotype) {
		GeneAxon geneAxon = (GeneAxon)hostCell.gene.GetGeneSignalUnit(signalUnitEnum);
		for (int i = 0; i < 2; i++) {
			if (geneAxon.GetInput(i).valveMode == SignalValveModeEnum.Pass) {
				inputNerves[i].headCell = hostCell;
				inputNerves[i].headSignalUnitEnum = signalUnitEnum;
				inputNerves[i].headSignalUnitSlotEnum = SignalUnit.IndexToSignalInputSlotUnit(i);

				GeneNerve geneNerve = geneAxon.GetInput(i).nerve;
				inputNerves[i].tailSignalUnitEnum = geneNerve.tailUnitEnum;
				inputNerves[i].tailSignalUnitSlotEnum = geneNerve.tailUnitSlotEnum;
				if (geneNerve.isLocal) {
					inputNerves[i].tailCell = hostCell;
					inputNerves[i].nerveStatusEnum = NerveStatusEnum.Input_GenotypeLocal;
				} else {
					inputNerves[i].nerveVector = geneNerve.nerveVector;
					inputNerves[i].tailCell = GeneNerve.GetGeneCellAtNerveTail(hostCell, geneNerve, genotype);


					inputNerves[i].nerveStatusEnum = NerveStatusEnum.Input_GenotypeExternal;
					//if (inputNerves[i].tailCell != null) {
					//	inputNerves[i].nerveStatusEnum = NerveStatusEnum.Input_GenotypeExternal;
					//} else {
					//	inputNerves[i].nerveStatusEnum = NerveStatusEnum.Input_GenotypeExternalVoid;
					//}


				}
			} else {
				// blocked input ==> void nerve
				inputNerves[i].nerveStatusEnum = NerveStatusEnum.Void;
			}
		}
	}

	// Assume all input nerves are updated at this stage
	public override void RootRecursivlyGenotype(Genotype genotype, Nerve nerve) {
		bool wasAllreadyRooted = isRooted;
		base.RootRecursivlyGenotype(genotype, nerve); // roots me!

		if (wasAllreadyRooted) {
			return;
		}

		// reach out through input nerves
		for (int i = 0; i < 2; i++) {
			if (inputNerves[i].nerveStatusEnum == NerveStatusEnum.Input_GenotypeLocal ) {
				// ask to which this genes unit where tail is "pointing"
				SignalUnit childSignalUnit = hostCell.GetSignalUnit(inputNerves[i].tailSignalUnitEnum);
				if (childSignalUnit != null) {
					childSignalUnit.RootRecursivlyGenotype(genotype, inputNerves[i]);
				}
			} else if (inputNerves[i].nerveStatusEnum == NerveStatusEnum.Input_GenotypeExternal) {
				// ask external unit where tail is pointing
				Cell childCell = inputNerves[i].tailCell;
				if (childCell != null) {
					SignalUnit childSignalUnit = childCell.GetSignalUnit(inputNerves[i].tailSignalUnitEnum);
					if (childSignalUnit != null) {
						childSignalUnit.RootRecursivlyGenotype(genotype, inputNerves[i]);
					}
				}
			}
		}
	}

	public override List<Nerve> GetAllNervesGenotype() {
		List<Nerve> nerves = new List<Nerve>();
		foreach (Nerve n in inputNerves) {
			if (n.nerveStatusEnum != NerveStatusEnum.Void) {
				nerves.Add(n);
			}
		}
		nerves.AddRange(base.GetAllNervesGenotype());
		return nerves;
	}

	public override List<Nerve> GetInputNervesGenotype() {
		List<Nerve> nerves = new List<Nerve>();
		foreach (Nerve n in inputNerves) {
			if (n.nerveStatusEnum != NerveStatusEnum.Void) {
				nerves.Add(n);
			}
		}
		return nerves;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return outputLate[SignalUnitSlotOutputToIndex(signalUnitSlot)];
	}

	public override void ComputeSignalOutput(int deltaTicks) {
		if (signalUnitEnum == SignalUnitEnum.Axon) { // redundant check ? 
			if (!isRooted) {
				return;
			}

			outputEarly[0] = selectedProgram == 1; // a
			outputEarly[1] = selectedProgram == 2; // b
			outputEarly[2] = selectedProgram == 3; // c
			outputEarly[3] = selectedProgram == 4; // d
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
			return hostCell.gene.axon.isEnabled;
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
		return (input as IGeneInput).valveMode == SignalValveModeEnum.Pass && (input as IGeneInput).nerve.tailUnitEnum != SignalUnitEnum.Void && hostCell.GetOutputFromUnit((input as IGeneInput).nerve.tailUnitEnum, (input as IGeneInput).nerve.tailUnitSlotEnum);
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
