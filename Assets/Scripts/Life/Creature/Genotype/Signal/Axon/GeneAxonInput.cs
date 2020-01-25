using UnityEngine;

public class GeneAxonInput : IGeneInput {

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

	public LocknessEnum lockness = LocknessEnum.Unlocked;// No need to save/load this one as it is hardcoded, set by gene (would be the same every time)

	public GeneAxonInput(int column, SignalUnitEnum signalUnit) {
		nerve.outputUnit = signalUnit;
		nerve.outputUnitSlot = (SignalUnitSlotEnum)column;
	}

	public void Defaultify() {
		valveMode = SignalValveModeEnum.Block;
		nerve.Defaultify();
	}

	public void Mutate(float strength, bool isOrigin) {
		GlobalSettings gs = GlobalSettings.instance;
		float rnd;

		rnd = Random.Range(0, gs.mutation.logicBoxInputValveToggle * strength + 1000f);
		if (rnd < gs.mutation.logicBoxInputValveToggle * strength) {
			valveMode = (valveMode == SignalValveModeEnum.Pass ? SignalValveModeEnum.Block : SignalValveModeEnum.Pass);
		}

		nerve.Mutate(strength, isOrigin);
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
