using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {
	public GameObject departureArea;
	public GameObject arrivalArea;

	public Vector2 telepokeDirection;

	private Rect departureRect;
	private Rect arrivalRect;
	private Vector2 departureToArrival;

	public void UpdateFlights() {
		departureRect = new Rect(departureArea.transform.position, departureArea.transform.localScale);
		arrivalRect = new Rect(arrivalArea.transform.position, arrivalArea.transform.localScale);
		departureToArrival = arrivalArea.transform.position - departureArea.transform.position;
	}

	public void TryTeleportCreature(List<Creature> creatures, ulong worldTicks) {
		List<Creature> canTeleport = new List<Creature>();
		List<Creature> canNotTeleport = new List<Creature>();

		List<Creature> shouldBeTelepoked = new List<Creature>();

		bool clusterCouldTravel;
		List<Creature> travelerCluster = new List<Creature>();
		foreach (Creature travelLeader in creatures) {
			if (canTeleport.Contains(travelLeader) || canNotTeleport.Contains(travelLeader)) {
				continue;
			}

			if (travelLeader.IsPhenotypeCompletelyInside(departureRect)) {
				travelerCluster = travelLeader.creaturesInCluster;

				clusterCouldTravel = true; //assume we can travel

				//Check if whole cluser is inside & we are not grabbed & we have all connections in order
				foreach (Creature companion in travelerCluster) {

					if ((companion != travelLeader && !companion.IsPhenotypeCompletelyInside(departureRect)) ||
						companion.phenotype.isGrabbed ||
						companion.phenotype.isInterCellDirty) { // FOUND IT :D We need to be sure that everything is connected otherwise we might teleport away from our attached child (???)

						clusterCouldTravel = false;
						break;
					}
				}

				//We dont check for blockers if we were screwed in the previous test, since we can not go anyway
				//If we are ready to go with the whole cluser we will check for blockers on the other side
				//It is inportant not to punish someone on the other side only if that creature is the reason i can not go
				if (clusterCouldTravel) {
					foreach (Creature companion in travelerCluster) {
						Cell blockingCell = GetAnyBlockingCell(creatures, travelLeader, departureToArrival);
						if (blockingCell != null) { // FOUND IT :D We need to be sure that everything is connected otherwise we might teleport away from our attached child (???)

							if (blockingCell != null) {
								shouldBeTelepoked.Add(blockingCell.creature);
							}

							clusterCouldTravel = false;
							break;
						}
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

		//// Move obstructing creatures
		if (PhenotypePhysicsPanel.instance.telepoke.isOn) {
			foreach (Creature pokeMe in shouldBeTelepoked) {
				pokeMe.phenotype.Telepoke(pokeMe, telepokeDirection * GlobalSettings.instance.phenotype.telepokeImpulseStrength);
			}
		}

		if (canTeleport.Count > 0) {
			foreach (Creature traveler in canTeleport) {
				PlayTeleportEffect(traveler);
				traveler.phenotype.Move(departureToArrival);
				PlayTeleportEffect(traveler);
			}
		}
	}

	private void PlayTeleportEffect(Creature traveler) {
		bool hasAudio; float audioVolume; bool hasParticles;
		SpatialUtil.FxGrade(traveler.phenotype.originCell.position, false, out hasAudio, out audioVolume, out hasParticles);
		if (hasAudio) {
			Audio.instance.CreatureTeleport(audioVolume);
		}
		if (hasParticles ||
			(GlobalPanel.instance.graphicsEffectsToggle.isOn &&
			CreatureSelectionPanel.instance.hasSoloSelected && PhenotypePanel.instance.followToggle.isOn && traveler == CreatureSelectionPanel.instance.soloSelected &&
			SpatialUtil.IsDetailedGraphicsDistance() &&
			CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && GlobalPanel.instance.isRunPhysics)) {

			PlayTeleportParticles(traveler.phenotype);
		}
	}

	private void PlayTeleportParticles(Phenotype phenotype) {
		foreach (Cell cell in phenotype.cellList) {
			ParticlesCellTeleport teleport = ParticlePool.instance.Borrow(ParticleTypeEnum.cellTeleport) as ParticlesCellTeleport;
			teleport.transform.position = cell.position;
			teleport.Play(Color.white);
			teleport.transform.parent = Morphosis.instance.transform;
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
