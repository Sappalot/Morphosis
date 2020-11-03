using UnityEngine;
using UnityEngine.UI;

public class SurroundingSensorCreatureCellFovCovPanel : SurroundingSensorChannelSensorPanel {

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

		((GeneSurroundingSensorChannelCreatureCellFovCov)cellAndGenePanel.gene.surroundingSensor.GeneSensorAtChannelByType(motherPanel.viewedChannel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov)).threshold = thresholdSlider.value;
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
					currentValueLabel.text = string.Format("Creature Cell FOV Coverage: {0:F1} % ", motherPanel.selectedCell.surroundingSensor.CellsByTypeFovCov(motherPanel.viewedChannel) * 100f);
				} else {
					currentValueLabel.text = string.Format("Creature Cell FOV Coverage: -");
				}
			} else /* Genotype */ {
				currentValueLabel.text = string.Format("Creature Cell FOV Coverage: -");
			}

			float threshold = ((GeneSurroundingSensorChannelCreatureCellFovCov)cellAndGenePanel.gene.surroundingSensor.GeneSensorAtChannelByType(motherPanel.viewedChannel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov)).threshold;
			thresholdSliderLabel.text = string.Format("Threshold > {0:F0} %", threshold * 100f);
			thresholdSlider.value = threshold;
			thresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

			ignoreHumanInput = false;

			isDirty = false;
		}
	}
}