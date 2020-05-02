using System.Collections.Generic;

public class LogicBox : SignalUnit {
	private bool outputEarly;
	private bool outputLate; // TODO: save/load state

	private List<Nerve> outputNerve = new List<Nerve>(); // there might be many
	private Nerve[] inputNerves = new Nerve[6];

	public override void PreUpdateNervesGenotype() {
		outputNerve.Clear();
	}

	public override void UpdateInputNervesGenotype(Genotype genotype) {
		GeneLogicBox geneLogicBox = (GeneLogicBox)hostCell.gene.GetGeneSignalUnit(hostSignalUnitEnum);
		for (int i = 0; i < GeneLogicBox.columnCount; i++) {
			if (geneLogicBox.GetInput(i).valveMode == SignalValveModeEnum.Pass) {
				inputNerves[i].headCell = hostCell;
				inputNerves[i].headSignalUnitEnum = hostSignalUnitEnum;
				inputNerves[i].headSignalUnitSlotEnum = SignalUnit.IndexToSignalInputSlotUnit(i);

				GeneNerve geneNerve = geneLogicBox.GetInput(i).nerve;
				inputNerves[i].tailSignalUnitEnum = geneNerve.tailUnitEnum;
				inputNerves[i].tailSignalUnitSlotEnum = geneNerve.tailUnitSlotEnum;
				if (geneNerve.isLocal) {
					inputNerves[i].tailCell = hostCell;
					inputNerves[i].nerveStatusEnum = NerveStatusEnum.Input_GenotypeListensToTargetLocal;
				} else {
					inputNerves[i].nerveVector = geneNerve.nerveVector;
					inputNerves[i].tailCell = GeneNerve.GetGeneCellAtNerveTail(hostCell, geneNerve, genotype);
					if (inputNerves[i].tailCell != null) {
						inputNerves[i].nerveStatusEnum = NerveStatusEnum.Input_GenotypeListensToTargetExternal;
					} else {
						inputNerves[i].nerveStatusEnum = NerveStatusEnum.Input_GenotypeListensToVoidTargetExternal;
					}
				}
			} else {
				// blocked input ==> void nerve
				inputNerves[i].nerveStatusEnum = NerveStatusEnum.Input_GenotypeIsBlockedByValve;
			}
		}
	}

	public override void UpdateConnectionsNervesGenotype(Genotype genotype) {
		//hostCell.creature.genotype.Get
	}

	public override List<Nerve> GetAllNervesGenotype() {
		List<Nerve> nerves = new List<Nerve>();
		nerves.AddRange(inputNerves);
		// TODO: add external as well
		return nerves;
	}

	public LogicBox(SignalUnitEnum signalUnitEnum, Cell hostCell) : base(hostCell) {
		base.hostSignalUnitEnum = signalUnitEnum;
		for (int i = 0; i < 6; i++) {
			inputNerves[i] = new Nerve();
		}
	}

	public override void Clear() {
		outputEarly = false;
		outputLate = false;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		if (signalUnitSlot == SignalUnitSlotEnum.outputEarlyA) {
			return outputEarly;
		} else if (signalUnitSlot == SignalUnitSlotEnum.outputLateA) {
			return outputLate;
		}
		return false;
	}

	public override void FeedSignal() {
		outputLate = outputEarly;
	}

	public override void ComputeSignalOutput(int deltaTicks) {
		if (hostCell.GetCellType() == CellTypeEnum.Egg && hostSignalUnitEnum == SignalUnitEnum.WorkLogicBoxA) {
			outputEarly = ThroughGates(hostCell.gene.eggCellFertilizeLogic);
		} else if (hostSignalUnitEnum == SignalUnitEnum.DendritesLogicBox) {
			outputEarly = ThroughGates(hostCell.gene.dendritesLogicBox);
		} else if (hostSignalUnitEnum == SignalUnitEnum.OriginDetatchLogicBox) {
			outputEarly = ThroughGates(hostCell.gene.originDetatchLogicBox);
		}
	}

