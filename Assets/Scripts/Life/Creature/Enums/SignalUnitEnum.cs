using System;
[Serializable]

// En between cell Panel and Cell.Signal, to know which output we are trying to update in panel, we must know its name (can't have a reference in a good way)

public enum SignalUnitEnum {
	Void,

	WorkHibernate, //4 cell types' is hibernating
	

	WorkLogicBoxA, // Egg: Fertilize asexually
	WorkLogicBoxB, // Egg: fertilize sexually
	WorkSensorA, // Egg: Energy fertilize
	WorkSensorB,
	WorkSensorC, 
	WorkSensorD, 
	WorkSensorE, // All: constant

	Axon,

	FilterChange,
	FilterTrend,
	
	Dendrites, //Dendrites
	
	EnergySensor,

	OriginDetatch,
}
