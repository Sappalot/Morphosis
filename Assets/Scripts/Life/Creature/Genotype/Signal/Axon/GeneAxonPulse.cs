using UnityEngine;

public class GeneAxonPulse {

	private float m_axonFromOriginOffset;
	public float axonFromOriginOffset { 
		get {
			return m_axonFromOriginOffset;
		}
		set {
			m_axonFromOriginOffset = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	public bool m_axonIsFromOriginPlus180;
	public bool axonIsFromOriginPlus180 {
		get {
			return m_axonIsFromOriginPlus180;
		}
		set {
			m_axonIsFromOriginPlus180 = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	public float m_axonFromMeOffset;
	public float axonFromMeOffset {
		get {
			return m_axonFromMeOffset;
		}
		set {
			m_axonFromMeOffset = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	public float m_axonRelaxContract;
	public float axonRelaxContract {
		get {
			return m_axonRelaxContract;
		}
		set {
			m_axonRelaxContract = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	public bool m_axonIsReverse;
	public bool axonIsReverse {
		get {
			return m_axonIsReverse;
		}
		set {
			m_axonIsReverse = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	private IGenotypeDirtyfy genotypeDirtyfy;
	public GeneAxonPulse(IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
	}

	public void SetDefault() {
		axonFromOriginOffset = 0f;
		axonIsFromOriginPlus180 = false;
		axonFromMeOffset = 0f;
		axonRelaxContract = 0f;
		axonIsReverse = false;

		genotypeDirtyfy.MakeGeneCellPatternDirty();
	}

	public void Mutate(float strength) {
		GlobalSettings gs = GlobalSettings.instance;


		float mut = Random.Range(0, 1000f + gs.mutation.axonFromOriginOffsetChange * strength);
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

		genotypeDirtyfy.MakeGeneCellPatternDirty();
	}

	// Save
	private GeneAxonPulseData data = new GeneAxonPulseData();
	public GeneAxonPulseData UpdateData() {
		data.axonFromMeOffset = axonFromMeOffset;
		data.axonFromOriginOffset = axonFromOriginOffset;
		data.axonIsFromOriginPlus180 = axonIsFromOriginPlus180;
		data.axonIsReverse = axonIsReverse;
		data.axonRelaxContract = axonRelaxContract;
		data.axonFromMeOffset = axonFromMeOffset;

		return data;
	}

	// Load
	public void ApplyData(GeneAxonPulseData axonData) {
		axonFromMeOffset = axonData.axonFromMeOffset;
		axonFromOriginOffset = axonData.axonFromOriginOffset;
		axonIsFromOriginPlus180 = axonData.axonIsFromOriginPlus180;
		axonIsReverse = axonData.axonIsReverse;
		axonRelaxContract = axonData.axonRelaxContract;
		axonFromMeOffset = axonData.axonFromMeOffset;
	}
}