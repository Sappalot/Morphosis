using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicBox : SignalUnit {
	
	public LogicBox(SignalUnitEnum outputUnit) {
		this.signalUnit = outputUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[0];
	}

	public override void UpdateOutputs(Cell hostCell, int deltaTicks, ulong worldTicks) {
		if (signalUnit == SignalUnitEnum.WorkLogicBoxA) {
			// TODO: let input go through gates to form an answer output
			//output = hostCell.GetOutputFromUnit(hostCell.gene.eggCellFertilizeLogic.GetInput(0).internalInput); // hack connection
			output[0] = ThroughGates(hostCell.gene.eggCellFertilizeLogic, hostCell);
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
}