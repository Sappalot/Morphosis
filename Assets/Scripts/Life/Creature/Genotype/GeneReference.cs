public class GeneReference {
    public readonly Gene gene;
    public readonly FlipSideEnum flipSide;

    public GeneReference(Gene gene, FlipSideEnum flip) {
        this.gene = gene;
        this.flipSide = flip;
    }
}
