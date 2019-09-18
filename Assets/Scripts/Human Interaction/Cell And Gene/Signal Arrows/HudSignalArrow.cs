using UnityEngine;

public class HudSignalArrow : MonoBehaviour {
	[HideInInspector]
	public SignalUnitEnum outputUnit = SignalUnitEnum.Void; // The output from me "the nerve" The GeneSensor that created this one
	[HideInInspector]
	public SignalUnitSlotEnum outputUnitSlot; // The slot on that (above) unit
	[HideInInspector]
	public SignalUnitEnum inputUnit = SignalUnitEnum.Void; // The input to me "the nerve" (Somebodey elses output)
	[HideInInspector]
	public SignalUnitSlotEnum inputUnitSlot; // The slot on that (above) unit

	public void OnRecycle() {
		outputUnit = SignalUnitEnum.Void;
		inputUnit = SignalUnitEnum.Void;
		gameObject.SetActive(false);
	}
}
