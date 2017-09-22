public class EdgeAttachment { 
	public EdgeAttachmentType type = EdgeAttachmentType.unassigned;

	public EdgeAttachment(Cell cell, int direction) {
		this.mCell = cell;
		this.mDirection = direction;
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