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
			CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
		}
		MakeDirty();
	}

	public void OnCanFertilizeWhenAttachedToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.eggCellCanFertilizeWhenAttached = canFertilizeWhenAttachedToggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
		}
		MakeDirty();
	}

	public void OnDetatchModeToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.eggCellDetatchMode = detatchSizeToggle.isOn ? ChildDetatchModeEnum.Size : ChildDetatchModeEnum.Energy;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
		}
		MakeDirty();
	}

	public void OnDetatchSizeSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.eggCellDetatchSizeThreshold = detatchSizeSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
		}
		MakeDirty();
	}

	public void OnDetatchEnergySliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.eggCellDetatchEnergyThreshold = detatchEnergySlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
		}
		MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update EggGenePanel, text");
			}

			if (CellPanel.instance.selectedCell != null) {
				fertilizeSliderText.text =     string.Format("EN ≥ {0:F1} J",        GenePanel.instance.selectedGene.eggCellFertilizeThreshold);
				detatchSizeSliderText.text =   string.Format("Size ≥ {0:F0} Cells", GenePanel.instance.selectedGene.eggCellDetatchSizeThreshold);
				detatchEnergySliderText.text = string.Format("EN ≥ {0:F1} J",        GenePanel.instance.selectedGene.eggCellDetatchEnergyThreshold);
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
