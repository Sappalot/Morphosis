using System.Collections.Generic;

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

	public override void PreUpdateNervesGenotype() {
		base.PreUpdateNervesGenotype();
		fertilizeLogicBox.PreUpdateNervesGenotype();
		fertilizeEnergySensor.PreUpdateNervesGenotype();
		fertilizeAttachmentSensor.PreUpdateNervesGenotype();
	}

	public override void UpdateInputNervesGenotype(Genotype genotype) {
		base.UpdateInputNervesGenotype(genotype);
		fertilizeLogicBox.UpdateInputNervesGenotype(genotype);
		//fertilizeEnergySensor, no input
		//fertilizeAttachmentSensor, no input
	}

	public override void UpdateConnectionsNervesGenotypePhenotype(bool addOutputNere) {
		base.UpdateConnectionsNervesGenotypePhenotype(addOutputNere);
		fertilizeLogicBox.RootRecursivlyGenotypePhenotype(null, addOutputNere); // root
	}

	public override List<Nerve> GetAllNervesGenotypePhenotype() {
		List<Nerve> nerves = new List<Nerve>();
		nerves.AddRange(base.GetAllNervesGenotypePhenotype());

		nerves.AddRange(fertilizeLogicBox.GetAllNervesGenotypePhenotype());
		nerves.AddRange(fertilizeEnergySensor.GetAllNervesGenotypePhenotype());
		nerves.AddRange(fertilizeAttachmentSensor.GetAllNervesGenotypePhenotype());

		return nerves;
	}

	public override float Transparency() {
		return GlobalSettings.instance.phenotype.eggCell.transparency;
	}

	//--

	// no need to override
	//PreUpdateNervesPhenotype...


	public override void CloneNervesFromGenotypeToPhenotype(Cell geneCell, Phenotype phenotype) {
		base.CloneNervesFromGenotypeToPhenotype(geneCell, phenotype);

		if (((EggCell)geneCell).fertilizeLogicBox.rootnessEnum == RootnessEnum.Rooted) {
			fertilizeLogicBox.CloneNervesFromGenotypeToPhenotype(geneCell, phenotype);
		}

		if (((EggCell)geneCell).fertilizeEnergySensor.rootnessEnum == RootnessEnum.Rooted) {
			fertilizeEnergySensor.CloneNervesFromGenotypeToPhenotype(geneCell, phenotype);
		}

		if (((EggCell)geneCell).fertilizeAttachmentSensor.rootnessEnum == RootnessEnum.Rooted) {
			fertilizeAttachmentSensor.CloneNervesFromGenotypeToPhenotype(geneCell, phenotype);
		}
	}

	public override void UpdateRootable(Cell geneCell) {
		base.UpdateRootable(geneCell);
		fertilizeLogicBox.rootnessEnum = ((EggCell)geneCell).fertilizeLogicBox.rootnessEnum;
		fertilizeEnergySensor.rootnessEnum = ((EggCell)geneCell).fertilizeEnergySensor.rootnessEnum;
		fertilizeAttachmentSensor.rootnessEnum = ((EggCell)geneCell).fertilizeAttachmentSensor.rootnessEnum;
	}

	public override void UpdateSensorAreaTablesPhenotype() {
		base.UpdateSensorAreaTablesPhenotype();

		fertilizeLogicBox.UpdateAreaTablesPhenotype();
		fertilizeEnergySensor.UpdateAreaTablesPhenotype();
		fertilizeAttachmentSensor.UpdateAreaTablesPhenotype();
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