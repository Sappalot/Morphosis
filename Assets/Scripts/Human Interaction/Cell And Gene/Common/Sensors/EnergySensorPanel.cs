using UnityEngine;
using UnityEngine.UI;

public class EnergySensorPanel : SensorPanel {

	public Text cellEnergyMoreThanLabel;
	public Text cellEnergyLessThanLabel;
	public Text areaEnergyMoreThanLabel;
	public Text areaEnergyLessThanLabel;
	public Text creatureEnergyMoreThanLabel;
	public Text creatureEnergyLessThanLabel;

	public Text areaRadiusSliderLabel;
	public Slider areaRadiusSlider;

	public Slider energyThresholdSlider;

	public void OnEnergyThresholdSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSensor as GeneEnergySensor).threshold = energyThresholdSlider.value;
		OnGenomeChanged();
	}

	public void OnAreaRadiusSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSensor as GeneEnergySensor).areaRadius = (int)areaRadiusSlider.value;
		OnGenomeChanged();
	}

	public override void Update() {
		if (isDirty) {
			base.Update();

			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Energy Sensor Panel");
			}

			if (gene != null && affectedGeneSensor != null) {
				ignoreHumanInput = true;

				cellEnergyMoreThanLabel.text = string.Format("Cl. E ≥ {0:F0} J", (affectedGeneSensor as GeneEnergySensor).threshold);
				cellEnergyLessThanLabel.text = string.Format("Cl. E < {0:F0} J", (affectedGeneSensor as GeneEnergySensor).threshold);

				areaEnergyMoreThanLabel.text = string.Format("Hx. E ≥ {0:F0} J", (affectedGeneSensor as GeneEnergySensor).threshold);
				areaEnergyLessThanLabel.text = string.Format("Hx. E < {0:F0} J", (affectedGeneSensor as GeneEnergySensor).threshold);

				creatureEnergyMoreThanLabel.text = string.Format("Ct. E ≥ {0:F0} J", (affectedGeneSensor as GeneEnergySensor).threshold);
				creatureEnergyLessThanLabel.text = string.Format("Ct. E < {0:F0} J", (affectedGeneSensor as GeneEnergySensor).threshold);

				areaRadiusSliderLabel.text = string.Format("Hexagon radius: {0:F0} m", (affectedGeneSensor as GeneEnergySensor).areaRadius);

				areaRadiusSlider.value = (affectedGeneSensor as GeneEnergySensor).areaRadius;
				areaRadiusSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				energyThresholdSlider.value = (affectedGeneSensor as GeneEnergySensor).threshold;
				energyThresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				ignoreHumanInput = false;
			}

			isDirty = false;
		}
	}
}