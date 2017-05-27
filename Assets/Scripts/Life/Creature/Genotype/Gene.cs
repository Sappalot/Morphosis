public class Gene {
    public CellType type = CellType.Vein;
    private int?[] reference = new int?[6];

    public Gene() {
        reference[0] = null;
        reference[1] = null;
        reference[2] = null;
        reference[3] = null;
        reference[4] = null;
        reference[5] = null;
    }

    public Gene(int ne, int n, int nw, int sw, int s, int se) {
        reference[0] = ne;
        reference[1] = n;
        reference[2] = nw;
        reference[3] = sw;
        reference[4] = s;
        reference[5] = se;
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

