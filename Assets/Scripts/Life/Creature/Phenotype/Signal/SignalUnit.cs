// 
using System.Collections.Generic;

public abstract class SignalUnit {
	protected SignalUnitEnum hostSignalUnitEnum;
	protected Cell hostCell;

	// ... nerves ...


	public virtual void PreUpdateNervesGenotype() {

	}

	public virtual void UpdateInputNervesGenotype(Genotype genotype) {
		//hostCell.creature.genotype.Get
	}

	public virtual void UpdateConnectionsNervesGenotype(Genotype genotype) {
		//hostCell.creature.genotype.Get
	}

	// Has to be found first, using methods above
	public virtual List<Nerve> GetAllNervesGenotype() {
		return null;
	}

	public virtual void ReachOutNervesPhenotype() {
		// TODO: update newrve connections signals
	}

	// ^ nerves ^ 

	public SignalUnit(Cell hostCell) {
		this.hostCell = hostCell;
	}

	public virtual bool GetOutput(SignalUnitSlotEnum signalUnitSlot) { return false; }

	public virtual void ComputeSignalOutput(int deltaTicks) { }

	public virtual void FeedSignal() { }

	public abstract void Clear();

	public int SignalUnitSlotOutputToIndex(SignalUnitSlotEnum slot) {
		if (slot == SignalUnitSlotEnum.outputLateA) {
			return 0;
		} else if (slot == SignalUnitSlotEnum.outputLateB) {
			return 1;
		} else if (slot == SignalUnitSlotEnum.outputLateC) {
			return 2;
		} else if (slot == SignalUnitSlotEnum.outputLateD) {
			return 3;
		} else if (slot == SignalUnitSlotEnum.outputLateE) {
			return 4;
		} else if (slot == SignalUnitSlotEnum.outputLateF) {
			return 5;
		}
		return -1;
	}

	public static SignalUnitSlotEnum IndexToSignalInputSlotUnit(int index) {
		if (index == 0) {
			return SignalUnitSlotEnum.inputA;
		} else if (index == 1) {
			return SignalUnitSlotEnum.inputB;
		} else if (index == 2) {
			return SignalUnitSlotEnum.inputC;
		} else if (index == 3) {
			return SignalUnitSlotEnum.inputD;
		} else if (index == 4) {
			return SignalUnitSlotEnum.inputE;
		} else if (index == 5) {
			return SignalUnitSlotEnum.inputF;
		}
		return SignalUnitSlotEnum.inputA;
	}
}
