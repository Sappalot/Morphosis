using UnityEngine;
using UnityEngine.UI;

public class SurroundingSensorTerrainRockFovCovPanel : SurroundingSensorChannelSensorPanel {

	public Text currentValueLabel;
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

		((GeneSurroundingSensorChannelTerrainRockFovCov)cellAndGenePanel.gene.surroundingSensor.GeneSensorAtChannelByType(motherPanel.viewedChannel, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov)).threshold = thresholdSlider.value;
		OnGenomeChanged();
	}

	private void Update() {
		if (isDirty) {

			if (!CreatureSelectionPanel.instance.hasSoloSelected) {
				return;
			}

			ignoreHumanInput = true;
			if (mode == PhenoGenoEnum.Phenotype) {
				if (!motherPanel.isGhost && motherPanel.selectedCell != null && motherPanel.selectedCell.surroundingSensor != null && motherPanel.selectedCell.surroundingSensor.rootnessEnum == RootnessEnum.Rooted) {
					currentValueLabel.text = string.Format("Coverage: {0:F1} % ", motherPanel.selectedCell.surroundingSensor.TerrainRockFovCov(motherPanel.viewedChannel) * 100f);
				} else {
					currentValueLabel.text = string.Format("Coverage: -");
				}
			} else /* Genotype */ {
				currentValueLabel.text = string.Format("Coverage: -");
			}

			float threshold = ((GeneSurroundingSensorChannelTerrainRockFovCov)cellAndGenePanel.gene.surroundingSensor.GeneSensorAtChannelByType(motherPanel.viewedChannel, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov)).threshold;
			thresholdSliderLabel.text = string.Format("On when coverage > {0:F0} %", threshold * 100f);
			thresholdSlider.value = threshold;
			thresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

			ignoreHumanInput = false;

			isDirty = false;
		}
	}
}