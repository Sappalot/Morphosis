using UnityEngine;
using UnityEngine.UI;

public class EggCellPanel : MetabolismCellPanel {
	public Text productionEffectText;

	public Text fertilizeHeadingText;
	public Text fertilizeSliderText;
	public Slider fertilizeSlider;

	public Text fertilizeButtonText;

	public Text detatchHeadingText;

	public Toggle detatchSizeToggle;
	public Toggle detatchEnergyToggle;

	public Text detatchSizeSliderTextPercentage;
	public Text detatchSizeSliderTextCellCount;
	public Slider detatchSizeSlider;

	public Text detatchEnergySliderText;
	public Slider detatchEnergySlider;

	public Text idleWhenAttachedText;
	public Toggle idleWhenAttachedToggle;

	private void Awake() {
		ignoreSliderMoved = true;
		fertilizeSlider.minValue = GlobalSettings.instance.phenotype.eggCellFertilizeThresholdMin;
		fertilizeSlider.maxValue = GlobalSettings.instance.phenotype.eggCellFertilizeThresholdMax;

		detatchSizeSlider.minValue = GlobalSettings.instance.phenotype.eggCellDetatchSizeThresholdMin;
		detatchSizeSlider.maxValue = GlobalSettings.instance.phenotype.eggCellDetatchSizeThresholdMax;

		detatchEnergySlider.minValue = GlobalSettings.instance.phenotype.eggCellDetatchEnergyThresholdMin;
		detatchEnergySlider.maxValue = GlobalSettings.instance.phenotype.eggCellDetatchEnergyThresholdMax;

		ignoreSliderMoved = false;
	}

	public void OnClickFertilize() {
		if (CreatureSelectionPanel.instance.hasSoloSelected && mode == PhenoGenoEnum.Phenotype) {
			World.instance.life.FertilizeCreature(CellPanel.instance.selectedCell, true, World.instance.worldTicks, true);
		}
	}

	public void OnFertilizeSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.eggCellFertilizeThreshold = fertilizeSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		MakeDirty();
	}

	public void OnDetatchModeToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.eggCellDetatchMode = detatchSizeToggle.isOn ? ChildDetatchModeEnum.Size : ChildDetatchModeEnum.Energy;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		MakeDirty();
	}

	public void OnDetatchSizeSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.eggCellDetatchSizeThreshold = detatchSizeSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		MakeDirty();
	}

	public void OnDetatchEnergySliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.eggCellDetatchEnergyThreshold = detatchEnergySlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		MakeDirty();
	}

	public void OnIdleWhenAttachedToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.eggCellIdleWhenAttached = idleWhenAttachedToggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
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

					fertilizeHeadingText.color = ColorScheme.instance.grayedOutGenotype;

					fertilizeSliderText.color = ColorScheme.instance.grayedOutGenotype;
					fertilizeSlider.interactable = false;

					fertilizeButtonText.color = Color.black;

					detatchHeadingText.color = ColorScheme.instance.grayedOutGenotype;

					detatchSizeToggle.interactable = false;
					detatchEnergyToggle.interactable = false;

					detatchSizeSliderTextPercentage.color = ColorScheme.instance.grayedOutGenotype;
					detatchSizeSliderTextCellCount.color = ColorScheme.instance.grayedOutGenotype;
					detatchSizeSlider.interactable = false;

					detatchEnergySliderText.color = ColorScheme.instance.grayedOutGenotype;
					detatchEnergySlider.interactable = false;

					idleWhenAttachedText.color = ColorScheme.instance.grayedOutGenotype;
					idleWhenAttachedToggle.interactable = false;
				}
			} else if (mode == PhenoGenoEnum.Genotype) {
				productionEffectText.text = string.Format("Production Effect: 0.00 - {0:F2} W", GlobalSettings.instance.phenotype.eggCellEffectCost);

				fertilizeHeadingText.color = Color.black;

				fertilizeSliderText.color = Color.black;
				fertilizeSlider.interactable = isUnlocked();

				fertilizeButtonText.color = ColorScheme.instance.grayedOutPhenotype;

				detatchHeadingText.color = Color.black;

				detatchSizeToggle.interactable = isUnlocked();
				detatchEnergyToggle.interactable = isUnlocked();

				detatchSizeSliderTextPercentage.color = Color.black;
				detatchSizeSliderTextCellCount.color = Color.black;
				detatchSizeSlider.interactable = isUnlocked();

				detatchEnergySliderText.color = Color.black;
				detatchEnergySlider.interactable = isUnlocked();

				idleWhenAttachedText.color = Color.black;
				idleWhenAttachedToggle.interactable = isUnlocked();
			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;

				fertilizeSlider.value = selectedGene.eggCellFertilizeThreshold;
				fertilizeSliderText.text = string.Format("Egg Energy ≥ {0:F1}%", selectedGene.eggCellFertilizeThreshold * 100f);
				
				detatchSizeToggle.isOn = selectedGene.eggCellDetatchMode == ChildDetatchModeEnum.Size;
				detatchEnergyToggle.isOn = selectedGene.eggCellDetatchMode == ChildDetatchModeEnum.Energy;

				detatchSizeSlider.value = selectedGene.eggCellDetatchSizeThreshold;
				detatchSizeSliderTextPercentage.text = string.Format("Body size ≥ {0:F1}%", selectedGene.eggCellDetatchSizeThreshold * 100f);
				int cellCount = CreatureSelectionPanel.instance.soloSelected.genotype.geneCellCount;
				detatchSizeSliderTextCellCount.text = string.Format("{0:F0} of {1:F0} cells", Mathf.Clamp(Mathf.RoundToInt(selectedGene.eggCellDetatchSizeThreshold * cellCount), 1, cellCount), cellCount);

				detatchEnergySlider.value = selectedGene.eggCellDetatchEnergyThreshold;
				detatchEnergySliderText.text = string.Format("Can't grow more and cell energy ≥ {0:F1}%", selectedGene.eggCellDetatchEnergyThreshold * 100f);

				idleWhenAttachedToggle.isOn = selectedGene.eggCellIdleWhenAttached;

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}