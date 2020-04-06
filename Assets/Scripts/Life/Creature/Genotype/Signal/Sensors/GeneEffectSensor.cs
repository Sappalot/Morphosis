using UnityEngine;

public class GeneEffectSensor : GeneSignalUnit {
	public GeneEffectSensor(SignalUnitEnum signalUnit) {
		this.signalUnit = signalUnit;
	}

	public EffectMeassureEnum effectMeassure = EffectMeassureEnum.Total;

	public int areaRadiusTotal = 1;
	public float effectThresholdTotal;

	public int areaRadiusProduction = 1;
	public float effectThresholdProduction;

	public int areaRadiusExternal = 1;
	public float effectThresholdExternal; // hurt by jaw

	public int areaRadiusFlux = 1;
	public float effectThresholdFlux;

	public int usedAreaRadius {
		get {
			switch (effectMeassure) {
				case EffectMeassureEnum.Total:
					return areaRadiusTotal;
				case EffectMeassureEnum.Production:
					return areaRadiusProduction;
				case EffectMeassureEnum.Flux:
					return areaRadiusFlux;
				case EffectMeassureEnum.External:
					return areaRadiusExternal;
			}
			return -1;
		}
		set {
			switch (effectMeassure) {
				case EffectMeassureEnum.Total:
					areaRadiusTotal = value;
					break;
				case EffectMeassureEnum.Production:
					areaRadiusProduction = value;
					break;
				case EffectMeassureEnum.Flux:
					areaRadiusFlux = value;
					break;
				case EffectMeassureEnum.External:
					areaRadiusExternal = value;
					break;
			}
		}
	}

	public float usedThreshold {
		get {
			switch (effectMeassure) {
				case EffectMeassureEnum.Total:
					return effectThresholdTotal;
				case EffectMeassureEnum.Production:
					return effectThresholdProduction;
				case EffectMeassureEnum.Flux:
					return effectThresholdFlux;
				case EffectMeassureEnum.External:
					return effectThresholdExternal;
			}
			return -666666666f;
		}
		set {
			switch (effectMeassure) {
				case EffectMeassureEnum.Total:
					effectThresholdTotal = value;
					break;
				case EffectMeassureEnum.Production:
					effectThresholdProduction = value;
					break;
				case EffectMeassureEnum.Flux:
					effectThresholdFlux = value;
					break;
				case EffectMeassureEnum.External:
					effectThresholdExternal = value;
					break;
			}
		}
	}

	public void Defaultify() {
		effectMeassure = EffectMeassureEnum.Total;

		areaRadiusTotal = 1;
		effectThresholdTotal = 0f;

		areaRadiusProduction = 1;
		effectThresholdProduction = 0f;

		areaRadiusFlux = 1;
		effectThresholdFlux = 0f;

		areaRadiusExternal = 1;
		effectThresholdExternal = 0f; // hurt by jaw
	}

	public void Randomize() {

	}

