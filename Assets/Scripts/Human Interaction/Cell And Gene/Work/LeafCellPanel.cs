﻿using UnityEngine;
using UnityEngine.UI;

public class LeafCellPanel : ComponentPanel {
	public Text exposure;
	public Text creatureSizeFactor;
	public Text overPopulationFactor;

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					componentFooterPanel.SetProductionEffectText(selectedCell.effectProductionInternalUp, GlobalSettings.instance.phenotype.leafCell.effectProductionDown);

					exposure.text = string.Format("Exposure: {0:F2}%", (selectedCell as LeafCell).lowPassExposure * 100f);
					creatureSizeFactor.text = string.Format("Creature size factor: {0:F2}%", GlobalSettings.instance.phenotype.leafCell.exposureFactorAtBodySize.Evaluate(selectedCell.creature.cellCount) * 100f);
					overPopulationFactor.text = string.Format("Over population factor: {0:F2}%", GlobalSettings.instance.phenotype.leafCell.exposureFactorAtPopulation.Evaluate(World.instance.life.cellAliveCount) * 100f);
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				componentFooterPanel.SetProductionEffectText(string.Format("Production Effect: [exposure (0...1)] * {0:F2} - {0:F2} W", GlobalSettings.instance.phenotype.leafCell.effectProductionUpMax, GlobalSettings.instance.phenotype.leafCell.effectProductionDown));

				exposure.text = "Exposure : -";
				creatureSizeFactor.text = "Creature size factor: -";
				overPopulationFactor.text = "Over population factor: -";
			}

			if (selectedGene != null) {
				ignoreHumanInput = true;

				ignoreHumanInput = false;
			}

			isDirty = false;
		}
	}
}
