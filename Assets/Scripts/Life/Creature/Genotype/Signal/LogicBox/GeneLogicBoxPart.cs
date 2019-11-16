using System.Collections.Generic;

// Gate and input
public abstract class GeneLogicBoxPart {
	public int row;
	public int leftFlank;
	public int rightFlank;

	public LocknessEnum lockness = LocknessEnum.Unlocked;// No need to save/load this one as it is hardcoded, set by gene (would be the same every time)
	public bool isTransmittingSignal = false;
	
	public int width {
		get {
			return rightFlank - leftFlank;
		}
	}

	public int GetColumnLeftOfFlank(int flank) {
		return flank - 1;
	}

	public int GetColumnRightOfFlank(int flank) {
		return flank;
	}

	public int GetFlankLeftOfColumn(int column) {
		return column;
	}

	public int GetFlankRightOfColumn(int column) {
		return column + 1;
	}

	public virtual bool IsOccupyingColumn(int column) {
		return leftFlank <= column && rightFlank > column;
	}

	public abstract int GetTransmittingInputCount();
}