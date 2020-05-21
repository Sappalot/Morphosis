// 
using System.Collections.Generic;
using UnityEngine;

public abstract class SignalUnit {
	protected SignalUnitEnum signalUnitEnum;
	protected Cell hostCell;

	protected List<Nerve> outputNerves = new List<Nerve>(); // there might be many, per slot even

	public bool isRooted;

	// 1.
	public void PreUpdateNervesGenotype() {
		outputNerves.Clear();
		isRooted = false;
	}

	// 2
	public virtual void UpdateInputNervesGenotype(Genotype genotype) { }

	// 3
	public virtual void RootRecursivlyGenotype(Genotype genotype, Nerve nerve) {
		// store output nerve from mother (nerve's head) to me (nerve's tail)
		// we need to do this even if this signalUnit is allready marked as root
		// the same nerve will not be added twice

		if (nerve != null) {
			// Output nerve is the same as the input but, status is changed, do we need to change the vector as well??
			Nerve outputNerve = new Nerve(nerve);

			if (nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeLocal) {
				outputNerve.nerveStatusEnum = NerveStatusEnum.Output_GenotypeLocal;
			} else if (nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeExternal) {
				outputNerve.nerveStatusEnum = NerveStatusEnum.Output_GenotypeExternal;
			}
			
			//Debug.Assert(nerve.nerveStatusEnum != NerveStatusEnum.Input_GenotypeExternalVoid, "This is strange, we were just contacted from a nerve with its head in the void.");
			Debug.Assert(nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeLocal || nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeExternal, "This kind of nerve should not be able to contact me");

			outputNerves.Add(outputNerve); // all nerves tails will be: this host call -> this signal unit enum -> may be different slots
		}

		isRooted = true;
	}

	// 4. Has to be found first, using methods above
	public virtual List<Nerve> GetAllNervesGenotype() {
		// Signal Units overriding this function (the ones with input as well: logicBox & Axon) need to call this one in order to get output as well
		return outputNerves;
	}

	public virtual List<Nerve> GetOutputNervesGenotype() {
		return outputNerves;
	}

	public virtual List<Nerve> GetInputNervesGenotype() {
		return null;
	}

	//--

	public virtual void ReachOutNervesPhenotype() {
		// TODO: update newrve connections signals
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
