using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSensor : Sensor {
	public bool isOutputOn { get; private set; }

	public override SensorTypeEnum GetSensorType() {
		return SensorTypeEnum.Effect;
	}

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		isOutputOn = cell.GetEffect(true, true, true, true) > cell.gene.effectSensorThresholdEffect;
	}

	public bool IsUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;
	}

	public Color IsUnlockedColor() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome ? Color.black : ColorScheme.instance.grayedOut;
	}

	public void MakeCreatureChanged() {
		CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
		CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
		CreatureSelectionPanel.instance.soloSelected.generation = 1;
	}
}