public class Gene {
    public CellTypeEnum type = CellTypeEnum.Leaf; // + this vein cell's settings 
    public readonly int index;

    public readonly Arrangement[] arrangements = new Arrangement[3];
    //------------------old shit ---------------------

    private int?[] reference = new int?[6];

    public Gene(int index) {
        this.index = index;

        arrangements[0] = new Arrangement();
        arrangements[1] = new Arrangement();
        arrangements[2] = new Arrangement();

        //--------------old 
        reference[0] = null;
        reference[1] = null;
        reference[2] = null;
        reference[3] = null;
        reference[4] = null;
        reference[5] = null;
    }

    public void SetDefaultReferenceGene(Gene gene) {
        for (int index = 0; index < arrangements.Length; index++) {
            arrangements[index].referenceGene = gene;
        }
    }

    public void setReference(int direction, int reference) {
        this.reference[direction] = reference;
    }

    public int? getReference(int direction) {
        return this.reference[direction];
    }

    public void Clear() {
        for (int i = 0; i < 6; i++) {
            reference[0] = null;
        }
        type = CellTypeEnum.Vein;
    }

   
}

