public class Gene {
    public CellTypeEnum type = CellTypeEnum.Leaf; // + this vein cell's settings 
    public readonly int index;

    public readonly Arrangement[] arrangements = new Arrangement[3];
    //------------------old shit ---------------------

    private int?[] referenceDeprecated = new int?[6];

    public Gene(int index) {
        this.index = index;

        arrangements[0] = new Arrangement();
        arrangements[1] = new Arrangement();
        arrangements[2] = new Arrangement();

        //--------------old 
        referenceDeprecated[0] = null;
        referenceDeprecated[1] = null;
        referenceDeprecated[2] = null;
        referenceDeprecated[3] = null;
        referenceDeprecated[4] = null;
        referenceDeprecated[5] = null;
    }

    public GeneReference GetFlippableReference(int referenceCardinalIndex, FlipSideEnum viewedFlipSide) {
        GeneReference first = null;
        for (int index = 0; index < arrangements.Length; index++) {
            GeneReference found = arrangements[index].GetFlippableReference(referenceCardinalIndex, viewedFlipSide);
            if (found != null && arrangements[index].isEnabled) {
                first = found;
                break;
            }
        }
        return first;
    }

    public void SetDefaultReferenceGene(Gene[] genome) {
        arrangements[0].referenceGene = genome[1];
        arrangements[1].referenceGene = genome[1];
        arrangements[2].referenceGene = genome[1];

        arrangements[0].isEnabled = false;
        arrangements[1].isEnabled = false;
        arrangements[2].isEnabled = false;
    }

    //------------------------ Deprecated

    public void setReferenceDeprecated(int direction, int reference) {
        this.referenceDeprecated[direction] = reference;
    }

    public int? getReferenceDeprecated(int direction) {
        return this.referenceDeprecated[direction];
    }

    public void ClearDeprecated() {
        for (int i = 0; i < 6; i++) {
            referenceDeprecated[0] = null;
        }
        type = CellTypeEnum.Vein;
    }   
}

