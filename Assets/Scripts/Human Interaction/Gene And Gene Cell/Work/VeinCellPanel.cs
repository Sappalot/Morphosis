using UnityEngine;
using UnityEngine.UI;

public class VeinCellPanel : CellWorkPanel {
	public Text productionEffectText;

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					productionEffectText.text = productionEffectPhenotypeString;
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				productionEffectText.text = string.Format("Production Effect: 0.00 - {0:F2} W", GlobalSettings.instance.phenotype.veinCellEffectCost);
			}

			isDirty = false; 
		}
	}
}
