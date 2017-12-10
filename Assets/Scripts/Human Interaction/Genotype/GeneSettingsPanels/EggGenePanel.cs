using UnityEngine;
using UnityEngine.UI;

public class EggGenePanel : MonoSingleton<EggGenePanel> {

	public Text fertilizeSliderText;
	public Slider fertilizeSlider;

	public Text detatchSliderText;
	public Slider detatchSlider;

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

	public void OnDetatchSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.eggCellDetatchThreshold = detatchSlider.value;
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
				fertilizeSliderText.text = string.Format("Fertilize at EN > {0:F1} J", GenePanel.instance.selectedGene.eggCellFertilizeThreshold);
				detatchSliderText.text = string.Format("Detatch at EN > {0:F1} J", GenePanel.instance.selectedGene.eggCellDetatchThreshold);
			}

			isDirty = false;
		}

		if (areSlidersDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update EggGenePanel, move sliders");
			}

			if (CellPanel.instance.selectedCell != null) {
				ignoreSliderMoved = true;
				fertilizeSlider.value = GenePanel.instance.selectedGene.eggCellFertilizeThreshold;
				detatchSlider.value = GenePanel.instance.selectedGene.eggCellDetatchThreshold;
				ignoreSliderMoved = false;
			}

			areSlidersDirty = false;
		}

	}
}