	private bool ThroughGates(GeneLogicBox geneLogicBox) {
		if (!geneLogicBox.isRooted) {
			return false;
		}
		return HasSignalPostGate(geneLogicBox.GetGate(0, 0), hostCell);
	}

	// TODO: find out a way not to use static functions here
	// we need to know what logic box we are talking to in panel as we want to update signal colors

	public static bool HasSignalPostGate(GeneLogicBoxGate gate, Cell hostCell) {
		if (gate.row == 0 && !gate.isTransmittingSignal) {
			return false; // If we are not transmitting anything through top gate, this box is useless. undefined but let's just turn output off
		}

		if (gate.operatorType == LogicOperatorEnum.And) {
			foreach (GeneLogicBoxPart nextPart in gate.partsConnected) {
				if (nextPart.isTransmittingSignal) {
					if (nextPart is GeneLogicBoxInput) {
						if ((nextPart as GeneLogicBoxInput).valveMode == SignalValveModeEnum.Pass && ((nextPart as GeneLogicBoxInput).nerve.tailUnitEnum == SignalUnitEnum.Void || !hostCell.GetOutputFromUnit((nextPart as GeneLogicBoxInput).nerve.tailUnitEnum, (nextPart as GeneLogicBoxInput).nerve.tailUnitSlotEnum))) {
							// next part turned out to be an open input valve with its input off
							return false; // one off ==> AND is off :(
						}
					} else if (nextPart is GeneLogicBoxGate) {
						if (!HasSignalPostGate((nextPart as GeneLogicBoxGate), hostCell)) {
							// next part turned out to be a gate with output off
							return false; // one false ==> AND is off :(
						}
					}
				}
			}
			return true; // We didn't find an input or a gate with an off signal ==> all signals must have been ON :)
		} else {
			foreach (GeneLogicBoxPart nextPart in gate.partsConnected) {
				if (nextPart.isTransmittingSignal) {
					if (nextPart is GeneLogicBoxInput) {
						if ((nextPart as GeneLogicBoxInput).valveMode == SignalValveModeEnum.Pass && (nextPart as GeneLogicBoxInput).nerve.tailUnitEnum != SignalUnitEnum.Void && hostCell.GetOutputFromUnit((nextPart as GeneLogicBoxInput).nerve.tailUnitEnum, (nextPart as GeneLogicBoxInput).nerve.tailUnitSlotEnum)) {
							// next part turned out to be an open input valve with its input on
							return true; // one on ==> OR is on :)
						}
					} else if (nextPart is GeneLogicBoxGate) {
						if (HasSignalPostGate((nextPart as GeneLogicBoxGate), hostCell)) {
							// next part turned out to be a gate with output on
							return true; // one on ==> OR is on :)
						}
					}
				}
			}
			return false; // We didnt find any input or gate with an ON-signal ==> all singnals must have been OFF :(
		}
	}

	public static bool HasSignalPostInputValve(IGeneInput input, Cell hostCell) {
		return (input as IGeneInput).valveMode == SignalValveModeEnum.Pass && (input as IGeneInput).nerve.tailUnitEnum != SignalUnitEnum.Void && hostCell.GetOutputFromUnit((input as IGeneInput).nerve.tailUnitEnum, (input as IGeneInput).nerve.tailUnitSlotEnum);
	}

	private bool TestInput(int leftFlank) {
		return leftFlank == 0 || leftFlank == 2 || leftFlank == 4; 
	}

	// Load Save
	private LogicBoxData logicBoxData = new LogicBoxData();

	// Save
	public LogicBoxData UpdateData() {
		logicBoxData.outputEarly = outputEarly;
		logicBoxData.outputLate = outputLate;
		return logicBoxData;
	}

	// Load
	public void ApplyData(LogicBoxData logicBoxData) {
		outputEarly = logicBoxData.outputEarly;
		outputLate = logicBoxData.outputLate;
	}


	// ^ Load Save ^
}