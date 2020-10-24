using UnityEngine;
using UnityEngine.UI;

// All cells knows this one in order to handle common stuff
// Made to be able to change all (types) at once via the CellCommon prefab
// stuff that need not be overridden
//[ExecuteInEditMode]
public class CellEyeBall : MonoBehaviour {


	public Transform leftShutter;
	public Transform rightShutter;
	public Transform backShutter;
	public Transform pupil;

	public float pupilPositionXSmall;
	public float pupilPositionXBig;
	public AnimationCurve pupilPositionDevelopment;

	public Vector2 pupilSizeSmall;
	public Vector2 pupilSizeBig;
	public AnimationCurve pupilSizeDevelopmentWidth;
	public AnimationCurve pupilSizeDevelopmentHeight;

	public float leftShutterAngelBig;

	public float rightShutterAngelBig;

	public float backShutterDepthSmall;
	public float backShutterDepthBig;

	public float graphicalMinFOV;

	//[Range(0f, 360F)]
	//public float testFOV = 90f;

	private float cachedFOV = -1f;

	public float fieldOfView {
		set {
			if (value == this.cachedFOV) {
				return;
			}

			cachedFOV = value;

			float fovNormalized = Mathf.InverseLerp(graphicalMinFOV, 360f, cachedFOV);
			float fovNormalized180 = Mathf.InverseLerp(graphicalMinFOV, 360f, 180f);

			//Debug.Log("FOV: " + testFOV + " ==> normalized: " + fovNormalized);

			float pupilPositionX = Mathf.Lerp(pupilPositionXSmall, pupilPositionXBig, pupilPositionDevelopment.Evaluate(fovNormalized));
			pupil.transform.localPosition = new Vector3(pupilPositionX, 0f, -1f);

			Vector2 pupilSize = new Vector2(Mathf.Lerp(pupilSizeSmall.x, pupilSizeBig.x, pupilSizeDevelopmentWidth.Evaluate(fovNormalized)),
											Mathf.Lerp(pupilSizeSmall.y, pupilSizeBig.y, pupilSizeDevelopmentHeight.Evaluate(fovNormalized)));
			pupil.transform.localScale = new Vector3(pupilSize.x, pupilSize.y, 1f);

			float shutterLeftAngle = Mathf.Lerp(graphicalMinFOV / 2f, leftShutterAngelBig, Mathf.Min(1f, fovNormalized));
			leftShutter.transform.localRotation = Quaternion.Euler(-2f, 0f, shutterLeftAngle);

			float shutterRightAngle = Mathf.Lerp(180f - graphicalMinFOV / 2f, rightShutterAngelBig, Mathf.Min(1f, fovNormalized));
			rightShutter.transform.localRotation = Quaternion.Euler(2f, 0f, shutterRightAngle);

			float backShutterDepth = backShutterDepthSmall;
			if (cachedFOV > 180f) {
				backShutterDepth = Mathf.Lerp(backShutterDepthSmall, backShutterDepthBig, Mathf.Sin((cachedFOV - 180f) * 0.5f * Mathf.Deg2Rad));
			}
			backShutter.transform.localPosition = new Vector3(backShutter.transform.localPosition.x, backShutter.transform.localPosition.y, backShutterDepth);
		}
	}
}
