using UnityEngine;
using UnityEngine.UI;

public class EggGenePanel : MonoSingleton<EggGenePanel> {

	public Text fertilizeSliderText;
	public Slider fertilizeSlider;
	public Toggle canFertilizeWhenAttachedToggle;

	public Toggle detatchSizeToggle;
	public Toggle detatchEnergyToggle;

	public Text detatchSizeSliderTextPercentage;
	public Text detatchSizeSliderTextCellCount;
	public Text detatchEnergySliderText;

	public Slider detatchSizeSlider;
	public Slider detatchEnergySlider;

	private bool isDirty = false;

	private bool ignoreSliderMoved = false; // Work around

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

	public void SetInteractable(bool interactable) {
		fertilizeSlider.interactable = interactable;
		canFertilizeWhenAttachedToggle.interactable = interactable;
		detatchSizeToggle.interactable = interactable;
		detatchEnergyToggle.interactable = interactable;
		detatchSizeSlider.interactable = interactable;
		detatchEnergySlider.interactable = interactable;
	}

	public void MakeDirty() {
		isDirty = true;
	}

	private bool areSlidersDirty = false;
	public void MakeSlidersDirty() {
		areSlidersDirty = true;
	}

	public void OnFertilizeSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		GeneNeighboursPanel.instance.selectedGene.eggCellFertilizeThreshold = fertilizeSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnEggChanged();
		}
		MakeDirty();
	}

	public void OnCanFertilizeWhenAttachedToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		GeneNeighboursPanel.instance.selectedGene.eggCellCanFertilizeWhenAttached = canFertilizeWhenAttachedToggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnEggChanged();
		}
		MakeDirty();
	}

	public void OnDetatchModeToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		GeneNeighboursPanel.instance.selectedGene.eggCellDetatchMode = detatchSizeToggle.isOn ? ChildDetatchModeEnum.Size : ChildDetatchModeEnum.Energy;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnEggChanged();
		}
		MakeDirty();
	}

	public void OnDetatchSizeSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		GeneNeighboursPanel.instance.selectedGene.eggCellDetatchSizeThreshold = detatchSizeSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnEggChanged();
		}
		MakeDirty();
	}

	public void OnDetatchEnergySliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		GeneNeighboursPanel.instance.selectedGene.eggCellDetatchEnergyThreshold = detatchEnergySlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnEggChanged();
		}
		MakeDirty();
	}

	private void OnEggChanged() {
		CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
		CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
		CreatureSelectionPanel.instance.soloSelected.generation = 1;
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update EggGenePanel, text");
			}

			if (CellPanel.instance.selectedCell != null) {
				fertilizeSliderText.text =     string.Format("Egg E ≥ {0:F1}%",           GeneNeighboursPanel.instance.selectedGene.eggCellFertilizeThreshold * 100f);
				detatchSizeSliderTextPercentage.text =   string.Format("Size ≥ {0:F1}%",  GeneNeighboursPanel.instance.selectedGene.eggCellDetatchSizeThreshold * 100f);
				int cellCount = CreatureSelectionPanel.instance.soloSelected.genotype.geneCellCount;
				detatchSizeSliderTextCellCount.text = string.Format("{0:F0} of {1:F0} cells", Mathf.Clamp(Mathf.RoundToInt(GeneNeighboursPanel.instance.selectedGene.eggCellDetatchSizeThreshold * cellCount), 1, cellCount), cellCount);
				if (GeneNeighboursPanel.instance.selectedGene.eggCellDetatchSizeThreshold > 1f) {
					detatchSizeSliderTextPercentage.color = Color.red;
					detatchSizeSliderTextCellCount.text = "";
				} else {
					detatchSizeSliderTextPercentage.color = Color.black;
				}

				detatchEnergySliderText.text = string.Format("Origin E ≥ {0:F1}%",                GeneNeighboursPanel.instance.selectedGene.eggCellDetatchEnergyThreshold * 100f);
				if (GeneNeighboursPanel.instance.selectedGene.eggCellDetatchEnergyThreshold > 1f) {
					detatchEnergySliderText.color = Color.red;
				} else {
					detatchEnergySliderText.color = Color.black;
				}
			}

			isDirty = false;
		}

		if (areSlidersDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update EggGenePanel, move sliders");
			}

			if (CellPanel.instance.selectedCell != null) {
				ignoreSliderMoved = true;
				fertilizeSlider.value =               GeneNeighboursPanel.instance.selectedGene.eggCellFertilizeThreshold;
				canFertilizeWhenAttachedToggle.isOn = GeneNeighboursPanel.instance.selectedGene.eggCellCanFertilizeWhenAttached;

				detatchSizeToggle.isOn =              GeneNeighboursPanel.instance.selectedGene.eggCellDetatchMode == ChildDetatchModeEnum.Size;
				detatchEnergyToggle.isOn =            GeneNeighboursPanel.instance.selectedGene.eggCellDetatchMode == ChildDetatchModeEnum.Energy;

				detatchSizeSlider.value =             GeneNeighboursPanel.instance.selectedGene.eggCellDetatchSizeThreshold;
				detatchEnergySlider.value =           GeneNeighboursPanel.instance.selectedGene.eggCellDetatchEnergyThreshold;

				ignoreSliderMoved = false;
			}

			areSlidersDirty = false;
		}
	}
}
