using UnityEngine;
using UnityEngine.UI;

public class EnergySensorPanel : CellSensorPanel {
	public Image output;

	public Text energyThresholdSliderLabel;
	public Slider energyThresholdSlider;

	private GeneEnergySensor affectedGeneEnergySensor;

	public void ConnectToGeneLogic(GeneEnergySensor geneEnergySensor) {
		affectedGeneEnergySensor = geneEnergySensor;
	}

	public void OnEnergyThresholdSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		affectedGeneEnergySensor.threshold = energyThresholdSlider.value;
		ApplyChange();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Effect Sensor Panel");
			}

			if (mode == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					output.color = selectedCell.signal.effectSensor.isOutputOn ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				}
				energyThresholdSlider.interactable = false;
			} else if (mode == PhenoGenoEnum.Genotype) {
				energyThresholdSlider.interactable = IsUnlocked();
			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;
				energyThresholdSliderLabel.text = string.Format("On if energy > {0:F1} J", affectedGeneEnergySensor.threshold);
				energyThresholdSlider.value = affectedGeneEnergySensor.threshold;
				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}