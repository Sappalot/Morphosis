using System;
[Serializable]
public class GeneData {
	public CellTypeEnum type = CellTypeEnum.Leaf;
	public int index;

	//Egg
	public float eggCellFertilizeThreshold; // J 
	public float eggCellDetatchThreshold; //J 

	public ArrangementData[] arrangementData = new ArrangementData[3];
}