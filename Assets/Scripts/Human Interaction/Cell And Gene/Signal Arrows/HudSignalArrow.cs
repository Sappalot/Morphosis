using UnityEngine;

public class HudSignalArrow : MonoBehaviour {
	[HideInInspector]
	public SignalUnitEnum headUnit = SignalUnitEnum.Void; // The output from me "the nerve" The GeneSensor that created this one
	[HideInInspector]
	public SignalUnitSlotEnum headUnitSlot; // The slot on that (above) unit
	[HideInInspector]
	public SignalUnitEnum tailUnit = SignalUnitEnum.Void; // The input to me "the nerve" (Somebodey elses output)
	[HideInInspector]
	public SignalUnitSlotEnum tailUnitSlot; // The slot on that (above) unit

	public void OnRecycle() {
		headUnit = SignalUnitEnum.Void;
		tailUnit = SignalUnitEnum.Void;
		gameObject.SetActive(false);
	}
}
