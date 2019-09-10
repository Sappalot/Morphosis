using System.Collections.Generic;

public class GeneLogicBox : GeneSensor {
	public static int rowCount = 3; // excluding, bottom input row
	public static int columnCount = 5;
	public static int rightmostFlank = columnCount;
	public static int maxGatesPerRow = columnCount / 2;

	// All possible Gates are spawned when this logicBox is created
	// They are setup either in hard code (that is the locked gates) OR as they are loaded (loaded ones will not change locked gates)
	// They might be set as used/unused along the way but never removed
	public GeneLogicBoxGate gateRow0; 
	public GeneLogicBoxGate[] gateRow1 = new GeneLogicBoxGate[maxGatesPerRow];
	public GeneLogicBoxGate[] gateRow2 = new GeneLogicBoxGate[maxGatesPerRow];
	public GeneLogicBoxInput[] inputRow3 = new GeneLogicBoxInput[columnCount];

	public GeneLogicBox(SignalUnitEnum signalUnit, bool isLocked) {
		this.signalUnit = signalUnit;
		this.isLocked = isLocked;

		gateRow0 = new GeneLogicBoxGate(this, 0);
		for (int g = 0; g < maxGatesPerRow; g++) {
			gateRow1[g] = new GeneLogicBoxGate(this, 1);
			gateRow2[g] = new GeneLogicBoxGate(this, 2);
		}
		for (int i = 0; i < columnCount; i++) {
			inputRow3[i] = new GeneLogicBoxInput(3, i);
		}
	}

	public void UpdateConnections() {
		// Row 3, input
		foreach (GeneLogicBoxInput input3 in inputRow3) {
			input3.isTransmittingSignal = input3.valveMode == SignalValveModeEnum.Pass;
		}

		// Row 2
		// Clear
		foreach (GeneLogicBoxGate gate in gateRow2) {
			gate.inputsConnected.Clear();
		}
		// look for input in row 3: connect to all 
		foreach (GeneLogicBoxGate gate2 in gateRow2) {
			if (gate2.isUsed) {
				gate2.isTransmittingSignal = false;
				for (int column = gate2.GetColumnRightOfFlank(gate2.leftFlank); column <= gate2.GetColumnLeftOfFlank(gate2.rightFlank); column++) {
					gate2.inputsConnected.Add(inputRow3[column]);
					if (inputRow3[column].isTransmittingSignal) {
						gate2.isTransmittingSignal = true;
					}
				}
			}
		}

		// Row 1
		// Clear
		foreach (GeneLogicBoxGate gate1 in gateRow1) {
			gate1.inputsConnected.Clear();
		}
		// look for gates in row 2: connnect to all that is under me (overlapping with at leas 1 cell)
		foreach (GeneLogicBoxGate gate1 in gateRow1) {
			if (gate1.isUsed) {
				gate1.isTransmittingSignal = false;
			}
		}
		foreach (GeneLogicBoxGate gate1 in gateRow1) {
			if (gate1.isUsed) {
				foreach (GeneLogicBoxGate gate2 in gateRow2) {
					if (gate2.isUsed && gate2.leftFlank < gate1.rightFlank && gate2.rightFlank > gate1.leftFlank) {
						gate1.inputsConnected.Add(gate2);
						if (gate2.isTransmittingSignal) {
							gate1.isTransmittingSignal = true;
						}
					}
				}
			}
		}
		// look for input in row 3: connect to all that are not blocked by gates in row 2
		foreach (GeneLogicBoxGate gate1 in gateRow1) {
			if (gate1.isUsed) {
				for (int column = gate1.GetColumnRightOfFlank(gate1.leftFlank); column <= gate1.GetColumnLeftOfFlank(gate1.rightFlank); column++) {
					if (!IsCellOccupiedByGate(2, column)) {
						gate1.inputsConnected.Add(inputRow3[column]);
						if (inputRow3[column].isTransmittingSignal) {
							gate1.isTransmittingSignal = true;
						}
					}
				}
			}
		}

		// Row 0
		gateRow0.inputsConnected.Clear();
		gateRow0.isTransmittingSignal = false;

		// look for gates in row 1: connect to all ot them
		foreach (GeneLogicBoxGate gate1 in gateRow1) {
			if (gate1.isUsed) {
				gateRow0.inputsConnected.Add(gate1);
				if (gate1.isTransmittingSignal) {
					gateRow0.isTransmittingSignal = true;
				}
			}
		}
		// look for gates in row 2: connnect to all that are not (even partly) blocked by gates in row 1
		foreach(GeneLogicBoxGate gate2 in gateRow2) {
			if (gate2.isUsed && !HasGateAbove(gate2)) {
				gateRow0.inputsConnected.Add(gate2);
				if (gate2.isTransmittingSignal) {
					gateRow0.isTransmittingSignal = true;
				}
			}
		}
		//look for input in row 3: connect to all that are not blocked by gates in row 1 and 2
		for (int column = 0; column < columnCount; column++) {
			if (!IsCellOccupiedByGate(1, column) && !IsCellOccupiedByGate(2, column)) {
				gateRow0.inputsConnected.Add(inputRow3[column]);
				if (inputRow3[column].isTransmittingSignal) {
					gateRow0.isTransmittingSignal = true;
				}
			}
		}
	}

