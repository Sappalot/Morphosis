using UnityEngine;

public class GeneLogicBoxInput : GeneLogicBoxPart, IGeneInput {

	// TODO make it so that nerve input can't be changed if locked
	// TODO don't access nerve directly
	public GeneNerve m_nerve = new GeneNerve();

	public GeneNerve nerve {
		get {
			return m_nerve;
		}
		set {
			m_nerve = value;
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
			}
		}
	}

	public GeneLogicBoxInput(int row, int column, SignalUnitEnum signalUnit) {
		this.row = row;
		leftFlank = GetFlankLeftOfColumn(column);
		rightFlank = GetFlankRightOfColumn(column);
		nerve.outputUnit = signalUnit;
		nerve.outputUnitSlot = (SignalUnitSlotEnum)column;
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
		nerve.Defaultify();
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
			didMutate |= nerve.Mutate(strength, isOrigin);
		}

		return didMutate;
	}

	// Save
	private GeneLogicBoxInputData geneLogicBoxInputData = new GeneLogicBoxInputData();
	public GeneLogicBoxInputData UpdateData() {
		geneLogicBoxInputData.valveMode = valveMode;
		geneLogicBoxInputData.geneNerveData = nerve.UpdateData();
		return geneLogicBoxInputData;
	}

	// Load
	public void ApplyData(GeneLogicBoxInputData geneLogicBoxInputData) {
		valveMode = geneLogicBoxInputData.valveMode;
		nerve.ApplyData(geneLogicBoxInputData.geneNerveData); 
	}
}
