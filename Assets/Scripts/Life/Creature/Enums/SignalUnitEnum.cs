using System;


// In between cell Panel and Cell.Signal, to know which output we are trying to update in panel, we must know its name (can't have a reference in a good way)
[Serializable]
public enum SignalUnitEnum {
	Void,

	WorkHibernate, //4 cell types' is hibernating
	

	WorkLogicBoxA, // Egg: Fertilize asexually
	WorkLogicBoxB, // Egg: fertilize sexually
	WorkSensorA, // Egg: Energy fertilize
	WorkSensorB, // Egg: Attachment fertilize
	WorkSensorC, 
	WorkSensorD, 

	ConstantSensor,

	Axon,

	FilterChange,
	FilterTrend,
	
	DendritesLogicBox, //Dendrites
	
	EnergySensor,
	EffectSensor,

	OriginDetatchLogicBox,
	OriginSizeSensor,
}
