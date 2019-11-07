using UnityEngine;

public class GeneEnergySensor : GeneSignalUnit {
	public GeneEnergySensor(SignalUnitEnum signalUnit) {
		this.signalUnit = signalUnit;
	}

	private float m_threshold = 50f;
	public float threshold { 
		get {
			return Mathf.Max(m_threshold, thresholdMin);
		}
		set {
			m_threshold = Mathf.Max(value, thresholdMin);
		}

	} // joules

	public float thresholdMin = 0f; // hardcoded, no load save 

	public void Mutate(float strength) {
		GlobalSettings gs = GlobalSettings.instance;
		float rnd;

		rnd = Random.Range(0, gs.mutation.energySensorThresholdChange * strength + 1000f);
		if (rnd < gs.mutation.energySensorThresholdChange * strength) {
			threshold = Mathf.Clamp(threshold + gs.mutation.energySensorThresholdChangeMaxAmount * gs.mutation.RandomDistributedValue(), 0f, gs.phenotype.cellMaxEnergy);
		}
	}

	// Save
	private GeneEnergySensorData geneEnergySensorData = new GeneEnergySensorData();
	public GeneEnergySensorData UpdateData() {
		geneEnergySensorData.energyThreshold = threshold;
		return geneEnergySensorData;
	}

	// Load
	public void ApplyData(GeneEnergySensorData geneEnergySensorData) {
		threshold = geneEnergySensorData.energyThreshold;
	}
}
