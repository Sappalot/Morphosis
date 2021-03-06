﻿using System.Collections.Generic;
using UnityEngine;

public class LeafCell : Cell {
	//public LineRenderer[] testRays = new LineRenderer[6];

	private const int exposureRecordCapacity = 20;
	private float[] exposureRecord = new float[exposureRecordCapacity];
	private int exposureRecorCursor = 0;
	private float exposureRecordSum; // this sum is updated to be the sum of watever is in the exposure record. An optimization, so that we dont have to go through the entire buffer and sum up the records each frame

	public override void OnBorrowToWorld() {
		base.OnBorrowToWorld(); // will call Set Default state from base class back to leaf (since this cell is a leaf)
		if (raycastHitArray == null) {
			raycastHitArray = new RaycastHit2D[(int)GlobalSettings.instance.phenotype.leafCell.sunRayMaxRange];
		}
	}

	public override void SetDefaultState() {
		base.SetDefaultState();
		lowPassExposure = exposureAtProductionEffectZero;
	}

	private float exposureAtProductionEffectZero {
		get {
			return GlobalSettings.instance.phenotype.leafCell.effectProductionDown / GlobalSettings.instance.phenotype.leafCell.effectProductionUpMax; ;
		}
	}
	

	// As the low pass exposure is loaded, all records are set to the average value of the saved cell
	// That is, we replace the various records in the buffer all with average values, This is not the same but does it really matter?
	private float m_lowPassExposure;
	public float lowPassExposure {
		get {
			return m_lowPassExposure;
		}
		set { 
			for (int i = 0; i < exposureRecordCapacity; i++) {
				exposureRecord[i] = value;
			}
			m_lowPassExposure = value;
			exposureRecordSum = value * exposureRecordCapacity;
			exposureRecorCursor = 0;
		}
	}

	public float speed {
		get {
			return velocity.magnitude; // use squared instead and adapt animation curve to match it!
		}
	}

	public float absoluteEffectCalmnessFactor {
		get {
			return GlobalSettings.instance.phenotype.leafCell.exposureFactorAtSpeed.Evaluate(creature.phenotype.IsSliding(World.instance.worldTicks) ? 0f : speed);
		}
	}

	// Friction (Drag)
	public override void SetFrictionNormal() {
		if (creature.phenotype.cellCount >= 3) {
			theRigidBody.drag = PhenotypePhysicsPanel.instance.frictionWater.isOn ? GlobalSettings.instance.phenotype.frictionUnderNormalLeaf : 0f;
		} else if (creature.phenotype.cellCount == 2) {
			theRigidBody.drag = PhenotypePhysicsPanel.instance.frictionWater.isOn ? GlobalSettings.instance.phenotype.frictionUnderNormal2CellsLeaf : 0f;
		} else {
			theRigidBody.drag = PhenotypePhysicsPanel.instance.frictionWater.isOn ? GlobalSettings.instance.phenotype.frictionUnderNormal1CellLeaf : 0f;
		}
	}

	public override void SetFrictionSliding() {
		if (creature.phenotype.cellCount >= 3) {
			theRigidBody.drag = PhenotypePhysicsPanel.instance.frictionWater.isOn ? GlobalSettings.instance.phenotype.frictionUnderNormalLeaf * GlobalSettings.instance.phenotype.frictionUnderSlidingFactor : 0f;
		} else if (creature.phenotype.cellCount == 2) {
			theRigidBody.drag = PhenotypePhysicsPanel.instance.frictionWater.isOn ? GlobalSettings.instance.phenotype.frictionUnderNormal2CellsLeaf * GlobalSettings.instance.phenotype.frictionUnderSlidingFactor : 0f;
		} else {
			theRigidBody.drag = PhenotypePhysicsPanel.instance.frictionWater.isOn ? GlobalSettings.instance.phenotype.frictionUnderNormal1CellLeaf * GlobalSettings.instance.phenotype.frictionUnderSlidingFactor : 0f;
		}
	}
	// ^ Friction (Drag) ^

	public override float Transparency() {
		return GlobalSettings.instance.phenotype.leafCell.transparency;
	}

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		base.UpdateCellWork(deltaTicks, worldTicks);

