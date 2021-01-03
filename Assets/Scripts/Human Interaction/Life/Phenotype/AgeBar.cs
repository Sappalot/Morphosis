using UnityEngine;
using UnityEngine.UI;

public class AgeBar : MonoBehaviour {
	public Image background;
	public Image bar;
	public Text text;

	public Color colorSelected;
	public Color colorNotSelected;

	private ulong age;
	public void SetAge(ulong age, ulong maxAge) { //age in s
		this.age = age;
		if (!isOn) {
			return;
		}
		float backgroundWidth = background.rectTransform.rect.width;
		bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backgroundWidth * Mathf.Min(1f, (float)this.age / (float)maxAge));
		bar.color = ColorScheme.instance.creatureAgeGradient.Evaluate((float)this.age / (float)maxAge);
		if (age < maxAge) {
			text.text = "Age: " + TimeUtil.GetTimeString(age);
		} else {
			text.text = "Waiting for grim reaper...";
		}
		
		text.color = ColorScheme.instance.creatureAgeTextGradient.Evaluate((float)this.age / (float)maxAge);
		background.color = Color.black;
	}

	private bool m_isOn;
	public bool isOn {
		get {
			return m_isOn;
		}
		set {
			m_isOn = value;
			if (!m_isOn) {
				bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
				text.text = "";
				background.color = ColorScheme.instance.grayedOut;
			}
		}
	}
}