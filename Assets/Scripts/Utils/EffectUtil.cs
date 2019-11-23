using System.Collections.Generic;

// Audio, marker and particles
static class EffectsUtils {

	public static void SpawnAddCreatures(List<Creature> creatures) {
		foreach (Creature c in creatures) {
			EventSymbolPlayer.instance.Play(EventSymbolEnum.CreatureAdd, c.GetOriginPosition(CreatureEditModePanel.instance.mode), 0f, SpatialUtil.MarkerScale());
		}
		Audio.instance.CreatureAdd(1f);
	}

	public static void SpawnAddCreature(Creature creature) {
		EventSymbolPlayer.instance.Play(EventSymbolEnum.CreatureAdd, creature.GetOriginPosition(CreatureEditModePanel.instance.mode), 0f, SpatialUtil.MarkerScale());
		Audio.instance.CreatureAdd(1f);
	}
}
