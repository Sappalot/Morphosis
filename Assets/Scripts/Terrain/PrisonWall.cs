using System.Collections.Generic;
using UnityEngine;

public class PrisonWall : MonoSingleton<PrisonWall> {

	public GameObject legalArea;

	//TODO: Update uppon load

	public int runnersKilledCount;

	private Rect legalRect;
	public void Start() {
		legalRect = new Rect(legalArea.transform.position, legalArea.transform.localScale);
	}

	private int escapistCleanupTicks = 0;
	public void UpdatePhysics(List<Creature> creatures, ulong worldTicks) {
		escapistCleanupTicks++;
		if (escapistCleanupTicks >= GlobalSettings.instance.quality.escapistCleanupPeriod) {

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
		}
	}

	public void Restart() {
		runnersKilledCount = 0;
	}
}
