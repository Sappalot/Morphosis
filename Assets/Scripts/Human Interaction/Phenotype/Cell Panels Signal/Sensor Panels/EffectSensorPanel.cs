using UnityEngine;
using UnityEngine.UI;

public class EffectSensorPanel : SensorPanel {
	public Image output;

	public Text effectThresholdSliderLabel;
	public Slider effectThresholdSlider;

	public Text radiusSliderLabel;
	public Slider radiusSlider;

	public void OnEffectThresholdSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.effectSensorThresholdEffect = effectThresholdSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Effect Sensor Panel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					output.color = selectedCell.signal.effectSensor.isOutputOn ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				}

				effectThresholdSliderLabel.color = ColorScheme.instance.grayedOutGenotype;
				effectThresholdSlider.interactable = false;

				radiusSliderLabel.color = ColorScheme.instance.grayedOutGenotype;
				radiusSlider.interactable = false;
				

			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				output.color = ColorScheme.instance.grayedOutPhenotype;

				effectThresholdSliderLabel.color = Color.black;
				effectThresholdSlider.interactable = IsUnlocked();

				radiusSliderLabel.color = Color.black;
				radiusSlider.interactable = IsUnlocked();
			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;

				effectThresholdSliderLabel.text = string.Format("On if effect > {0:F1} W", selectedGene.effectSensorThresholdEffect);
				effectThresholdSlider.value = selectedGene.effectSensorThresholdEffect;

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}