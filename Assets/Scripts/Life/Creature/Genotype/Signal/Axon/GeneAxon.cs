using System.Collections.Generic;
using UnityEngine;

public class GeneAxon : GeneSignalUnit {
	
	// is it trying to affect muscles?
	// it is able to handle and transmit signals even if disabled
	private bool m_isEnabled;
	public bool isEnabled {
		get {
			return m_isEnabled || isOrigin;
		}
		set {
			m_isEnabled = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	public bool isOrigin;

	private IGenotypeDirtyfy genotypeDirtyfy;

	public GeneAxon(IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
		axonInputLeft = new GeneAxonInput(0, SignalUnitEnum.Axon, this.genotypeDirtyfy); // left, A
		axonInputRight = new GeneAxonInput(1, SignalUnitEnum.Axon, this.genotypeDirtyfy); // right, B

		pulseA = new GeneAxonPulse(this.genotypeDirtyfy);
		pulseB = new GeneAxonPulse(this.genotypeDirtyfy);
		pulseC = new GeneAxonPulse(this.genotypeDirtyfy);
		pulseD = new GeneAxonPulse(this.genotypeDirtyfy);
	}

	private GeneAxonPulse pulseA;
	private GeneAxonPulse pulseB;
	private GeneAxonPulse pulseC;
	private GeneAxonPulse pulseD;

	public GeneAxonPulse GetPulse(int index) {
		return pulses[index - 1]; // pulse A = 1
	}

	public GeneAxonPulse[] pulses = new GeneAxonPulse[4];

	public GeneAxonInput axonInputLeft;
	public GeneAxonInput axonInputRight;

	private int m_pulseProgram3 = 3; //
	public int pulseProgram3 {
		get {
			return m_pulseProgram3;
		}
		set {
			m_pulseProgram3 = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	private int m_pulseProgram2 = 2; // ...
	public int pulseProgram2 {
		get {
			return m_pulseProgram2;
		}
		set {
			m_pulseProgram2 = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	private int m_pulseProgram1 = 1; // 1 = A
	public int pulseProgram1 {
		get {
			return m_pulseProgram1;
		}
		set {
			m_pulseProgram1 = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	private int m_pulseProgram0 = 0; // 0 == relaxed
	public int pulseProgram0 {
		get {
			return m_pulseProgram0;
		}
		set {
			m_pulseProgram0 = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	public void UpdateConnections() {
		// what do we want to do here?????
	}

	public void ConnectAllInputInputTo(SignalUnitEnum signalUnit, SignalUnitSlotEnum signalUnitSlot) {
		axonInputLeft.nerve.tailUnitEnum = signalUnit;
		axonInputLeft.nerve.tailUnitSlotEnum = signalUnitSlot;

		axonInputRight.nerve.tailUnitEnum = signalUnit;
		axonInputRight.nerve.tailUnitSlotEnum = signalUnitSlot;

		genotypeDirtyfy.MakeGeneCellPatternDirty();
	}

	public void SetAllInputToBlocked() {
		axonInputLeft.valveMode = SignalValveModeEnum.Block;
		axonInputRight.valveMode = SignalValveModeEnum.Block;

		genotypeDirtyfy.MakeGeneCellPatternDirty();
	}

	public override void MarkThisAndChildrenAsRooted(Genotype genotype, Cell geneCell, SignalUnitEnum signalUnit) {
		// avoid looping forever

		isRooted = true;
		
		if (geneCell.gene == null) {
			return;
		}
		
		// mark children as well
		if (axonInputLeft.valveMode == SignalValveModeEnum.Pass) {
			if (axonInputLeft.nerve.isLocal) {
				// ask to which this genes  unit where tail is "pointing"
				GeneSignalUnit child = geneCell.gene.GetGeneSignalUnit(axonInputLeft.nerve.tailUnitEnum);
				if (child != null) {
					child.MarkThisAndChildrenAsRooted(genotype, geneCell, signalUnit);
				}
			} else {
				// ask external unit where tail is pointing
				Cell childCell = GeneNerve.GetGeneCellAtNerveTail(geneCell, axonInputLeft.nerve, genotype);
				if (childCell != null) {
					GeneSignalUnit child = childCell.gene.GetGeneSignalUnit(axonInputLeft.nerve.tailUnitEnum);
					if (child != null) {
						child.MarkThisAndChildrenAsRooted(genotype, childCell, signalUnit);
					}
				}
			}
		}
		if (axonInputRight.valveMode == SignalValveModeEnum.Pass) {
			if (axonInputRight.nerve.isLocal) {
				// ask local unit where tail is "pointing"
				GeneSignalUnit child = geneCell.gene.GetGeneSignalUnit(axonInputRight.nerve.tailUnitEnum);
				if (child != null) {
					child.MarkThisAndChildrenAsRooted(genotype, geneCell, signalUnit);
				}
			} else {
				Cell childCell = GeneNerve.GetGeneCellAtNerveTail(geneCell, axonInputRight.nerve, genotype);
				if (childCell != null) {
					GeneSignalUnit child = childCell.gene.GetGeneSignalUnit(axonInputRight.nerve.tailUnitEnum);
					if (child != null) {
						child.MarkThisAndChildrenAsRooted(genotype, childCell, signalUnit);
					}
				}
			}
		}
	}

	public override List<GeneNerve> GetExternalGeneNerves() {
		if (!isRooted) { // TODO: take isUsed from outside into account as well
			return null;
		}

		List<GeneNerve> nerves = new List<GeneNerve>();
		if (axonInputLeft.valveMode == SignalValveModeEnum.Pass && axonInputLeft.nerve.nerveVector != null) {
			nerves.Add(axonInputLeft.nerve);
		}

		if (axonInputRight.valveMode == SignalValveModeEnum.Pass && axonInputRight.nerve.nerveVector != null) {
			nerves.Add(axonInputRight.nerve);
		}

		return nerves;
	}

	public void Defaultify() {
		isEnabled = false;
		ConnectAllInputInputTo(SignalUnitEnum.ConstantSensor, SignalUnitSlotEnum.outputLateA); // constant 0
		SetAllInputToBlocked();

		pulses[0] = pulseA;
		pulses[1] = pulseB;
		pulses[2] = pulseC;
		pulses[3] = pulseD;

		pulseA.SetDefault();
		pulseB.SetDefault();
		pulseC.SetDefault();
		pulseD.SetDefault();

		axonInputLeft.Defaultify();
		axonInputRight.Defaultify();

		genotypeDirtyfy.MakeGeneCellPatternDirty();
	}

	public void Randomize() {
		// TODO

		genotypeDirtyfy.MakeGeneCellPatternDirty();
	}

	public void Mutate(float strength, bool isOrigin) {
		GlobalSettings gs = GlobalSettings.instance;

		float mut = Random.Range(0, 1000f + gs.mutation.axonEnabledToggle * strength);
		if (mut < gs.mutation.axonEnabledToggle * strength) {
			isEnabled = !isEnabled; //toggle
		}

		pulseA.Mutate(strength);
		pulseB.Mutate(strength);
		pulseC.Mutate(strength);
		pulseD.Mutate(strength);

		axonInputLeft.Mutate(strength, isOrigin);
		axonInputRight.Mutate(strength, isOrigin);

		genotypeDirtyfy.MakeGeneCellPatternDirty();
	}

	// Save
	private GeneAxonData data = new GeneAxonData();
	public GeneAxonData UpdateData() {
		data.axonIsEnabled = isEnabled;

		data.pulseDataA = pulseA.UpdateData();
		data.pulseDataB = pulseB.UpdateData();
		data.pulseDataC = pulseC.UpdateData();
		data.pulseDataD = pulseD.UpdateData();

		data.pulseProgram3 = pulseProgram3;
		data.pulseProgram2 = pulseProgram2;
		data.pulseProgram1 = pulseProgram1;
		data.pulseProgram0 = pulseProgram0;

		data.axonInputLeft = axonInputLeft.UpdateData();
		data.axonInputRight = axonInputRight.UpdateData();
		return data;
	}

	// Load
	public void ApplyData(GeneAxonData axonData) {
		isEnabled = axonData.axonIsEnabled;

		pulseA.ApplyData(axonData.pulseDataA);
		pulseB.ApplyData(axonData.pulseDataB);
		pulseC.ApplyData(axonData.pulseDataC);
		pulseD.ApplyData(axonData.pulseDataD);

		pulseProgram3 = axonData.pulseProgram3;
		pulseProgram2 = axonData.pulseProgram2;
		pulseProgram1 = axonData.pulseProgram1;
		pulseProgram0 = axonData.pulseProgram0;

		axonInputLeft.ApplyData(axonData.axonInputLeft);
		axonInputRight.ApplyData(axonData.axonInputRight);
	}
}