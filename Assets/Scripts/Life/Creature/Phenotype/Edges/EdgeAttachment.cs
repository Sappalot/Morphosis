public class EdgeAttachment { 
	public EdgeAttachment(Cell cell, int direction) {
		mCell = cell;
		mDirection = direction;
	}

	private Cell mCell;
	public Cell cell {
		get { return mCell; }
		private set {}
	}

	private int mDirection; //CardialDirection from edge to this attachment
	public int direction {
		get { return mDirection; }
		private set { }
	}
}