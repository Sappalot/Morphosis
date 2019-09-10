using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneEffectSensor : GeneSensor {
	public GeneEffectSensor(SignalUnitEnum signalUnit, bool isLocked) {
		this.signalUnit = signalUnit;
		this.isLocked = isLocked;
	}

	public float threshold = 1f; // W
}
