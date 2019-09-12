using UnityEngine;
using UnityEngine.UI;

public class EffectSensorPanel : CellSensorPanel {
	public Image outputImage;

	public Text effectThresholdSliderLabel;
	public Slider effectThresholdSlider;

	private GeneEffectSensor affectedGeneEffectSensor;

	public void ConnectToGeneLogic(GeneEffectSensor geneEffectSensor) {
		affectedGeneEffectSensor = geneEffectSensor;
	}

	public void OnEffectThresholdSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		affectedGeneEffectSensor.threshold = effectThresholdSlider.value;
		ApplyChange();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Effect Sensor Panel");
			}

			if (mode == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					outputImage.color = selectedCell.GetOutputFromUnit(outputUnit, SignalUnitSlotEnum.Whatever) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				}
				effectThresholdSlider.interactable = false;
			} else if (mode == PhenoGenoEnum.Genotype) {
				effectThresholdSlider.interactable = IsUnlocked();
			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;
				effectThresholdSliderLabel.text = string.Format("If effect > {0:F1} J", affectedGeneEffectSensor.threshold);
				effectThresholdSlider.value = affectedGeneEffectSensor.threshold;
				ignoreSliderMoved = false;
			}
			isDirty = false;
		}
	}
}