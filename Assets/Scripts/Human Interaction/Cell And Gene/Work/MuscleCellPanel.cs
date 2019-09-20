using UnityEngine;
using UnityEngine.UI;

public class MuscleCellPanel : CellAndGeneComponentPanel {
	public Text productionEffectText;

	public Text frequenzy;

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					productionEffectText.text = productionEffectPhenotypeString;
					frequenzy.text = string.Format("Frequenzy: {0:F2} Hz", selectedCell.creature.phenotype.originCell.originPulseFequenzy);
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				productionEffectText.text = string.Format("Production Effect: 0.00 - {0:F2} W - {1:F2} J per contraction", GlobalSettings.instance.phenotype.cellHibernateEffectCost, GlobalSettings.instance.phenotype.muscleCellEnergyCostPerContraction);

				frequenzy.text = string.Format("Frequenzy: -");
			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;

				ignoreSliderMoved = false;
			}

			isDirty = false; 
		}
	}
}
