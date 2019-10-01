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
	public SignalUnit fertilizeLogicBox = new LogicBox(SignalUnitEnum.WorkLogicBoxA);
	public SignalUnit fertilizeEnergySensor = new EnergySensor(SignalUnitEnum.WorkSensorA); // locked one

	public override void FeedSignal() {
		base.FeedSignal();
		fertilizeLogicBox.FeedSignal();
	}

	public override void ComputeSignalOutputs(int deltaTicks, ulong worldTicks) {
		//TODO: Check with gene if anybody is listening to this output
		base.ComputeSignalOutputs(deltaTicks, worldTicks);
		fertilizeLogicBox.ComputeSignalOutput(this, deltaTicks, worldTicks);
		fertilizeEnergySensor.ComputeSignalOutput(this, deltaTicks, worldTicks);
	}


	public override bool GetOutputFromUnit(SignalUnitEnum outputUnit, SignalUnitSlotEnum outputUnitSlot) {
		if (outputUnit == SignalUnitEnum.WorkLogicBoxA) {
			return fertilizeLogicBox.GetOutput(outputUnitSlot);
		} else if (outputUnit == SignalUnitEnum.WorkSensorA) {
			return fertilizeEnergySensor.GetOutput(outputUnitSlot);
		}
		return base.GetOutputFromUnit(outputUnit, outputUnitSlot); //Couldnt find output unith here in egg work, 
	}
}