	public void Mutate(float strength) {
		GlobalSettings gs = GlobalSettings.instance;
		float rnd;
		rnd = Random.Range(0, gs.mutation.effectSensorMeassureChange * strength + 1000f);
		if (rnd < gs.mutation.effectSensorMeassureChange * strength) {
			int meassure = Random.Range(0, 4);
			effectMeassure = (EffectMeassureEnum)meassure;
		}

		rnd = Random.Range(0, gs.mutation.effectSensorAreaRadiusChange * strength + 1000f);
		if (rnd < gs.mutation.effectSensorAreaRadiusChange * strength) {
			areaRadiusTotal = (int)Mathf.Clamp(areaRadiusTotal + gs.mutation.effectSensorAreaRadiusChangeMaxAmount * gs.mutation.RandomDistributedValue(), 0f, Creature.maxRadiusHexagon - 1);
		}
		rnd = Random.Range(0, gs.mutation.effectSensorThresholdChange * strength + 1000f);
		if (rnd < gs.mutation.effectSensorThresholdChange * strength) {
			effectThresholdTotal = Mathf.Clamp(effectThresholdTotal + gs.mutation.effectSensorThresholdChangeMaxAmount * gs.mutation.RandomDistributedValue(), -GlobalSettings.instance.mutation.effectSensorThresholdMax, GlobalSettings.instance.mutation.effectSensorThresholdMax);
		}

		rnd = Random.Range(0, gs.mutation.effectSensorAreaRadiusChange * strength + 1000f);
		if (rnd < gs.mutation.effectSensorAreaRadiusChange * strength) {
			areaRadiusProduction = (int)Mathf.Clamp(areaRadiusProduction + gs.mutation.effectSensorAreaRadiusChangeMaxAmount * gs.mutation.RandomDistributedValue(), 0f, Creature.maxRadiusHexagon - 1);
		}
		rnd = Random.Range(0, gs.mutation.effectSensorThresholdChange * strength + 1000f);
		if (rnd < gs.mutation.effectSensorThresholdChange * strength) {
			effectThresholdProduction = Mathf.Clamp(effectThresholdProduction + gs.mutation.effectSensorThresholdChangeMaxAmount * gs.mutation.RandomDistributedValue(), -GlobalSettings.instance.mutation.effectSensorThresholdMax, GlobalSettings.instance.mutation.effectSensorThresholdMax);
		}

		rnd = Random.Range(0, gs.mutation.effectSensorAreaRadiusChange * strength + 1000f);
		if (rnd < gs.mutation.effectSensorAreaRadiusChange * strength) {
			areaRadiusFlux = (int)Mathf.Clamp(areaRadiusFlux + gs.mutation.effectSensorAreaRadiusChangeMaxAmount * gs.mutation.RandomDistributedValue(), 0f, Creature.maxRadiusHexagon - 1);
		}
		rnd = Random.Range(0, gs.mutation.effectSensorThresholdChange * strength + 1000f);
		if (rnd < gs.mutation.effectSensorThresholdChange * strength) {
			effectThresholdFlux = Mathf.Clamp(effectThresholdFlux + gs.mutation.effectSensorThresholdChangeMaxAmount * gs.mutation.RandomDistributedValue(), -GlobalSettings.instance.mutation.effectSensorThresholdMax, GlobalSettings.instance.mutation.effectSensorThresholdMax);
		}

		rnd = Random.Range(0, gs.mutation.effectSensorAreaRadiusChange * strength + 1000f);
		if (rnd < gs.mutation.effectSensorAreaRadiusChange * strength) {
			areaRadiusExternal = (int)Mathf.Clamp(areaRadiusExternal + gs.mutation.effectSensorAreaRadiusChangeMaxAmount * gs.mutation.RandomDistributedValue(), 0f, Creature.maxRadiusHexagon - 1);
		}
		rnd = Random.Range(0, gs.mutation.effectSensorThresholdChange * strength + 1000f);
		if (rnd < gs.mutation.effectSensorThresholdChange * strength) {
			effectThresholdExternal = Mathf.Clamp(effectThresholdExternal + gs.mutation.effectSensorThresholdChangeMaxAmount * gs.mutation.RandomDistributedValue(), -GlobalSettings.instance.mutation.effectSensorThresholdMax, GlobalSettings.instance.mutation.effectSensorThresholdMax);
		}
	}

	// Save
	private GeneEffectSensorData data = new GeneEffectSensorData();
	public GeneEffectSensorData UpdateData() {
		data.effectMeassure = effectMeassure;

		data.areaRadiusTotal = areaRadiusTotal;
		data.effectThresholdTotal = effectThresholdTotal;

		data.areaRadiusProduction = areaRadiusProduction;
		data.effectThresholdProduction = effectThresholdProduction;

		data.areaRadiusFlux = areaRadiusFlux;
		data.effectThresholdFlux = effectThresholdFlux;

		data.areaRadiusExternal = areaRadiusExternal;
		data.effectThresholdExternal = effectThresholdExternal;

		return data;
	}

	// Load
	public void ApplyData(GeneEffectSensorData geneEffectSensorData) {
		effectMeassure = geneEffectSensorData.effectMeassure;

		areaRadiusTotal = Mathf.Max(1, geneEffectSensorData.areaRadiusTotal);
		effectThresholdTotal = geneEffectSensorData.effectThresholdTotal;

		areaRadiusProduction = Mathf.Max(1, geneEffectSensorData.areaRadiusProduction);
		effectThresholdProduction = geneEffectSensorData.effectThresholdProduction;

		areaRadiusFlux = Mathf.Max(1, geneEffectSensorData.areaRadiusFlux);
		effectThresholdFlux = geneEffectSensorData.effectThresholdFlux;

		areaRadiusExternal = Mathf.Max(1, geneEffectSensorData.areaRadiusExternal);
		effectThresholdExternal = geneEffectSensorData.effectThresholdExternal;
	}
}
