using System.Collections.Generic;
using UnityEngine;


public class Edges : MonoBehaviour {

	public float timeOffset;

	public void Awake() {
		timeOffset = UnityEngine.Random.Range(0f, 7f);
	}

	public Edge edgePrefab;

	private List<Edge> edgeList = new List<Edge>();

	private void Clear() {
		for (int index = 0; index < edgeList.Count; index++) {
			Destroy(edgeList[index].gameObject);
		}
		edgeList.Clear();
	}

	//All wings will apply forces to their cells 
	public void UpdatePhysics(Vector3 creatureVelocity, Creature creature) {
		//Todo do this more seldom
		for (int index = 0; index < edgeList.Count; index++) {
			Edge edge = edgeList[index];
			if (edge.IsWing) {
				edge.UpdateNormal();
				edge.UpdateVelocity();
				edge.UpdateForce(creatureVelocity, creature);
			}
		}
		for (int index = 0; index < edgeList.Count; index++) {
			Edge edge = edgeList[index];
			if (edge.IsWing && GlobalPanel.instance.physicsApplyWingForce.isOn) {
				edge.ApplyForce();
			}
		}
	}

	public void UpdateGraphics() {
		for (int index = 0; index < edgeList.Count; index++) {
			Edge edge = edgeList[index];
			if (edge.IsWing) {
				edge.UpdateGraphics();
			}
		}
	} 

	public void ShuffleEdgeUpdateOrder() {
		ListUtils.Shuffle(edgeList);
	}

	public void GenerateWings(Creature creature, CellMap cellMap) {
		Clear();

		if (cellMap.cellCount == 1)
			return;

		Cell firstCell = cellMap.GetRightmostCellInModelSpace();

		Cell currentCell = firstCell;
		Cell previousCell = null;
		for (int safe = 0; safe < 1000; safe++) {
			Cell nextCell = GetNextOwnPeripheryCell(creature, currentCell, previousCell);
			if (nextCell == null) {
				Debug.Log("We don't have a next periphery cell");
			}
			Edge edge = (GameObject.Instantiate(edgePrefab, transform.position, Quaternion.identity) as Edge);
			edge.transform.parent = transform;
			edgeList.Add(edge);
			edge.Setup(currentCell, nextCell, currentCell.GetDirectionOfOwnNeighbourCell(creature, nextCell));
			edge.MakeWing(nextCell);
			if (nextCell == firstCell) {
				break;
			}
			previousCell = currentCell;
			currentCell = nextCell;
		}
	}

	private Cell GetNextOwnPeripheryCell(Creature creature, Cell currentCell, Cell previousCell) {
		int previousDirection = 0; // We need to have a previous direction which is pointing east in world space
		if (currentCell == null) {
			Debug.LogError("currentCell = NULL");
		}

		if (previousCell != null) {
			previousDirection = currentCell.GetDirectionOfOwnNeighbourCell(creature, previousCell);
		}

		for (int index = previousDirection + 1; index < previousDirection + 7; index++) {
			Cell tryCell = currentCell.GetNeighbourCell(index);
			if (tryCell != null && tryCell.creature == creature) { // We dont want to trace around connected creatures
				return tryCell;
			}
		}

		return null;
	}

	//TODO: Find rightmose cell from bind pose rather than flexi pose
	private Cell GetRightmostCell(List<Cell> cellList) {
		float leftValueRecord = float.NegativeInfinity;
		Cell leftCellRecord = null;
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			if (cell.transform.localPosition.x > leftValueRecord) {
				leftCellRecord = cell;
				leftValueRecord = cell.transform.localPosition.x;
			}
		}
		return leftCellRecord;
	}
}