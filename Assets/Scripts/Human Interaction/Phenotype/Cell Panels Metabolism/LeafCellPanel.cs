﻿using UnityEngine;
using UnityEngine.UI;

public class LeafCellPanel : MetabolismCellPanel {
	public Text productionEffectText;

	public HibernatePanel hibernatePanel;

	public Text exposure;
	public Text creatureSizeFactor;

	public override void SetMode(PhenoGenoEnum mode) {
		hibernatePanel.SetMode(mode);
		base.SetMode(mode);
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					productionEffectText.text = productionEffectPhenotypeString;

					exposure.text = string.Format("Exposure: {0:F2}%", (selectedCell as LeafCell).lowPassExposure * 100f);
					creatureSizeFactor.text = string.Format("Creature size factor: {0:F2}%", GlobalSettings.instance.phenotype.leafCellSunEffectFactorAtBodySize.Evaluate(selectedCell.creature.cellCount) * 100f);
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				float bodySize1Value = GlobalSettings.instance.phenotype.leafCellSunEffectFactorAtBodySize.Evaluate(1f);
				float bodySize100Value = GlobalSettings.instance.phenotype.leafCellSunEffectFactorAtBodySize.Evaluate(100f);
				productionEffectText.text = string.Format("Production Effect: [exposure (0...1)] * [creature size factor ({0:F2}...{1:F2})] * {2:F2} - {3:F2} W", bodySize1Value, bodySize100Value, GlobalSettings.instance.phenotype.leafCellSunMaxEffect, GlobalSettings.instance.phenotype.leafCellEffectCost);

				exposure.text = "Exposure : -";
				creatureSizeFactor.text = "Creature size factor: -";

			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;

				ignoreSliderMoved = false;
			}

			// Subpanels
			hibernatePanel.MakeDirty();

			isDirty = false;
		}
	}
}
