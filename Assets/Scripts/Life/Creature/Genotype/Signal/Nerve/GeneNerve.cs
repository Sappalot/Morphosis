// Information on how to connect any of creatures outputs to an input

using UnityEngine;

public class GeneNerve {
	public SignalUnitEnum outputUnit = SignalUnitEnum.Void; // The output from me "the nerve head" received as input by the GeneSensor that created this one
	public SignalUnitSlotEnum outputUnitSlot; // The slot on that (above) unit

	private SignalUnitEnum m_inputUnit = SignalUnitEnum.Void; // The input to me "the nerve" sent from some singalUnits output
	public SignalUnitEnum inputUnit {
		get {
			return m_inputUnit;
		}
		set {
			m_inputUnit = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	private SignalUnitSlotEnum m_inputUnitSlot; // The slot on that (above) unit
	public SignalUnitSlotEnum inputUnitSlot {
		get {
			return m_inputUnitSlot;
		}
		set {
			m_inputUnitSlot = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	//public enum State {
	//	Resting, // normal state
	//	BeingChanged,
	//	BeingChangedTooLong,
	//}
	//public State state = State.Resting;

	// If other than null we listen to a local output
	// ...otherwise we try to listen to an output from another geneCell (phenotype cell)
	// This vector is expressed in cell space so it is moved and turned as the GeneCell is relocated and turned
	// Further it is defined by a cell flipped (black|white) meaning possitive x values will reach out to cells white side while negative will reach out to the black side
	//    (black|white)
	//     (left|right)
	// (negative|positive)

	// example: nerveVectorLocal = (2,2) is reaching out diagonaly in front of the cell to its white side (that is right side if turned black|white)

	// nerveVectorLocal needs to be transformed to local space when set from worldspace
	// nerveVectorLocal needs to be transformed to world space in order for us to know where we are listening 
	public Vector2i nerveVector = null;

	private IGenotypeDirtyfy genotypeDirtyfy;

	public GeneNerve(IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
	}

	public void Defaultify() {
		inputUnit = SignalUnitEnum.ConstantSensor;
		inputUnitSlot = SignalUnitSlotEnum.outputLateA;
		genotypeDirtyfy.MakeGeneCellPatternDirty();
	}

	public void Randomize() {

		genotypeDirtyfy.MakeGeneCellPatternDirty();
	}

	public bool Mutate(float strength, bool isOrigin) {
		bool didMutate = false;

		// slot
		float mut = Random.Range(0, 1000f + GlobalSettings.instance.mutation.nerveSlotChange * strength);
		if (mut < GlobalSettings.instance.mutation.nerveSlotChange * strength) {
			RandomizeSlot(); // slot will be limited depending on unit
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

			RandomizeSlot(); // newrves input slot needs to be limited to the available output slots (usualy 6)

			didMutate |= true;
		}

		return didMutate;
	}

	private void RandomizeSlot() {
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

		genotypeDirtyfy.MakeGeneCellPatternDirty();
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
		geneNerveData.nerveVector = nerveVector;
		return geneNerveData;
	}

	//Load
	public void ApplyData(GeneNerveData geneNerveData) {
		outputUnit = geneNerveData.outputUnit;
		outputUnitSlot = geneNerveData.outputUnitSlot;
		inputUnit = geneNerveData.inputUnit;
		inputUnitSlot = geneNerveData.inputUnitSlot;
		nerveVector = geneNerveData.nerveVector;

		genotypeDirtyfy.MakeGeneCellPatternDirty();
	}
}