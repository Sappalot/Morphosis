using System;
[Serializable]
public class GeneData {
	public CellTypeEnum type = CellTypeEnum.Leaf;
	public int index;

	// Egg
	public GeneLogicBoxData eggFertilizeLogicBoxData;
	public GeneEnergySensorData eggFertilizeEnergySensorData;

	// Jaw
	public bool jawCellCannibalizeKin;
	public bool jawCellCannibalizeMother;
	public bool jawCellCannibalizeFather;
	public bool jawCellCannibalizeSiblings;
	public bool jawCellCannibalizeChildren;

	// Leaf

	// Muscle

	public int shellCellArmourClass;
	public int shellCellTransparancyClass;

	// Axon
	public bool axonIsEnabled;
	public float axonFromOriginOffset;
	public bool axonIsFromOriginPlus180;
	public float axonFromMeOffset;
	public float axonRelaxContract;
	public bool axonIsReverse;

	// Dendrites
	public GeneLogicBoxData dendritesLogicBoxData;

	// Sensors
	public GeneEnergySensorData energySensorData;
	public GeneEffectSensorData effectSensorData;

	// origin
	public int originPulsePeriodTicks; // ticks / complete wave
	public GeneLogicBoxData originDetatchLogicBoxData;
	public GeneSizeSensorData originSizeSensorData;
	public float embryoMaxSizeCompleteness;
	public int growPriorityCellPersistance; // s

	public float buildPriorityBias;
	public ArrangementData[] arrangementData = new ArrangementData[3];
}