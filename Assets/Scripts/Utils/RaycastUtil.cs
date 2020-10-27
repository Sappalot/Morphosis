using System.Collections.Generic;
using UnityEngine;

static class RaycastUtil {
	public enum CollisionType {
		ownCell,
		connectedViaClusterCell,
		othersCellDontCare,
		othersCell,
		otherObstacle,
		undefined,
	}

	public static CollisionType GetCollisionType(RaycastHit2D hit, Creature me, bool isEyeAsking = false) {
		if (hit.collider.gameObject.GetComponent<Cell>() != null) {
			Creature hitCreature = hit.collider.gameObject.GetComponent<Cell>().creature;
			if (hitCreature == me) {
				return CollisionType.ownCell;
			} else {
				List<Creature> creaturesInCluster = me.creaturesInCluster; // is this one expensive? in case it is cache it and update it when cluster might change
				foreach (Creature c in creaturesInCluster) {
					if (hitCreature == c) {
						return CollisionType.connectedViaClusterCell;
					}
				}
				if (isEyeAsking) {
					if (hit.collider.gameObject.GetComponent<Cell>() is JawCell || me.GetChildrenAlive().Contains(hit.collider.gameObject.GetComponent<Cell>().creature) || me.GetMotherAlive() == hit.collider.gameObject.GetComponent<Cell>().creature || me.GetMotherIdDeadOrAlive() == hit.collider.gameObject.GetComponent<Cell>().creature.GetMotherIdDeadOrAlive()) {
						return CollisionType.othersCellDontCare;
					}
				}
				return CollisionType.othersCell;
			}
		}
		return CollisionType.otherObstacle;
	}
}