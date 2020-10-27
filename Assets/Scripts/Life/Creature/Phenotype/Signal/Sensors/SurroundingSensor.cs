﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurroundingSensor : SignalUnit {
	private bool[] output = new bool[6]; // outputs 
	private RaycastHit2D[] raycastHitArrayOne;

	public SurroundingSensor(SignalUnitEnum signalUnit, Cell hostCell) : base(hostCell) {
		base.signalUnitEnum = signalUnit;
		if (raycastHitArrayOne == null) {
			raycastHitArrayOne = new RaycastHit2D[1]; // Raycast function will return as the array is full, so by making it 1 big we can ignore everything that is behind the first hit
		}
	}

	public GeneSurroundingSensorChannel SensorAtChannelByType(int channel, SurroundingSensorChannelSensorTypeEnum type) {
		return geneSurroundingSensor.SensorAtChannelByType(channel, type);
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
	private GeneSurroundingSensor geneSurroundingSensor {
		get {
			return (hostCell.gene.surroundingSensor as GeneSurroundingSensor);
		}
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[SignalUnitSlotOutputToIndex(signalUnitSlot)];
	}

	public override void ComputeSignalOutput(int deltaTicks) {
		if (signalUnitEnum == SignalUnitEnum.SurroundingSensor) { // redundant check ? 
			if (rootnessEnum != RootnessEnum.Rooted) {
				return;
			}

			Vector2 rayVectorNormalized = GeometryUtil.GetVector(eyeHeading, 1f);
			Vector2 rayStart = hostCell.position + rayVectorNormalized * hostCell.radius; // start at rim of cell

			int raycastHitCount = Physics2D.RaycastNonAlloc(rayStart, rayVectorNormalized, raycastHitArrayOne, rangeFar - hostCell.radius, 1);
			RaycastUtil.CollisionType hitType = RaycastUtil.CollisionType.undefined;
			if (raycastHitCount == 0) {
				//Debug.Log("See only the void");
			} else {
				hitType = RaycastUtil.GetCollisionType(raycastHitArrayOne[0], hostCell.creature, true);
				//Debug.Log("See " + hitType.ToString());
			}

			for (int channel = 0; channel < 6; channel++) {
				bool evaluatedOutput = false;

				//Hack
				if (channel == 2) {
					if (hitType == RaycastUtil.CollisionType.othersCell) {
						evaluatedOutput = false;
					} else {
						evaluatedOutput = true;
					}
					output[channel] = evaluatedOutput;
					continue;
				}

				if (channel == 3) {
					if (hitType == RaycastUtil.CollisionType.otherObstacle) {
						evaluatedOutput = false;
					} else {
						evaluatedOutput = true;
					}
					output[channel] = evaluatedOutput;
					continue;
				}


				if (OperatingSensorAtChannel(channel) == SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov) {
					if (hitType == RaycastUtil.CollisionType.othersCell) {
						evaluatedOutput = true;
					}
				} else if (OperatingSensorAtChannel(channel) == SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov) {
					if (hitType == RaycastUtil.CollisionType.otherObstacle) {
						evaluatedOutput = true;
					}
				} else {
					Debug.LogError("SurroundingSensor: Trying to evaluate output for an unknown SurroundingSensorChannelSensorTypeEnum");
				}
				output[channel] = evaluatedOutput;
			}
		}
	}

	public override void Clear() {
		for (int i = 0; i < output.Length; i++) {
			output[i] = false;
		}
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