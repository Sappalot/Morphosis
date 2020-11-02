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

	public int[,] cellsByTypeRecord; //[channel, rays] We need one for each channel since the types asked for might be different in them.   0 = no cell, 1 = cell by type
	public int[] cellsByTypeSum; // one for each channel

	private bool hasBeenSetup;

	public float CellsByTypeFovCov(int channel) {
		return (float)cellsByTypeSum[channel] / (float)raySlotCount;
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

			// ... records ...
			cellsByTypeRecord = new int[6, raySlotCount];
			cellsByTypeSum = new int[6];
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



	private GeneSurroundingSensor geneSurroundingSensor {
		get {
			return (hostCell.gene.surroundingSensor as GeneSurroundingSensor);
		}
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
			Vector2 rayStart = hostCell.position + rayVectorNormalized * hostCell.radius; // start at rim of cell

			lastDebugRay.rayStart = hostCell.position + rayVectorNormalized * hostCell.radius;
			lastDebugRay.rayEnd = hostCell.position + rayVectorNormalized * rangeFar;

			rayCursor++;
			if (rayCursor >= raySlotCount) {
				rayCursor = 0;
			}

			//return;

			int layerMask = 1; // default
			int raycastHitCount = Physics2D.RaycastNonAlloc(rayStart, rayVectorNormalized, raycastHitArrayOne, rangeFar - hostCell.radius, layerMask);
			CollisionType hitType = CollisionType.undefined;
			if (raycastHitCount == 0) {
				//Debug.Log("See only the void");
			} else {
				hitType = GetCollisionType(raycastHitArrayOne[0], hostCell.creature);
				//Debug.Log("See " + hitType.ToString());
			}

			for (int channel = 0; channel < 6; channel++) {

				int newHit = (hitType == CollisionType.othersCell ? 1 : 0);
				cellsByTypeSum[channel] -= cellsByTypeRecord[channel, rayCursor];
				cellsByTypeRecord[channel, rayCursor] = newHit;
				cellsByTypeSum[channel] += newHit;

				output[channel] = false;

				if (OperatingSensorAtChannel(channel) == SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov) {
					output[channel] = CellsByTypeFovCov(channel) > ((GeneSurroundingSensorChannelCreatureCellFovCov)GeneSurroundingSensorAtChannelByType(channel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov)).threshold;
				}
				// else if (OperatingSensorAtChannel(channel) == SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov) {
				//	if (hitType == CollisionType.nonCellObstacle) {

				//	}
				//} else {
				//	Debug.LogError("SurroundingSensor: Trying to evaluate output for an unknown SurroundingSensorChannelSensorTypeEnum");
				//}
				// output[channel] = false;
			}
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