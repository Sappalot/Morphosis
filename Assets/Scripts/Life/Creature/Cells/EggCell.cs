using UnityEngine;

public class EggCell : Cell {

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		if (PhenotypePhysicsPanel.instance.functionEgg.isOn) {
			if (IsHibernating()) {
				effectProductionInternalUp = 0f;
				effectProductionInternalDown = GlobalSettings.instance.phenotype.cellHibernateEffectCost;
			} else {
				effectProductionInternalUp = 0f;
				effectProductionInternalDown = GlobalSettings.instance.phenotype.eggCellEffectCost;
				if (energyFullness > gene.eggCellFertilizeThreshold) {
					shouldFertilize = true;
				}
			}

			base.UpdateCellWork(deltaTicks, worldTicks);
		} else {
			effectProductionInternalUp = 0f;
			effectProductionInternalDown = 0f;
		}
	}

	override public bool IsHibernating() {
		return (gene.eggCellHibernateWhenAttachedToMother && creature.IsAttachedToMotherAlive()) || (gene.eggCellHibernateWhenAttachedToChild && creature.IsAttachedToChildAlive());
	}

	[HideInInspector]
	public bool shouldFertilize = false;

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Egg;
	}

	// Signal

	public SignalUnit fertilizeEnergySensor = new EnergySensor(SignalUnitEnum.WorkSensorA); // locked one
	public SignalUnit fertilizeLogicBox = new LogicBox(SignalUnitEnum.WorkLogicBoxA);
	public SignalUnit effectSensor = new EffectSensor(SignalUnitEnum.EffectSensor);

	public override void UpdateCellSignal(int deltaTicks, ulong worldTicks) {
		//TODO: Check with gene if anybody is listening to this output
		fertilizeEnergySensor.UpdateOutputs(this, deltaTicks, worldTicks);
		fertilizeLogicBox.UpdateOutputs(this, deltaTicks, worldTicks);

		effectSensor.UpdateOutputs(this, deltaTicks, worldTicks);
	}


	public override bool GetOutputFromUnit(SignalUnitEnum outputUnit, SignalUnitSlotEnum outputUnitSlot) {
		if (outputUnit == SignalUnitEnum.WorkLogicBoxA) {
			return fertilizeLogicBox.GetOutput(outputUnitSlot);
		} else if (outputUnit == SignalUnitEnum.WorkSensorA) {
			return fertilizeEnergySensor.GetOutput(outputUnitSlot);
		} else if (outputUnit == SignalUnitEnum.EffectSensor) {
			return effectSensor.GetOutput(outputUnitSlot);
		}
		return base.GetOutputFromUnit(outputUnit, outputUnitSlot); //Couldnt find output unith here in egg work, 
	}
}