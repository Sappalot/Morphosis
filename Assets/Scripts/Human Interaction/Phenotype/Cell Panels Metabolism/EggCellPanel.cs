using UnityEngine;
using UnityEngine.UI;

public class EggCellPanel : MetabolismCellPanel {
	public Text fertilizeHeadingText;
	public Text fertilizeSliderText;
	public Slider fertilizeSlider;

	public Text canFertilizeWhenAttachedText;
	public Toggle canFertilizeWhenAttachedToggle;

	public Text fertilizeButtonText;

	public Text detatchHeadingText;

	public Toggle detatchSizeToggle;
	public Toggle detatchEnergyToggle;

	public Text detatchSizeSliderTextPercentage;
	public Text detatchSizeSliderTextCellCount;
	public Slider detatchSizeSlider;

	public Text detatchEnergySliderText;
	public Slider detatchEnergySlider;

	private bool ignoreSliderMoved = false;

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

		GenePanel.instance.selectedGene.eggCellFertilizeThreshold = fertilizeSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	public void OnCanFertilizeWhenAttachedToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.eggCellCanFertilizeWhenAttached = canFertilizeWhenAttachedToggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	public void OnDetatchModeToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.eggCellDetatchMode = detatchSizeToggle.isOn ? ChildDetatchModeEnum.Size : ChildDetatchModeEnum.Energy;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	public void OnDetatchSizeSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.eggCellDetatchSizeThreshold = detatchSizeSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	public void OnDetatchEnergySliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.eggCellDetatchEnergyThreshold = detatchEnergySlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (mode == PhenoGenoEnum.Phenotype) {
				fertilizeHeadingText.color = Color.gray;

				fertilizeSliderText.color = Color.gray;
				fertilizeSlider.interactable = false;

				canFertilizeWhenAttachedText.color = Color.gray;
				canFertilizeWhenAttachedToggle.interactable = false;

				fertilizeButtonText.color = Color.black;

				detatchHeadingText.color = Color.gray;

				detatchSizeToggle.interactable =   false;
				detatchEnergyToggle.interactable = false;

				detatchSizeSliderTextPercentage.color = Color.gray;
				detatchSizeSliderTextCellCount.color = Color.gray;
				detatchSizeSlider.interactable = false;

				detatchEnergySliderText.color = Color.gray;
				detatchEnergySlider.interactable = false;

			} else if (mode == PhenoGenoEnum.Genotype) {
				fertilizeHeadingText.color = Color.black;

				fertilizeSliderText.color = Color.black;
				fertilizeSlider.interactable = isUnlocked();

				canFertilizeWhenAttachedText.color = Color.black;
				canFertilizeWhenAttachedToggle.interactable = isUnlocked();

				fertilizeButtonText.color = Color.gray;

				detatchHeadingText.color = Color.black;

				detatchSizeToggle.interactable = isUnlocked();
				detatchEnergyToggle.interactable = isUnlocked();

				detatchSizeSliderTextPercentage.color = Color.black;
				detatchSizeSliderTextCellCount.color = Color.black;
				detatchSizeSlider.interactable = isUnlocked();

				detatchEnergySliderText.color = Color.black;
				detatchEnergySlider.interactable = isUnlocked();
			}

			if (GenePanel.instance.selectedGene != null) {
				ignoreSliderMoved = true;

				fertilizeSlider.value = GenePanel.instance.selectedGene.eggCellFertilizeThreshold;
				fertilizeSliderText.text = string.Format("Egg Energy ≥ {0:F1}%", GenePanel.instance.selectedGene.eggCellFertilizeThreshold * 100f);
				
				canFertilizeWhenAttachedToggle.isOn = GenePanel.instance.selectedGene.eggCellCanFertilizeWhenAttached;

				detatchSizeToggle.isOn = GenePanel.instance.selectedGene.eggCellDetatchMode == ChildDetatchModeEnum.Size;
				detatchEnergyToggle.isOn = GenePanel.instance.selectedGene.eggCellDetatchMode == ChildDetatchModeEnum.Energy;

				detatchSizeSlider.value = GenePanel.instance.selectedGene.eggCellDetatchSizeThreshold;
				detatchSizeSliderTextPercentage.text = string.Format("Body size ≥ {0:F1}%", GenePanel.instance.selectedGene.eggCellDetatchSizeThreshold * 100f);
				int cellCount = CreatureSelectionPanel.instance.soloSelected.genotype.geneCellCount;
				detatchSizeSliderTextCellCount.text = string.Format("{0:F0} of {1:F0} cells", Mathf.Clamp(Mathf.RoundToInt(GenePanel.instance.selectedGene.eggCellDetatchSizeThreshold * cellCount), 1, cellCount), cellCount);

				detatchEnergySlider.value = GenePanel.instance.selectedGene.eggCellDetatchEnergyThreshold;
				detatchEnergySliderText.text = string.Format("Can't grow more and cell energy ≥ {0:F1}%", GenePanel.instance.selectedGene.eggCellDetatchEnergyThreshold * 100f);

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}