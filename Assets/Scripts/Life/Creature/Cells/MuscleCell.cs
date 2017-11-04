using UnityEngine;
using System.Collections.Generic;

public class MuscleCell : Cell {

	public MuscleCell() : base() {
		springFrequenzy = 20f;
		springDamping = 11f;
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Muscle;

	}
	private float modularTime = 0f;
	private bool isContracting;

	public override void UpdateRadius(float fixedTime) {
		float muscleSpeed = creature.muscleSpeed;
		float radiusDiff = creature.muscleRadiusDiff;
		float curveOffset = creature.muscleContractRetract;


		//modularTime += Time.fixedDeltaTime * muscleSpeed;

		//float deltaTime = time - lastTime;
		//lastTime = time;

		//if (Mathf.Sign(curveOffset + Mathf.Cos(modularTime / (2f * Mathf.PI))) > 0) {
		//    radius = 0.5f;
		//} else {
		//    radius = 0.5f - radiusDiff;
		//}

		//--------------------

		//modularTime += Time.fixedDeltaTime * muscleSpeed;

		modularTime = fixedTime * muscleSpeed;

		float deltaTime = fixedTime - lastTime;
		lastTime = fixedTime;

		float expandContract = Mathf.Sign(curveOffset + Mathf.Cos(timeOffset + modularTime / (2f * Mathf.PI)));
		float radiusGoal = 0.5f - 0.5f * radiusDiff + 0.5f * radiusDiff * expandContract;

		float goingSmallSpeed = 0.5f; //units per second
		float goingBigSpeed = 0.02f;
		//float goingSmallSpeed = 0.02f; //units per second
		//float goingBigSpeed = 0.5f;

		if (radiusGoal > radius) {
			isContracting = false;
			radius = radius + goingBigSpeed * deltaTime;
			if (radius > radiusGoal)
				radius = radiusGoal;
		}
		else {
			isContracting = true;
			radius = radius - goingSmallSpeed * deltaTime;
			if (radius < radiusGoal)
				radius = radiusGoal;
		}

		//transform.localScale = new Vector3(radius * 2, radius * 2, 1f); //costy, only if in frustum

		//--------------------------------------------------

		// Note: It is cost to change scale, do we really have to? Maybee we could just change the graphics
		//gameObject.transform.localScale = new Vector3(radius * 2, radius * 2, 1f);

		/*float specialMuscleSpeed = 6f;
		if (type == CellType.Leaf) {
			float radiusDiff = 0.2f;
			float radiusGoal = 0.5f - 0.5f * radiusDiff + 0.5f * radiusDiff * Mathf.Sign(Mathf.Cos(-Mathf.PI * 0.5f + Mathf.PI + time * specialMuscleSpeed / (2f * Mathf.PI)));

			float goingSmallSpeed = 0.1f; //units per second
			float goingBigSpeed = 0.1f;

			if (radiusGoal > radius) {
				radius = radius + goingBigSpeed * Time.fixedDeltaTime;
				if (radius > radiusGoal)
					radius = radiusGoal;
			}
			else {
				radius = radius - goingSmallSpeed * Time.fixedDeltaTime;
				if (radius < radiusGoal)
					radius = radiusGoal;
			}

			gameObject.transform.localScale = new Vector3(radius * 2, radius * 2, 1f);
		}
		if (type == CellType.Mouth) {
			float radiusDiff = 0.2f;
			float radiusGoal = 0.5f - 0.5f * radiusDiff + 0.5f * radiusDiff * Mathf.Sign(Mathf.Cos(Mathf.PI * 0.5f + Mathf.PI + time * specialMuscleSpeed / (2f * Mathf.PI)));

			float goingSmallSpeed = 0.1f; //units per second
			float goingBigSpeed = 0.1f;

			if (radiusGoal > radius) {
				radius = radius + goingBigSpeed * Time.fixedDeltaTime;
				if (radius > radiusGoal)
					radius = radiusGoal;
			}
			else {
				radius = radius - goingSmallSpeed * Time.fixedDeltaTime;
				if (radius < radiusGoal)
					radius = radiusGoal;
			} 

			gameObject.transform.localScale = new Vector3(radius * 2, radius * 2, 1f); //costy
		}*/
	}
    
	public override bool IsContracting() {
		return isContracting;
	}

	long seldom = 0;
	public override void UpdateSpringLengths() {
		if (seldom % 5 == 0) {
			if (HasNeighbourCell(CardinalEnum.northEast, false)) {
				northEastNeighbour.cell.GetSpring(this).distance = this.radius + northEastNeighbour.cell.radius;
			}

			if (HasNeighbourCell(CardinalEnum.north, false)) {
				northSpring.distance = this.radius + northNeighbour.cell.radius;
			}

			if (HasNeighbourCell(CardinalEnum.northWest, false)) {
				northWestNeighbour.cell.GetSpring(this).distance = this.radius + northWestNeighbour.cell.radius;
			}

			if (HasNeighbourCell(CardinalEnum.southWest, false)) {
				southWestSpring.distance = this.radius + southWestNeighbour.cell.radius;
			}

			if (HasNeighbourCell(CardinalEnum.south, false)) {
				southNeighbour.cell.GetSpring(this).distance = this.radius + southNeighbour.cell.radius;
			}

			if (HasNeighbourCell(CardinalEnum.southEast, false)) {
				southEastSpring.distance = this.radius + southEastNeighbour.cell.radius;
			}
		}
		seldom++;
	}

	public override void UpdateSpringFrequenzy() {
		base.UpdateSpringFrequenzy();

		if (HasNeighbourCell(CardinalEnum.north, false)) {
			northSpring.frequency = (this.springFrequenzy + northNeighbour.cell.springFrequenzy) / 2f;
			northSpring.dampingRatio = (this.springDamping + northNeighbour.cell.springDamping) / 2f;
		}

		if (HasNeighbourCell(CardinalEnum.southWest, false)) {
			southWestSpring.frequency = (this.springFrequenzy + southWestNeighbour.cell.springFrequenzy) / 2f;
			southWestSpring.dampingRatio = (this.springDamping + southWestNeighbour.cell.springDamping) / 2f;
		}

		if (HasNeighbourCell(CardinalEnum.southEast, false)) {
			southEastSpring.frequency = (this.springFrequenzy + southEastNeighbour.cell.springFrequenzy) / 2f;
			southEastSpring.dampingRatio = (this.springDamping + southEastNeighbour.cell.springDamping) / 2f;
		}
	}

}

