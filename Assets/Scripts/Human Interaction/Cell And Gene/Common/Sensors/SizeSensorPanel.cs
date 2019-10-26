using UnityEngine;
using UnityEngine.UI;

// TODO: rename growth sensor panel
public class SizeSensorPanel : SensorPanel {
	public Text sizeMoreThanSliderLabel;
	public Text sizeLessThanSliderLabel;
	public Slider sizeThresholdSlider;

	public Text growthBlockedPatienseThresholdSliderLabel;
	public Slider growthBlockedPatienseThresholdSlider;

	public void OnSizeThresholdSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		(affectedGeneSensor as GeneSizeSensor).sizeThreshold = sizeThresholdSlider.value;
		OnGenomeChanged(false);
	}

	public void OnBlockedGrowthPatienseThresholdSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		(affectedGeneSensor as GeneSizeSensor).growthBlockedPatienseThreshold = (int)growthBlockedPatienseThresholdSlider.value;
		OnGenomeChanged(false);
	}

	public override void Update() {
		if (isDirty) {
			base.Update();

			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Size Sensor Panel");
			}

			if (selectedGene != null && affectedGeneSensor != null) {
				ignoreSliderMoved = true;
				sizeMoreThanSliderLabel.text = string.Format("A: Size ≥ {0:F0} % ({1:F0} cells or bigger)", (affectedGeneSensor as GeneSizeSensor).sizeThreshold * 100f, CreatureSelectionPanel.instance.soloSelected.CellCountAtCompleteness((affectedGeneSensor as GeneSizeSensor).sizeThreshold));
				sizeLessThanSliderLabel.text = string.Format("B: Size < {0:F0} % ({1:F0} cells or smaller)", (affectedGeneSensor as GeneSizeSensor).sizeThreshold * 100f, CreatureSelectionPanel.instance.soloSelected.CellCountAtCompleteness((affectedGeneSensor as GeneSizeSensor).sizeThreshold) - 1);
				sizeThresholdSlider.value = (affectedGeneSensor as GeneSizeSensor).sizeThreshold;
				sizeThresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				growthBlockedPatienseThresholdSliderLabel.text = string.Format("E: Growth blocked ≥ {0:F0} s", (affectedGeneSensor as GeneSizeSensor).growthBlockedPatienseThreshold);
				growthBlockedPatienseThresholdSlider.value = (affectedGeneSensor as GeneSizeSensor).growthBlockedPatienseThreshold;
				growthBlockedPatienseThresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}