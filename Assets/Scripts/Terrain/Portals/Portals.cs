using System.Collections.Generic;
using UnityEngine;

public class Portals : MonoBehaviour {

	public Portal[] portals;

	public void UpdatePortalFlights() {
		foreach(Portal p in portals) {
			p.UpdateFlights();
		}
	}

	private int teleportTicks = 0;
	public void UpdatePhysics(List<Creature> creatures, ulong worldTicks) {
		teleportTicks++;
		if (teleportTicks >= GlobalSettings.instance.quality.portalTeleportTickPeriod) {
			for(int portalIndex = 0; portalIndex < portals.Length; portalIndex++) {
				portals[portalIndex].TryTeleportCreature(creatures, worldTicks);
			}
			teleportTicks = 0;
		}
	}
}
