using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GeneEffectSensorData {
	public EffectMeassureEnum effectMeassure = EffectMeassureEnum.Total;

	public int areaRadiusTotal;
	public float effectThresholdTotal;

	public int areaRadiusProduction;
	public float effectThresholdProduction;

	public int areaRadiusFlux;
	public float effectThresholdFlux;

	public int areaRadiusExternal; 
	public float effectThresholdExternal; // hurt by jaw
}