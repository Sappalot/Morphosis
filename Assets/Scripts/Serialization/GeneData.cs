using System;
[Serializable]
public class GeneData {
	public CellTypeEnum type = CellTypeEnum.Leaf;
	public int index;

	//Egg
	public float eggCellFertilizeThreshold; // J
	public bool eggCellCanFertilizeWhenAttached;
	public ChildDetatchModeEnum eggCellDetatchMode; //J 
	public float eggCellDetatchSizeThreshold; //J 
	public float eggCellDetatchEnergyThreshold; //J 

	//Jaw
	public bool jawCellCannibalizeKin;
	public bool jawCellCannibalizeMother;
	public bool jawCellCannibalizeFather;
	public bool jawCellCannibalizeSiblings;
	public bool jawCellCannibalizeChildren;

	public ArrangementData[] arrangementData = new ArrangementData[3];
}