public class JawCell : Cell {

	public JawCellMouth mouth;

	public JawCell() : base() {
		springFrequenzy = 5f;
		springDamping = 11f;
	}

	public override void UpdateMetabolism(float deltaTime) {
		//const float eatEffect = 3f;
		//float deltaEnergyPrayToPredator = eatEffect * deltaTime;
		//foreach (Cell prayCell in mouth.prayCells) {
		//	prayCell.energy -= deltaEnergyPrayToPredator;
		//	energy += deltaEnergyPrayToPredator;
		//}

		effectProduction = mouth.prayCount * GlobalSettings.instance.jawCellEatEffect;

		base.UpdateMetabolism(deltaTime);
	}

	override public Creature creature {
		get {
			return m_creature;
		}
		set {
			m_creature = value;
			mouth.creature = value;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Jaw;
	}

	public override void UpdateSpringFrequenzy() {
		base.UpdateSpringFrequenzy();

		if (HasOwnNeighbourCell(CardinalEnum.north)) {
			northSpring.frequency = (this.springFrequenzy + northNeighbour.cell.springFrequenzy) / 2f;
			northSpring.dampingRatio = (this.springDamping + northNeighbour.cell.springDamping) / 2f;
		}

		if (HasOwnNeighbourCell(CardinalEnum.southWest)) {
			southWestSpring.frequency = (this.springFrequenzy + southWestNeighbour.cell.springFrequenzy) / 2f;
			southWestSpring.dampingRatio = (this.springDamping + southWestNeighbour.cell.springDamping) / 2f;
		}

		if (HasOwnNeighbourCell(CardinalEnum.southEast)) {
			southEastSpring.frequency = (this.springFrequenzy + southEastNeighbour.cell.springFrequenzy) / 2f;
			southEastSpring.dampingRatio = (this.springDamping + southEastNeighbour.cell.springDamping) / 2f;
		}
	}
}
