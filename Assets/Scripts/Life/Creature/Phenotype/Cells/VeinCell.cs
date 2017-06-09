using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class VeinCell : Cell {

    public VeinCell() : base() {
        springFrequenzy = 5f;
    }

    public override void UpdateSpringFrequenzy() {

        if (HasNeighbour(CardinalDirectionEnum.northEast)) {
            northEastNeighbour.cell.GetSpring(this).frequency = this.springFrequenzy;
        }

        if (HasNeighbour(CardinalDirectionEnum.north)) {
            northSpring.frequency = this.springFrequenzy;
        }

        if (HasNeighbour(CardinalDirectionEnum.northWest)) {
            northWestNeighbour.cell.GetSpring(this).frequency = this.springFrequenzy;
        }

        if (HasNeighbour(CardinalDirectionEnum.southWest)) {
            southWestSpring.frequency = this.springFrequenzy;
        }

        if (HasNeighbour(CardinalDirectionEnum.south)) {
            southNeighbour.cell.GetSpring(this).frequency = this.springFrequenzy;
        }

        if (HasNeighbour(CardinalDirectionEnum.southEast)) {
            southEastSpring.frequency = this.springFrequenzy;
        }
    }
}