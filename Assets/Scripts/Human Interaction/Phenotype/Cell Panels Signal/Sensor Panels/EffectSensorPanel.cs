using UnityEngine;
using UnityEngine.UI;

public class EffectSensorPanel : SensorPanel {
	public Image output;

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Effect Sensor Panel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					output.color = selectedCell.signal.effectSensor.isOutputOn ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				output.color = ColorScheme.instance.grayedOutPhenotype;
			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}

}