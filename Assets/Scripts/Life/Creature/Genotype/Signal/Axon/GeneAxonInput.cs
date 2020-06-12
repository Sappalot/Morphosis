﻿using UnityEngine;

public class GeneAxonInput : IGeneInput {

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

	public LocknessEnum lockness = LocknessEnum.Unlocked;// No need to save/load this one as it is hardcoded, set by gene (would be the same every time)

	private IGenotypeDirtyfy genotypeDirtyfy;

	public GeneAxonInput(int column, SignalUnitEnum signalUnit, IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
		m_nerve = new GeneNerve(genotypeDirtyfy);
		geneNerve.headUnitEnum = signalUnit;
		geneNerve.headUnitSlotEnum = (SignalUnitSlotEnum)column;
	}

	public void Defaultify() {
		valveMode = SignalValveModeEnum.Block;
		geneNerve.Defaultify();
	}

	public void Mutate(float strength, bool isOrigin) {
		GlobalSettings gs = GlobalSettings.instance;
		float rnd;

		rnd = Random.Range(0, gs.mutation.logicBoxInputValveToggle * strength + 1000f);
		if (rnd < gs.mutation.logicBoxInputValveToggle * strength) {
			valveMode = (valveMode == SignalValveModeEnum.Pass ? SignalValveModeEnum.Block : SignalValveModeEnum.Pass);
		}

		geneNerve.Mutate(strength, isOrigin); // never locked
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

		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}
}
