// 
using System.Collections.Generic;
using UnityEngine;

public abstract class SignalUnit {
	protected SignalUnitEnum signalUnitEnum;
	protected Cell hostCell;

	protected List<Nerve> outputNerves = new List<Nerve>(); // there might be many, per slot even

	public RootnessEnum rootnessEnum;

	// 1.
	public void PreUpdateNervesGenotype() {
		outputNerves.Clear();
		rootnessEnum = RootnessEnum.Unrooted;
	}

	// 2
	public virtual void UpdateInputNervesGenotype(Genotype genotype) { }

	// 3
	public virtual void RootRecursivlyGenotypePhenotype(Nerve nerve, bool addOutputNere) {
		// store output nerve from mother calling me (nerve's head) to me (nerve's tail)
		// we need to do this even if this signalUnit is allready marked as root, because maybee somebodey else ask me this time
		// If the same one is calling me we need to refuse this guy
		

		//if (rootnessEnum == RootnessEnum.Rooted) {
		//	return;
		//}

		if (nerve != null && addOutputNere) {
			// Output nerve is the same as the input but, status is changed, do we need to change the vector as well??
			Nerve outputNerve = new Nerve(nerve);

			if (nerve.nerveStatusEnum == NerveStatusEnum.InputLocal) {
				outputNerve.nerveStatusEnum = NerveStatusEnum.OutputLocal;
			} else if (nerve.nerveStatusEnum == NerveStatusEnum.InputExternal) {
				outputNerve.nerveStatusEnum = NerveStatusEnum.OutputExternal;
			}
			
			Debug.Assert(nerve.nerveStatusEnum == NerveStatusEnum.InputLocal || nerve.nerveStatusEnum == NerveStatusEnum.InputExternal, "This kind of nerve should not be able to contact me");

			// dont add the same one twice
			if (outputNerves.Find(n => Nerve.AreTwinNerves(n, nerve, true)) == null) {
				outputNerves.Add(outputNerve); // all nerves tails will be: this host call -> this signal unit enum -> may be different slots
			}
		}

		rootnessEnum = RootnessEnum.Rooted;
	}

	// 4. Has to be found first, using methods above
	public virtual List<Nerve> GetAllNervesGenotypePhenotype() {
		// Signal Units overriding this function (the ones with input as well: logicBox & Axon) need to call this one in order to get output as well
		return outputNerves;
	}

	public virtual List<Nerve> GetOutputNervesGenotypePhenotype() {
		return outputNerves;
	}

	public virtual List<Nerve> GetInputNervesGenotypePhenotype() {
		return null;
	}

	//--

	public virtual void PostUpdateNervesPhenotype() {}

	public virtual void CloneNervesFromGenotypeToPhenotype(Cell geneCell, Phenotype phenotype) {
		// clone output
		List<Nerve> outputNervesGenotype = geneCell.GetSignalUnit(signalUnitEnum).outputNerves;

		foreach (Nerve genotypeNerve in outputNervesGenotype) {
			Nerve phenotypeNerve = new Nerve(genotypeNerve);
			phenotypeNerve.headCell = phenotype.GetCellAtMapPosition(genotypeNerve.headCell.mapPosition);
			phenotypeNerve.tailCell = hostCell;
			outputNerves.Add(phenotypeNerve);
		}
	}


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
