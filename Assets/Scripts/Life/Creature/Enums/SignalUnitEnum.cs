using System;


// In between cell Panel and Cell.Signal, to know which output we are trying to update in panel, we must know its name (can't have a reference in a good way)
[Serializable]
public enum SignalUnitEnum {
	Void = 0,

	WorkHibernate = 1, //4 cell types' is hibernating
	

	WorkLogicBoxA = 2, // Egg: Fertilize asexually
	WorkLogicBoxB = 3, // Egg: fertilize sexually
	WorkSensorA = 4, // Egg: Energy fertilize
	WorkSensorB = 5, // Egg: Attachment fertilize
	WorkSensorC = 6, 
	WorkSensorD = 7, 

	ConstantSensor = 8,

	Axon = 9,

	FilterChange = 10,
	FilterTrend = 11,
	
	DendritesLogicBox = 12, //Dendrites
	
	EnergySensor = 13,
	EffectSensor = 14,
	SurroundingSensor = 17, //eye

	OriginDetatchLogicBox = 15,
	OriginSizeSensor = 16,
}