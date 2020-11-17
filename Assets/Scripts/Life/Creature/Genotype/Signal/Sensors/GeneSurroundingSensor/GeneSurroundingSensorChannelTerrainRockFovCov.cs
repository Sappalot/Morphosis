using UnityEngine;

public class GeneSurroundingSensorChannelTerrainRockFovCov : GeneSurroundingSensorChannel {

	public float threshold;

	public GeneSurroundingSensorChannelTerrainRockFovCov(IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
	}

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
	private GeneSurroundingSensorChannelTerrainRockFovCovData data = new GeneSurroundingSensorChannelTerrainRockFovCovData();
	public GeneSurroundingSensorChannelTerrainRockFovCovData UpdateData() {
		data.threshold = threshold;
		return data;
	}

	// Load
	public void ApplyData(GeneSurroundingSensorChannelTerrainRockFovCovData data) {
		threshold = data.threshold;
	}
}
