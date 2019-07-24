using UnityEngine;

public class MuscleCell : Cell {
	public Transform scale;

	private float modularTime = 0f;
	private bool isContracting;
	private bool scaleIsDirty = true;

	public Vector2i masterAxonGridPosition;
	public int? masterAxoneDistance;

	private const float minRadius = 0.3f; // meters
	private const float medRadius = 0.5f; // meters
	private const float contractSpeed = 0.2f; // meters / second
	private const float relaxSpeed = 0.2f; // meters / second

	private static float shrinkageRadiusDiffConstant;
	private static float relaxRadiusDiffConstant;
	public static float contractionCostEffect { get; private set; }

	private new void Awake() {
		shrinkageRadiusDiffConstant = Time.fixedDeltaTime * GlobalSettings.instance.quality.muscleCellTickPeriod * contractSpeed;
		relaxRadiusDiffConstant = Time.fixedDeltaTime * GlobalSettings.instance.quality.muscleCellTickPeriod * contractSpeed;
		contractionCostEffect = GlobalSettings.instance.phenotype.muscleCellEnergyCostPerContraction / ((medRadius - minRadius) / contractSpeed);
		base.Awake();
	}

	public void UpdateMasterAxon() {
		masterAxonGridPosition = creature.genotype.GetClosestAxonGeneCellUpBranch(mapPosition).mapPosition;
		masterAxoneDistance = creature.genotype.GetDistanceToClosestAxonGeneCellUpBranch(mapPosition);
	}

	override public bool IsHibernating() {
		return (gene.muscleCellHibernateWhenAttachedToMother && creature.IsAttachedToMotherAlive()) || (gene.muscleCellHibernateWhenAttachedToChild && creature.IsAttachedToChildAlive());
	}

	override public float springFrequenzy {
		get {
			return 20f;
		}
	}

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		effectProductionInternalUp = 0f;

		if (PhenotypePhysicsPanel.instance.functionMuscle.isOn) {
			effectProductionInternalDown = GlobalSettings.instance.phenotype.cellIdleEffectCost;

			bool contracting = false;

			if (!IsHibernating()) {
				theRigidBody.WakeUp();
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
			}

			bool isRadiusDirty = false;

			if (contracting && !creature.phenotype.IsSliding(worldTicks)) {
				// Contracting
				if (radius > minRadius) {
					effectProductionInternalDown = contractionCostEffect + GlobalSettings.instance.phenotype.cellIdleEffectCost;
					isRadiusDirty = true;
				}
				isContracting = true;
				radius = Mathf.Max(radius - shrinkageRadiusDiffConstant, minRadius);
			} else {
				// You have Leelax
				if (radius < medRadius) {
					isRadiusDirty = true;
				}
				isContracting = false;
				radius = Mathf.Min(radius + relaxRadiusDiffConstant, medRadius);
			}

			if (isRadiusDirty) {
				if (SpatialUtil.IsInsideDetailedGraphicsVolume(position)) {
					scale.localScale = new Vector3(radius * 2f, radius * 2f, 1f); //costy, only if in frustum and close
					scaleIsDirty = true;
				} else if (scaleIsDirty) {
					scale.localScale = new Vector3(1f, 1f, 1f);
					scaleIsDirty = false;
				}
				UpdateSpringLengths();
			}

			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectProductionInternalDown = 0f;
			isContracting = false;
			radius = 0.5f;
			scale.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Muscle;
	}

	public override bool IsContracting() {
		return isContracting;
	}

	//long seldom = 0;
	public override void UpdateSpringLengths() {

		//Intra creature
		if (HasOwnNeighbourCell(CardinalEnum.northEast)) {
			SpringJoint2D spring = northEastNeighbour.cell.GetSpring(this);
			if (spring != null) {
				spring.distance = this.radius + northEastNeighbour.cell.radius;
			} else {
				Debug.LogError("Spring missing north east");
			}
		}

		if (HasOwnNeighbourCell(CardinalEnum.north)) {
			if (northSpring != null) {
				northSpring.distance = this.radius + northNeighbour.cell.radius;
			} else {
				Debug.LogError("Spring missing north");
			}
			
		}

		if (HasOwnNeighbourCell(CardinalEnum.northWest)) {
			SpringJoint2D spring = northWestNeighbour.cell.GetSpring(this);
			if (spring != null) {
				spring.distance = this.radius + northWestNeighbour.cell.radius;
			} else {
				Debug.LogError("Spring missing north west"); // This has occured :/ TODO: figgure out and fix!
			}
			
		}

		if (HasOwnNeighbourCell(CardinalEnum.southWest)) {
			if (southWestSpring != null) {
				southWestSpring.distance = this.radius + southWestNeighbour.cell.radius;
			} else {
				Debug.LogError("Spring missing south west");
			}
		}

		if (HasOwnNeighbourCell(CardinalEnum.south)) {
			SpringJoint2D spring = southNeighbour.cell.GetSpring(this);
			if (spring != null) {
				spring.distance = this.radius + southNeighbour.cell.radius;
			} else {
				Debug.LogError("Spring missing south");
			}
		}

		if (HasOwnNeighbourCell(CardinalEnum.southEast)) {
			if (southEastSpring != null) {
				southEastSpring.distance = this.radius + southEastNeighbour.cell.radius;
			} else {
				Debug.LogError("Spring missing south east");
			}
		}

		// Update placenta spring lengths from me to mother placenta, if i happen to be origin
		if (placentaSprings != null) {
			if (placentaSprings.Length > 0) {
				//i am origin and is connected to mother via her placenta
				UpdatePlacentaSpringLengths();
			}
		} else {
			//Debug.LogError("Placenta springs array is missing (the container)");
		}


		// Update placenta spring lengths from me to child origin, if i happen to be placenta
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

	public override void SetDefaultState() {
		base.SetDefaultState();
		radius = 0.5f;
		scale.localScale = new Vector3(1f, 1f, 1f);
	}

	public override void OnRecycleCell() {
		base.OnRecycleCell();
		SetDefaultState();
		isContracting = false;
		scaleIsDirty = true;
		masterAxonGridPosition = null;
		masterAxoneDistance = null;
	}
}