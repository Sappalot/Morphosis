using UnityEngine;
using UnityEngine.UI;

// TODO: rename growth sensor panel
public class SizeSensorPanel : SensorPanel {
	

	public Text sizeThresholdSliderLabel;
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
				sizeThresholdSliderLabel.text = string.Format("A: Creature Size > {0:F1} %", (affectedGeneSensor as GeneSizeSensor).sizeThreshold);
				sizeThresholdSlider.value = (affectedGeneSensor as GeneSizeSensor).sizeThreshold;
				sizeThresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				growthBlockedPatienseThresholdSliderLabel.text = string.Format("E: Growth blocked > {0:F0} s", (affectedGeneSensor as GeneSizeSensor).growthBlockedPatienseThreshold);
				growthBlockedPatienseThresholdSlider.value = (affectedGeneSensor as GeneSizeSensor).growthBlockedPatienseThreshold;
				growthBlockedPatienseThresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}