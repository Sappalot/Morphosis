using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour {
	public Image background;
	public Image bar;
	public Text text;

	public Image effectArrowPos;
	public Image effectArrowNeg;

	private float m_fullness = 1f;
	public float fullness {
		get {
			return m_fullness;
		}
		set {
			m_fullness = value;
			if (!isOn) {
				return;
			}
			float backgroundWidth = background.rectTransform.rect.width;
			bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backgroundWidth * m_fullness);
			bar.color = ColorScheme.instance.cellGradientEnergy.Evaluate(m_fullness);
			text.text = string.Format("{0:F0}%", m_fullness * 100f);
			text.color = new Color(1f - bar.color.r, 1f - bar.color.g, 1f - bar.color.b);
			background.color = Color.black;
		}
	}

	private float m_effect = 0f;
	public float effect {
		get {
			return m_effect;
		}
		set {
			m_effect = value;
			if (!isOn) {
				return;
			}
			//arrow
			float backgroundWidth = background.rectTransform.rect.width;
			if (m_effect >= 0f) {
				effectArrowPos.rectTransform.anchoredPosition = new Vector2(backgroundWidth * m_fullness, effectArrowPos.rectTransform.anchoredPosition.y);
				effectArrowPos.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (m_effect / GlobalSettings.instance.phenotype.cellMaxEnergy) * backgroundWidth * 10f);
				effectArrowPos.color = ColorScheme.instance.cellGradientEffect.Evaluate(0.5f + m_effect * 0.1f);

				effectArrowNeg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
			} else {
				effectArrowNeg.rectTransform.anchoredPosition = new Vector2(backgroundWidth * m_fullness, effectArrowPos.rectTransform.anchoredPosition.y);
				effectArrowNeg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, -(m_effect / GlobalSettings.instance.phenotype.cellMaxEnergy) * backgroundWidth * 10f);
				effectArrowNeg.color = ColorScheme.instance.cellGradientEffect.Evaluate(0.5f + m_effect * 0.1f);

				effectArrowPos.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
			}

		}
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
				background.color = Color.gray;

				effectArrowPos.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
				effectArrowNeg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
				effectArrowPos.color = Color.gray;
				effectArrowNeg.color = Color.gray;
			}
		}
	}
}