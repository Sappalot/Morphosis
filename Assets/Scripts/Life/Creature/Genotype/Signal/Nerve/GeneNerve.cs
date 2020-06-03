// Information on how to connect any of creatures outputs to an input

using System.Runtime.CompilerServices;
using UnityEngine;

public class GeneNerve {
	public SignalUnitEnum headUnitEnum = SignalUnitEnum.Void; // attached to axon input OR logic box input
	public SignalUnitSlotEnum headUnitSlotEnum;

	private SignalUnitEnum m_tailUnitEnum = SignalUnitEnum.Void; // Attached to an output
	public SignalUnitEnum tailUnitEnum {
		get {
			return m_tailUnitEnum;
		}
		set {
			m_tailUnitEnum = value;
			genotypeDirtyfy.ReforgeInterGeneCellAndForward();
		}
	}

	private SignalUnitSlotEnum m_tailUnitSlotEnum; // The slot on that (above) unit
	public SignalUnitSlotEnum tailUnitSlotEnum {
		get {
			return m_tailUnitSlotEnum;
		}
		set {
			m_tailUnitSlotEnum = value;
			genotypeDirtyfy.ReforgeInterGeneCellAndForward();
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

	public bool isLocal {
		get {
			return nerveVector == null || nerveVector == Vector2i.zero;
		}
	}

	private IGenotypeDirtyfy genotypeDirtyfy;

	public static Cell GetGeneCellAtNerveTail(Cell headCell, GeneNerve geneNerve, Genotype genotype) {
		if (geneNerve.isLocal) {
			return null;
		}

		Vector2i nerveVector = geneNerve.nerveVector;


		// flip vector horizontally only if cell flip side is (white|black) 
		if (headCell.flipSide == FlipSideEnum.WhiteBlack) {
			nerveVector = CellMap.HexagonalFlip(nerveVector);
		}

		// rotate
		int rootDirection = headCell.bindCardinalIndex;
		int turnToCreatureAngle = 0;
		if (rootDirection == 0) { // ne
			turnToCreatureAngle = 5; // +300
		} else if (rootDirection == 1) { // n
			turnToCreatureAngle = 0; // just fine
		} else if (rootDirection == 2) { // nw
			turnToCreatureAngle = 1; // +60
		} else if (rootDirection == 3) { // sw
			turnToCreatureAngle = 2; // +120
		} else if (rootDirection == 4) { // s
			turnToCreatureAngle = 3; // +180
		} else if (rootDirection == 5) { // se
			turnToCreatureAngle = 4; // +240
		}
		nerveVector = CellMap.HexagonalRotate(nerveVector, turnToCreatureAngle);

		// move
		nerveVector = CellMap.HexagonalPlus(headCell.mapPosition, nerveVector);

		return genotype.GetCellAtMapPosition(nerveVector);
	}

	public GeneNerve(IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
	}

	public void Defaultify() {
		tailUnitEnum = SignalUnitEnum.ConstantSensor;
		tailUnitSlotEnum = SignalUnitSlotEnum.outputLateA;
		genotypeDirtyfy.ReforgeInterGeneCellAndForward();
	}

	public void Randomize() {
		genotypeDirtyfy.ReforgeInterGeneCellAndForward();
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
			tailUnitEnum = (SignalUnitEnum)unitRandom;

			RandomizeSlot(); // newrves input slot needs to be limited to the available output slots (usualy 6)

			didMutate |= true;
		}

		return didMutate;
	}

	private void RandomizeSlot() {
		if (tailUnitEnum == SignalUnitEnum.WorkLogicBoxA ||
			tailUnitEnum == SignalUnitEnum.WorkLogicBoxB ||
			tailUnitEnum == SignalUnitEnum.DendritesLogicBox ||
			tailUnitEnum == SignalUnitEnum.OriginDetatchLogicBox) {

			tailUnitSlotEnum = SignalUnitSlotEnum.outputLateA;
		} else if (tailUnitEnum == SignalUnitEnum.Axon) {
			RandomizeSlot(4);
		} else {
			RandomizeSlot(5); // all of them
		}

		genotypeDirtyfy.ReforgeInterGeneCellAndForward();
	}

	private void RandomizeSlot(int maxIndex) {
		int slotRandom = Random.Range(0, maxIndex + 1);
		if (slotRandom == 0) {
			tailUnitSlotEnum = SignalUnitSlotEnum.outputLateA;
		} else if (slotRandom == 1) {
			tailUnitSlotEnum = SignalUnitSlotEnum.outputLateB;
		} else if (slotRandom == 2) {
			tailUnitSlotEnum = SignalUnitSlotEnum.outputLateC;
		} else if (slotRandom == 3) {
			tailUnitSlotEnum = SignalUnitSlotEnum.outputLateD;
		} else if (slotRandom == 4) {
			tailUnitSlotEnum = SignalUnitSlotEnum.outputLateE;
		} else if (slotRandom == 5) {
			tailUnitSlotEnum = SignalUnitSlotEnum.outputLateF;
		}
	}

	// Save
	private GeneNerveData geneNerveData = new GeneNerveData();
	public GeneNerveData UpdateData() {
		geneNerveData.outputUnit = headUnitEnum;
		geneNerveData.outputUnitSlot = headUnitSlotEnum;
		geneNerveData.inputUnit = tailUnitEnum;
		geneNerveData.inputUnitSlot = tailUnitSlotEnum;
		geneNerveData.nerveVector = nerveVector;
		return geneNerveData;
	}

	//Load
	public void ApplyData(GeneNerveData geneNerveData) {
		headUnitEnum = geneNerveData.outputUnit;
		headUnitSlotEnum = geneNerveData.outputUnitSlot;
		tailUnitEnum = geneNerveData.inputUnit;
		tailUnitSlotEnum = geneNerveData.inputUnitSlot;
		nerveVector = geneNerveData.nerveVector;

		genotypeDirtyfy.ReforgeInterGeneCellAndForward();
	}
}