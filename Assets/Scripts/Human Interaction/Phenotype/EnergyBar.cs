using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour {
	public Image background;
	public Image bar;
	public Text text;

	public Image effectArrowTotalPos;
	public Image effectArrowTotalNeg;
	public Text totalText;

	public Image effectArrowProdPos;
	public Image effectArrowProdNeg;
	public Text prodText;

	public Image effectArrowFluxPos;
	public Image effectArrowFluxNeg;
	public Text fluxText;

	public Color colorSelected;
	public Color colorNotSelected;

	public float alphaArrow = 0.5f;

	public EffectTempEnum effectMeasure {
		set {
			totalText.color = value == EffectTempEnum.Total ? colorSelected : colorNotSelected;
			prodText.color = value == EffectTempEnum.Production ? colorSelected : colorNotSelected;
			fluxText.color = value == EffectTempEnum.Flux ? colorSelected : colorNotSelected;
		}
	}

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

	private float m_effectTotal = 0f;
	public float effectTotal {
		get {
			return m_effectTotal;
		}
		set {
			m_effectTotal = value;
			if (!isOn) {
				return;
			}
			//arrow
			float backgroundWidth = background.rectTransform.rect.width;
			if (m_effectTotal >= 0f) {
				effectArrowTotalPos.rectTransform.anchoredPosition = new Vector2(backgroundWidth * m_fullness, effectArrowTotalPos.rectTransform.anchoredPosition.y);
				effectArrowTotalPos.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (m_effectTotal / GlobalSettings.instance.phenotype.cellMaxEnergy) * backgroundWidth * 10f);
				effectArrowTotalPos.color = ColorScheme.instance.cellGradientEffect.Evaluate(0.5f + m_effectTotal * 0.1f);

				effectArrowTotalNeg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
			} else {
				effectArrowTotalNeg.rectTransform.anchoredPosition = new Vector2(backgroundWidth * m_fullness, effectArrowTotalPos.rectTransform.anchoredPosition.y);
				effectArrowTotalNeg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, -(m_effectTotal / GlobalSettings.instance.phenotype.cellMaxEnergy) * backgroundWidth * 10f);
				effectArrowTotalNeg.color = ColorScheme.instance.cellGradientEffect.Evaluate(0.5f + m_effectTotal * 0.1f);

				effectArrowTotalPos.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
			}

		}
	}

	private float m_effectProd = 0f;
	public float effectProd {
		get {
			return m_effectProd;
		}
		set {
			m_effectProd = value;
			if (!isOn) {
				return;
			}
			//arrow
			float backgroundWidth = background.rectTransform.rect.width;
			Color color = ColorScheme.instance.cellGradientEffect.Evaluate(0.5f + m_effectProd * 0.1f);
			if (m_effectProd >= 0f) {
				effectArrowProdPos.rectTransform.anchoredPosition = new Vector2(backgroundWidth * m_fullness, effectArrowProdPos.rectTransform.anchoredPosition.y);
				effectArrowProdPos.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (m_effectProd / GlobalSettings.instance.phenotype.cellMaxEnergy) * backgroundWidth * 10f);
				effectArrowProdPos.color = new Color(color.r, color.g, color.b, alphaArrow);
				effectArrowProdNeg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
			} else {
				effectArrowProdNeg.rectTransform.anchoredPosition = new Vector2(backgroundWidth * m_fullness, effectArrowProdPos.rectTransform.anchoredPosition.y);
				effectArrowProdNeg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, -(m_effectProd / GlobalSettings.instance.phenotype.cellMaxEnergy) * backgroundWidth * 10f);
				effectArrowProdNeg.color = new Color(color.r, color.g, color.b, alphaArrow);
				effectArrowProdPos.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
			}

		}
	}

	private float m_effectFlux = 0f;
	public float effectFlux {
		get {
			return m_effectFlux;
		}
		set {
			m_effectFlux = value;
			if (!isOn) {
				return;
			}
			//arrow
			float backgroundWidth = background.rectTransform.rect.width;
			Color color = ColorScheme.instance.cellGradientEffect.Evaluate(0.5f + m_effectFlux * 0.1f);
			if (m_effectFlux >= 0f) {
				effectArrowFluxPos.rectTransform.anchoredPosition = new Vector2(backgroundWidth * m_fullness, effectArrowFluxPos.rectTransform.anchoredPosition.y);
				effectArrowFluxPos.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (m_effectFlux / GlobalSettings.instance.phenotype.cellMaxEnergy) * backgroundWidth * 10f);
				effectArrowFluxPos.color = new Color(color.r, color.g, color.b, alphaArrow);

				effectArrowFluxNeg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
			} else {
				effectArrowFluxNeg.rectTransform.anchoredPosition = new Vector2(backgroundWidth * m_fullness, effectArrowFluxPos.rectTransform.anchoredPosition.y);
				effectArrowFluxNeg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, -(m_effectFlux / GlobalSettings.instance.phenotype.cellMaxEnergy) * backgroundWidth * 10f);
				effectArrowFluxNeg.color = new Color(color.r, color.g, color.b, alphaArrow);

				effectArrowFluxPos.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
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

				effectArrowTotalPos.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
				effectArrowTotalNeg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
				effectArrowTotalPos.color = Color.gray;
				effectArrowTotalNeg.color = Color.gray;

				effectArrowProdPos.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
				effectArrowProdNeg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
				effectArrowProdPos.color = Color.gray;
				effectArrowProdNeg.color = Color.gray;

				effectArrowFluxPos.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
				effectArrowFluxNeg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
				effectArrowFluxPos.color = Color.gray;
				effectArrowFluxNeg.color = Color.gray;
			}
		}
	}
}