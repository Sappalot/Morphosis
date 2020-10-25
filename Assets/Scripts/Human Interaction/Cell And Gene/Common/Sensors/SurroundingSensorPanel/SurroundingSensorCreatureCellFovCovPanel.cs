using UnityEngine;
using UnityEngine.UI;

public class SurroundingSensorCreatureCellFovCovPanel : SurroundingSensorChannelSensorPanel {

	public Text thresholdSliderLabel;
	public Slider thresholdSlider;

	override public void Initialize(PhenoGenoEnum mode, CellAndGenePanel cellAndGenePanel, SurroundingSensorPanel motherPanel) {
		base.Initialize(mode, cellAndGenePanel, motherPanel);
		MakeDirty();
	}

	public void OnThresholdSliderMoved() {
		if (ignoreHumanInput || mode == PhenoGenoEnum.Phenotype) {
			return;
		}

		((GeneSurroundingSensorChannelCreatureCellFovCov)cellAndGenePanel.gene.surroundingSensor.GetGeneSensorChannel(motherPanel.viewedChannel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov)).threshold = thresholdSlider.value;
		OnGenomeChanged();
	}

	private void Update() {
		if (isDirty) {

			if (!CreatureSelectionPanel.instance.hasSoloSelected) {
				return;
			}

			ignoreHumanInput = true;

			float threshold = ((GeneSurroundingSensorChannelCreatureCellFovCov)cellAndGenePanel.gene.surroundingSensor.GetGeneSensorChannel(motherPanel.viewedChannel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov)).threshold;
			thresholdSliderLabel.text = string.Format("Creature Cell FOV Coverage > {0:F0} %", threshold * 100f);
			thresholdSlider.value = threshold;
			thresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

			ignoreHumanInput = false;

			isDirty = false;
		}
	}
}