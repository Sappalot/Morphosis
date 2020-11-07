using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurroundingSensor : SignalUnit {
	private bool[] output = new bool[6]; // outputs 
	private RaycastHit2D[] raycastHitArrayOne;

	private int raySlotCount; // number of rays int the field of view. The rays are supposed to cover the entire area with no gap bigger than 0.9 meters 
	private const float smallestArcGap = 0.95f;
	private float[] raySlotLocalDirectionsBlackWhite; // the direction of each slot relative to the directionLocal
	private float[] raySlotLocalDirectionsWhiteBlack; // the direction of each slot relative to the directionLocal
	private int rayCursor;

	private int[,] cellsByTypeRecord; //[channel, rays] We need one for each channel since the types asked for might be different in them.   0 = no cell, 1 = cell by type
	private int[] cellsByTypeSum = new int[6];

	private int[,] terrainRockRecord;
	private int[] terrainRockSum = new int[6];

	private bool hasBeenSetup;

	public float CellsByTypeFovCov(int channel) {
		return (float)cellsByTypeSum[channel] / (float)raySlotCount;
	}

	public float TerrainRockFovCov(int channel) {
		return (float)terrainRockSum[channel] / (float)raySlotCount;
	}

	public SurroundingSensor(SignalUnitEnum signalUnit, Cell hostCell) : base(hostCell) {
		base.signalUnitEnum = signalUnit;
		if (raycastHitArrayOne == null) {
			raycastHitArrayOne = new RaycastHit2D[1]; // Raycast function will return as the array is full, so by making it 1 big we can ignore everything that is behind the first hit
		}
	}

	public override void PostUpdateNervesPhenotype() {
		// Just needed to be set up once after cell has been spawned and before being used first time
		if (rootnessEnum == RootnessEnum.Rooted && !hasBeenSetup) {
			float arcLength = rangeFar * 2f * Mathf.PI * (fieldOfView / 360f);
			raySlotCount = Mathf.CeilToInt(arcLength / smallestArcGap) + 1;

			float strideAngle = raySlotCount == 0 ? 0f : fieldOfView / (float)(raySlotCount - 1f);

			// Store the fan of slots relative to localDirection
			// Dont care about flip side since we can scan either way... doesn't matter
			raySlotLocalDirectionsBlackWhite = new float[raySlotCount];
			raySlotLocalDirectionsWhiteBlack = new float[raySlotCount];
			for (int slot = 0; slot < raySlotCount; slot++) {
				raySlotLocalDirectionsBlackWhite[slot] = (hostCell.flipSide == FlipSideEnum.BlackWhite ? directionLocal : -directionLocal) - fieldOfView / 2f + slot * strideAngle;
				raySlotLocalDirectionsWhiteBlack[slot] = (hostCell.flipSide == FlipSideEnum.BlackWhite ? directionLocal : -directionLocal) + fieldOfView / 2f - slot * strideAngle;
			}

			rayCursor = 0;

			// ... reset records ...
			cellsByTypeRecord = new int[6, raySlotCount];
			terrainRockRecord = new int[6, raySlotCount];
			for (int c = 0; c < 6; c++) {
				for (int r = 0; r < raySlotCount; r++) {
					cellsByTypeRecord[c, r] = 0;
					terrainRockRecord[c, r] = 0;
				}
			}

			for (int c = 0; c < 6; c++) {
				cellsByTypeSum[c] = 0;
				terrainRockSum[c] = 0;
			}
		}
	}

	public GeneSurroundingSensorChannel GeneSurroundingSensorAtChannelByType(int channel, SurroundingSensorChannelSensorTypeEnum type) {
		return geneSurroundingSensor.GeneSensorAtChannelByType(channel, type);
	}

	public SurroundingSensorChannelSensorTypeEnum OperatingSensorAtChannel(int channel) { // index: 0 is ditched,  1 = output at A, ....
		return geneSurroundingSensor.OperatingSensorAtChannel(channel);
	}

	public float fieldOfView {
		get {
			return geneSurroundingSensor.fieldOfView;
		}
	}

	// direction in local space, that is offset from cells heading

	public float directionLocal {
		get {
			return geneSurroundingSensor.directionLocal;
		}
	}

	// the eyes direction in world space, 0 is east 90 is north 
	public float eyeHeading {
		get {
			return AngleUtil.AngleRawToSafe(hostCell.heading + (hostCell.flipSide == FlipSideEnum.BlackWhite ? directionLocal : -directionLocal));
		}
	}
	
	public float rangeNear {
		get {
			return geneSurroundingSensor.rangeNear;
		}
	}

	public float rangeFar {
		get {
			return geneSurroundingSensor.rangeFar;
		}
	}

	public void OnCellSpawned() {
		hasBeenSetup = false;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[SignalUnitSlotOutputToIndex(signalUnitSlot)];
	}

	public class DebugRay {
		public Vector2 rayStart;
		public Vector2 rayEnd;
	}

	public DebugRay lastDebugRay = new DebugRay();


	public override void ComputeSignalOutput(int deltaTicks) {
		if (signalUnitEnum == SignalUnitEnum.SurroundingSensor) { // redundant check ? 
			if (rootnessEnum != RootnessEnum.Rooted) {
				return;
			}

			Vector2 rayVectorNormalized = GeometryUtil.GetVector(hostCell.heading + (hostCell.flipSide == FlipSideEnum.BlackWhite ? raySlotLocalDirectionsBlackWhite[rayCursor] : raySlotLocalDirectionsWhiteBlack[rayCursor]), 1f);
			Vector2 rayStart = hostCell.position + rayVectorNormalized * (hostCell.radius - 0.1f); // start at a little bit inside rim of cell. Reason: ray only hits collision areas as it enters them and if they are kissing eye it might start inside them

			lastDebugRay.rayStart = hostCell.position + rayVectorNormalized * hostCell.radius;
			lastDebugRay.rayEnd = hostCell.position + rayVectorNormalized * rangeFar;

			rayCursor++;
			if (rayCursor >= raySlotCount) {
				rayCursor = 0;
			}

			int layerMask = 1; // default
			int raycastHitCount = Physics2D.RaycastNonAlloc(rayStart, rayVectorNormalized, raycastHitArrayOne, rangeFar - hostCell.radius, layerMask);

			for (int channel = 0; channel < 6; channel++) {
				output[channel] = false;
				if (OperatingSensorAtChannel(channel) == SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov) {
					int newHit = 0; // 1 if this ray is hitting something that the eye sees
					if (raycastHitCount > 0) {
						Cell hitCell = raycastHitArrayOne[0].collider.gameObject.GetComponent<Cell>();
						if (hitCell != null) {
							// ray was hitting a cell
							Creature hitCreature = hitCell.creature;
							if (hitCreature != hostCell.creature) {
								// ray was hitting a cell in another creature (never count own cells)
								switch (hitCell.GetCellType()) {
									case CellTypeEnum.Egg:
										newHit = GeneSurroundingSensorChannelFovCov(channel).seeEgg ? 1 : 0;
										break;
									case CellTypeEnum.Fungal:
										newHit = GeneSurroundingSensorChannelFovCov(channel).seeFungal ? 1 : 0;
										break;
									case CellTypeEnum.Jaw:
										if (GeneSurroundingSensorChannelFovCov(channel).seeJawThreat || GeneSurroundingSensorChannelFovCov(channel).seeJawHarmless) {
											bool isThreatToMe = IsJawThreatToMe((JawCell)hitCell, hostCell.creature);
											newHit = (isThreatToMe && GeneSurroundingSensorChannelFovCov(channel).seeJawThreat) || (!isThreatToMe && GeneSurroundingSensorChannelFovCov(channel).seeJawHarmless) ? 1 : 0;
										}
										break;
									case CellTypeEnum.Leaf:
										newHit = GeneSurroundingSensorChannelFovCov(channel).seeLeaf ? 1 : 0;
										break;
									case CellTypeEnum.Muscle:
										newHit = GeneSurroundingSensorChannelFovCov(channel).seeMuscle ? 1 : 0;
										break;
									case CellTypeEnum.Root:
										newHit = GeneSurroundingSensorChannelFovCov(channel).seeRoot ? 1 : 0;
										break;
									case CellTypeEnum.Shell:
										newHit = GeneSurroundingSensorChannelFovCov(channel).seeShell ? 1 : 0;
										break;
									case CellTypeEnum.Vein:
										newHit = GeneSurroundingSensorChannelFovCov(channel).seeVein ? 1 : 0;
										break;
								} // end switch cell type
							} // end if hit other than me
						} // end if hit cell
					} // end if hitting anything at all

					cellsByTypeSum[channel] -= cellsByTypeRecord[channel, rayCursor];
					cellsByTypeRecord[channel, rayCursor] = newHit;
					cellsByTypeSum[channel] += newHit;

					output[channel] = CellsByTypeFovCov(channel) > ((GeneSurroundingSensorChannelCreatureCellFovCov)GeneSurroundingSensorAtChannelByType(channel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov)).threshold;
				} else if (OperatingSensorAtChannel(channel) == SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov) {
					int newHit = 0; // 1 if this ray is hitting something that the eye sees
					if (raycastHitCount > 0) {
						newHit = (raycastHitArrayOne[0].collider.gameObject.GetComponent<Cell>() == null ? 1 : 0);
					}
					terrainRockSum[channel] -= terrainRockRecord[channel, rayCursor];
					terrainRockRecord[channel, rayCursor] = newHit;
					terrainRockSum[channel] += newHit;

					output[channel] = TerrainRockFovCov(channel) > ((GeneSurroundingSensorChannelTerrainRockFovCov)GeneSurroundingSensorAtChannelByType(channel, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov)).threshold;
				}
			} // end for every channel
		}
	}

	public override void Clear() {
		for (int i = 0; i < output.Length; i++) {
			output[i] = false;
		}
	}

	private enum CollisionType {
		ownCell,
		othersCellIgnored,
		othersCell,
		nonCellObstacle,
		undefined,
	}

	private static CollisionType GetCollisionType(RaycastHit2D hit, Creature me) {
		Cell hitCell = hit.collider.gameObject.GetComponent<Cell>();
		if (hitCell != null) {
			Creature hitCreature = hitCell.creature;
			if (hitCreature == me) {
				return CollisionType.ownCell;
			} else {
				bool isJawCell = hitCell is JawCell;
				bool hitIsChildOfMe = me.GetChildrenAlive().Contains(hitCell.creature);
				bool hitIsMotherOfMe = me.GetMotherAlive() == hitCell.creature;
				string myMotherId = me.GetMotherIdDeadOrAlive();
				string hitMotherId = hitCell.creature.GetMotherIdDeadOrAlive();
				bool meAndHitHasSameMother = myMotherId != null && hitMotherId != null && myMotherId == hitMotherId;
				if (isJawCell || hitIsChildOfMe || hitIsMotherOfMe || meAndHitHasSameMother) {
					return CollisionType.othersCellIgnored;
				}
				return CollisionType.othersCell;
			}
		}

		return CollisionType.nonCellObstacle;
	}

	private static bool IsJawThreatToMe(JawCell jaw, Creature me) {
		Creature otherCreature = jaw.creature;
		bool otherCreatureIsMyMother = me.HasMotherAlive() && me.GetMotherAlive() == otherCreature;
		bool iAmOtherCreaturesMother = otherCreature.HasMotherAlive() && otherCreature.GetMotherAlive() == me;
		bool otherCreatureAndMeAreSiblings = otherCreature.GetMotherIdDeadOrAlive() == me.GetMotherIdDeadOrAlive();

		// don't eat attached mother
		if (me.HasMotherAlive() && otherCreature == me.GetMotherAlive() && me.IsAttachedToMotherAlive()) {
			return false;
		}

		// don't eat attached children
		if (otherCreature.HasMotherAlive() && me == otherCreature.GetMotherAlive() && otherCreature.IsAttachedToMotherAlive()) {
			return false;
		}

		// TODO: kin and father
		if (otherCreatureIsMyMother) {
			return jaw.gene.jawCellCannibalizeChildren; // there is a chance she will not eat me
		} else if (iAmOtherCreaturesMother) {
			return jaw.gene.jawCellCannibalizeMother; // ther eis a cahance it will not eat me
		} else if (otherCreatureAndMeAreSiblings) {
			return jaw.gene.jawCellCannibalizeSiblings; // ther eis a cahance it will not eat me
		} else /* We are strangers to eachothers*/ {
			return true;
		}
	}

	private GeneSurroundingSensor geneSurroundingSensor {
		get {
			return (hostCell.gene.surroundingSensor as GeneSurroundingSensor);
		}
	}

	private GeneSurroundingSensorChannelCreatureCellFovCov GeneSurroundingSensorChannelFovCov(int channel) {
		return (GeneSurroundingSensorChannelCreatureCellFovCov)geneSurroundingSensor.GeneSensorAtChannelByType(channel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov);
	}

	// Load Save
	private CommonSensorData sensorData = new CommonSensorData();

	// Save
	public CommonSensorData UpdateData() {
		sensorData.slotA = output[0];
		sensorData.slotB = output[1];
		sensorData.slotC = output[2];
		sensorData.slotD = output[3];
		sensorData.slotE = output[4];
		sensorData.slotF = output[5];
		return sensorData;
	}

	// Load
	public void ApplyData(CommonSensorData sensorData) {
		output[0] = sensorData.slotA;
		output[1] = sensorData.slotB;
		output[2] = sensorData.slotC;
		output[3] = sensorData.slotD;
		output[4] = sensorData.slotE;
		output[5] = sensorData.slotF;
	}
}

				////Hack
				//if (channel == 2) {
				//	if (hitType == CollisionType.othersCell) {
				//		evaluatedOutput = false;
				//	} else {
				//		evaluatedOutput = true;
				//	}
				//	output[channel] = evaluatedOutput;
				//	continue;
				//}

				//if (channel == 3) {
				//	if (hitType == CollisionType.nonCellObstacle) {
				//		evaluatedOutput = false;
				//	} else {
				//		evaluatedOutput = true;
				//	}
				//	output[channel] = evaluatedOutput;
				//	continue;
				//}