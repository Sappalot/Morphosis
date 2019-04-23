using UnityEngine;

static class CameraUtils {

	public static float GetEffectScaleLazy() {
		//Debug.Log(World.instance.worldCamera.orthographicSize);
		return Mathf.Lerp(2f, 6f, Mathf.InverseLerp(10f, 50f, Morphosis.instance.camera.orthographicSize));
	}

	public static bool IsObservedLazy(Vector3 position, float orthoMaxWidth) {
		return IsInsideFrustum(Morphosis.instance.camera, position) && Morphosis.instance.camera.orthographicSize < orthoMaxWidth;
	}

	public static bool IsInsideFrustum(Camera camera, Vector3 position) {
		Vector3 viewportPosition = Morphosis.instance.camera.WorldToViewportPoint(position);
		return viewportPosition.x > 0f && viewportPosition.x < 1f && viewportPosition.y > 0f && viewportPosition.y < 1f;
	}

	public static bool IsCloseEnoughLazy(float orthoMaxWidth) {
		return Morphosis.instance.camera.orthographicSize < orthoMaxWidth;
	}

	public static float GetEffectStrengthLazy() {
		return 1f;
		//return GetEffectStrength(Morphosis.instance.camera, GlobalSettings.instance.orthoMinStrongFX, GlobalSettings.instance.orthoMaxHorizonFx);
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

	public static bool ShouldPlayFx(Vector2 position) {
		return IsInsideFrustum(Morphosis.instance.camera, position) && Morphosis.instance.camera.orthographicSize < GlobalSettings.instance.orthoPlayFxLimit;
	}

}