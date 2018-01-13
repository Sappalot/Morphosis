using UnityEngine;
using System.Collections.Generic;

public class MuscleCell : Cell {

	public MuscleCell() : base() {
		springFrequenzy = 20f;
		springDamping = 11f;
	}

	public override void UpdateMetabolism(int deltaTicks, ulong worldTicks) {
		effectConsumptionInternal = GlobalSettings.instance.phenotype.muscleCellEffectCost;
		effectProduction = 0f;

		UpdateRadius(worldTicks);
		UpdateSpringLengths();

		base.UpdateMetabolism(deltaTicks, worldTicks);
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Muscle;

	}
	private float modularTime = 0f;
	private bool isContracting;
	private bool scaleIsDirty = true;

	public override void UpdateRadius(ulong worldTicks) {
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

		modularTime = worldTicks * Time.fixedDeltaTime * muscleSpeed;

		float deltaTime = worldTicks * Time.fixedDeltaTime - lastTime;
		lastTime = worldTicks * Time.fixedDeltaTime;

		float expandContract = Mathf.Sign(curveOffset + Mathf.Cos(timeOffset + modularTime / (2f * Mathf.PI)));
		float radiusGoal = 0.5f - 0.5f * radiusDiff + 0.5f * radiusDiff * expandContract;

		float goingSmallSpeed = 0.5f * 4f; //units per second
		float goingBigSpeed = 0.02f * 4f;
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

		if (CameraUtils.IsObservedLazy(position, GlobalSettings.instance.orthoMaxHorizonDetailedCell)) {
			transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f); //costy, only if in frustum and close
			scaleIsDirty = true;
		} else if (scaleIsDirty) {
			transform.localScale = new Vector3(1f, 1f, 1f);
			scaleIsDirty = false;
		}

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

	//long seldom = 0;
	public override void UpdateSpringLengths() {

		//Intra creature
		if (HasOwnNeighbourCell(CardinalEnum.northEast)) {
			northEastNeighbour.cell.GetSpring(this).distance = this.radius + northEastNeighbour.cell.radius;
		}

		if (HasOwnNeighbourCell(CardinalEnum.north)) {
			northSpring.distance = this.radius + northNeighbour.cell.radius;
		}

		if (HasOwnNeighbourCell(CardinalEnum.northWest)) {
			northWestNeighbour.cell.GetSpring(this).distance = this.radius + northWestNeighbour.cell.radius;
		}

		if (HasOwnNeighbourCell(CardinalEnum.southWest)) {
			southWestSpring.distance = this.radius + southWestNeighbour.cell.radius;
		}

		if (HasOwnNeighbourCell(CardinalEnum.south)) {
			southNeighbour.cell.GetSpring(this).distance = this.radius + southNeighbour.cell.radius;
		}

		if (HasOwnNeighbourCell(CardinalEnum.southEast)) {
			southEastSpring.distance = this.radius + southEastNeighbour.cell.radius;
		}

		// Update placenta spring lengths from me to mother placenta, if i happen to be origin
		if (placentaSprings.Length > 0) {
			//i am origin and is connected to mother via placenta
			UpdatePlacentaSpringLengths();
		}

		// If i have a neighbouring cell which does not belong to my body and is origin: ask this cell to update its spring lengths (to placenta)
		if (isPlacenta) {
			for (int index = 0; index < 6; index++) {
				Cell neighbour = GetNeighbourCell(index);
				if (neighbour != null && neighbour.creature != creature && neighbour.isOrigin) {
					// i am a placenta cell
					neighbour.UpdatePlacentaSpringLengths();
					break;
				}
			}
		}
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

