using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {
	public GameObject departureArea;
	public GameObject arrivalArea;

	private Rect departureRect;
	private Rect arrivalRect;
	private Vector2 departureToArrival;


	public void Start() {
		departureRect = new Rect(departureArea.transform.position, departureArea.transform.localScale);
		arrivalRect = new Rect(arrivalArea.transform.position, arrivalArea.transform.localScale);
		departureToArrival = arrivalArea.transform.position - departureArea.transform.position;
	}

	public void TryTeleportCreature(List<Creature> creatures, ulong worldTicks) {
		List<Creature> canTeleport = new List<Creature>();
		List<Creature> canNotTeleport = new List<Creature>();

		List<Creature> shouldBeTelefragged = new List<Creature>();

		bool clusterCouldTravel;
		List<Creature> travelerCluster = new List<Creature>();
		foreach (Creature travelLeader in creatures) {
			if (canTeleport.Contains(travelLeader) || canNotTeleport.Contains(travelLeader)) {
				continue;
			}

			if (travelLeader.IsPhenotypeCompletelyInside(departureRect)) {
				travelerCluster = travelLeader.creaturesInCluster;
				clusterCouldTravel = true; //hopefully...

				foreach (Creature companion in travelerCluster) {
					Cell blockingCell = GetAnyBlockingCell(creatures, travelLeader, departureToArrival);
					if ((companion != travelLeader && !companion.IsPhenotypeCompletelyInside(departureRect)) ||
						blockingCell != null ||
						companion.phenotype.isGrabbed ||
						companion.phenotype.connectionsDiffersFromCells) { // FOUND IT :D We need to be sure that everything is connected otherwise we might teleport away from our attached child (???)

						if (blockingCell != null) {
							shouldBeTelefragged.Add(blockingCell.creature);
						}

						clusterCouldTravel = false;
						break;
					} 
				}

				if (clusterCouldTravel) {
					foreach (Creature sucessful in travelerCluster) {
						if (!canTeleport.Contains(sucessful)) {
							canTeleport.Add(sucessful);
						}
					}
					//break; //One cluser is enough to teleport at a time
				} else {
					foreach (Creature failure in travelerCluster) {
						if (!canNotTeleport.Contains(failure)) {
							canNotTeleport.Add(failure);
						}
					}
				}
			}
		}

		//// Telefrag obstructing creatures
		foreach (Creature fragMe in shouldBeTelefragged) {
			fragMe.ChangeEnergy(-GlobalSettings.instance.quality.portalTeleportPeriod * GlobalSettings.instance.phenotype.telefragDamage);
		}

		if (canTeleport.Count > 0) {
			//Move (teleport) creatures
			//foreach (Creature traveler in canTeleport) {
			//	traveler.phenotype.EnableCollider(false);
			//	traveler.phenotype.SetKinematic(true);
			//}

			foreach (Creature traveler in canTeleport) {
				traveler.phenotype.Move(departureToArrival);
			}

			//foreach (Creature traveler in canTeleport) {
			//	traveler.phenotype.EnableCollider(true);
			//	traveler.phenotype.SetKinematic(false);
			//}
		}
	}

	private Cell GetAnyBlockingCell(List<Creature> creatures, Creature traveler, Vector2 departureToArrival) {
		foreach (Creature blocker in creatures) {
			if (blocker == traveler) {
				continue;
			}
			if (blocker.IsPhenotypePartlyInside(arrivalRect)) {
				//Debug.Log("Blocker: " + blocker.id + " found");
				// Oooops Ordo N^2
				foreach (Cell travelerCell in traveler.phenotype.cellList) {
					Vector2 travelerCellArrivalPosition = travelerCell.position + departureToArrival;
					foreach (Cell blockerCell in blocker.phenotype.cellList) {
						if (Vector2.Distance(blockerCell.position, travelerCellArrivalPosition) < 1f) {
							return blockerCell;
						}
					}
				}
			}
		}
		return null;
	}
}
