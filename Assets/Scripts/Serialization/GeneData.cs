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

	public ArrangementData[] arrangementData = new ArrangementData[3];
}