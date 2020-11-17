using UnityEngine;

public class GeneSurroundingSensorChannelCreatureCellFovCov : GeneSurroundingSensorChannel {

	public float threshold;

	private bool m_seeEgg;
	public bool seeEgg {
		get {
			return m_seeEgg;
		}
		set {
			m_seeEgg = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	private bool m_seeFungal;
	public bool seeFungal {
		get {
			return m_seeFungal;
		}
		set {
			m_seeFungal = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	private bool m_seeJawThreat;
	public bool seeJawThreat {
		get {
			return m_seeJawThreat;
		}
		set {
			m_seeJawThreat = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	private bool m_seeJawHarmless;
	public bool seeJawHarmless {
		get {
			return m_seeJawHarmless;
		}
		set {
			m_seeJawHarmless = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	private bool m_seeLeaf;
	public bool seeLeaf {
		get {
			return m_seeLeaf;
		}
		set {
			m_seeLeaf = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	private bool m_seeMuscle;
	public bool seeMuscle {
		get {
			return m_seeMuscle;
		}
		set {
			m_seeMuscle = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	private bool m_seeRoot;
	public bool seeRoot {
		get {
			return m_seeRoot;
		}
		set {
			m_seeRoot = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	private bool m_seeShell;
	public bool seeShell {
		get {
			return m_seeShell;
		}
		set {
			m_seeShell = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	private bool m_seeVein;
	public bool seeVein {
		get {
			return m_seeVein;
		}
		set {
			m_seeVein = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	public GeneSurroundingSensorChannelCreatureCellFovCov(IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
	}

	public override void Defaultify() {
		threshold = 0f;
		seeEgg = true;
		seeFungal = true;
		seeJawThreat = true;
		seeJawHarmless = true;
		seeLeaf = true;
		seeMuscle = true;
		seeRoot = true;
		seeShell = true;
		seeVein = true;
	}



	public override void Mutate(float strength) {
		// TODO: mutate stuff
	}

	public override void Randomize() {
		// TODO: mutate stuff
	}

	// Save
	private GeneSurroundingSensorChannelCreatureCellFovCovData data = new GeneSurroundingSensorChannelCreatureCellFovCovData();
	public GeneSurroundingSensorChannelCreatureCellFovCovData UpdateData() {
		data.threshold = threshold;
		data.seeEgg = seeEgg;
		data.seeFungal = seeFungal;
		data.seeJawThreat = seeJawThreat;
		data.seeJawHarmless = seeJawHarmless;
		data.seeLeaf = seeLeaf;
		data.seeMuscle = seeMuscle;
		data.seeRoot = seeRoot;
		data.seeShell = seeShell;
		data.seeVein = seeVein;
		return data;
	}

	// Load
	public void ApplyData(GeneSurroundingSensorChannelCreatureCellFovCovData data) {
		threshold = data.threshold;
		seeEgg = data.seeEgg;
		seeFungal = data.seeFungal;
		seeJawThreat = data.seeJawThreat;
		seeJawHarmless = data.seeJawHarmless;
		seeLeaf = data.seeLeaf;
		seeMuscle = data.seeMuscle;
		seeRoot = data.seeRoot;
		seeShell = data.seeShell;
		seeVein = data.seeVein;
	}
}
