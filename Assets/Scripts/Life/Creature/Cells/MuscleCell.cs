using UnityEngine;

public class MuscleCell : Cell {
	public Transform scale;

	private float modularTime = 0f;
	private bool isContracting;
	private bool scaleIsDirty = true;

	public Vector2i masterAxonGridPosition;
	public int? masterAxoneDistance;

	public void UpdateMasterAxon() {
		masterAxonGridPosition = creature.genotype.GetClosestAxonGeneCellUpBranch(mapPosition).mapPosition;
		masterAxoneDistance = creature.genotype.GetDistanceToClosestAxonGeneCellUpBranch(mapPosition);
	}

	override public bool IsIdle() {
		return gene.muscleCellIdleWhenAttached && creature.IsAttachedToMotherAlive();
	}

	override public float springFrequenzy {
		get {
			return 20f;
		}
	}

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (PhenotypePhysicsPanel.instance.effectMuscle.isOn && PhenotypePhysicsPanel.instance.functionMuscle.isOn) {
			effectProductionInternalDown = GlobalSettings.instance.phenotype.muscleCellEffectCostPerHz * creature.phenotype.originCell.originPulseFequenzy;
		} else {
			effectProductionInternalDown = 0f;
		}
		effectProductionInternalUp = 0f;
		if (PhenotypePhysicsPanel.instance.functionMuscle.isOn) {
			if (IsIdle()) {
				effectProductionInternalUp = 0f;
				effectProductionInternalDown = GlobalSettings.instance.phenotype.cellIdleEffectCost;

				scale.localScale = new Vector3(1f, 1f, 1f); //costy, only if in frustum and close
				scaleIsDirty = true;
			} else {
				UpdateRadius(worldTicks);
			}
			UpdateSpringLengths();
			base.UpdateCellFunction(deltaTicks, worldTicks);
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Muscle;
	}

	

	public override void UpdateRadius(ulong worldTicks) {
		float muscleSpeed = creature.muscleSpeed;
		float radiusDiff = creature.muscleRadiusDiff;
		float curveOffset = creature.muscleContractRetract;

		//modularTime = worldTicks * Time.fixedDeltaTime * muscleSpeed;

		//float deltaTime = worldTicks * Time.fixedDeltaTime - lastTime;
		//lastTime = worldTicks * Time.fixedDeltaTime;

		//Debug.Log("offset" + timeOffset);
		///float expandContract = Mathf.Sign(curveOffset + Mathf.Cos(creature.phenotype.originCell.originPulseCompleteness * (2f * Mathf.PI)));
		//float radiusGoal = 0.5f - 0.5f * radiusDiff + 0.5f * radiusDiff * expandContract;

		//float goingSmallSpeed = 0.5f * 4f * 0f; //units per second
		//float goingBigSpeed = 0.02f * 4f * 0f;

		//--
		bool contracting = false;
		if (masterAxonGridPosition != null) {
			Cell masterAxon = creature.phenotype.cellMap.GetCell(masterAxonGridPosition);
			if (masterAxon != null) {
				if (masterAxoneDistance != null) {
					contracting = masterAxon.IsAxonePulseContracting((int)masterAxoneDistance);
				} else {
					Debug.LogError("We have found a master axone, but failed to calculate the distance there from me!");
				}
			}
		}

		if (contracting && !creature.phenotype.IsSliding(worldTicks)) {
			isContracting = true;
			radius -= Time.fixedDeltaTime * 2f;
		} else {
			isContracting = false;
			radius += Time.fixedDeltaTime * 2f;
		}
		radius = Mathf.Clamp(radius, 0.3f, 0.5f);
		//radius = radiusGoal;

		if (CameraUtils.IsObservedLazy(position, GlobalSettings.instance.orthoPlayFxLimit)) {
			scale.localScale = new Vector3(radius * 2f, radius * 2f, 1f); //costy, only if in frustum and close
			scaleIsDirty = true;
		} else if (scaleIsDirty) {
			scale.localScale = new Vector3(1f, 1f, 1f);
			scaleIsDirty = false;
		}
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

		// Update placenta spring lengths from me to child root, if i happen to be placenta
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

	public override void OnRecycleCell() {
		base.OnRecycleCell();
		isContracting = false;
		scaleIsDirty = true;
		masterAxonGridPosition = null;
		masterAxoneDistance = null;
	}
}