using UnityEngine;
using UnityEngine.UI;

public class LeafCellPanel : MetabolismCellPanel {
	public Text productionEffectText;

	public Text exposure;
	public Text creatureSizeFactor;

	public Text idleWhenAttachedText;
	public Toggle idleWhenAttachedToggle;

	public void OnIdleWhenAttachedToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.leafCellIdleWhenAttached = idleWhenAttachedToggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			MakeCreatureChanged();
		}
		MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (mode == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					productionEffectText.text = productionEffectPhenotypeString;

					exposure.text = string.Format("Exposure: {0:F2}%", (selectedCell as LeafCell).lowPassExposure * 100f);
					creatureSizeFactor.text = string.Format("Creature size factor: {0:F2}%", GlobalSettings.instance.phenotype.leafCellSunEffectFactorAtBodySize.Evaluate(selectedCell.creature.cellCount) * 100f);

					exposure.color = Color.black;

					idleWhenAttachedText.color = ColorScheme.instance.grayedOutGenotype;
					idleWhenAttachedToggle.interactable = false;
				}
			} else if (mode == PhenoGenoEnum.Genotype) {
				float bodySize1Value = GlobalSettings.instance.phenotype.leafCellSunEffectFactorAtBodySize.Evaluate(1f);
				float bodySize100Value = GlobalSettings.instance.phenotype.leafCellSunEffectFactorAtBodySize.Evaluate(100f);
				productionEffectText.text = string.Format("Production Effect: [exposure (0...1)] * [creature size factor ({0:F2}...{1:F2})] * {2:F2} - {3:F2} W", bodySize1Value, bodySize100Value, GlobalSettings.instance.phenotype.leafCellSunMaxEffect, GlobalSettings.instance.phenotype.leafCellEffectCost);

				exposure.text = "Exposure : -";
				exposure.color = ColorScheme.instance.grayedOutPhenotype;

				creatureSizeFactor.text = "Creature size factor: -";
				creatureSizeFactor.color = ColorScheme.instance.grayedOutPhenotype;

				idleWhenAttachedText.color = Color.black;
				idleWhenAttachedToggle.interactable = isUnlocked();
			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;

				idleWhenAttachedToggle.isOn = selectedGene.leafCellIdleWhenAttached;

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}
