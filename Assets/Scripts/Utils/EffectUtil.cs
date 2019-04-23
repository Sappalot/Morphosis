using System.Collections.Generic;

// Audio, marker and particles
static class EffectsUtils {

	public static void SpawnAddCreatures(List<Creature> creatures) {

		foreach (Creature c in creatures) {
			EffectPlayer.instance.Play(EffectEnum.CreatureAdd, c.GetOriginPosition(CreatureEditModePanel.instance.mode), 0f, CameraUtils.GetEffectScaleLazy());
		}

		Audio.instance.CreatureAdd(1f);

	}

	public static void SpawnAddCreature(Creature creature) {
		EffectPlayer.instance.Play(EffectEnum.CreatureAdd, creature.GetOriginPosition(CreatureEditModePanel.instance.mode), 0f, CameraUtils.GetEffectScaleLazy());
		Audio.instance.CreatureAdd(1f);
	}
}
