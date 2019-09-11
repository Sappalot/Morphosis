﻿using System;
[Serializable]

// En between cell Panel and Cell.Signal, to know which output we are trying to update in panel, we must know its name (can't have a reference in a good way)

public enum SignalUnitEnum {
	Void,

	WorkHibernate, //4 cell types' is hibernating
	
	// Egg
	WorkEggFertilizeLogicBox, // Locked, Egg will fertilize when true, Will we be able to read this one before egg is gone?
	WorkEggEnergySensor, // Locked to WorkEggFertilize

	AxoneProgramA,
	AxoneProgramB,
	AxoneProgramC,
	AxoneProgramD,

	ChangeFilter,
	TrendFilter,
	
	LogicBox, //Dendrites
	
	EffectSensor,
	SensorA,
	SensorB,
	SensorC,

	OriginDetatch,
}
