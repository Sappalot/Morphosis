using UnityEngine;

public class EggCell : Cell {

	private new void Awake() {
		fertilizeLogicBox = new LogicBox(SignalUnitEnum.WorkLogicBoxA, this);
		fertilizeEnergySensor = new EnergySensor(SignalUnitEnum.WorkSensorA, this);
		fertilizeAttachmentSensor = new AttachmentSensor(SignalUnitEnum.WorkSensorB, this);
		base.Awake();
	}

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		if (PhenotypePhysicsPanel.instance.functionEgg.isOn) {
			effectProductionInternalUp = 0f;
			effectProductionInternalDown = GlobalSettings.instance.phenotype.eggCell.effectProductionDown;

			// Fertilization is made in Creature.UpdateFertilize(...)

			base.UpdateCellWork(deltaTicks, worldTicks);
		} else {
			effectProductionInternalUp = 0f;
			effectProductionInternalDown = 0f;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Egg;
	}

	// Signal
	public LogicBox fertilizeLogicBox;
	public EnergySensor fertilizeEnergySensor;
	public AttachmentSensor fertilizeAttachmentSensor;

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
		fertilizeLogicBox.UpdateSignalConnections();
		fertilizeEnergySensor.UpdateSignalConnections();
	}

	public override void ComputeSignalOutputs(int deltaTicks) {
		//TODO: Check with gene if anybody is listening to this output
		base.ComputeSignalOutputs(deltaTicks);
		
		fertilizeLogicBox.ComputeSignalOutput(deltaTicks);
		fertilizeEnergySensor.ComputeSignalOutput( deltaTicks);
		fertilizeAttachmentSensor.ComputeSignalOutput( deltaTicks);
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