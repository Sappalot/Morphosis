using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySensor : SignalUnit {
	private bool[] output = new bool[6]; // outputs 

	private List<Cell> areaCells = new List<Cell>();

	public float threshold;

	public EnergySensor(SignalUnitEnum signalUnit, Cell hostCell) : base(hostCell) {
		this.signalUnit = signalUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[SignalUnitSlotOutputToIndex(signalUnitSlot)];
	}

	public override void UpdateSignalConnections() {
		areaCells.Clear();
		if (signalUnit == SignalUnitEnum.WorkSensorA && hostCell.GetCellType() == CellTypeEnum.Egg) {
			areaCells = hostCell.creature.phenotype.cellMap.GetCellsInHexagonAroundPosition(hostCell.mapPosition, (hostCell.gene.eggCellFertilizeEnergySensor as GeneEnergySensor).areaRadius);
		} else if (signalUnit == SignalUnitEnum.EnergySensor) {
			areaCells = hostCell.creature.phenotype.cellMap.GetCellsInHexagonAroundPosition(hostCell.mapPosition, (hostCell.gene.energySensor as GeneEnergySensor).areaRadius);
		} 
	}

	public override void ComputeSignalOutput(int deltaTicks) {
		if (signalUnit == SignalUnitEnum.WorkSensorA && hostCell.GetCellType() == CellTypeEnum.Egg) {
			output[0] = hostCell.energy >= (hostCell.gene.eggCellFertilizeEnergySensor as GeneEnergySensor).threshold;
			output[1] = hostCell.energy < (hostCell.gene.eggCellFertilizeEnergySensor as GeneEnergySensor).threshold;

			if (areaCells != null) {
				float averageEnergy = GetAverageEnergy(areaCells);
				output[2] = averageEnergy >= (hostCell.gene.eggCellFertilizeEnergySensor as GeneEnergySensor).threshold;
				output[3] = averageEnergy < (hostCell.gene.eggCellFertilizeEnergySensor as GeneEnergySensor).threshold;
			}

			float creatureEnergy = hostCell.creature.phenotype.energyPerCell;
			output[4] = creatureEnergy >= (hostCell.gene.eggCellFertilizeEnergySensor as GeneEnergySensor).threshold;
			output[5] = creatureEnergy < (hostCell.gene.eggCellFertilizeEnergySensor as GeneEnergySensor).threshold;

		} else if (signalUnit == SignalUnitEnum.EnergySensor) {
			output[0] = hostCell.energy >= (hostCell.gene.energySensor as GeneEnergySensor).threshold;
			output[1] = hostCell.energy < (hostCell.gene.energySensor as GeneEnergySensor).threshold;

			if (areaCells != null) {
				float averageEnergy = GetAverageEnergy(areaCells);
				output[2] = averageEnergy >= (hostCell.gene.energySensor as GeneEnergySensor).threshold;
				output[3] = averageEnergy < (hostCell.gene.energySensor as GeneEnergySensor).threshold;
			}

			float creatureEnergy = hostCell.creature.phenotype.energyPerCell;
			output[4] = creatureEnergy >= (hostCell.gene.energySensor as GeneEnergySensor).threshold;
			output[5] = creatureEnergy < (hostCell.gene.energySensor as GeneEnergySensor).threshold;
		}
		// Other energy sensor
	}

	private float GetAverageEnergy(List<Cell> cells) {
		float energySum = 0f;
		foreach (Cell c in cells) {
			energySum += c.energy;
		}
		return energySum / cells.Count;
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