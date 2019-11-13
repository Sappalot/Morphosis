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

	public int areaRadiusFlux = 1;
	public float effectThresholdFlux;

	public int areaRadiusExternal = 1;
	public float effectThresholdExternal; // hurt by jaw

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

	public void Mutate(float strength) {
		GlobalSettings gs = GlobalSettings.instance;
		float rnd;

		// TODO
		//rnd = Random.Range(0, gs.mutation.energySensorAreaRadiusChange * strength + 1000f);
		//if (rnd < gs.mutation.energySensorAreaRadiusChange * strength) {
		//	areaRadius = (int)Mathf.Clamp(areaRadius + gs.mutation.energySensorAreaRadiusChangeMaxAmount * gs.mutation.RandomDistributedValue(), 0f, Creature.maxRadiusHexagon - 1);
		//}

		//rnd = Random.Range(0, gs.mutation.energySensorThresholdChange * strength + 1000f);
		//if (rnd < gs.mutation.energySensorThresholdChange * strength) {
		//	threshold = Mathf.Clamp(threshold + gs.mutation.energySensorThresholdChangeMaxAmount * gs.mutation.RandomDistributedValue(), 0f, gs.phenotype.cellMaxEnergy);
		//}
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
	public void ApplyData(GeneEffectSensorData geneEnergySensorData) {
		effectMeassure = data.effectMeassure;

		areaRadiusTotal = Mathf.Max(1, data.areaRadiusTotal);
		effectThresholdTotal = data.effectThresholdTotal;

		areaRadiusProduction = Mathf.Max(1, data.areaRadiusProduction);
		effectThresholdProduction = data.effectThresholdProduction;

		areaRadiusFlux = Mathf.Max(1, data.areaRadiusFlux);
		effectThresholdFlux = data.effectThresholdFlux;

		areaRadiusExternal = Mathf.Max(1, data.areaRadiusExternal);
		effectThresholdExternal = data.effectThresholdExternal;
	}
}
