using System.Collections.Generic;

public class GeneLogicBox {
	public static int rowCount = 3;
	public static int columnCount = 5;
	public static int rightmostFlank = columnCount;
	public static int maxGatesPerLayer = columnCount / 2;

	// All possible Gates are spawned when this logicBox is created
	// They are setup either in hard code (that is the locked gates) OR as they are loaded (loaded ones will not change locked gates)
	// They might be set as used/unused along the way but never removed
	public GeneLogicBoxGate gateRow0; 
	public GeneLogicBoxGate[] gateRow1 = new GeneLogicBoxGate[maxGatesPerLayer];
	public GeneLogicBoxGate[] gateRow2 = new GeneLogicBoxGate[maxGatesPerLayer];

	public GeneLogicBox() {
		gateRow0 = new GeneLogicBoxGate(this, 0);
		for (int g = 0; g < 2; g++) {
			gateRow1[g] = new GeneLogicBoxGate(this, 1);
			gateRow2[g] = new GeneLogicBoxGate(this, 2);
		}
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

	public bool TryCreateGate(int row, LogicOperatorEnum operatorType, int leftFlank, int rightFlank, bool isLocked) {
		if (AreAllCellsFree(row, leftFlank, rightFlank)) {
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
			return gateRow0;
		} else {
			foreach (GeneLogicBoxGate g in (row == 1 ? gateRow1 : gateRow2)) {
				if (!g.isUsed) {
					return g;
				}
			}
		} 
		return null;
	}

	public bool AreAllCellsFree(int row, int leftFlank, int rightFlank) {
		return !AreSomeCellsOccupied(row, leftFlank, rightFlank);
	}

	public bool AreSomeCellsOccupied(int row, int leftFlank, int rightFlank) {
		for (int column = leftFlank; column < rightFlank; column++) {
			if (IsCellOccupied(row, column)) {
				return true;
			}
		}
		return false;
	}

	public bool IsCellOccupied(int row, int column) {
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
		geneLogicBoxData.layer0LogicBoxGateData = gateRow0.UpdateData();
		
		// TODO: layer 1 & 2

		return geneLogicBoxData;
	}

	// Load
	public void ApplyData(GeneLogicBoxData geneLogicBoxData) {
		gateRow0.ApplyData(geneLogicBoxData.layer0LogicBoxGateData);

		// TODO: layer 1 & 2
	}
}
