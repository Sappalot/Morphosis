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

	public Image measureDropdownImageShow;
	public Image measureDropdownImageList;

	public override void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit, CellAndGenePanel cellAndGenePanel) {
		base.Initialize(mode, signalUnit, cellAndGenePanel);
		measureDropdownImageShow.color = ColorScheme.instance.selectedChanged;
		measureDropdownImageList.color = ColorScheme.instance.selectedChanged;
	}

	public void OnDropdownMeasureChanged() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSensor as GeneEffectSensor).effectMeassure = (EffectMeassureEnum)effectMeasuredDropdown.value;

		OnGenomeChanged();
	}

	public void OnAreaRadiusSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSensor as GeneEffectSensor).usedAreaRadius = (int)areaRadiusSlider.value;

		OnGenomeChanged();
	}

	public void OnEffectThresholdSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSensor as GeneEffectSensor).usedThreshold = effectThresholdSlider.value;

		OnGenomeChanged();
	}


	public override void Update() {
		if (isDirty) {
			base.Update();

			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Effect Sensor Panel");
			}

			effectMeasuredDropdown.interactable = IsUnlocked();

			if (gene != null && affectedGeneSensor != null) {
				ignoreHumanInput = true;

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

				ignoreHumanInput = false;
			}

			isDirty = false;
		}
	}
}