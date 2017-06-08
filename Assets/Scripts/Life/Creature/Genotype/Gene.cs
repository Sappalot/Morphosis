public class Gene {
    public CellType type = CellType.Vein; // + this vein cell's settings 

    public Arrangement primary = new Arrangement();
    public Arrangement secondary = new Arrangement();
    public Arrangement tertiary = new Arrangement();

    public Arrangement[] arrangements = new Arrangement[3];
    //------------------old shit ---------------------

    private int?[] reference = new int?[6];

    public Gene() {
        arrangements[0] = primary;
        arrangements[1] = secondary;
        arrangements[2] = tertiary;

        //--------------old 
        reference[0] = null;
        reference[1] = null;
        reference[2] = null;
        reference[3] = null;
        reference[4] = null;
        reference[5] = null;
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
        type = CellType.Vein;
    }
}

