using UnityEngine;

public class GeneSizeSensor : GeneSignalUnit {
	public GeneSizeSensor(SignalUnitEnum signalUnit) {
		this.signalUnit = signalUnit;
	}

	public float sizeThreshold = 0.5f; // 50% of full size
	public int cantGrowMorePatienseThreshold = 10; // seconds of blocked growth

	public void Mutate(float strength) {
		GlobalSettings gs = GlobalSettings.instance;
		float rnd;

		rnd = Random.Range(0, gs.mutation.sizeSensorSizeThresholdChange * strength + 1000f);
		if (rnd < gs.mutation.sizeSensorSizeThresholdChange * strength) {
			sizeThreshold = Mathf.Clamp(sizeThreshold + gs.mutation.sizeSensorSizeThresholdChangeMaxAmount * gs.mutation.RandomDistributedValue(), 0f, 1f);
		}

		rnd = Random.Range(0, gs.mutation.sizeSensorCantGrowMorePatienseChange * strength + 1000f);
		if (rnd < gs.mutation.sizeSensorCantGrowMorePatienseChange * strength) {
			cantGrowMorePatienseThreshold = (int)Mathf.Clamp(cantGrowMorePatienseThreshold + gs.mutation.originGrowPriorityCellPersistenceMaxAmount * gs.mutation.RandomDistributedValue(), 0f, 120f);
		}
	}

	// Save
	private GeneSizeSensorData geneSizeSensorData = new GeneSizeSensorData();
	public GeneSizeSensorData UpdateData() {
		geneSizeSensorData.sizeThreshold = sizeThreshold;
		geneSizeSensorData.growthBlockedPatienseThreshold = cantGrowMorePatienseThreshold;
		return geneSizeSensorData;
	}

	// Load
	public void ApplyData(GeneSizeSensorData geneSizeSensorData) {
		sizeThreshold = geneSizeSensorData.sizeThreshold;
		cantGrowMorePatienseThreshold = geneSizeSensorData.growthBlockedPatienseThreshold;
	}
}
