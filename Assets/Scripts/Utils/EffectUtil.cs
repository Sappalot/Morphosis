using System.Collections.Generic;

static class EffectsUtils {

	public static void SpawnAddCreature(List<Creature> creatures, bool visual, bool tryAudio) {
		if (visual) {
			foreach (Creature c in creatures) {
				EffectPlayer.instance.Play(EffectEnum.CreatureAdd, c.GetOriginPosition(CreatureEditModePanel.instance.mode), 0f, CameraUtils.GetEffectScaleLazy());
			}
		}

		if (tryAudio && GlobalPanel.instance.soundCreatures.isOn) {
			Audio.instance.PlaceCreature(CameraUtils.GetEffectStrengthLazy());
		}
	}
}
