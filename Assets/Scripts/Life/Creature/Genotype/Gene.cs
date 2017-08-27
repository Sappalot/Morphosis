public class Gene {
    public CellTypeEnum type = CellTypeEnum.Leaf; // + this vein cell's settings 
    public int index;

    public readonly Arrangement[] arrangements = new Arrangement[3];
    //------------------old shit ---------------------

    //private int?[] referenceDeprecated = new int?[6];

    public Gene(int index) {
        this.index = index;

        arrangements[0] = new Arrangement();
        arrangements[1] = new Arrangement();
        arrangements[2] = new Arrangement();

        ////--------------old 
        //referenceDeprecated[0] = null;
        //referenceDeprecated[1] = null;
        //referenceDeprecated[2] = null;
        //referenceDeprecated[3] = null;
        //referenceDeprecated[4] = null;
        //referenceDeprecated[5] = null;
    }

    public void SetReferenceGeneFromReferenceGeneIndex(Gene[] genes) {
        arrangements[0].SetReferenceGeneFromReferenceGeneIndex(genes);
        arrangements[1].SetReferenceGeneFromReferenceGeneIndex(genes);
        arrangements[2].SetReferenceGeneFromReferenceGeneIndex(genes);
    }

    public GeneReference GetFlippableReference(int referenceCardinalIndex, FlipSideEnum flipSide) {
        GeneReference first = null;
        for (int index = 0; index < arrangements.Length; index++) {
            GeneReference found = arrangements[index].GetFlippableReference(referenceCardinalIndex, flipSide);
            if (found != null && arrangements[index].isEnabled) {
                first = found;
                break;
            }
        }
        return first;
    }

    public void SetDefault(Gene[] genome) {
        arrangements[0].referenceGene = genome[1];
        arrangements[1].referenceGene = genome[1];
        arrangements[2].referenceGene = genome[1];

        arrangements[0].isEnabled = false;
        arrangements[1].isEnabled = false;
        arrangements[2].isEnabled = false;
    }

    private GeneData geneData = new GeneData();
    public GeneData UpdateData() {
        geneData.type = type;
        geneData.index = index;

        geneData.arrangementData[0] = arrangements[0].UpdateData();
        geneData.arrangementData[1] = arrangements[1].UpdateData();
        geneData.arrangementData[2] = arrangements[2].UpdateData();

        return geneData;
    }

    public void ApplyData(GeneData geneData) {
        type = geneData.type;
        index = geneData.index;

        arrangements[0].ApplyData(geneData.arrangementData[0]);
        arrangements[1].ApplyData(geneData.arrangementData[1]);
        arrangements[2].ApplyData(geneData.arrangementData[2]);
    }
}

