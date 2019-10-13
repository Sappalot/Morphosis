using System;
[Serializable]
public class GeneData {
	public CellTypeEnum type = CellTypeEnum.Leaf;
	public int index;

	// Egg
	public ChildDetatchModeEnum eggCellDetatchMode; //J 
	public float eggCellDetatchSizeThreshold; //J 
	public float eggCellDetatchEnergyThreshold; //J
	public bool eggCellHibernateWhenAttachedToMother;
	public bool eggCellHibernateWhenAttachedToChild;
	public GeneLogicBoxData eggFertilizeLogicBoxData;
	public GeneEnergySensorData eggFertilizeEnergySensorData;

	// Jaw
	public bool jawCellCannibalizeKin;
	public bool jawCellCannibalizeMother;
	public bool jawCellCannibalizeFather;
	public bool jawCellCannibalizeSiblings;
	public bool jawCellCannibalizeChildren;
	public bool jawCellHibernateWhenAttachedToMother;
	public bool jawCellHibernateWhenAttachedToChild;

	// Leaf
	public bool leafCellHibernateWhenAttachedToMother;
	public bool leafCellHibernateWhenAttachedToChild;

	// Muscle
	public bool muscleCellHibernateWhenAttachedToMother;
	public bool muscleCellHibernateWhenAttachedToChild;

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
	
	// origin
	public int originPulsePeriodTicks; // ticks / complete wave
	public GeneLogicBoxData originDetatchLogicBoxData;

	public float buildPriorityBias;
	public ArrangementData[] arrangementData = new ArrangementData[3];
}