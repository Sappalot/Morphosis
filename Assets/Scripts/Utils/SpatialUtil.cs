using UnityEngine;

static class SpatialUtil {

	public static float GetMarkerScale() {
		return Mathf.Lerp(2f, 6f, Mathf.InverseLerp(10f, 50f, Morphosis.instance.camera.orthographicSize));
	}

	public static void GetFxGrade(Vector2 position, bool isLoud, out bool hasAudio, out float audioVolume) {
		hasAudio = false;
		audioVolume = 0f;

		if (GlobalPanel.instance.soundCreatures.isOn && (isLoud ? IsInsideLoudAudioVolume(position) : IsInsideQuietAudioVolume(position))) {
			hasAudio = true;
			audioVolume = GetAudioVolume(isLoud);
		}
	}

	public static void GetFxGrade(Vector2 position, bool isLoud, out bool hasAudio, out float audioVolume, out bool hasParticles) {
		hasAudio = false;
		audioVolume = 0f;
		hasParticles = false;

		bool insideFxVolume = IsInsideFrustum(position);
		if (!insideFxVolume) {
			return;
		}

		if (GlobalPanel.instance.soundCreatures.isOn && (isLoud ? IsInsideLoudAudioVolume(position) : IsInsideQuietAudioVolume(position))) {
			hasAudio = true;
			audioVolume = GetAudioVolume(isLoud);
		}

		if (GlobalPanel.instance.graphicsEffectsToggle.isOn && IsDetailedGraphicsDistance() && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && GlobalPanel.instance.isRunPhysics) {
			hasParticles = true;
		}
	}

	public static void GetFxGrade(Vector2 position, bool isLoud, out bool hasAudio, out float audioVolume, out bool hasParticles, out bool hasMarker) {
		hasAudio = false;
		audioVolume = 0f;
		hasParticles = false;
		hasMarker = false;

		if (!IsInsideFrustum(position)) {
			return;
		}


		if (GlobalPanel.instance.soundCreatures.isOn && (isLoud ? IsInsideLoudAudioVolume(position) : IsInsideQuietAudioVolume(position))) {
			hasAudio = true;
			audioVolume = GetAudioVolume(isLoud);
		}

		if (GlobalPanel.instance.graphicsEffectsToggle.isOn) {
			if (IsDetailedGraphicsDistance() && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && GlobalPanel.instance.isRunPhysics) {
				hasParticles = true;
			}
			hasMarker = true;
		}
	}

	public static bool IsInsideDetailedGraphicsVolume(Vector2 position) {
		return IsInsideFrustum(position) && IsDetailedGraphicsDistance();
	}

	private static bool IsInsideLoudAudioVolume(Vector2 position) {
		return IsInsideFrustum(position) && IsLoudAudioDistance();
	}

	private static bool IsInsideQuietAudioVolume(Vector2 position) {
		return IsInsideFrustum(position) && IsLoudAudioDistance();
	}

	private static float GetAudioVolume(bool isLoud) {
		if (isLoud) {
			return GlobalSettings.instance.loudAudioVolumeAtOrtho.Evaluate(Morphosis.instance.camera.orthographicSize);
		} else {
			return GlobalSettings.instance.quietAudioVolumeAtOrtho.Evaluate(Morphosis.instance.camera.orthographicSize);
		}
	}

	public static bool IsInsideFrustum(Vector2 position) {
		Vector3 viewportPosition = Morphosis.instance.camera.WorldToViewportPoint(position);
		return viewportPosition.x > 0 && viewportPosition.x < 1f && viewportPosition.y > 0f && viewportPosition.y < 1f;
	}

	public static bool IsDetailedGraphicsDistance() {
		return Morphosis.instance.camera.orthographicSize < GlobalSettings.instance.detailedGraphicsOrthoLimit;
	}

	private static bool IsLoudAudioDistance() {
		return Morphosis.instance.camera.orthographicSize < GlobalSettings.instance.loudAudioVolumeAtOrtho[GlobalSettings.instance.loudAudioVolumeAtOrtho.length - 1].time;
	}

	private static bool IsQuietAudioDistance() {
		return Morphosis.instance.camera.orthographicSize < GlobalSettings.instance.loudAudioVolumeAtOrtho[GlobalSettings.instance.loudAudioVolumeAtOrtho.length - 1].time;
	}
}