public class LeafCell : Cell {

    public LeafCell() : base() {
        springFrequenzy = 5f;
        springDamping = 11f;
    }

    public override void UpdateSpringFrequenzy() {
        if (HasNeighbour(CardinalDirectionEnum.north)) {
            northSpring.frequency = (this.springFrequenzy + northNeighbour.cell.springFrequenzy) / 2f;
            northSpring.dampingRatio = (this.springDamping + northNeighbour.cell.springDamping) / 2f;
        }

        if (HasNeighbour(CardinalDirectionEnum.southWest)) {
            southWestSpring.frequency = (this.springFrequenzy + southWestNeighbour.cell.springFrequenzy) / 2f;
            southWestSpring.dampingRatio = (this.springDamping + southWestNeighbour.cell.springDamping) / 2f;
        }

        if (HasNeighbour(CardinalDirectionEnum.southEast)) {
            southEastSpring.frequency = (this.springFrequenzy + southEastNeighbour.cell.springFrequenzy) / 2f;
            southEastSpring.dampingRatio = (this.springDamping + southEastNeighbour.cell.springDamping) / 2f;
        }
    }
}