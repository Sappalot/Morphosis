using UnityEngine;

public class GeneLogicBoxInput : GeneLogicBoxPart {

	// TODO make it so that nerve input can't be changed if locked
	public GeneNerve nerve = new GeneNerve();

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

	public void Mutate(float strength) {
		GlobalSettings gs = GlobalSettings.instance;
		float rnd;

		rnd = Random.Range(0, gs.mutation.logicBoxInputValveToggle * strength + 1000f);
		if (rnd < gs.mutation.logicBoxInputValveToggle * strength) {
			valveMode = (valveMode == SignalValveModeEnum.Pass ? SignalValveModeEnum.Block : SignalValveModeEnum.Pass);
		}
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
