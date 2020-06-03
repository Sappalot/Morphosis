using UnityEngine;

public class GeneEnergySensor : GeneSignalUnit {
	public GeneEnergySensor(SignalUnitEnum signalUnit, IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
		this.signalUnit = signalUnit;
		Defaultify();
	}

	private int m_areaRadius = 1; // m
	public int areaRadius {
		get {
			return m_areaRadius;
		}
		set {
			m_areaRadius = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	private float m_threshold = 50f;
	public float threshold {
		get {
			return Mathf.Max(m_threshold, thresholdMin);
		}
		set {
			m_threshold = Mathf.Max(value, thresholdMin);
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}

	} // joules, [0 .... 100]

	public float thresholdMin = 0f; // hardcoded, no load save , no change from user

	private IGenotypeDirtyfy genotypeDirtyfy;

	public void Defaultify() {
		areaRadius = 1;
		m_threshold = 50f;
		thresholdMin = 0f;

		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	public void Randomize() {

		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	public void Mutate(float strength) {
		GlobalSettings gs = GlobalSettings.instance;
		float rnd;

		rnd = Random.Range(0, gs.mutation.energySensorAreaRadiusChange * strength + 1000f);
		if (rnd < gs.mutation.energySensorAreaRadiusChange * strength) {
			areaRadius = (int)Mathf.Clamp(areaRadius + gs.mutation.energySensorAreaRadiusChangeMaxAmount * gs.mutation.RandomDistributedValue(), 0f, Creature.maxRadiusHexagon - 1);
		}

		rnd = Random.Range(0, gs.mutation.energySensorThresholdChange * strength + 1000f);
		if (rnd < gs.mutation.energySensorThresholdChange * strength) {
			threshold = Mathf.Clamp(threshold + gs.mutation.energySensorThresholdChangeMaxAmount * gs.mutation.RandomDistributedValue(), 0f, gs.phenotype.cellMaxEnergy);
		}
		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	// Save
	private GeneEnergySensorData data = new GeneEnergySensorData();
	public GeneEnergySensorData UpdateData() {
		data.areaRadius = areaRadius;
		data.energyThreshold = threshold;
		return data;
	}

	// Load
	public void ApplyData(GeneEnergySensorData geneEnergySensorData) {
		areaRadius = Mathf.Max(1, geneEnergySensorData.areaRadius);
		threshold = geneEnergySensorData.energyThreshold;
	}
}
