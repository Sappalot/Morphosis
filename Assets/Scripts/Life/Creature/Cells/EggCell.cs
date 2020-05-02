using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class EggCell : Cell {

	// ... Signal ...
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
		fertilizeEnergySensor.ComputeSignalOutput(deltaTicks);
		fertilizeAttachmentSensor.ComputeSignalOutput(deltaTicks);
	}

	public override SignalUnit GetSignalUnit(SignalUnitEnum signalUnit) {
		if (signalUnit == SignalUnitEnum.WorkLogicBoxA) {
			return fertilizeLogicBox;
		} else if (signalUnit == SignalUnitEnum.WorkSensorA) {
			return fertilizeEnergySensor;
		} else if (signalUnit == SignalUnitEnum.WorkSensorB) {
			return fertilizeAttachmentSensor;
		}

		return base.GetSignalUnit(signalUnit);
	}

	// ^ Signal ^

	public override void Initialize(PhenoGenoEnum phenoGeno) {
		base.Initialize(phenoGeno);
		fertilizeLogicBox = new LogicBox(SignalUnitEnum.WorkLogicBoxA, this);
		fertilizeEnergySensor = new EnergySensor(SignalUnitEnum.WorkSensorA, this);
		fertilizeAttachmentSensor = new AttachmentSensor(SignalUnitEnum.WorkSensorB, this);
	}

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		base.UpdateCellWork(deltaTicks, worldTicks);

		if (PhenotypePhysicsPanel.instance.functionEgg.isOn) {
			effectProductionInternalUp = 0f;
			effectProductionInternalDown = GlobalSettings.instance.phenotype.eggCell.effectProductionDown;

			// Fertilization is made in Creature.UpdateFertilize(...)
		} else {
			effectProductionInternalUp = 0f;
			effectProductionInternalDown = 0f;
		}
	}

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Egg;
	}
}