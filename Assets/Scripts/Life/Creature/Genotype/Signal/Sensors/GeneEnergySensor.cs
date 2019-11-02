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
