using System.Collections.Generic;
using UnityEngine;

public class TerrainPerimeter : MonoBehaviour {

	public Terrain terrain;
	public GameObject legalArea; // just for visuals

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

	public Vector2i liveZoneSize {
		set {
			Vector2 position = new Vector2((float)value.x / 2f, (float)-value.y / 2f);
			Vector2 scale = new Vector2(value.x, value.y);

			legalRect = new Rect(position, scale);
			legalArea.transform.position = position;
			legalArea.transform.localScale = scale;
		}
	}

	private Rect legalRect;


	//public void Start() {
	//	legalRect = new Rect(legalArea.transform.position, legalArea.transform.localScale);
	//}

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
				World.instance.life.KillCreatureByEscaping(kill, true);
			}
			escapistCleanupTicks = 0;
		}
	}
}
