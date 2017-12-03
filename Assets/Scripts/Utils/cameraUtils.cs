using UnityEngine;

static class CameraUtils {

	public static bool IsObservedLazy(Vector3 position) {
		return IsInsideFrustum(World.instance.worldCamera, position) && World.instance.worldCamera.orthographicSize < GlobalSettings.instance.orthoMaxHorizonFx;
	}

	public static bool IsInsideFrustum(Camera camera, Vector3 position) {
		Vector3 viewportPosition = World.instance.worldCamera.WorldToViewportPoint(position);
		return viewportPosition.x > 0f && viewportPosition.x < 1f && viewportPosition.y > 0f && viewportPosition.y < 1f;
	}

	public static float GetEffectStrengthLazy() {
		return GetEffectStrength(World.instance.worldCamera, GlobalSettings.instance.orthoMinStrongFX, GlobalSettings.instance.orthoMaxHorizonFx);
	}

	public static float GetEffectStrength(Camera camera, float orthoMinStrongFX, float orthoMaxHorizonFx) {
		float volume = 0f;
		if (camera.orthographicSize > orthoMaxHorizonFx) {
			volume = 0f;
		} else if (camera.orthographicSize < orthoMinStrongFX) {
			volume = 1f;
		} else {
			volume = 1f - (camera.orthographicSize - orthoMinStrongFX) / (orthoMaxHorizonFx - orthoMinStrongFX);
		}
		return volume;
	}
}
