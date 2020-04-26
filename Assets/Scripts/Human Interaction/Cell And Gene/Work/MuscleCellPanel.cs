using UnityEngine;
using UnityEngine.UI;

public class MuscleCellPanel : ComponentPanel {
	public Text frequenzy;

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (cellAndGenePanel.cell != null) {
					componentFooterPanel.SetProductionEffectText(0f, selectedCell.effectProductionInternalDown);
					frequenzy.text = string.Format("Frequenzy: {0:F2} Hz", selectedCell.creature.phenotype.originCell.originPulseFequenzy);
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				componentFooterPanel.SetProductionEffectText(string.Format("Production Effect: -{0:F2} W (- {1:F2} J per contraction)", GlobalSettings.instance.phenotype.muscleCell.effectProductionDown, GlobalSettings.instance.phenotype.muscleCell.energyProductionDownPerContraction));

				frequenzy.text = string.Format("Frequenzy: -");
			}

			if (gene != null) {
				ignoreHumanInput = true;

				ignoreHumanInput = false;
			}

			isDirty = false; 
		}
	}
}
