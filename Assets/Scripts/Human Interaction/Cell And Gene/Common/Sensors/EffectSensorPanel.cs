using UnityEngine;
using UnityEngine.UI;

public class EffectSensorPanel : SensorPanel {

	public Text cellEffectMoreThanLabel;
	public Text cellEffectLessThanLabel;

	public Text areaEffectMoreThanLabel;
	public Text areaEffectLessThanLabel;

	public Text creatureEffectMoreThanLabel;
	public Text creatureEffectLessThanLabel;

	public Text areaRadiusSliderLabel;
	public Slider areaRadiusSlider;

	public Dropdown effectMeasuredDropdown;

	public Slider effectThresholdSlider;

	public void OnDropdownMeasureChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		(affectedGeneSensor as GeneEffectSensor).effectMeassure = (EffectMeassureEnum)effectMeasuredDropdown.value;

		OnGenomeChanged(false);
	}

	public void OnAreaRadiusSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		(affectedGeneSensor as GeneEffectSensor).usedAreaRadius = (int)areaRadiusSlider.value;

		OnGenomeChanged(false);
	}

	public void OnEffectThresholdSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		(affectedGeneSensor as GeneEffectSensor).usedThreshold = effectThresholdSlider.value;

		OnGenomeChanged(false);
	}


	public override void Update() {
		if (isDirty) {
			base.Update();

			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Effect Sensor Panel");
			}

			if (selectedGene != null && affectedGeneSensor != null) {
				ignoreSliderMoved = true;

				effectMeasuredDropdown.value = (int)(affectedGeneSensor as GeneEffectSensor).effectMeassure;

				cellEffectMoreThanLabel.text = string.Format("Cl. P ≥ {0:F1} W", (affectedGeneSensor as GeneEffectSensor).usedThreshold);
				cellEffectLessThanLabel.text = string.Format("Cl. P < {0:F1} W", (affectedGeneSensor as GeneEffectSensor).usedThreshold);

				areaEffectMoreThanLabel.text = string.Format("Hx. P ≥ {0:F1} W", (affectedGeneSensor as GeneEffectSensor).usedThreshold);
				areaEffectLessThanLabel.text = string.Format("Hx. P < {0:F1} W", (affectedGeneSensor as GeneEffectSensor).usedThreshold);

				creatureEffectMoreThanLabel.text = string.Format("Ct. P ≥ {0:F1} W", (affectedGeneSensor as GeneEffectSensor).usedThreshold);
				creatureEffectLessThanLabel.text = string.Format("Ct. P < {0:F1} W", (affectedGeneSensor as GeneEffectSensor).usedThreshold);

				areaRadiusSliderLabel.text = string.Format("Hexagon radius: {0:F0} m", (affectedGeneSensor as GeneEffectSensor).usedAreaRadius);

				areaRadiusSlider.value = (affectedGeneSensor as GeneEffectSensor).usedAreaRadius;
				areaRadiusSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				effectThresholdSlider.value = (affectedGeneSensor as GeneEffectSensor).usedThreshold;
				effectThresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}