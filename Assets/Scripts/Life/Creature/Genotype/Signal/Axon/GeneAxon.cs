using UnityEngine;

public class GeneAxon {
	private bool m_axonIsEnabled;
	public bool axonIsEnabled {
		get {
			return m_axonIsEnabled || isOrigin;
		}
		set {
			m_axonIsEnabled = value;
		}
	}

	public bool isOrigin;

	private GeneAxonPulse pulseA = new GeneAxonPulse();
	private GeneAxonPulse pulseB = new GeneAxonPulse();
	private GeneAxonPulse pulseC = new GeneAxonPulse();
	private GeneAxonPulse pulseD = new GeneAxonPulse();

	public GeneAxonPulse GetPulse(int index) {
		return pulses[index - 1]; // pulse A = 1
	}

	public GeneAxonPulse[] pulses = new GeneAxonPulse[4];

	public GeneAxonInput axonInputLeft = new GeneAxonInput(0, SignalUnitEnum.Axon); // left, A
	public GeneAxonInput axonInputRight = new GeneAxonInput(1, SignalUnitEnum.Axon); // right, B

	public int pulseProgram3 = 3; //  
	public int pulseProgram2 = 2; // ...
	public int pulseProgram1 = 1; // 1 = A
	public int pulseProgram0 = 0; // 0 == relaxed

	public GeneAxon() {
		pulses[0] = pulseA;
		pulses[1] = pulseB;
		pulses[2] = pulseC;
		pulses[3] = pulseD;
	}

	public void UpdateConnections() {
		// what do we want to do here?????
	}

	public void ConnectAllInputInputTo(SignalUnitEnum signalUnit, SignalUnitSlotEnum signalUnitSlot) {
		axonInputLeft.nerve.inputUnit = signalUnit;
		axonInputLeft.nerve.inputUnitSlot = signalUnitSlot;

		axonInputRight.nerve.inputUnit = signalUnit;
		axonInputRight.nerve.inputUnitSlot = signalUnitSlot;
	}

	public void Mutate(float strength) {
		GlobalSettings gs = GlobalSettings.instance;

		float mut = Random.Range(0, 1000f + gs.mutation.axonEnabledToggle * strength);
		if (mut < gs.mutation.axonEnabledToggle * strength) {
			axonIsEnabled = !axonIsEnabled; //toggle
		}

		pulseA.Mutate(strength);
		pulseB.Mutate(strength);
		pulseC.Mutate(strength);
		pulseD.Mutate(strength);
	}

	// Save
	private GeneAxonData data = new GeneAxonData();
	public GeneAxonData UpdateData() {
		data.axonIsEnabled = axonIsEnabled;

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
		axonIsEnabled = axonData.axonIsEnabled;

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