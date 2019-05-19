using System;
[Serializable]
public class GeneData {
	public CellTypeEnum type = CellTypeEnum.Leaf;
	public int index;

	// Egg
	public float eggCellFertilizeThreshold; // J
	public ChildDetatchModeEnum eggCellDetatchMode; //J 
	public float eggCellDetatchSizeThreshold; //J 
	public float eggCellDetatchEnergyThreshold; //J
	public bool eggCellIdleWhenAttached;

	// Jaw
	public bool jawCellCannibalizeKin;
	public bool jawCellCannibalizeMother;
	public bool jawCellCannibalizeFather;
	public bool jawCellCannibalizeSiblings;
	public bool jawCellCannibalizeChildren;
	public bool jawCellIdleWhenAttached;

	// Leaf
	public bool leafCellIdleWhenAttached;

	// Muscle
	public bool muscleCellIdleWhenAttached;

	public int shellCellArmourClass;
	public int shellCellTransparancyClass;

	// Axon
	public bool axonIsEnabled;
	public float axonFromOriginOffset;
	public bool axonIsFromOriginPlus180;
	public float axonFromMeOffset;
	public float axonRelaxContract;
	public bool axonIsReverse;

	public int originPulsePeriodTicks; // ticks / complete wave
	public float buildPriorityBias;
	public ArrangementData[] arrangementData = new ArrangementData[3];
}