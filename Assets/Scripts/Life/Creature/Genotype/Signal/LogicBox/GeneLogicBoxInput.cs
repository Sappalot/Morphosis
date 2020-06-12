using UnityEngine;

public class GeneLogicBoxInput : GeneLogicBoxPart, IGeneInput {

	// TODO make it so that nerve input can't be changed if locked
	// TODO don't access nerve directly
	public GeneNerve m_nerve;

	public GeneNerve geneNerve {
		get {
			return m_nerve;
		}
	}

	private SignalValveModeEnum m_valveMode;
	public SignalValveModeEnum valveMode { 
		get {
			return m_valveMode;
		}
		set {
			if (lockness == LocknessEnum.Unlocked || lockness == LocknessEnum.SemiLocked) { 
				m_valveMode = value;
				genotypeDirtyfy.ReforgeInterGeneCellAndForward();
			}
		}
	}

	public GeneLogicBoxInput(int row, int column, SignalUnitEnum signalUnit, IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
		this.row = row;
		leftFlank = GetFlankLeftOfColumn(column);
		rightFlank = GetFlankRightOfColumn(column);

		m_nerve = new GeneNerve(this.genotypeDirtyfy);
		geneNerve.headUnitEnum = signalUnit; // me
		geneNerve.headUnitSlotEnum = (SignalUnitSlotEnum)column; // me
	}

	public int column {
		get {
			return GetColumnRightOfFlank(leftFlank);
		}
	}



	public override int GetTransmittingInputCount() {
		return isTransmittingSignal ? 1 : 0;
	}

	public void Defaultify() {
		valveMode = SignalValveModeEnum.Block;
		geneNerve.Defaultify();
	}

	public void Randomize(bool isOrigin) {

	}

	public bool Mutate(float strength, bool isOrigin) {
		bool didMutate = false;

		GlobalSettings gs = GlobalSettings.instance;
		float rnd;

		rnd = Random.Range(0, gs.mutation.logicBoxInputValveToggle * strength + 1000f);
		if (rnd < gs.mutation.logicBoxInputValveToggle * strength) {
			valveMode = (valveMode == SignalValveModeEnum.Pass ? SignalValveModeEnum.Block : SignalValveModeEnum.Pass);
			didMutate = true;
		}

		// we wont change where we sample from if we are locked
		if (lockness == LocknessEnum.Unlocked) {
			didMutate |= geneNerve.Mutate(strength, isOrigin);
		}

		return didMutate;
	}

	// Save
	private GeneLogicBoxInputData geneLogicBoxInputData = new GeneLogicBoxInputData();
	public GeneLogicBoxInputData UpdateData() {
		geneLogicBoxInputData.valveMode = valveMode;
		geneLogicBoxInputData.geneNerveData = geneNerve.UpdateData();
		return geneLogicBoxInputData;
	}

	// Load
	public void ApplyData(GeneLogicBoxInputData geneLogicBoxInputData) {
		valveMode = geneLogicBoxInputData.valveMode;
		geneNerve.ApplyData(geneLogicBoxInputData.geneNerveData); 
	}
}
