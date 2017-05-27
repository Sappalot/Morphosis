using System;
using UnityEngine;

public class EdgeAttachment { 
    private Cell mCell;
    private int mDirection; //CardialDirection from edge to this attachment
    public EdgeAttachmentType type = EdgeAttachmentType.unassigned;

    public EdgeAttachment(Cell cell, int direction) {
        this.mCell = cell;
        this.mDirection = direction;
    }

    public Cell cell {
        get { return mCell; }
        private set {}
    }

    public int direction {
        get { return mDirection; }
        private set { }
    }


}
