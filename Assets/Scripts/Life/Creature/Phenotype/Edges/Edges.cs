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

    //All wings will apply forces to their cells 
    public void UpdateWingForces(Vector3 creatureVelocity, Creature creature) {
        foreach (Edge edge in edgeList) {
            if (edge.isWing) {
                edge.UpdateNormal();
                edge.UpdateVelocity();
                edge.UpdateForce(creatureVelocity, creature);
            }
        }
        foreach (Edge edge in edgeList) {
            if (edge.isWing) {
                edge.ApplyForce();
            }
        }
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

    public void GenerateWings(List<Cell> cellList) {
        if (cellList.Count < 2)
            return;

        Cell firstCell = GetRightmostCell(cellList);
        Cell currentCell = firstCell;
        Cell previousCell = null;
        for (int safe = 0; safe < 1000; safe++) {
            Cell nextCell = getNextPeripheryCell(currentCell, previousCell);
            Edge edge = (GameObject.Instantiate(edgePrefab, transform.position, Quaternion.identity) as Edge);
            edge.transform.parent = transform;
            //edge.frontCell = nextCell;
            //edge.backCell = currentCell;
            edgeList.Add(edge);
            edge.Setup(currentCell, nextCell, currentCell.getDirectionOfNeighbourCell(nextCell), false );
            edge.MakeWing(nextCell);
            if (nextCell == firstCell) {
                break;
            }
            previousCell = currentCell;
            currentCell = nextCell;
        }
    }

    private Cell getNextPeripheryCell(Cell currentCell, Cell previousCell) {
        int previousDirection = 0;
        if (previousCell != null) {
            previousDirection = currentCell.getDirectionOfNeighbourCell(previousCell);
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

