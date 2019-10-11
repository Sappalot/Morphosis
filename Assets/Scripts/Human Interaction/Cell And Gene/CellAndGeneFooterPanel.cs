using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellAndGeneFooterPanel : MonoBehaviour {
	public Text productionEffectLabel;

	public void SetProductionEffectText(float produce, float consume) {
		if (produce == 0f) {
			productionEffectLabel.text = string.Format("Production Effect: -{0:F2} W", consume);
		} else {
			productionEffectLabel.text = string.Format("Production Effect: {0:F2} - {1:F2} = {2:F2} W", produce, consume, produce - consume);
		}
		
	}

	public void SetProductionEffectText(string text) {
		productionEffectLabel.text = text;
	}
}
