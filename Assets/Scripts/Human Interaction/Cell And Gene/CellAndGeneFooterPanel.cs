using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellAndGeneFooterPanel : MonoBehaviour {
	public Text productionEffectLabel;

	public void SetProductionEffectText(float produce, float consume) {
		productionEffectLabel.text = produce + " - " + consume;
	}

	public void SetProductionEffectText(string text) {
		productionEffectLabel.text = text;
	}
}
