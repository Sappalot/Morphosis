using UnityEngine;
using UnityEngine.UI;

public class SensorPanel : CellAndGeneSignalUnitPanel {
	public SensorOutputPanel[] outputPanels;

	public Text energyThresholdSliderLabel;
	public Slider energyThresholdSlider;

	public GeneSignalUnit affectedGeneSensor {
		get {
			if (selectedGene.type == CellTypeEnum.Egg && signalUnit == SignalUnitEnum.WorkSensorA) {
				return selectedGene.eggCellFertilizeEnergySensor;
			}
			return null;
		}
	}

	override public void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit) {
		base.Initialize(mode, signalUnit);
		for (int i = 0; i < outputPanels.Length; i++) {
			outputPanels[i].Initialize(mode, signalUnit, (SignalUnitSlotEnum)i);
		}
		
	}

	public void OnEnergyThresholdSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		(affectedGeneSensor as GeneEnergySensor).threshold = energyThresholdSlider.value;
		ApplyChange();
	}

	public void OnClickedOutputButtonA() {
		if (MouseAction.instance.actionState == MouseActionStateEnum.selectSignalOutput && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			LogicBoxInputPanel.AnswerSetReference(affectedGeneSensor.signalUnit, 0);
			MouseAction.instance.actionState = MouseActionStateEnum.free;
		}
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Energy Sensor Panel");
			}

			//if (mode == PhenoGenoEnum.Phenotype) {
			//	if (CellPanel.instance.selectedCell != null) {
			//		outputImage.color = selectedCell.GetOutputFromUnit(signalUnit, SignalUnitSlotEnum.Whatever) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
			//	}
			//	energyThresholdSlider.interactable = false;
			//} else if (mode == PhenoGenoEnum.Genotype) {
			//	energyThresholdSlider.interactable = IsUnlocked();
			//}

			if (selectedGene != null) { 
				ignoreSliderMoved = true;
				energyThresholdSliderLabel.text = string.Format("On if energy > {0:F1} J", (affectedGeneSensor as GeneEnergySensor).threshold);
				energyThresholdSlider.value = (affectedGeneSensor as GeneEnergySensor).threshold;
				ignoreSliderMoved = false;
			}

			foreach (SensorOutputPanel output in outputPanels) {
				output.MakeDirty();
			}

			isDirty = false;
		}
	}
}