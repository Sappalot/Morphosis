using UnityEngine;
using UnityEngine.UI;

public class MuscleCellPanel : CellAndGeneComponentPanel {
	public Text frequenzy;

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					footerPanel.SetProductionEffectText(0f, selectedCell.effectProductionInternalDown);
					frequenzy.text = string.Format("Frequenzy: {0:F2} Hz", selectedCell.creature.phenotype.originCell.originPulseFequenzy);
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				footerPanel.SetProductionEffectText(string.Format("Production Effect: -{0:F2} J per contraction", GlobalSettings.instance.phenotype.muscleCellEnergyCostPerContraction));

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
