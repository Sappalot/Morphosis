using UnityEngine;

public class CellNeighbour
{
    public CellNeighbour(int index) {
        this.index = index;
    }

    public Cell cell;
    public float angle;
    public Vector3 coreToThis;
    public int index; //cardinal direction index
}

