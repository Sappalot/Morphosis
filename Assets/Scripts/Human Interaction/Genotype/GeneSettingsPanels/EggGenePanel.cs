using UnityEngine;
using UnityEngine.UI;

public class EggGenePanel : MonoSingleton<EggGenePanel> {

	public Text fertilizeSliderText;
	public Slider fertilizeSlider;
	public Toggle canFertilizeWhenAttachedToggle;

	public Toggle detatchSizeToggle;
	public Toggle detatchEnergyToggle;

	public Text detatchSizeSliderText;
	public Text detatchEnergySliderText;

	public Slider detatchSizeSlider;
	public Slider detatchEnergySlider;

	private bool isDirty = false;

	private bool ignoreSliderMoved = false; // Work around

	private void Awake() {
		ignoreSliderMoved = true;
		fertilizeSlider.minValue = GlobalSettings.instance.phenotype.eggCellFertilizeThresholdMin;
		fertilizeSlider.maxValue = GlobalSettings.instance.phenotype.eggCellFertilizeThresholdMax;
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

		GenePanel.instance.selectedGene.eggCellFertilizeThreshold = fertilizeSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnEggChanged();
		}
		MakeDirty();
	}

	public void OnCanFertilizeWhenAttachedToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.eggCellCanFertilizeWhenAttached = canFertilizeWhenAttachedToggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnEggChanged();
		}
		MakeDirty();
	}

	public void OnDetatchModeToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.eggCellDetatchMode = detatchSizeToggle.isOn ? ChildDetatchModeEnum.Size : ChildDetatchModeEnum.Energy;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnEggChanged();
		}
		MakeDirty();
	}

	public void OnDetatchSizeSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.eggCellDetatchSizeThreshold = detatchSizeSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnEggChanged();
		}
		MakeDirty();
	}

	public void OnDetatchEnergySliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.eggCellDetatchEnergyThreshold = detatchEnergySlider.value;
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
				fertilizeSliderText.text =     string.Format("Cell E ≥ {0:F1}%?",       GenePanel.instance.selectedGene.eggCellFertilizeThreshold * 100f);
				detatchSizeSliderText.text =   string.Format("Size ≥ {0:F0} Cells", GenePanel.instance.selectedGene.eggCellDetatchSizeThreshold);
				detatchEnergySliderText.text = string.Format("EN ≥ {0:F1} J",       GenePanel.instance.selectedGene.eggCellDetatchEnergyThreshold);
			}

			isDirty = false;
		}

		if (areSlidersDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update EggGenePanel, move sliders");
			}

			if (CellPanel.instance.selectedCell != null) {
				ignoreSliderMoved = true;
				fertilizeSlider.value =               GenePanel.instance.selectedGene.eggCellFertilizeThreshold;
				canFertilizeWhenAttachedToggle.isOn = GenePanel.instance.selectedGene.eggCellCanFertilizeWhenAttached;

				detatchSizeToggle.isOn =              GenePanel.instance.selectedGene.eggCellDetatchMode == ChildDetatchModeEnum.Size;
				detatchEnergyToggle.isOn =            GenePanel.instance.selectedGene.eggCellDetatchMode == ChildDetatchModeEnum.Energy;

				detatchSizeSlider.value =             GenePanel.instance.selectedGene.eggCellDetatchSizeThreshold;
				detatchEnergySlider.value =           GenePanel.instance.selectedGene.eggCellDetatchEnergyThreshold;

				ignoreSliderMoved = false;
			}

			areSlidersDirty = false;
		}
	}
}
