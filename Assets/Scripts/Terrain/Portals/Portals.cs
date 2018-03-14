using System.Collections.Generic;

public class Portals : MonoSingleton<Portals> {

	public Portal[] portals;

	private int teleportTicks = 0;
	public void UpdatePhysics(List<Creature> creatures, ulong worldTicks) {
		teleportTicks++;
		if (teleportTicks >= GlobalSettings.instance.quality.portalTeleportPeriod) {
			for(int portalIndex = 0; portalIndex < portals.Length; portalIndex++) {
				portals[portalIndex].UpdatePhysics(creatures, worldTicks);
			}
			teleportTicks = 0;
		}
	}
}