	public bool HasGateAbove(GeneLogicBoxGate gate) {
		return AreSomeGateCellsOccupied(gate.row - 1, gate.leftFlank, gate.rightFlank);
	}

	public bool HasGatBelow(GeneLogicBoxGate gate) {
		return AreSomeGateCellsOccupied(gate.row - 1, gate.leftFlank, gate.rightFlank);
	}

	public void RemoveAllGates() {
		gateRow0.isUsed = false;
		foreach (GeneLogicBoxGate g in gateRow1) {
			g.isUsed = false;
		}
		foreach (GeneLogicBoxGate g in gateRow2) {
			g.isUsed = false;
		}
	}

	public bool TryCreateGate(int row, LogicOperatorEnum operatorType) {
		if (row == 0) {
			return TryCreateGate(row, operatorType, 0, rightmostFlank, false);
		} else {
			for (int leftFlank = 0; leftFlank < rightmostFlank - 1; leftFlank++) {
				if (TryCreateGate(row, operatorType, leftFlank, leftFlank + 2, false)) {
					return true;
				}
			}
		} 
		return false;
	}

	// When bringing back a gate from the dead all data will be lost, so there is no chance to get back some old good stuff, in a mutation, that was disabled
	// Guess it doesn't mather, since we cant store so much 'good stuff' in a gate anyway
	public bool TryCreateGate(int row, LogicOperatorEnum operatorType, int leftFlank, int rightFlank, bool isLocked) {
		if (AreAllGateCellsFree(row, leftFlank, rightFlank)) {
			GeneLogicBoxGate newGate = GetAnUnusedGate(row);
			if (newGate != null) {
				newGate.operatorType = operatorType;
				newGate.leftFlank = leftFlank;
				newGate.rightFlank = rightFlank;
				newGate.isLocked = isLocked;
				newGate.isUsed = true;
				return true;
			}
		}
		return false;
	}

	private GeneLogicBoxGate GetAnUnusedGate(int row) { // may be "behind" some other gate
		if (row == 0 && !gateRow0.isUsed) {
			return (gateRow0 as GeneLogicBoxGate);
		} else {
			foreach (GeneLogicBoxGate g in (row == 1 ? gateRow1 : gateRow2)) {
				if (!g.isUsed) {
					return g;
				}
			}
		} 
		return null;
	}

	public bool AreAllGateCellsFree(int row, int leftFlank, int rightFlank) {
		return !AreSomeGateCellsOccupied(row, leftFlank, rightFlank);
	}

	public bool AreSomeGateCellsOccupied(int row, int leftFlank, int rightFlank) {
		for (int column = leftFlank; column < rightFlank; column++) {
			if (IsCellOccupiedByGate(row, column)) {
				return true;
			}
		}
		return false;
	}

	public bool IsCellOccupiedByGate(int row, int column) {
		if (row == 0) {
			return gateRow0.isUsed;
		} else if (row == 1) {
			for (int i = 0; i < gateRow1.Length; i++) {
				if (gateRow1[i].IsOccupyingColumn(column)) {
					return true;
				}
			}
		} else if (row == 2) {
			for (int i = 0; i < gateRow2.Length; i++) {
				if (gateRow2[i].IsOccupyingColumn(column)) {
					return true;
				}
			}
		}
		return false;
	}

	// Save
	private GeneLogicBoxData geneLogicBoxData = new GeneLogicBoxData();
	public GeneLogicBoxData UpdateData() {
		// Row 0
		geneLogicBoxData.layer0LogicBoxGateData = gateRow0.UpdateData();
		
		// Row 1
		for (int i = 0; i < geneLogicBoxData.layer1LogicBoxGateData.Length; i++) {
			geneLogicBoxData.layer1LogicBoxGateData[i] = gateRow1[i].UpdateData();
		}

		// Row 2 
		for (int i = 0; i < geneLogicBoxData.layer2LogicBoxGateData.Length; i++) {
			geneLogicBoxData.layer2LogicBoxGateData[i] = gateRow2[i].UpdateData();
		}

		// Row 3 input
		for (int i = 0; i < geneLogicBoxData.layer3LogicBoxInputData.Length; i++) {
			geneLogicBoxData.layer3LogicBoxInputData[i] = inputRow3[i].UpdateData();
		}

		return geneLogicBoxData;
	}

	// Load
	public void ApplyData(GeneLogicBoxData geneLogicBoxData) {
		// Row 0
		gateRow0.ApplyData(geneLogicBoxData.layer0LogicBoxGateData);
		
		// Row 1
		for (int i = 0; i < gateRow1.Length; i++) {
			if (geneLogicBoxData.layer1LogicBoxGateData.Length > 0) // temporary backwards compatibility
				gateRow1[i].ApplyData(geneLogicBoxData.layer1LogicBoxGateData[i]);
		}
		
		// Row 2
		for (int i = 0; i < gateRow2.Length; i++) {
			if (geneLogicBoxData.layer2LogicBoxGateData.Length > 0) // temporary backwards compatibility
				gateRow2[i].ApplyData(geneLogicBoxData.layer2LogicBoxGateData[i]);
		}

		// Row 3 Input
		for (int i = 0; i < inputRow3.Length; i++) {
			if (geneLogicBoxData.layer3LogicBoxInputData.Length > 0) // temporary backwards compatibility
				inputRow3[i].ApplyData(geneLogicBoxData.layer3LogicBoxInputData[i]);
		}

		UpdateConnections();
	}
}
