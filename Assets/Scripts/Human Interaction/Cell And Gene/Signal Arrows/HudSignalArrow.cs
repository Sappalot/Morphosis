using UnityEngine;
using UnityEngine.UI;

public class HudSignalArrow : MonoBehaviour {
	public Text headLabel;
	public Image headBackground;
	public Text tailLabel;
	public Image tailBackground;

	[HideInInspector]
	public SignalUnitEnum headUnit = SignalUnitEnum.Void; // The output from me "the nerve" The GeneSensor that created this one
	[HideInInspector]
	public SignalUnitSlotEnum headUnitSlot = SignalUnitSlotEnum.Void; // The slot on that (above) unit
	[HideInInspector]
	public SignalUnitEnum tailUnit = SignalUnitEnum.Void; // The input to me "the nerve" (Somebodey elses output)
	[HideInInspector]
	public SignalUnitSlotEnum tailUnitSlot = SignalUnitSlotEnum.Void; // The slot on that (above) unit

	[HideInInspector]
	public Cell headCell;

	[HideInInspector]
	public Cell tailCell;

	public Vector2 headPosition;
	public Vector2 tailPosition;

	public string ToTailString() {
		return tailUnit.ToString() + "_" + tailUnitSlot.ToString();
	}

	private bool m_showHeadLabel;
	public bool showHeadLabel {
		get {
			return m_showHeadLabel;
		}
		set {
			m_showHeadLabel = value;
			isDirty = true;
		}
	}

	private bool m_showTailLabel;
	public bool showTailLabel {
		get {
			return m_showTailLabel;
		}
		set {
			m_showTailLabel = value;
			isDirty = true;
		}
	}

	private bool isDirty;

	public void OnRecycle() {
		headUnit = SignalUnitEnum.Void;
		headUnitSlot = SignalUnitSlotEnum.Void;

		tailUnit = SignalUnitEnum.Void;
		tailUnitSlot = SignalUnitSlotEnum.Void;

		gameObject.SetActive(false);
	}

	private void Update() {
		if (isDirty) {
			headLabel.gameObject.SetActive(showHeadLabel);
			headBackground.gameObject.SetActive(showHeadLabel);
			if (showHeadLabel) {
				headLabel.text = "To: " + headUnit.ToString() + ", " + headUnitSlot.ToString();
			}

			tailLabel.gameObject.SetActive(showTailLabel);
			tailBackground.gameObject.SetActive(showTailLabel);
			if (showTailLabel) {
				tailLabel.text = "From: " + tailUnit.ToString() + ", " + tailUnitSlot.ToString();
			}

			isDirty = false;
		} 
	}
}
