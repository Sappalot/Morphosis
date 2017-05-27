using UnityEngine;
using System.Collections.Generic;

public class Wings : MonoBehaviour {
    public Wing wingPrefab;
    private List<Wing> wingList = new List<Wing>();

    //All wings will apply forces to their cells 
    public void Action(Vector3 creatureVelocity) {
        foreach (Wing wing in wingList) {
            wing.UpdateNormal();
            wing.UpdateVelocity();
            wing.UpdateForce(creatureVelocity);
        }
        foreach (Wing wing in wingList) {
            wing.ApplyForce();
        }
    }

    public void Regenerate(List<Cell> cellList) {
        Remove();
        Generate(cellList);
    }

    public void Remove() {
        foreach (Wing wing in wingList) {
            GameObject.Destroy(wing);
        }
        wingList.Clear();
    }

    public void Generate(List<Cell> cellList) {
        if (cellList.Count < 2)
            return;

        Cell firstCell = GetRightmostCell(cellList);
        Cell currentCell = firstCell;
        Cell previousCell = null;
        for (int safe = 0; safe < 1000; safe++) {
            Cell nextCell = getNextPeripheryCell(currentCell, previousCell);
            Wing wing = (GameObject.Instantiate(wingPrefab, transform.position, Quaternion.identity) as Wing);
            wing.transform.parent = transform;
            wing.frontCell = nextCell;
            wing.backCell = currentCell;
            wingList.Add(wing);

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
