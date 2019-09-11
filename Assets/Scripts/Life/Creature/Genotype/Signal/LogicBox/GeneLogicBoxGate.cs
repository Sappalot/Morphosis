using System.Collections.Generic;

public class GeneLogicBoxGate : GeneLogicBoxPart {
	public LogicOperatorEnum operatorType = LogicOperatorEnum.And;
	public List<GeneLogicBoxPart> partsConnected = new List<GeneLogicBoxPart>(); // store conections even if they are not used
	
	//public List<GeneLogicBoxGate> gatesConnected {
	//	get {
	//		List<GeneLogicBoxGate> newList = new List<GeneLogicBoxGate>();
	//		foreach(GeneLogicBoxPart p in partsConnected) {
	//			if (p is GeneLogicBoxGate) {
	//				newList.Add(p as GeneLogicBoxGate);
	//			}
	//		}
	//		return newList;
	//	}
	//}

	//public List<GeneLogicBoxInput> inputsConnected {
	//	get {
	//		List<GeneLogicBoxInput> newList = new List<GeneLogicBoxInput>();
	//		foreach (GeneLogicBoxInput p in partsConnected) {
	//			if (p is GeneLogicBoxInput) {
	//				newList.Add(p as GeneLogicBoxInput);
	//			}
	//		}
	//		return newList;
	//	}
	//}

	public bool isUsed = false; // is taking place inside logic box (might be blocked, might not)
	
	private GeneLogicBox geneLogicBox;
	public GeneLogicBoxGate(GeneLogicBox geneLogicBox, int row) {
		this.geneLogicBox = geneLogicBox;
		this.row = row;
	}

	public override int GetTransmittingInputCount() {
		int count = 0;
		foreach (GeneLogicBoxPart i in partsConnected) {
			if (i.isTransmittingSignal) {
				count++;
			}
		}
		return count;
	}

	public bool TryMoveLeftFlankLeft() {
		if (leftFlank > 0 && !geneLogicBox.IsCellOccupiedByGate(row, GetColumnLeftOfFlank(leftFlank))) {
			leftFlank--;
			return true;
		}
		return false;
	}

	public bool TryMoveLeftFlankRight() {
		if (leftFlank < GeneLogicBox.rightmostFlank - 2) {
			if (width == 2) {
				rightFlank++;
			}
			leftFlank++;
			return true;
		}
		return false;
	}

	public bool TryMoveRightFlankRight() {
		if (rightFlank < GeneLogicBox.rightmostFlank && !geneLogicBox.IsCellOccupiedByGate(row, GetColumnRightOfFlank(rightFlank))) {
			rightFlank++;
			return true;
		}
		return false;
	}

	public bool TryMoveRightFlankLeft() {
		if (rightFlank > 2) {
			if (width == 2) {
				leftFlank--;
			}
			rightFlank--;
			return true;
		}
		return false;
	}

	public override bool IsOccupyingColumn(int column) {
		return (isUsed && base.IsOccupyingColumn(column));
	}

	// Save
	private GeneLogicBoxGateData geneLogicBoxGateData = new GeneLogicBoxGateData();
	public GeneLogicBoxGateData UpdateData() {
		geneLogicBoxGateData.operatorType = operatorType;
		geneLogicBoxGateData.leftFlank = leftFlank;
		geneLogicBoxGateData.rightFlank = rightFlank;
		geneLogicBoxGateData.isUsed = isUsed;
		return geneLogicBoxGateData;
	}

	// Load
	public void ApplyData(GeneLogicBoxGateData geneLogicBoxGateData) {
		if (!isLocked) {
			operatorType = geneLogicBoxGateData.operatorType;
			leftFlank = geneLogicBoxGateData.leftFlank;
			rightFlank = geneLogicBoxGateData.rightFlank;
			isUsed = geneLogicBoxGateData.isUsed;
		}
	}
}
