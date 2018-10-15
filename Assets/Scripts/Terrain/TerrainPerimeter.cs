using System.Collections.Generic;
using UnityEngine;

public class TerrainPerimeter : MonoSingleton<TerrainPerimeter> {

	public GameObject legalArea;

	//TODO: Update uppon load

	public int runnersKilledCount;

	public bool IsInside(Vector2 position) {
		float top = legalRect.y + legalRect.height / 2f;
		float bottom = legalRect.y - legalRect.height / 2f;
		float left = legalRect.x - legalRect.width / 2f;
		float right = legalRect.x + legalRect.width / 2f;

		if (position.x > right || position.x < left || position.y > top || position.y < bottom) {
			return false;
		}
		return true;
	}

	public bool IsCompletelyInside(Creature creature) {
		if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			return creature.phenotype.IsCompletelyInside(legalRect);
		} else {
			return creature.genotype.IsCompletelyInside(legalRect);
		}
		
	}

	public bool KillIfOutside(Creature creature) {
		if (!IsCompletelyInside(creature)) {
			World.instance.life.KillCreatureSafe(creature, false);
			return true;
		}
		return false;
	}

	private Rect legalRect;
	public void Start() {
		legalRect = new Rect(legalArea.transform.position, legalArea.transform.localScale);
	}

	private int escapistCleanupTicks = 0;
	public void UpdatePhysics(List<Creature> creatures, ulong worldTicks) {
		escapistCleanupTicks++;
		if (escapistCleanupTicks >= GlobalSettings.instance.quality.escapistCleanupTickPeriod) {

			List<Creature> killList = new List<Creature>();
			foreach (Creature inmate in creatures) {
				if (!inmate.IsPhenotypePartlyInside(legalRect)) {
					killList.Add(inmate);
				}
			}
			foreach (Creature kill in killList) {
				World.instance.life.KillCreatureSafe(kill, true);
				runnersKilledCount++;
			}
			escapistCleanupTicks = 0;

			if (killList.Count > 0) {
				World.instance.AddHistoryEvent(new HistoryEvent("RK: " + killList.Count, false, Color.red));
			}
		}
	}

	public void Restart() {
		runnersKilledCount = 0;
	}
}
