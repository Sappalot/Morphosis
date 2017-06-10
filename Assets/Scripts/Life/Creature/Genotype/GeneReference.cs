public class GeneReference {
    public readonly Gene gene;
    public readonly FlipSideEnum flip;

    public GeneReference(Gene gene, FlipSideEnum flip) {
        this.gene = gene;
        this.flip = flip;
    }
}
