// Information on how to connect any of creatures outputs output to an input

using UnityEngine;

public class GeneNerve {
	public SignalUnitEnum outputUnit = SignalUnitEnum.Void; // The output from me "the nerve" The GeneSensor that created this one
	public SignalUnitSlotEnum outputUnitSlot; // The slot on that (above) unit
	public SignalUnitEnum inputUnit = SignalUnitEnum.Void; // The input to me "the nerve" (Somebodey elses output)
	public SignalUnitSlotEnum inputUnitSlot; // The slot on that (above) unit

	public void Defaultify() {
		inputUnit = SignalUnitEnum.ConstantSensor;
		inputUnitSlot = SignalUnitSlotEnum.outputLateA;
	}

	public void Randomize() {

	}

	public bool Mutate(float strength, bool isOrigin) {
		bool didMutate = false;

		// slot
		float mut = Random.Range(0, 1000f + GlobalSettings.instance.mutation.nerveSlotChange * strength);
		if (mut < GlobalSettings.instance.mutation.nerveSlotChange * strength) {
			MutateSlot(); // slot will be limited depending on unit
			didMutate = true;
		}

		// unit & slot
		mut = Random.Range(0, 1000f + GlobalSettings.instance.mutation.nerveUnitAndSlotChange * strength);
		if (mut < GlobalSettings.instance.mutation.nerveUnitAndSlotChange * strength) {
			int unitRandom = Random.Range(1, isOrigin ? 15 : 17);
			if (unitRandom == 1) {
				unitRandom = 2;
			} if (unitRandom == 10 || unitRandom == 11) {
				unitRandom = 12;
			}
			inputUnit = (SignalUnitEnum)unitRandom;

			MutateSlot(); // newrves input slot needs to be limited to the available output slots (usualy 6)

			didMutate |= true;
		}
		return didMutate;
	}

	private void MutateSlot() {
		if (inputUnit == SignalUnitEnum.WorkLogicBoxA ||
			inputUnit == SignalUnitEnum.WorkLogicBoxB ||
			inputUnit == SignalUnitEnum.DendritesLogicBox ||
			inputUnit == SignalUnitEnum.OriginDetatchLogicBox) {

			inputUnitSlot = SignalUnitSlotEnum.outputLateA;
		} else if (inputUnit == SignalUnitEnum.Axon) {
			RandomizeSlot(4);
		} else {
			RandomizeSlot(5); // all of them
		}
	}

	private void RandomizeSlot(int maxIndex) {
		int slotRandom = Random.Range(0, maxIndex + 1);
		if (slotRandom == 0) {
			inputUnitSlot = SignalUnitSlotEnum.outputLateA;
		} else if (slotRandom == 1) {
			inputUnitSlot = SignalUnitSlotEnum.outputLateB;
		} else if (slotRandom == 2) {
			inputUnitSlot = SignalUnitSlotEnum.outputLateC;
		} else if (slotRandom == 3) {
			inputUnitSlot = SignalUnitSlotEnum.outputLateD;
		} else if (slotRandom == 4) {
			inputUnitSlot = SignalUnitSlotEnum.outputLateE;
		} else if (slotRandom == 5) {
			inputUnitSlot = SignalUnitSlotEnum.outputLateF;
		}
	}



	// Save
	private GeneNerveData geneNerveData = new GeneNerveData();
	public GeneNerveData UpdateData() {
		geneNerveData.outputUnit = outputUnit;
		geneNerveData.outputUnitSlot = outputUnitSlot;
		geneNerveData.inputUnit = inputUnit;
		geneNerveData.inputUnitSlot = inputUnitSlot;
		return geneNerveData;
	}

	//Load
	public void ApplyData(GeneNerveData geneNerveData) {
		outputUnit = geneNerveData.outputUnit;
		outputUnitSlot = geneNerveData.outputUnitSlot;
		inputUnit = geneNerveData.inputUnit;
		inputUnitSlot = geneNerveData.inputUnitSlot;
	}
}