		if (PhenotypePhysicsPanel.instance.functionLeaf.isOn) {
			effectProductionInternalDown = GlobalSettings.instance.phenotype.leafCell.effectProductionDown;
			bool debugRender = PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.leafExposure && CreatureSelectionPanel.instance.selectedCell == this;

			float startEnergy = 2f; //matters only graphically during debug
			float maxRange = GlobalSettings.instance.phenotype.leafCell.sunRayMaxRange; // how many meters an unblocked light beam will travel
			float energyLossAir = startEnergy / maxRange; //J / m (allways reaches max range if nothing blocks it)

			Vector2 direction = GeometryUtil.GetVector(Random.Range(0f, 360f), 1f);

			Vector2 start = position + direction * (radius - 0.1f); // we have to start inside ourselves to be able to catch touching part of neighbours
			Vector2 perpendicular = new Vector2(-direction.y, direction.x); //debug shit

			float rayEnergy = startEnergy; //100% when leaving
			float rayRange = maxRange;

			//RaycastHit2D[] hits = Physics2D.RaycastAll(start, direction, maxRange, 1);
			int layerMask = 1 << 0; // default
			layerMask |= 1 << 8; // wall shade (a fake creature that steals light)
			// we listens to both layers so that we have some fake competition at the edges of the world, where the shades are

				
			int raycastHitCount = Physics2D.RaycastNonAlloc(start, direction, raycastHitArray, maxRange, layerMask);
			//Store all entries --> exits
			List<HitPoint> enterExit = new List<HitPoint>();

			float transparentTravelDistance = 0f;

			for (int index = 0; index < raycastHitCount; index++) {
				RaycastHit2D hit = raycastHitArray[index];
				RaycastHit2D previousHit = raycastHitArray[Mathf.Max(index - 1, 0)];

				if (index == 0) {
					//first hitpoint
					if (hit.distance > 0.5f) {
						//There is a gap from sending leaf to first body
						enterExit.Add(new HitPoint(HitType.beginAir, 0f));
						enterExit.Add(new HitPoint(HitType.beginBody, hit.distance));

						rayEnergy -= hit.distance * energyLossAir;

						if (debugRender) {
							DrawEnergyLine(Color.green, start, direction, 0f, hit.distance, startEnergy, rayEnergy);
						}
					} else {
						//The first body is laying tight to 
						enterExit.Add(new HitPoint(HitType.beginBody, 0f));
					}
				} else {
					//second hit point or more
					float distanceFromLastHit = hit.distance - previousHit.distance;
					if (distanceFromLastHit > 1.2f) {
						//here is a gap from the body that was last hit to this one
						float beginAirDistance = Mathf.Min(previousHit.distance + 1f, hit.distance - 0.1f);
						enterExit.Add(new HitPoint(HitType.beginAir, beginAirDistance));
						enterExit.Add(new HitPoint(HitType.beginBody, hit.distance));
						float energyBefore = rayEnergy;
						rayEnergy -= GetEnergyLoss(previousHit, energyLossAir);//energyLossOtherBody;
																				//if (IsTransparentCell(previousHit)) {
																				//	transparentTravelDistance += 1f;
																				//}
						transparentTravelDistance += GetTransparencyOfHit(previousHit);

						if (debugRender) {
							DrawEnergyLine(GetCollisionColor(previousHit), start, direction, previousHit.distance, previousHit.distance + 1f, energyBefore, rayEnergy);
						}
						if (rayEnergy < 0f) {
							rayRange = GetDistanceAtZeroEnergy(energyBefore, rayEnergy, previousHit.distance, previousHit.distance + 1f);
							break;
						}
						energyBefore = rayEnergy;
						rayEnergy -= (hit.distance - previousHit.distance - 1f) * energyLossAir;
						if (debugRender) {
							DrawEnergyLine(Color.green, start, direction, previousHit.distance + 1, hit.distance, energyBefore, rayEnergy);
						}
						if (rayEnergy < 0f) {
							rayRange = GetDistanceAtZeroEnergy(energyBefore, rayEnergy, previousHit.distance + 1, hit.distance);
							break;
						}

					} else {
						//the body that was hit this time is tight togeter with the previous one
						enterExit.Add(new HitPoint(HitType.insideBody, hit.distance));
						float energyBefore = rayEnergy;
						rayEnergy -= GetEnergyLoss(previousHit, energyLossAir);//energyLossOtherBody;
																				//if (IsTransparentCell(previousHit)) {
																				//	transparentTravelDistance += 1f;
																				//}
						transparentTravelDistance += GetTransparencyOfHit(previousHit);
						if (debugRender) {
							DrawEnergyLine(GetCollisionColor(previousHit), start, direction, previousHit.distance, hit.distance, energyBefore, rayEnergy);
						}
						if (rayEnergy < 0f) {
							rayRange = GetDistanceAtZeroEnergy(energyBefore, rayEnergy, previousHit.distance, hit.distance);
							break;
						}
					}
				}
			}

			if (raycastHitCount > 0 && rayEnergy >= 0f) {
				//Last hit: cell, then air after it
				RaycastHit2D hit = raycastHitArray[raycastHitCount - 1];

				enterExit.Add(new HitPoint(HitType.beginAir, enterExit[enterExit.Count - 1].distance + 1f));

				float energyBefore = rayEnergy;
				rayEnergy -= GetEnergyLoss(hit, energyLossAir);//energyLossOtherBody;
																//if (IsTransparentCell(hit)) {
																//	transparentTravelDistance += 1f;
																//}
				transparentTravelDistance += GetTransparencyOfHit(hit);
				if (debugRender) {
					DrawEnergyLine(GetCollisionColor(hit), start, direction, hit.distance, hit.distance + 1f, energyBefore, rayEnergy);
				}
				if (rayEnergy < 0f) {
					rayRange = GetDistanceAtZeroEnergy(energyBefore, rayEnergy, hit.distance, hit.distance + 1f);
				} else {
					energyBefore = rayEnergy;
					rayEnergy -= (maxRange - hit.distance - 1f) * energyLossAir;
					if (debugRender) {
						DrawEnergyLine(Color.green, start, direction, hit.distance + 1, maxRange, energyBefore, rayEnergy);
					}
					if (rayEnergy < 0f) {
						rayRange = GetDistanceAtZeroEnergy(energyBefore, rayEnergy, hit.distance + 1, maxRange);
					}
				}
			}

			if (debugRender) {
				float stayTime = GlobalSettings.instance.quality.leafCellTickPeriod * 0.05f;
				Debug.DrawLine(start, start + direction * maxRange, Color.black, stayTime);
				Debug.DrawLine(start, start + direction * rayRange, Color.white, stayTime);

				Debug.DrawLine(start + direction * (rayRange - transparentTravelDistance), start + direction * rayRange, Color.magenta, stayTime); //ray length minus length traveled on transparent cells

				//enter / exit lines
				for (int index = 0; index < enterExit.Count; index++) {
					HitPoint hit = enterExit[index];
					Color color = Color.black;

					if (hit.hitType == HitType.beginAir) {
						color = Color.cyan;
					} else if (hit.hitType == HitType.beginBody) {
						color = Color.red;
					} else if (hit.hitType == HitType.insideBody) {
						color = Color.yellow;
					}
					Vector2 begin = start + direction * hit.distance;

					Debug.DrawLine(begin - perpendicular * 0.8f, begin + perpendicular * 0.8f, color, 5f);
				}
			}
			// ^ enter / exit lines ^

			// bring exposure yield to the range [0 ... 1]
			// Keep the system if we decide to use it again
			float beamExposureNormalized = (rayRange - transparentTravelDistance) / maxRange;

			// make possible to tweak exposure yield at  
			float beamExposureNormalizedBalanced = GlobalSettings.instance.phenotype.leafCell.exposureFactorAtRayLengthNormalized.Evaluate(beamExposureNormalized);

			// if it is calm exposure goes towards the 'true' exposure value. If it is windy (cell is moving) we go towards the exposure which gives a net production of a little bit punished below zero
			// Beware a small penalty takes us a long way when it comes to leaf death
			float beamExposureNormalizedBalancedPunished = Mathf.Lerp(exposureAtProductionEffectZero - GlobalSettings.instance.phenotype.leafCell.exposurePenalty, beamExposureNormalizedBalanced, absoluteEffectCalmnessFactor);

			float beamExposureNormalizedBalancedPunishedSunyness = beamExposureNormalizedBalancedPunished * 1f; //0.8 seems fine for dark area

			// TODO: optimize how we go through and sum up exposure

			// sum -= old record (the one at exposureRecorCursor), last time we read this data
			exposureRecordSum -= exposureRecord[exposureRecorCursor];

			// write new record
			exposureRecord[exposureRecorCursor] = beamExposureNormalizedBalancedPunishedSunyness;

			// sum += new record
			exposureRecordSum += beamExposureNormalizedBalancedPunishedSunyness;

			// move and wrap cursor
			exposureRecorCursor++;
			if (exposureRecorCursor >= exposureRecordCapacity) {
				// start over
				exposureRecorCursor = 0;
			}

			// The average is boxed (as in not weighted)
			m_lowPassExposure = exposureRecordSum / exposureRecordCapacity;

						//m_lowPassExposure = 0f;
						//for (int i = 0; i < exposureRecordMaxCapacity; i++) {
						//	m_lowPassExposure += exposureRecord[i];
						//}
						//m_lowPassExposure /= exposureRecordMaxCapacity;

			// balance low pass exposure
			m_lowPassExposure *= GlobalSettings.instance.phenotype.leafCell.exposureFactorAtPopulation.Evaluate(World.instance.life.cellAliveCount) * GlobalSettings.instance.phenotype.leafCell.exposureFactorAtBodySize.Evaluate(creature.cellCount);

			effectProductionInternalUp = m_lowPassExposure * GlobalSettings.instance.phenotype.leafCell.effectProductionUpMax;

			if (CellPanel.instance.selectedCell == this) {
				CellPanel.instance.cellAndGenePanel.workPanel.leafPanel.MakeDirty();
			}
		} else {
			effectProductionInternalUp = 0f;
			effectProductionInternalDown = 0f;
			m_lowPassExposure = exposureAtProductionEffectZero;
		}
	}

	private float GetEnergyLoss(RaycastHit2D hit, float energyLossAir) {
		CollisionType type = GetCollisionType(hit);

		if (type == CollisionType.cell) {
			float transparencyAtHit = GetTransparencyOfHit(hit);
			return Mathf.Lerp(energyLossAir * GlobalSettings.instance.phenotype.leafCell.sunRayEffectLossPerDistanceThroughCell, energyLossAir, transparencyAtHit);
		} else {
			//Wall
			return energyLossAir * 1000f; //J / m; 
		}
	}

	//private bool IsTransparentCell(RaycastHit2D hit) {
	//	RaycastUtil.CollisionType type = RaycastUtil.GetCollisionType(hit, creature);

	//	if (type == RaycastUtil.CollisionType.ownCell) {
	//		return (GetCollisionCellType(hit) == CellTypeEnum.Shell || GetCollisionCellType(hit) == CellTypeEnum.Fungal);
	//	} else if (type == RaycastUtil.CollisionType.othersCell) {
	//		return (GetCollisionCellType(hit) == CellTypeEnum.Shell || GetCollisionCellType(hit) == CellTypeEnum.Fungal);
	//	} else {
	//		return false;
	//	}
	//}

	public float GetTransparencyOfHit(RaycastHit2D hit) {
		//return 0; //Everything has same transpareance when it comes to my cells, //Everything has same transpareance when it comes to opponent cells
		Cell hitCell = hit.collider.gameObject.GetComponent<Cell>();
		if (hitCell != null) {
			return hitCell.Transparency();
		}
		return 0f;
	}

	//Opt. this array should contain enoug fields to store all hits
	private RaycastHit2D[] raycastHitArray;


	//energy far is allways negative
	private float GetDistanceAtZeroEnergy(float energyClose, float energyFar, float distanceClose, float distanceFar) {
		return distanceClose + ((distanceFar - distanceClose) * energyClose) / (energyClose - energyFar);
	}
	private void DrawEnergyLine(Color color, Vector2 lightStart, Vector2 lightDirection, float distanceClose, float distanceFar, float energyClose, float energyFar) {
		Vector2 perpendicular = new Vector2(-lightDirection.y, lightDirection.x);
		Vector2 closeOnRay = lightStart + lightDirection * distanceClose;
		Vector2 farOnRay = lightStart + lightDirection * distanceFar;

		Debug.DrawLine(closeOnRay + perpendicular * energyClose, farOnRay + perpendicular * energyFar, color, 5f);
		Debug.DrawLine(closeOnRay - perpendicular * energyClose, farOnRay - perpendicular * energyFar, color, 5f);
	}

	private CellTypeEnum GetCollisionCellType(RaycastHit2D hit) {
		if (hit.collider.gameObject.GetComponent<Cell>() != null) {
			return hit.collider.gameObject.GetComponent<Cell>().GetCellType();
		}
		return CellTypeEnum.Error;
	}

	private Color GetColorForCollisionType(CollisionType type) {
		if (type == CollisionType.cell) {
			return Color.yellow;
		} else {
			//Other obstacle
			return Color.gray;
		}
	}

	private Color GetCollisionColor(RaycastHit2D hit) {
		return GetColorForCollisionType(GetCollisionType(hit));
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Leaf;
	}

	//public override void SetNormalDrag() {
	//	theRigidBody.drag = GlobalSettings.instance.phenotype.normalLeafDrag;
	//}

	public override Color GetColor(PhenoGenoEnum phenoGeno) {
		if (phenoGeno == PhenoGenoEnum.Genotype) {
			return ColorScheme.instance.ToColor(GetCellType());
		} else {
			return ColorScheme.instance.cellGradientLeafGreenExposure.Evaluate(lowPassExposure);
		}
	}

	private enum CollisionType {
		cell,
		nonCellObstacle,
		undefined,
	}

	private static CollisionType GetCollisionType(RaycastHit2D hit) {
		Cell hitCell = hit.collider.gameObject.GetComponent<Cell>();
		if (hitCell != null) {
			return CollisionType.cell;

		}
		return CollisionType.nonCellObstacle; // could be terrain (in layer 1) or shade (in layer 8)
	}
}