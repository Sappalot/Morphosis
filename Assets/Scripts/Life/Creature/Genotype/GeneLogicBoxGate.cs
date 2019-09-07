public class GeneLogicBoxGate {

	public LogicOperatorEnum operatorType = LogicOperatorEnum.And;
	public int leftFlank = 1;
	public int rightFlank = 3;
	public bool isLocked = false;
	public bool isUsed = false;

	private int row;
	private GeneLogicBox geneLogicBox;
	public GeneLogicBoxGate(GeneLogicBox geneLogicBox, int row) {
		this.geneLogicBox = geneLogicBox;
		this.row = row;
	}

	public int width {
		get {
			return rightFlank - leftFlank;
		}
	}

	public bool TryMoveLeftFlankLeft() {
		if (leftFlank > 0 && !geneLogicBox.IsCellOccupied(row, GetColumnLeftOfFlank(leftFlank))) {
			leftFlank--;
			return true;
		}
		return false;
	}

	public bool TryMoveLeftFlankRight() {
		if (leftFlank < GeneLogicBox.rightmostFlank - 2 && width > 2) {
			leftFlank++;
			return true;
		}
		return false;
	}

	public bool TryMoveRightFlankRight() {
		if (rightFlank < GeneLogicBox.rightmostFlank && !geneLogicBox.IsCellOccupied(row, GetColumnRightOfFlank(rightFlank))) {
			rightFlank++;
			return true;
		}
		return false;
	}

	public bool TryMoveRightFlankLeft() {
		if (rightFlank > 2 && width > 2) {
			rightFlank--;
			return true;
		}
		return false;
	}

	private int GetColumnLeftOfFlank(int flank) {
		return flank - 1;
	}

	private int GetColumnRightOfFlank(int flank) {
		return flank;
	}

	public bool IsOccupyingColumn(int column) {
		return (isUsed && leftFlank <= column && rightFlank > column);
	}



	// Save
	private GeneLogicBoxGateData geneLogicBoxGateData = new GeneLogicBoxGateData();
	public GeneLogicBoxGateData UpdateData() {
		geneLogicBoxGateData.operatorType = operatorType;

		return geneLogicBoxGateData;
	}

	// Load
	public void ApplyData(GeneLogicBoxGateData geneLogicBoxGateData) {
		if (!isLocked) {
			operatorType = geneLogicBoxGateData.operatorType;
		}
	}
}
