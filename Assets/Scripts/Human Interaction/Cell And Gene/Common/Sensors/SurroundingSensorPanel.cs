using UnityEngine;
using UnityEngine.UI;

public class SurroundingSensorPanel : SignalUnitPanel {
	public Text outputLabelA;
	public Text outputLabelB;
	public Text outputLabelC;
	public Text outputLabelD;
	public Text outputLabelE;
	public Text outputLabelF;

	public Text directionSliderLabel;
	public Slider directionSlider;

	public Text fieldOfViewSliderLabel;
	public Slider fieldOfViewSlider;

	public Text rangeFarSliderLabel;
	public Slider rangeFarSlider;

	public Text rangeNearSliderLabel;
	public Slider rangeNearSlider;

	public override void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit, CellAndGenePanel cellAndGenePanel) {
		base.Initialize(mode, signalUnit, cellAndGenePanel);
	}

	public void OnDropdownMeasureChanged() {
		if (ignoreHumanInput) {
			return;
		}

		//(affectedGeneSignalUnit as GeneEffectSensor).effectMeassure = (EffectMeassureEnum)effectMeasuredDropdown.value;

		OnGenomeChanged();
	}

	public void OnDirectionSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSignalUnit as GeneSurroundingSensor).direction = (float)directionSlider.value;
		OnGenomeChanged();
	}

	public void OnFieldOfViewSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSignalUnit as GeneSurroundingSensor).fieldOfView = (float)fieldOfViewSlider.value;
		OnGenomeChanged();
	}

	public void OnRangeFarSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSignalUnit as GeneSurroundingSensor).rangeFar = (float)rangeFarSlider.value;
		OnGenomeChanged();
	}

	public void OnRangeNearSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSignalUnit as GeneSurroundingSensor).rangeNear = (float)rangeNearSlider.value;
		OnGenomeChanged();
	}

	public override void Update() {
		if (isDirty) {
			base.Update();

			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
				DebugUtil.Log("Update Surrounding Sensor Panel");
			}

			if (selectedGene != null && affectedGeneSignalUnit != null) {
				ignoreHumanInput = true;

				directionSliderLabel.text = string.Format("Direction: {0:F1} °", (affectedGeneSignalUnit as GeneSurroundingSensor).direction);
				directionSlider.value = (affectedGeneSignalUnit as GeneSurroundingSensor).direction;
				directionSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				fieldOfViewSliderLabel.text = string.Format("Field Of View: {0:F1} °", (affectedGeneSignalUnit as GeneSurroundingSensor).fieldOfView);
				fieldOfViewSlider.value = (affectedGeneSignalUnit as GeneSurroundingSensor).fieldOfView;
				fieldOfViewSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				rangeFarSliderLabel.text = string.Format("Range far: {0:F1} m", (affectedGeneSignalUnit as GeneSurroundingSensor).rangeFar);
				rangeFarSlider.value = (affectedGeneSignalUnit as GeneSurroundingSensor).rangeFar;
				rangeFarSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				rangeNearSliderLabel.text = string.Format("Range near: {0:F1} m", (affectedGeneSignalUnit as GeneSurroundingSensor).rangeNear);
				rangeNearSlider.value = (affectedGeneSignalUnit as GeneSurroundingSensor).rangeNear;
				rangeNearSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				ignoreHumanInput = false;
			}

			isDirty = false;
		}
	}
}