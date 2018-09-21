using System.Collections.Generic;
using UnityEngine;

public class LeafCell : Cell {
	//public LineRenderer[] testRays = new LineRenderer[6];

	private const int exposureRecordMaxCapacity = 10;
	private float[] exposureRecord = new float[exposureRecordMaxCapacity];
	private int exposureRecorCursor = 0;
	private int exposureRecordCount = 0;

	private float m_lowPassExposure = 0.33f;
	public float lowPassExposure {
		get {
			return m_lowPassExposure;
		}
	}

	public void Awake() {
		OnBorrowToWorld();
		base.Init();
	}

	public override void OnBorrowToWorld() {
		if (raycastHitArray == null) {
			raycastHitArray = new RaycastHit2D[(int)GlobalSettings.instance.phenotype.leafCellSunMaxRange];
		}
		for (int i = 0; i < exposureRecord.Length; i++) {
			exposureRecord[i] = 0.6f; // 0.33f boost
		}
		base.OnBorrowToWorld();
	}

	private float GetEnergyLoss(RaycastHit2D hit, float energyLossAir) {
		CollisionType type = GetCollisionType(hit);

		int attachedMotherCellCount = 0;
		if (creature.IsAttachedToMotherAlive()) {
			attachedMotherCellCount = creature.GetMotherAlive().cellCount;
		}

		if (type == CollisionType.ownCell) {
			if (GetCollisionCellType(hit) == CellTypeEnum.Shell) {
				return 0f; // hack with transparent shells
			}
			return energyLossAir * GlobalSettings.instance.phenotype.leafCellSunLossFactorOwnCell.Evaluate(creature.cellCount); //J / m // creature.clusterCellCount
		} else if (type == CollisionType.othersCell) {
			return energyLossAir * GlobalSettings.instance.phenotype.leafCellSunLossFactorOtherCell.Evaluate(creature.cellCount); //J / m //creature.clusterCellCount
		} else {
			//Wall
			return energyLossAir * 1000f; //J / m; 
		}
	}

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (PhenotypePhysicsPanel.instance.functionLeaf.isOn) {
			effectProductionInternalDown = GlobalSettings.instance.phenotype.leafCellEffectCost;
			bool debugRender = PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.leafExposure && CreatureSelectionPanel.instance.selectedCell == this;

			float startEnergy = 2f; //matters only graphically during debug
			float maxRange = GlobalSettings.instance.phenotype.leafCellSunMaxRange; // how many meters an unblocked light beam will travel
			float energyLossAir = startEnergy / maxRange; //J / m (allways reaches max range if nothing blocks it)

			Vector2 direction = GeometryUtils.GetVector(Random.Range(0f, 360f), 1f);

			Vector2 start = position + direction * (radius + 0.1f);
			//Vector2 perpendicular = new Vector2(-direction.y, direction.x); //debug shit

			float rayEnergy = startEnergy; //100% when leaving
			float rayRange = maxRange;

			//RaycastHit2D[] hits = Physics2D.RaycastAll(start, direction, maxRange, 1);
			int raycastHitCount = Physics2D.RaycastNonAlloc(start, direction, raycastHitArray, maxRange, 1);
			//Store all entries --> exits
			//List<HitPoint> enterExit = new List<HitPoint>();

			for (int index = 0; index < raycastHitCount; index++) {
				RaycastHit2D hit = raycastHitArray[index];
				RaycastHit2D previousHit = raycastHitArray[Mathf.Max(index - 1, 0)];

				if (index == 0) {
					//first hitpoint
					if (hit.distance > 0.5f) {
						//There is a gap from sending leaf to first body
						//enterExit.Add(new HitPoint(HitType.beginAir, 0f));
						//enterExit.Add(new HitPoint(HitType.beginBody, hit.distance));

						rayEnergy -= hit.distance * energyLossAir;

						if (debugRender) {
							DrawEnergyLine(Color.green, start, direction, 0f, hit.distance, startEnergy, rayEnergy);
						}
					} else {
						//The first body is laying tight to 
						//enterExit.Add(new HitPoint(HitType.beginBody, 0));
					}
				} else {
					//second hit point or more
					float distanceFromLastHit = hit.distance - previousHit.distance;
					if (distanceFromLastHit > 1.2f) {
						//here is a gap from the body that was last hit to this one
						//float beginAirDistance = Mathf.Min(previousHit.distance + 1f, hit.distance - 0.1f);
						//enterExit.Add(new HitPoint(HitType.beginAir, beginAirDistance));
						//enterExit.Add(new HitPoint(HitType.beginBody, hit.distance));
						float energyBefore = rayEnergy;
						rayEnergy -= 1f * GetEnergyLoss(previousHit, energyLossAir);//energyLossOtherBody;
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
						//enterExit.Add(new HitPoint(HitType.insideBody, hit.distance));
						float energyBefore = rayEnergy;
						rayEnergy -= 1f * GetEnergyLoss(previousHit, energyLossAir);//energyLossOtherBody;
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

				//enterExit.Add(new HitPoint(HitType.beginAir, enterExit[enterExit.Count - 1].distance + 1f));

				float energyBefore = rayEnergy;
				rayEnergy -= 1f * GetEnergyLoss(hit, energyLossAir);//energyLossOtherBody;
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
				float stayTime = GlobalSettings.instance.quality.leafCellTickPeriodAtSpeed.Evaluate(creature.phenotype.speed) * 0.1f * 20f;
				Debug.DrawLine(start, start + direction * maxRange, Color.black, stayTime);
				Debug.DrawLine(start, start + direction * rayRange, Color.white, stayTime);
			}

			//if (debugRender) {
			//	Debug.DrawLine(start, start + direction * maxRange, Color.black, 1f);
			//	Debug.DrawLine(start, start + direction * rayRange, Color.white, 1f);

			//	//enter / exit lines
			//	for (int index = 0; index < enterExit.Count; index++) {
			//		HitPoint hit = enterExit[index];
			//		Color color = Color.black;

			//		if (hit.hitType == HitType.beginAir) {
			//			color = Color.cyan;
			//		} else if (hit.hitType == HitType.beginBody) {
			//			color = Color.red;
			//		} else if (hit.hitType == HitType.insideBody) {
			//			color = Color.yellow;
			//		}
			//		Vector2 begin = start + direction * hit.distance;

			//		Debug.DrawLine(begin - perpendicular * 0.8f, begin + perpendicular * 0.8f, color, 1f);
			//	}
			//}
			// ^ enter / exit lines ^

			float effect = (rayRange / maxRange);

			exposureRecord[exposureRecorCursor] = effect;
			exposureRecorCursor++;
			if (exposureRecorCursor >= exposureRecordMaxCapacity) {
				exposureRecorCursor = 0;
			}
			exposureRecordCount = (int)Mathf.Min(exposureRecordMaxCapacity, exposureRecordCount + 1);

			m_lowPassExposure = 0f;
			for (int i = 0; i < exposureRecordCount; i++) {
				m_lowPassExposure += exposureRecord[i];
			}
			m_lowPassExposure /= exposureRecordCount;

			int attachedMotherCellCount = 0;
			if (creature.IsAttachedToMotherAlive()) {
				attachedMotherCellCount = creature.GetMotherAlive().cellCount;
			}
			effectProductionInternalUp = m_lowPassExposure * GlobalSettings.instance.phenotype.leafCellSunMaxEffect * GlobalSettings.instance.phenotype.leafCellSunEffectFactorAtBodySize.Evaluate(creature.cellCount); //costy!! creature.clusterCellCount

			if (CellPanel.instance.selectedCell == this) {
				CellPanel.instance.leafCellPanel.MakeDirty();
			}
			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			effectProductionInternalUp = 0f;
			effectProductionInternalDown = 0f;
			m_lowPassExposure = 0.33f;
		}
	}
	//Opt. this array should contain enoug fields to store all hits
	private RaycastHit2D[] raycastHitArray;


	//energy far is allways negative
	private float GetDistanceAtZeroEnergy(float energyClose, float energyFar, float distanceClose, float distanceFar) {
		return distanceClose + ((distanceFar - distanceClose) * energyClose) / (energyClose - energyFar);
	}
	private void DrawEnergyLine(Color color, Vector2 lightStart, Vector2 lightDirection, float distanceClose, float distanceFar, float energyClose, float energyFar) {
		return;
		Vector2 perpendicular = new Vector2(-lightDirection.y, lightDirection.x);
		Vector2 closeOnRay = lightStart + lightDirection * distanceClose;
		Vector2 farOnRay = lightStart + lightDirection * distanceFar;

		Debug.DrawLine(closeOnRay + perpendicular * energyClose, farOnRay + perpendicular * energyFar, color, 1f);
		Debug.DrawLine(closeOnRay - perpendicular * energyClose, farOnRay - perpendicular * energyFar, color, 1f);
	}

	private enum CollisionType {
		ownCell,
		othersCell,
		otherObstacle,
	}

	private CollisionType GetCollisionType(RaycastHit2D hit) {
		if (hit.collider.gameObject.GetComponent<Cell>() != null) {

			if (hit.collider.gameObject.GetComponent<Cell>().creature == creature) {
				return CollisionType.ownCell;
			} else {
				return CollisionType.othersCell;
			}
		}
		return CollisionType.otherObstacle;
	}

	private CellTypeEnum GetCollisionCellType(RaycastHit2D hit) {
		if (hit.collider.gameObject.GetComponent<Cell>() != null) {
			return hit.collider.gameObject.GetComponent<Cell>().GetCellType();
		}
		return CellTypeEnum.Error;
	}

	private Color GetColorForCollisionType(CollisionType type) {
		if (type == CollisionType.ownCell) {
			return Color.yellow;
		} else if (type == CollisionType.othersCell) {
			return Color.red;
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
}