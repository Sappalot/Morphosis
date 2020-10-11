using UnityEngine;
using UnityEngine.UI;

public class EnergySensorPanel : SignalUnitPanel {

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

		(affectedGeneSignalUnit as GeneEnergySensor).threshold = energyThresholdSlider.value;
		OnGenomeChanged();
	}

	public void OnAreaRadiusSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSignalUnit as GeneEnergySensor).areaRadius = (int)areaRadiusSlider.value;
		OnGenomeChanged();
	}

	public override void Update() {
		if (isDirty) {
			base.Update();

			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
				DebugUtil.Log("Update Energy Sensor Panel");
			}

			if (selectedGene != null && affectedGeneSignalUnit != null) {
				ignoreHumanInput = true;

				cellEnergyMoreThanLabel.text = string.Format("Cl. E ≥ {0:F0} J", (affectedGeneSignalUnit as GeneEnergySensor).threshold);
				cellEnergyLessThanLabel.text = string.Format("Cl. E < {0:F0} J", (affectedGeneSignalUnit as GeneEnergySensor).threshold);

				areaEnergyMoreThanLabel.text = string.Format("Hx. E ≥ {0:F0} J", (affectedGeneSignalUnit as GeneEnergySensor).threshold);
				areaEnergyLessThanLabel.text = string.Format("Hx. E < {0:F0} J", (affectedGeneSignalUnit as GeneEnergySensor).threshold);

				creatureEnergyMoreThanLabel.text = string.Format("Ct. E ≥ {0:F0} J", (affectedGeneSignalUnit as GeneEnergySensor).threshold);
				creatureEnergyLessThanLabel.text = string.Format("Ct. E < {0:F0} J", (affectedGeneSignalUnit as GeneEnergySensor).threshold);

				areaRadiusSliderLabel.text = string.Format("Hexagon radius: {0:F0} m", (affectedGeneSignalUnit as GeneEnergySensor).areaRadius);

				areaRadiusSlider.value = (affectedGeneSignalUnit as GeneEnergySensor).areaRadius;
				areaRadiusSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				energyThresholdSlider.value = (affectedGeneSignalUnit as GeneEnergySensor).threshold;
				energyThresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				ignoreHumanInput = false;
			}

			isDirty = false;
		}
	}
}