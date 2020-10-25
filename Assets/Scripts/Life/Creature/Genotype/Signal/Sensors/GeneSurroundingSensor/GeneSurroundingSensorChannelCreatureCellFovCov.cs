using UnityEngine;

public class GeneSurroundingSensorChannelCreatureCellFovCov : GeneSurroundingSensorChannel {

	public float threshold;

	public override void Defaultify() {
		threshold = 0f;
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
		return data;
	}

	// Load
	public void ApplyData(GeneSurroundingSensorChannelCreatureCellFovCovData data) {
		threshold = data.threshold;
	}
}
