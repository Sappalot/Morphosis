using UnityEngine;
using UnityEngine.UI;

public class EnergySensorPanel : SensorPanel {
	public Text energyThresholdSliderLabel;
	public Slider energyThresholdSlider;

	public void OnEnergyThresholdSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		(affectedGeneSensor as GeneEnergySensor).threshold = energyThresholdSlider.value;
		ApplyChange();
	}

	public override void Update() {
		if (isDirty) {
			base.Update();

			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Energy Sensor Panel");
			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;
				energyThresholdSliderLabel.text = string.Format("On if energy > {0:F1} J", (affectedGeneSensor as GeneEnergySensor).threshold);
				energyThresholdSlider.value = (affectedGeneSensor as GeneEnergySensor).threshold;
				energyThresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;
				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}