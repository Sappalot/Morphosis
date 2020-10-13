using UnityEngine;
using UnityEngine.UI;

public class SurroundingSensorPanel : SignalUnitPanel {


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

	public void OnAreaRadiusSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		//(affectedGeneSignalUnit as GeneEffectSensor).usedAreaRadius = (int)areaRadiusSlider.value;

		OnGenomeChanged();
	}

	public override void Update() {
		if (isDirty) {
			base.Update();

			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
				DebugUtil.Log("Update Surrounding Sensor Panel");
			}

			//effectMeasuredDropdown.interactable = IsUnlocked();

			if (selectedGene != null && affectedGeneSignalUnit != null) {
				ignoreHumanInput = true;

				//effectMeasuredDropdown.value = (int)(affectedGeneSignalUnit as GeneEffectSensor).effectMeassure;

				//cellEffectMoreThanLabel.text = string.Format("Cl. P ≥ {0:F1} W", (affectedGeneSignalUnit as GeneEffectSensor).usedThreshold);

				//effectThresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				ignoreHumanInput = false;
			}

			isDirty = false;
		}
	}
}