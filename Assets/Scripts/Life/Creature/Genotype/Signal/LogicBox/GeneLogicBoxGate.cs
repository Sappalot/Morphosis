using System.Collections.Generic;

public class GeneLogicBoxGate : GeneLogicBoxPart {
	private LogicOperatorEnum m_operatorType = LogicOperatorEnum.And;
	public LogicOperatorEnum operatorType { 
		get {
			return m_operatorType;
		}
		set {
			if (lockness == LocknessEnum.Unlocked) {
				m_operatorType = value;
				genotypeDirtyfy.ReforgeCellPatternAndForward();
			}
		}
	}

	public List<GeneLogicBoxPart> partsConnected = new List<GeneLogicBoxPart>(); // store conections even if they are not used

	private bool m_isUsed = false;
	public bool isUsed { 
		get {
			return m_isUsed;
		}
		set {
			if (value || lockness == LocknessEnum.Unlocked) {
				m_isUsed = value;
				geneLogicBox.UpdateConnections();
				genotypeDirtyfy.ReforgeCellPatternAndForward();
			}
		}
	} // is taking place inside logic box (might be blocked, might not)

	private GeneLogicBox geneLogicBox;
	public GeneLogicBoxGate(GeneLogicBox geneLogicBox, int row, IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
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
		if (lockness == LocknessEnum.Locked) {
			return false;
		}

		if (row > 0 && leftFlank > 0 && !geneLogicBox.IsCellOccupiedByGateOrLock(row, GetColumnLeftOfFlank(leftFlank))) {
			leftFlank--;
			geneLogicBox.UpdateConnections();
			genotypeDirtyfy.ReforgeCellPatternAndForward();
			return true;
		}
		return false;
	}

	public bool TryMoveLeftFlankRight() {
		if (lockness == LocknessEnum.Locked) {
			return false;
		}

		if (row > 0 && leftFlank < GeneLogicBox.rightmostFlank - 2) {
			if (width == 2) {
				if (!geneLogicBox.IsCellOccupiedByGateOrLock(row, GetColumnRightOfFlank(rightFlank))) {
					rightFlank++;
					leftFlank++;
					geneLogicBox.UpdateConnections();
					genotypeDirtyfy.ReforgeCellPatternAndForward();
					return true;
				}
			} else {
				leftFlank++;
				geneLogicBox.UpdateConnections();
				genotypeDirtyfy.ReforgeCellPatternAndForward();
				return true;
			}
		}
		return false;
	}

	public bool TryMoveRightFlankRight() {
		if (lockness == LocknessEnum.Locked) {
			return false;
		}

		if (row > 0 && rightFlank < GeneLogicBox.rightmostFlank && !geneLogicBox.IsCellOccupiedByGateOrLock(row, GetColumnRightOfFlank(rightFlank))) {
			rightFlank++;
			geneLogicBox.UpdateConnections();
			genotypeDirtyfy.ReforgeCellPatternAndForward();
			return true;
		}
		return false;
	}

	public bool TryMoveRightFlankLeft() {
		if (lockness == LocknessEnum.Locked) {
			return false;
		}

		if (row > 0 && rightFlank > 2) {
			if (width == 2) {
				if (!geneLogicBox.IsCellOccupiedByGateOrLock(row, GetColumnLeftOfFlank(leftFlank))) {
					leftFlank--;
					rightFlank--;
					geneLogicBox.UpdateConnections();
					genotypeDirtyfy.ReforgeCellPatternAndForward();
					return true;
				}
			} else {
				rightFlank--;
				geneLogicBox.UpdateConnections();
				genotypeDirtyfy.ReforgeCellPatternAndForward();
				return true;
			}
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
		if (lockness == LocknessEnum.Unlocked) {
			operatorType = geneLogicBoxGateData.operatorType;
			leftFlank = geneLogicBoxGateData.leftFlank;
			rightFlank = geneLogicBoxGateData.rightFlank;
			isUsed = geneLogicBoxGateData.isUsed;
		}
	}
}
