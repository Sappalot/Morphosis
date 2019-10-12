public class LogicBox : SignalUnit {
	private bool outputEarly;
	private bool outputLate; // TODO: save/load state

	public LogicBox(SignalUnitEnum signalUnit) {
		base.signalUnit = signalUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		if (signalUnitSlot == SignalUnitSlotEnum.processedEarly) {
			return outputEarly;
		} else if (signalUnitSlot == SignalUnitSlotEnum.processedLate) {
			return outputLate;
		}
		return false;
	}

	public override void FeedSignal() {
		outputLate = outputEarly;
	}

	public override void ComputeSignalOutput(Cell hostCell, int deltaTicks) {
		if (hostCell.GetCellType() == CellTypeEnum.Egg && signalUnit == SignalUnitEnum.WorkLogicBoxA) {
			outputEarly = ThroughGates(hostCell.gene.eggCellFertilizeLogic, hostCell);
		} else if (signalUnit == SignalUnitEnum.Dendrites) {
			outputEarly = ThroughGates(hostCell.gene.dendrites, hostCell);
		}
	}

	private bool ThroughGates(GeneLogicBox geneLogicBox, Cell hostCell) {
		
		return GetGateResult(geneLogicBox.GetGate(0, 0), hostCell);
	}

	public static bool GetGateResult(GeneLogicBoxGate gate, Cell hostCell) {
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
						if (!GetGateResult((nextPart as GeneLogicBoxGate), hostCell)) {
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
						if (GetGateResult((nextPart as GeneLogicBoxGate), hostCell)) {
							// next part turned out to be a gate with output on
							return true; // one on ==> OR is on :)
						}
					}
				}
			}
			return false; // We didnt find any input or gate with an ON-signal ==> all singnals must have been OFF :(
		}
	}

	public static bool GetInputResult(GeneLogicBoxInput input, Cell hostCell) {
		return (input as GeneLogicBoxInput).valveMode == SignalValveModeEnum.Pass && (input as GeneLogicBoxInput).nerve.inputUnit != SignalUnitEnum.Void && hostCell.GetOutputFromUnit((input as GeneLogicBoxInput).nerve.inputUnit, (input as GeneLogicBoxInput).nerve.inputUnitSlot);
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