public class LogicBox : SignalUnit {
	private bool outputEarly;
	private bool outputLate; // TODO: save/load state

	public LogicBox(SignalUnitEnum signalUnit, Cell hostCell) : base(hostCell) {
		base.signalUnit = signalUnit;
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

	public override void UpdateSignalConnections() {
		// TODO: we are here since body has changed and signal nerves need to reconnect
	}

	public override void ComputeSignalOutput(int deltaTicks) {
		if (hostCell.GetCellType() == CellTypeEnum.Egg && signalUnit == SignalUnitEnum.WorkLogicBoxA) {
			outputEarly = ThroughGates(hostCell.gene.eggCellFertilizeLogic);
		} else if (signalUnit == SignalUnitEnum.DendritesLogicBox) {
			outputEarly = ThroughGates(hostCell.gene.dendritesLogicBox);
		} else if (signalUnit == SignalUnitEnum.OriginDetatchLogicBox) {
			outputEarly = ThroughGates(hostCell.gene.originDetatchLogicBox);
		}
	}

	private bool ThroughGates(GeneLogicBox geneLogicBox) {
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
						if ((nextPart as GeneLogicBoxInput).valveMode == SignalValveModeEnum.Pass && ((nextPart as GeneLogicBoxInput).nerve.inputUnit == SignalUnitEnum.Void || !hostCell.GetOutputFromUnit((nextPart as GeneLogicBoxInput).nerve.inputUnit, (nextPart as GeneLogicBoxInput).nerve.inputUnitSlot))) {
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
						if ((nextPart as GeneLogicBoxInput).valveMode == SignalValveModeEnum.Pass && (nextPart as GeneLogicBoxInput).nerve.inputUnit != SignalUnitEnum.Void && hostCell.GetOutputFromUnit((nextPart as GeneLogicBoxInput).nerve.inputUnit, (nextPart as GeneLogicBoxInput).nerve.inputUnitSlot)) {
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
		return (input as IGeneInput).valveMode == SignalValveModeEnum.Pass && (input as IGeneInput).nerve.inputUnit != SignalUnitEnum.Void && hostCell.GetOutputFromUnit((input as IGeneInput).nerve.inputUnit, (input as IGeneInput).nerve.inputUnitSlot);
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