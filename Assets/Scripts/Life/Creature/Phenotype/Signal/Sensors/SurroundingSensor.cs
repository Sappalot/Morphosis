using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurroundingSensor : SignalUnit {
	private bool[] output = new bool[6]; // outputs 

	public float fieldOfView {
		get {
			return (hostCell.gene.surroundingSensor as GeneSurroundingSensor).fieldOfView;
		}
	}

	public float direction {
		get {
			return (hostCell.gene.surroundingSensor as GeneSurroundingSensor).direction;
		}
	}

	public float rangeNear {
		get {
			return (hostCell.gene.surroundingSensor as GeneSurroundingSensor).rangeNear;
		}
	}

	public float rangeFar {
		get {
			return (hostCell.gene.surroundingSensor as GeneSurroundingSensor).rangeFar;
		}
	}

	public SurroundingSensor(SignalUnitEnum signalUnit, Cell hostCell) : base(hostCell) {
		base.signalUnitEnum = signalUnit;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		return output[SignalUnitSlotOutputToIndex(signalUnitSlot)];
	}

	public override void ComputeSignalOutput(int deltaTicks) {
		if (signalUnitEnum == SignalUnitEnum.SurroundingSensor) { // redundant check ? 
																  //if (!hostCell.gene.effectSensor.isRooted) {
																  //	return;
																  //}

			output[0] = false; //hostCell.Effect((hostCell.gene.effectSensor as GeneEffectSensor).effectMeassure) >= (hostCell.gene.effectSensor as GeneEffectSensor).usedThreshold;
			output[1] = true; //hostCell.Effect((hostCell.gene.effectSensor as GeneEffectSensor).effectMeassure) < (hostCell.gene.effectSensor as GeneEffectSensor).usedThreshold;


			// TODO: raycast world and update output according to settings

			//if (areaCells != null) {
			//	float averageEffect = GetAverageEffect(areaCells, (hostCell.gene.effectSensor as GeneEffectSensor).effectMeassure);
			//	output[2] = averageEffect >= (hostCell.gene.effectSensor as GeneEffectSensor).usedThreshold;
			//	output[3] = averageEffect < (hostCell.gene.effectSensor as GeneEffectSensor).usedThreshold;
			//}

			//float creatureEnergy = hostCell.creature.phenotype.EffectPerCell((hostCell.gene.effectSensor as GeneEffectSensor).effectMeassure);
			//output[4] = creatureEnergy >= (hostCell.gene.effectSensor as GeneEffectSensor).usedThreshold;
			//output[5] = creatureEnergy < (hostCell.gene.effectSensor as GeneEffectSensor).usedThreshold;
		}
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