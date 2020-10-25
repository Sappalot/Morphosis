using UnityEngine;
using UnityEngine.UI;

public class SurroundingSensorTerrainRockFovCovPanel : SurroundingSensorChannelSensorPanel {
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

		((GeneSurroundingSensorChannelTerrainRockFovCov)cellAndGenePanel.gene.surroundingSensor.GetGeneSensorChannel(motherPanel.viewedChannel, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov)).threshold = thresholdSlider.value;
		OnGenomeChanged();
	}

	private void Update() {
		if (isDirty) {

			if (!CreatureSelectionPanel.instance.hasSoloSelected) {
				return;
			}

			ignoreHumanInput = true;

			float threshold = ((GeneSurroundingSensorChannelTerrainRockFovCov)cellAndGenePanel.gene.surroundingSensor.GetGeneSensorChannel(motherPanel.viewedChannel, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov)).threshold;
			thresholdSliderLabel.text = string.Format("Terrain Rock FOV Coverage > {0:F0} %", threshold * 100f);
			thresholdSlider.value = threshold;
			thresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

			ignoreHumanInput = false;

			isDirty = false;
		}
	}
}