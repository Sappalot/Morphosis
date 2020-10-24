using UnityEngine;
using UnityEngine.UI;

// TODO: rename to growth sensor panel
public class SizeSensorPanel : SignalUnitPanel {
	public Text sizeMoreThanSliderLabel; // A
	public Text sizeLessThanSliderLabel; // B
	public Slider sizeThresholdSlider;

	public Text cantGrowMoreTime; // E
	public Slider cantGrowMoreTimeSlider;

	public void OnSizeThresholdSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSignalUnit as GeneSizeSensor).sizeThreshold = sizeThresholdSlider.value;
		OnGenomeChanged();
	}

	public void OnBlockedGrowthPatienseThresholdSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSignalUnit as GeneSizeSensor).cantGrowMorePatienseThreshold = (int)cantGrowMoreTimeSlider.value;
		OnGenomeChanged();
	}

	public override void Update() {
		if (isDirty) {
			base.Update();

			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
				Debug.Log("Update Size Sensor Panel");
			}

			if (isGhost) {
				return; // whole settings panel is allready gone, so we dont need to bother with stuff inside it
			}

			if (CreatureSelectionPanel.instance.hasSoloSelected && selectedGene != null && affectedGeneSignalUnit != null) {
				ignoreHumanInput = true;
				sizeMoreThanSliderLabel.text = string.Format("A: Size ≥ {0:F0} % ({1:F0} cells or bigger)", (affectedGeneSignalUnit as GeneSizeSensor).sizeThreshold * 100f, CreatureSelectionPanel.instance.soloSelected.CellCountAtCompleteness((affectedGeneSignalUnit as GeneSizeSensor).sizeThreshold));
				sizeLessThanSliderLabel.text = string.Format("B: Size < {0:F0} % ({1:F0} cells or smaller)", (affectedGeneSignalUnit as GeneSizeSensor).sizeThreshold * 100f, CreatureSelectionPanel.instance.soloSelected.CellCountAtCompleteness((affectedGeneSignalUnit as GeneSizeSensor).sizeThreshold) - 1);
				// C: Can grow more
				// D: Can't grow more
				cantGrowMoreTime.text = string.Format("E: Can't grow more for t > {0:F0} s", (affectedGeneSignalUnit as GeneSizeSensor).cantGrowMorePatienseThreshold);
				// F: Embry max size reached

				sizeThresholdSlider.value = (affectedGeneSignalUnit as GeneSizeSensor).sizeThreshold;
				sizeThresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;
				cantGrowMoreTimeSlider.value = (affectedGeneSignalUnit as GeneSizeSensor).cantGrowMorePatienseThreshold;
				cantGrowMoreTimeSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				ignoreHumanInput = false;
			}

			isDirty = false;
		}
	}
}