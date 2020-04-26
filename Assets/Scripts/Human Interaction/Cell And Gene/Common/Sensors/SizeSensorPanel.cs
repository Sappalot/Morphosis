using UnityEngine;
using UnityEngine.UI;

// TODO: rename growth sensor panel
public class SizeSensorPanel : SensorPanel {
	public Text sizeMoreThanSliderLabel;
	public Text sizeLessThanSliderLabel;
	public Slider sizeThresholdSlider;

	public Text cantGrowMoreTime;
	public Slider cantGrowMoreTimeSlider;

	public void OnSizeThresholdSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSensor as GeneSizeSensor).sizeThreshold = sizeThresholdSlider.value;
		OnGenomeChanged();
	}

	public void OnBlockedGrowthPatienseThresholdSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSensor as GeneSizeSensor).cantGrowMorePatienseThreshold = (int)cantGrowMoreTimeSlider.value;
		OnGenomeChanged();
	}

	public override void Update() {
		if (isDirty) {
			base.Update();

			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Size Sensor Panel");
			}

			if (CreatureSelectionPanel.instance.hasSoloSelected && gene != null && affectedGeneSensor != null) {
				ignoreHumanInput = true;
				sizeMoreThanSliderLabel.text = string.Format("A: Size ≥ {0:F0} % ({1:F0} cells or bigger)", (affectedGeneSensor as GeneSizeSensor).sizeThreshold * 100f, CreatureSelectionPanel.instance.soloSelected.CellCountAtCompleteness((affectedGeneSensor as GeneSizeSensor).sizeThreshold));
				sizeLessThanSliderLabel.text = string.Format("B: Size < {0:F0} % ({1:F0} cells or smaller)", (affectedGeneSensor as GeneSizeSensor).sizeThreshold * 100f, CreatureSelectionPanel.instance.soloSelected.CellCountAtCompleteness((affectedGeneSensor as GeneSizeSensor).sizeThreshold) - 1);
				// C: Can grow more
				// D: Can't grow more
				cantGrowMoreTime.text = string.Format("E: Can't grow more for t > {0:F0} s", (affectedGeneSensor as GeneSizeSensor).cantGrowMorePatienseThreshold);
				// F: Embry max size reached

				sizeThresholdSlider.value = (affectedGeneSensor as GeneSizeSensor).sizeThreshold;
				sizeThresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;
				cantGrowMoreTimeSlider.value = (affectedGeneSensor as GeneSizeSensor).cantGrowMorePatienseThreshold;
				cantGrowMoreTimeSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				ignoreHumanInput = false;
			}

			isDirty = false;
		}
	}
}