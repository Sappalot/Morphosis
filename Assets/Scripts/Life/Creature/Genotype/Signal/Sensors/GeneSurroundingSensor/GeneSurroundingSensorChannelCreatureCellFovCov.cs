using UnityEngine;

public class GeneSurroundingSensorChannelCreatureCellFovCov : GeneSurroundingSensorChannel {

	public float threshold;
	public bool seeEgg;
	public bool seeFungal;
	public bool seeJawThreat;
	public bool seeJawHarmless;
	public bool seeLeaf;
	public bool seeMuscle;
	public bool seeRoot;
	public bool seeShell;
	public bool seeVein;

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
