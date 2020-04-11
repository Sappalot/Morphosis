using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSensor : SignalUnit {
	private bool[] output = new bool[6]; // outputs 

	private List<Cell> areaCells = new List<Cell>();

	public float threshold;

	public EffectSensor(SignalUnitEnum signalUnit, Cell hostCell) : base(hostCell) {
		base.signalUnit = signalUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[SignalUnitSlotOutputToIndex(signalUnitSlot)];
	}

	public override void UpdateSignalConnections() {
		areaCells.Clear();
		if (signalUnit == SignalUnitEnum.EffectSensor) { // redundant check ? 
			areaCells = hostCell.creature.phenotype.cellMap.GetCellsInHexagonAroundPosition(hostCell.mapPosition, (hostCell.gene.effectSensor as GeneEffectSensor).usedAreaRadius);
		}
	}

	public override void ComputeSignalOutput(int deltaTicks) {
		if (signalUnit == SignalUnitEnum.EffectSensor) { // redundant check ? 
			if (!hostCell.gene.effectSensor.isUsedInternal) {
				return;
			}

			output[0] = hostCell.Effect((hostCell.gene.effectSensor as GeneEffectSensor).effectMeassure) >= (hostCell.gene.effectSensor as GeneEffectSensor).usedThreshold;
			output[1] = hostCell.Effect((hostCell.gene.effectSensor as GeneEffectSensor).effectMeassure) < (hostCell.gene.effectSensor as GeneEffectSensor).usedThreshold;

			if (areaCells != null) {
				float averageEffect = GetAverageEffect(areaCells, (hostCell.gene.effectSensor as GeneEffectSensor).effectMeassure);
				output[2] = averageEffect >= (hostCell.gene.effectSensor as GeneEffectSensor).usedThreshold;
				output[3] = averageEffect < (hostCell.gene.effectSensor as GeneEffectSensor).usedThreshold;
			}

			float creatureEnergy = hostCell.creature.phenotype.EffectPerCell((hostCell.gene.effectSensor as GeneEffectSensor).effectMeassure);
			output[4] = creatureEnergy >= (hostCell.gene.effectSensor as GeneEffectSensor).usedThreshold;
			output[5] = creatureEnergy < (hostCell.gene.effectSensor as GeneEffectSensor).usedThreshold;
		}
	}

	private float GetAverageEffect(List<Cell> cells, EffectMeassureEnum effectMeassure) {
		float effectSum = 0f;
		foreach (Cell c in cells) {
			effectSum += c.Effect(effectMeassure);
		}
		return effectSum / cells.Count;
	}

	public override void Clear() {
		for (int i = 0; i < output.Length; i++) {
			output[i] = false;
		}
	}

	// Load Save
	private CommonSensorData sensorData = new CommonSensorData();

	// Save
	public CommonSensorData UpdateData() {
		sensorData.slotA = output[0];
		sensorData.slotB = output[1];
		sensorData.slotC = output[2];
		sensorData.slotD = output[3];
		sensorData.slotE = output[4];
		sensorData.slotF = output[5];
		return sensorData;
	}

	// Load
	public void ApplyData(CommonSensorData sensorData) {
		output[0] = sensorData.slotA;
		output[1] = sensorData.slotB;
		output[2] = sensorData.slotC;
		output[3] = sensorData.slotD;
		output[4] = sensorData.slotE;
		output[5] = sensorData.slotF;
	}
}