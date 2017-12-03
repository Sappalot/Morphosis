using UnityEngine;

public class LeafCell : Cell {
	public LineRenderer[] testRays = new LineRenderer[6];
	private const int recorMaxCapacity = 5;
	private float[] effectRecord = new float[recorMaxCapacity];
	private int effectRecorCursor = 0;
	private int effectRecordCount = 0;

	public LeafCell() : base() {
		springFrequenzy = 5f;
		springDamping = 11f;
	}

	public override void UpdateMetabolism(float deltaTime) {
		effectConsumptionInternal = GlobalSettings.instance.phenotype.leafCellEffectCost;

		float effectSum = 0;
		for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
			if (!HasNeighbourCell(cardinalIndex)) {
				//testRays[cardinalIndex].enabled = true;


				float angle = angleDiffFromBindpose + AngleUtil.CardinalIndexToAngle(cardinalIndex) + Random.Range(-30f, 30);
				Vector2 directionVector = GeometryUtils.GetVector(angle, 1f);
				Vector2 origin = position + directionVector;

				RaycastHit2D hit = Physics2D.Raycast(origin, directionVector, GlobalSettings.instance.phenotype.leafCellSunMaxRange, 1);
				Vector2 hipPosition = hit.point;
				float range = hit.fraction;

				//testRays[cardinalIndex].GetComponent<LineRenderer>().SetPosition(1, origin);
				if (hit.collider != null) {
					//testRays[cardinalIndex].GetComponent<LineRenderer>().SetPosition(0, hipPosition);
				} else {
					range = 1f;
					//testRays[cardinalIndex].GetComponent<LineRenderer>().SetPosition(0, origin + directionVector * GlobalSettings.instance.leafCellSunMaxRange);
				}
				//testRays[cardinalIndex].GetComponent<LineRenderer>().startColor = new Color(1f, 1f, 1f, range);
				//testRays[cardinalIndex].GetComponent<LineRenderer>().endColor = new Color(1f, 1f, 1f, range);
				effectSum += range;
			} else {
				//testRays[cardinalIndex].enabled = false;
			}
		}
		float effect = GlobalSettings.instance.phenotype.leafCellSunMaxEffect * effectSum / 6f;

		
		effectRecord[effectRecorCursor] = effect;
		effectRecorCursor++;
		if (effectRecorCursor >= recorMaxCapacity) {
			effectRecorCursor = 0;
		}
		effectRecordCount = (int)Mathf.Min(recorMaxCapacity, effectRecordCount + 1);

		float lowPass = 0f;
		for (int i = 0; i < effectRecordCount; i++) {
			lowPass += effectRecord[i];
		}
		effectProduction = lowPass / effectRecordCount;


		base.UpdateMetabolism(deltaTime);
	}



	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Leaf;
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