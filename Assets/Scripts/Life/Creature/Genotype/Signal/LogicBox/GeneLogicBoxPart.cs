using System.Collections.Generic;

// Gate and input
public abstract class GeneLogicBoxPart {
	public int m_row;
	public int row {
		get {
			return m_row;
		}
		set {
			m_row = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}


	public int m_leftFlank;
	public int leftFlank {
		get {
			return m_leftFlank;
		}
		set {
			m_leftFlank = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	public int m_rightFlank;
	public int rightFlank {
		get {
			return m_rightFlank;
		}
		set {
			m_rightFlank = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	public LocknessEnum lockness = LocknessEnum.Unlocked;// No need to save/load this one as it is hardcoded, set by gene (would be the same every time)
	public bool isTransmittingSignal = false;

	public IGenotypeDirtyfy genotypeDirtyfy;

	public int width {
		get {
			return rightFlank - leftFlank;
		}
	}

	public static int GetColumnLeftOfFlank(int flank) {
		return flank - 1;
	}

	public static int GetColumnRightOfFlank(int flank) {
		return flank;
	}

	public static int GetFlankLeftOfColumn(int column) {
		return column;
	}

	public static int GetFlankRightOfColumn(int column) {
		return column + 1;
	}

	public virtual bool IsOccupyingColumn(int column) {
		return leftFlank <= column && rightFlank > column;
	}

	public abstract int GetTransmittingInputCount();
}