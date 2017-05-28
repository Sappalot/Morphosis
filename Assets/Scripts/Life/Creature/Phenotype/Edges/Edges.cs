using System.Collections.Generic;
using UnityEngine;

public class Edges : MonoBehaviour {

    public float timeOffset;

    public void Awake() {
        timeOffset = Random.Range(0f, 7f);
    }

    public Edge edgePrefab;

    private List<Edge> edgeList = new List<Edge>();

    public void Clear() {
        foreach (Edge edge in edgeList) {
            GameObject.Destroy(edge);
        }
        edgeList.Clear();
    }



    /*public void UpdateWings(List<Cell> cellList) {
        RemoveWings();
        GenerateWings(cellList);
    }

    private void RemoveWings() {
        foreach (Edge edge in edgeList) {
            //edge.isWing = false;
        }
    }*/

    //All wings will apply forces to their cells 
    public void EvoFixedUpdate(Vector3 creatureVelocity, Creature creature) {
        foreach (Edge edge in edgeList) {
            if (edge.IsWing) {
                edge.UpdateNormal();
                edge.UpdateVelocity();
                edge.UpdateForce(creatureVelocity, creature);
            }
        }
        foreach (Edge edge in edgeList) {
            if (edge.IsWing) {
                edge.ApplyForce();
            }
        }
    }

    public void EvoUpdate() {
        foreach (Edge edge in edgeList) {
            if (edge.IsWing) {
                edge.EvoUpdate();
            }
        }
    } 

    public void GenerateWings(List<Cell> cellList) {
        if (cellList.Count < 2)
            return;

        Cell firstCell = GetRightmostCell(cellList);
        Cell currentCell = firstCell;
        Cell previousCell = null;
        for (int safe = 0; safe < 1000; safe++) {
            Cell nextCell = GetNextPeripheryCell(currentCell, previousCell);
            Edge edge = (GameObject.Instantiate(edgePrefab, transform.position, Quaternion.identity) as Edge);
            edge.transform.parent = transform;
            //edge.frontCell = nextCell;
            //edge.backCell = currentCell;
            edgeList.Add(edge);
            edge.Setup(currentCell, nextCell, currentCell.GetDirectionOfNeighbourCell(nextCell), false );
            edge.MakeWing(nextCell);
            if (nextCell == firstCell) {
                break;
            }
            previousCell = currentCell;
            currentCell = nextCell;
        }
    }

    private Cell GetNextPeripheryCell(Cell currentCell, Cell previousCell) {
        int previousDirection = 0;
        if (previousCell != null) {
            previousDirection = currentCell.GetDirectionOfNeighbourCell(previousCell);
        }

        for (int index = previousDirection + 1; index < previousDirection + 7; index++) {
            if (currentCell.HasNeighbourCell(index)) {
                return currentCell.GetNeighbour(index).cell;
            }
        }
        return null;
    }

    private Cell GetRightmostCell(List<Cell> cellList) {
        float leftValueRecord = float.NegativeInfinity;
        Cell leftCellRecord = null;
        foreach (Cell cell in cellList) {
            if (cell.transform.localPosition.x > leftValueRecord) {
                leftCellRecord = cell;
                leftValueRecord = cell.transform.localPosition.x;
            }
        }
        return leftCellRecord;
    }

}

