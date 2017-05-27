using UnityEngine;
using System.Collections.Generic;

public class LeafCell : Cell {

    public LeafCell() : base() {
        springFrequenzy = 5f;
    }

    public override void UpdateSpringFrequenzy() {

        if (HasNeighbour(CardinalDirection.northEast)) {
            northEastNeighbour.cell.GetSpring(this).frequency = this.springFrequenzy;
        }

        if (HasNeighbour(CardinalDirection.north)) {
            northSpring.frequency = this.springFrequenzy;
        }

        if (HasNeighbour(CardinalDirection.northWest)) {
            northWestNeighbour.cell.GetSpring(this).frequency = this.springFrequenzy;
        }

        if (HasNeighbour(CardinalDirection.southWest)) {
            southWestSpring.frequency = this.springFrequenzy;
        }

        if (HasNeighbour(CardinalDirection.south)) {
            southNeighbour.cell.GetSpring(this).frequency = this.springFrequenzy;
        }

        if (HasNeighbour(CardinalDirection.southEast)) {
            southEastSpring.frequency = this.springFrequenzy;
        }
    }
}