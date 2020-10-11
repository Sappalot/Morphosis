using UnityEngine;
using UnityEngine.UI;

public class GraphSettings : MonoBehaviour {
	public Toggle toggle;
	public Button button;

	public float defaultMaxValue = 200;
	public Text maxValueButtonText;
	public InputField maxInputField;
	public float maxValue { get; private set; }
	
	public bool isOn {
		get {
			return toggle.isOn;
		}
	}

	private void Start() {
		maxInputField.gameObject.SetActive(false);
		maxValue = defaultMaxValue;
		UpdateButtonText();
	}

	private void UpdateButtonText() {
		maxValueButtonText.text = maxValue.ToString();
	}

	public void OnToggleValueChanged() {
		GraphPlotter.instance.MakeDirty();
	}

	public void OnButtonClicked() {
		DebugUtil.Log("Button clicked!");
	}

	public void OnInputButtonClicked() {
		maxInputField.gameObject.SetActive(true);
		maxInputField.Select();
	}

	public void OnEndEditInput() {
		float result;
		if (float.TryParse(maxInputField.text, out result)) {
			maxValue = result;
		} else {
			DebugUtil.Log("Not a number!");
		}
		maxInputField.gameObject.SetActive(false);
		UpdateButtonText();
		GraphPlotter.instance.MakeDirty();
	}
}