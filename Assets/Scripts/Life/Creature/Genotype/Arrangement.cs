public class Arrangement {
    public bool isEnabled = true;
    public int geneIndex;
    public ArrangementType type = ArrangementType.Side;

    public int referenceCount = 1; //ALL: negative value indicated cells on the white side (when arrangementtype = Side)
    public int angle = 0; //ALL: +- 30 degrees per step, 0 is straight up (possitive y), negative on white side possitive on black side  
    public int gap = 0; //MIRROR: number of cells in the gap
    public FlipType flipType = FlipType.Same; // SIDE and STAR use Same/Opposite, Mirror use BlackToArrow/WhiteToArrow 

    public GeneReference GetReference(FlipSide arrangementFlipSide, CardinalDirection direction) {
        GeneReference reference = new GeneReference();
        reference.isThere = (direction == CardinalDirection.northEast);
        reference.flip = FlipSide.WhiteBlack;
        reference.cellType = CellType.Vein;
        reference.geneIndex = 69;
        return reference;
    }

}
