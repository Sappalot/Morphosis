using System;
[Serializable]
public class GeneData {
    public CellTypeEnum type = CellTypeEnum.Leaf;
    public int index;

    public ArrangementData[] arrangementData = new ArrangementData[3];
}