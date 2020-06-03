using UnityEngine;


// TODO: Rename it growth sensor
public class GeneSizeSensor : GeneSignalUnit {
	public GeneSizeSensor(SignalUnitEnum signalUnit, IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
		this.signalUnit = signalUnit;
	}

	private float m_sizeThreshold = 0.5f; // 50% of full size
	public float sizeThreshold {
		get {
			return m_sizeThreshold;
		}
		set {
			m_sizeThreshold = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	private int m_cantGrowMorePatienseThreshold = 10; // seconds of blocked growth
	public int cantGrowMorePatienseThreshold {
		get {
			return m_cantGrowMorePatienseThreshold;
		}
		set {
			m_cantGrowMorePatienseThreshold = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	private IGenotypeDirtyfy genotypeDirtyfy;

	public void Defaultify() {
		sizeThreshold = 0.5f; // 50% of full size
		cantGrowMorePatienseThreshold = 10; // seconds of blocked growth

		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	public void Randomize() {

		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

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

		genotypeDirtyfy.ReforgeCellPatternAndForward();
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
