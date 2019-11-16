using UnityEngine;

public class GeneAxon {
	private bool m_axonIsEnabled;
	public bool axonIsEnabled {
		get {
			return m_axonIsEnabled /*|| isOrigin*/;
		}
		set {
			m_axonIsEnabled = value;
		}
	}

	public float axonFromOriginOffset;
	public bool axonIsFromOriginPlus180;
	public float axonFromMeOffset;
	public float axonRelaxContract;
	public bool axonIsReverse;

	public GeneAxonInput axonInputLeft = new GeneAxonInput(0, SignalUnitEnum.Axon); // left, A
	public GeneAxonInput axonInputRight = new GeneAxonInput(1, SignalUnitEnum.Axon); // right, B

	public void UpdateConnections() {
		// what do we want to do here?????
	}
	
	public void Mutate(float strength) {
		GlobalSettings gs = GlobalSettings.instance;

		float mut = Random.Range(0, 1000f + gs.mutation.axonEnabledToggle * strength);
		if (mut < gs.mutation.axonEnabledToggle * strength) {
			axonIsEnabled = !axonIsEnabled; //toggle
		}

		mut = Random.Range(0, 1000f + gs.mutation.axonFromOriginOffsetChange * strength);
		if (mut < gs.mutation.axonFromOriginOffsetChange * strength) {
			axonFromOriginOffset += gs.mutation.RandomDistributedValue() * gs.mutation.axonFromOriginOffsetChangeMaxAmount % 360f;
			if (axonFromOriginOffset < 0f) {
				axonFromOriginOffset += 360f;
			}
		}

		mut = Random.Range(0, 1000f + gs.mutation.axonIsFromOriginPlus180Toggle * strength);
		if (mut < gs.mutation.axonIsFromOriginPlus180Toggle * strength) {
			axonIsFromOriginPlus180 = !axonIsFromOriginPlus180; //toggle
		}

		mut = Random.Range(0, 1000f + gs.mutation.axonFromMeOffsetChange * strength);
		if (mut < gs.mutation.axonFromMeOffsetChange * strength) {
			axonFromMeOffset = Mathf.Clamp(axonFromMeOffset + gs.mutation.RandomDistributedValue() * gs.mutation.axonFromMeOffsetChangeMaxAmount, 0, 360);
			if (axonFromMeOffset < 0f) {
				axonFromMeOffset += 360f;
			}
		}

		mut = Random.Range(0, 1000f + gs.mutation.axonRelaxContractChange * strength);
		if (mut < gs.mutation.axonRelaxContractChange * strength) {
			axonRelaxContract = Mathf.Clamp(axonRelaxContract + gs.mutation.RandomDistributedValue() * gs.mutation.axonRelaxContractChangeMaxAmount, -1f, 1f);
		}

		mut = Random.Range(0, 1000f + gs.mutation.axonIsReverseToggle * strength);
		if (mut < gs.mutation.axonIsReverseToggle * strength) {
			axonIsReverse = !axonIsReverse; //toggle
		}

	}

	// Save
	private GeneAxonData data = new GeneAxonData();
	public GeneAxonData UpdateData() {
		data.axonIsEnabled = axonIsEnabled;
		data.axonFromOriginOffset = axonFromOriginOffset;
		data.axonIsFromOriginPlus180 = axonIsFromOriginPlus180;
		data.axonFromMeOffset = axonFromMeOffset;
		data.axonRelaxContract = axonRelaxContract;
		data.axonIsReverse = axonIsReverse;
		return data;
	}

	// Load
	public void ApplyData(GeneAxonData axonData) {
		axonIsEnabled = axonData.axonIsEnabled;
		axonFromOriginOffset = axonData.axonFromOriginOffset;
		axonIsFromOriginPlus180 = axonData.axonIsFromOriginPlus180;
		axonFromMeOffset = axonData.axonFromMeOffset;
		axonRelaxContract = axonData.axonRelaxContract;
		axonIsReverse = axonData.axonIsReverse;
	}
}