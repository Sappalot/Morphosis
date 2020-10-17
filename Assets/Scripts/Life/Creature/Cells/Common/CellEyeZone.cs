using UnityEngine;
using UnityEngine.UI;

// All cells knows this one in order to handle common stuff
// Made to be able to change all (types) at once via the CellCommon prefab
// stuff that need not be overridden
//[ExecuteInEditMode]
public class CellEyeZone : MonoBehaviour {


	public GameObject denyRoot;
	public Transform denyLeft;
	public Transform denyRight;
	public Transform denyDeadZoneCircle;
	public Transform denyFovCircle;

	public GameObject allowRoot;
	public Transform allowLeft;
	public Transform allowRight;
	public Transform allowDeadZoneCircle;
	public Transform allowFovCircle;


	private float cachedFOV = -1f;
	private float cachedRangeNear = -1f;
	private float cachedRangeFar = -1f;



	public void UpdateGraphics(float fieldOfView, float rangeNear, float rangeFar) {
		if (fieldOfView == this.cachedFOV && rangeNear == this.cachedRangeNear && rangeFar == this.cachedRangeFar) {
			return;
		}
		cachedFOV = fieldOfView;
		cachedRangeNear = rangeNear;
		cachedRangeFar = rangeFar;

		if (cachedFOV < 180f) {
			denyRoot.SetActive(true);
			allowRoot.SetActive(false);

			denyLeft.localRotation = Quaternion.Euler(0, 0f, fieldOfView / 2f);
			denyRight.localRotation = Quaternion.Euler(0, 0f, -90f - fieldOfView / 2f);

			denyDeadZoneCircle.localScale = new Vector3(rangeNear * 2f, rangeNear * 2f, 1f);
			denyFovCircle.localScale = new Vector3(rangeFar / 5f, rangeFar / 5f, 1f);
		} else {
			denyRoot.SetActive(false);
			allowRoot.SetActive(true);

			allowLeft.localRotation = Quaternion.Euler(0, 0f, (fieldOfView / 2f) - 90f );
			allowRight.localRotation = Quaternion.Euler(0, 0f, - fieldOfView / 2f);

			allowDeadZoneCircle.localScale = new Vector3(rangeNear * 2f, rangeNear * 2f, 1f);
			allowFovCircle.localScale = new Vector3(rangeFar / 5f, rangeFar / 5f, 1f);
		}

	}
}
