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
			}

			base.UpdateCellWork(deltaTicks, worldTicks);
		} else {
			effectProductionInternalUp = 0f;
			effectProductionInternalDown = 0f;
		}
	}

	// Are we going to let cell hibernate or not??
	override public bool IsHibernating() {
		return false;
		//return (gene.eggCellHibernateWhenAttachedToMother && creature.IsAttachedToMotherAlive()) || (gene.eggCellHibernateWhenAttachedToChild && creature.IsAttachedToChildAlive());
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Egg;
	}

	// Signal
	public LogicBox fertilizeLogicBox = new LogicBox(SignalUnitEnum.WorkLogicBoxA);
	public EnergySensor fertilizeEnergySensor = new EnergySensor(SignalUnitEnum.WorkSensorA);
	public AttachmentSensor fertilizeAttachmentSensor = new AttachmentSensor(SignalUnitEnum.WorkSensorB);

	public override void ClearSignal() {
		base.ClearSignal();
		fertilizeLogicBox.Clear();
		fertilizeEnergySensor.Clear();
		fertilizeAttachmentSensor.Clear();
	}

	public override void FeedSignal() {
		base.FeedSignal();
		fertilizeLogicBox.FeedSignal();
	}

	public override void UpdateSignalConnections() {
		base.UpdateSignalConnections();
		fertilizeLogicBox.UpdateSignalConnections(this);
		fertilizeEnergySensor.UpdateSignalConnections(this);
	}

	public override void ComputeSignalOutputs(int deltaTicks) {
		//TODO: Check with gene if anybody is listening to this output
		base.ComputeSignalOutputs(deltaTicks);
		
		fertilizeLogicBox.ComputeSignalOutput(this, deltaTicks);
		fertilizeEnergySensor.ComputeSignalOutput(this, deltaTicks);
		fertilizeAttachmentSensor.ComputeSignalOutput(this, deltaTicks);
	}

	public override bool GetOutputFromUnit(SignalUnitEnum outputUnit, SignalUnitSlotEnum outputUnitSlot) {
		if (outputUnit == SignalUnitEnum.WorkLogicBoxA) {
			return fertilizeLogicBox.GetOutput(outputUnitSlot);
		} else if (outputUnit == SignalUnitEnum.WorkSensorA) {
			return fertilizeEnergySensor.GetOutput(outputUnitSlot);
		} else if (outputUnit == SignalUnitEnum.WorkSensorB) {
			return fertilizeAttachmentSensor.GetOutput(outputUnitSlot);
		}
		return base.GetOutputFromUnit(outputUnit, outputUnitSlot); //Couldnt find output unith here in egg work, 
	}
}