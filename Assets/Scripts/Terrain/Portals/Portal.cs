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

	public void UpdatePhysics(List<Creature> creatures, ulong worldTicks) {
		List<Creature> canTeleport = new List<Creature>();
		List<Creature> canNotTeleport = new List<Creature>();
		bool clusterCouldTravel;
		foreach (Creature travelLeader in creatures) {
			if (canTeleport.Contains(travelLeader) || canNotTeleport.Contains(travelLeader)) {
				continue;
			}

			if (travelLeader.IsPhenotypeCompletelyInside(departureRect)) {
				List<Creature> travelerCluster = travelLeader.creaturesInCluster;
				clusterCouldTravel = true; //hopefully...
				foreach (Creature companion in travelerCluster) {
					if ((companion != travelLeader && !companion.IsPhenotypeCompletelyInside(departureRect)) ||
						!CanTeleportTo(creatures, travelLeader, departureToArrival) ||
						travelLeader.phenotype.isGrabbed) {

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
				} else {
					foreach (Creature failure in travelerCluster) {
						if (!canNotTeleport.Contains(failure)) {
							canNotTeleport.Add(failure);
						}
					}
				}
				//CanTeleportTo(creatures, traveler, departureToArrival) &&
				//!traveler.phenotype.isGrabbed) {

				//traveler.phenotype.Move(departureToArrival);
				//Debug.Log("Creature: " + traveler.id + " teleported");
			}
		}

		//Move creatures
		foreach (Creature traveler in canTeleport) {
			traveler.phenotype.Move(departureToArrival);
		}
	}

	private bool CanTeleportTo(List<Creature> creatures, Creature traveler, Vector2 departureToArrival) {
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
						if (Vector2.SqrMagnitude(blockerCell.position - travelerCellArrivalPosition) < 1f) {
							return false;
						}
					}
				}
			}
		}
		return true;
	}
}
