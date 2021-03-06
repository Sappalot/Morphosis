﻿using UnityEngine;

static class SpatialUtil {
	public static float MarkerScale() {
		return Mathf.Lerp(2f, 6f, Mathf.InverseLerp(10f, 50f, Morphosis.instance.cameraVirtual.orthographicSize));
	}

	public static void FxGrade(Vector2 position, bool isLoud, out bool hasAudio, out float audioVolume) {
		hasAudio = false;
		audioVolume = 0f;

		if (GlobalPanel.instance.soundCreatures.isOn && (isLoud ? IsInsideLoudAudioVolume(position) : IsInsideQuietAudioVolume(position))) {
			hasAudio = true;
			audioVolume = AudioVolume(isLoud);
		}
	}

	public static void FxGrade(Vector2 position, bool isLoud, out bool hasAudio, out float audioVolume, out bool hasParticles) {
		hasAudio = false;
		audioVolume = 0f;
		hasParticles = false;

		bool insideFxVolume = IsInsideFrustum(position);
		if (!insideFxVolume) {
			return;
		}

		if (GlobalPanel.instance.soundCreatures.isOn && (isLoud ? IsInsideLoudAudioVolume(position) : IsInsideQuietAudioVolume(position))) {
			hasAudio = true;
			audioVolume = AudioVolume(isLoud);
		}

		if (GlobalPanel.instance.graphicsEffectsToggle.isOn && IsDetailedGraphicsDistance() && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			hasParticles = true;
		}
	}

	public static void FxGrade(Vector2 position, bool isLoud, out bool hasAudio, out float audioVolume, out bool hasParticles, out bool hasMarker) {
		hasAudio = false;
		audioVolume = 0f;
		hasParticles = false;
		hasMarker = false;

		if (!IsInsideFrustum(position)) {
			return;
		}

		if (GlobalPanel.instance.soundCreatures.isOn && (isLoud ? IsInsideLoudAudioVolume(position) : IsInsideQuietAudioVolume(position))) {
			hasAudio = true;
			audioVolume = AudioVolume(isLoud);
		}

		if (GlobalPanel.instance.graphicsEffectsToggle.isOn) {
			if (IsDetailedGraphicsDistance() && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
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

	private static float AudioVolume(bool isLoud) {
		if (isLoud) {
			return GlobalSettings.instance.loudAudioVolumeAtOrtho.Evaluate(Morphosis.instance.cameraVirtual.orthographicSize);
		} else {
			return GlobalSettings.instance.quietAudioVolumeAtOrtho.Evaluate(Morphosis.instance.cameraVirtual.orthographicSize);
		}
	}

	public static bool IsInsideFrustum(Vector2 position) {
		Vector3 viewportPosition = Morphosis.instance.cameraVirtual.WorldToViewportPoint(position);
		return viewportPosition.x > 0 && viewportPosition.x < 1f && viewportPosition.y > 0f && viewportPosition.y < 1f;
	}

	public static bool IsDetailedGraphicsDistance() {
		return Morphosis.instance.cameraVirtual.orthographicSize < GlobalSettings.instance.detailedGraphicsOrthoLimit;
	}

	private static bool IsLoudAudioDistance() {
		return Morphosis.instance.cameraVirtual.orthographicSize < GlobalSettings.instance.loudAudioVolumeAtOrtho[GlobalSettings.instance.loudAudioVolumeAtOrtho.length - 1].time;
	}
}