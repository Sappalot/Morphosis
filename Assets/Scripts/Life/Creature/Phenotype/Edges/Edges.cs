using Boo.Lang.Runtime;
using System.Collections.Generic;
using UnityEngine;


public class Edges : MonoBehaviour {

	public float timeOffset;

	public LineRenderer peripheryLoop;

	public void Awake() {
		timeOffset = UnityEngine.Random.Range(0f, 7f);
	}

	public Edge edgePrefab;

	private List<Edge> edgeList = new List<Edge>();

	private void Clear() {
		for (int index = 0; index < edgeList.Count; index++) {
			//Destroy(edgeList[index].gameObject);
			Morphosis.instance.edgePool.Recycle(edgeList[index]);
		}
		edgeList.Clear();
		peripheryLoop.positionCount = 0;
	}

	//All wings will apply forces to their cells 
	
	public float longestEdge {
		get {
			float longestEdge = 0f;
			for (int index = 0; index < edgeList.Count; index++) {
				Edge e = edgeList[index];
				longestEdge = Mathf.Max(e.length, longestEdge);
			}
			return longestEdge;
		}
	}

	public void UpdatePhysics(Creature creature) {
		//Todo do this more seldom
		for (int index = 0; index < edgeList.Count; index++) {
			tempEdge = edgeList[index];
			if (tempEdge.IsWing) {
				tempEdge.UpdateNormal();
				tempEdge.UpdateVelocity();
				tempEdge.UpdateForce(creature);
			}
		}
		for (int index = 0; index < edgeList.Count; index++) {
			tempEdge = edgeList[index];
			if (tempEdge.IsWing) { // So, we can see forces even if they are not applied
				tempEdge.ApplyForce();
			}
		}
	}
	private Edge tempEdge;

	public void UpdateGraphics(bool show) {
		for (int index = 0; index < edgeList.Count; index++) {
			//edges
			Edge edge = edgeList[index];
			if (edge.IsWing) {
				edge.UpdateGraphics(); // force arrow only
			}

			//Periphery
			if (show) {
				peripheryLoop.enabled = true;
				if (index < peripheryLoop.positionCount) {
					if (edgeList[index].parentCell != null) {
						peripheryLoop.SetPosition(index, edgeList[index].parentCell.position);
					}
				}
			} else {
				peripheryLoop.enabled = false;
			}
		}
	} 

	public void ShuffleEdgeUpdateOrder() {
		ListUtil.Shuffle(edgeList);
	}

	public void GenerateWings(Creature creature, CellMap cellMap) {
		Clear();

		if (cellMap.cellCount == 1)
			return;

		//Cell firstCell = cellMap.GetRightmostCellInModelSpace();
		Cell firstCell = GetRightmostCellInModelSpace(creature); // We try to use cell list instead of map

		Cell currentCell = firstCell;
		Cell previousCell = null;
		for (int safe = 0; safe < 1000; safe++) {
			Cell nextCell = GetNextOwnPeripheryCell(creature, currentCell, previousCell);
			if (nextCell == null) {
				Debug.Log("We don't have a next periphery cell");
				throw new RuntimeException("Could not find a next periphery cell");
			}


			//We are around, since we are trying to create an edge which would be the same as the first one
			if (edgeList.Exists(e => e.parentCell == currentCell && e.childCell == nextCell)) {
				break;
			}

			//Edge edge = (GameObject.Instantiate(edgePrefab, transform.position, Quaternion.identity) as Edge);
			Edge edge = Morphosis.instance.edgePool.Borrow();
			edge.transform.parent = transform;
			edge.transform.position = transform.position;
			edgeList.Add(edge);
			edge.Setup(currentCell, nextCell, currentCell.GetDirectionOfOwnNeighbourCell(creature, nextCell));
			edge.MakeWing(nextCell);

			previousCell = currentCell;
			currentCell = nextCell;
		}

		//periphery
		peripheryLoop.positionCount = edgeList.Count;
	}

	private Cell GetRightmostCellInModelSpace(Creature creature) {
		Cell record = creature.phenotype.cellList[0]; //sure to have oirgin, at least
		foreach (Cell cell in creature.phenotype.cellList) {
			if (cell.modelSpacePosition.x > record.modelSpacePosition.x) {
				record = cell;
			}
		}
		return record;
	}

	private Cell GetNextOwnPeripheryCell(Creature creature, Cell currentCell, Cell previousCell) { 
		int previousDirection = -1; // -1 and 0 should both work, right? // We need to have a previous direction which is pointing east in model space
		if (currentCell == null) {
			Debug.LogError("currentCell = NULL");
		}

		if (previousCell != null) {
			previousDirection = currentCell.GetDirectionOfOwnNeighbourCell(creature, previousCell);
		}

		for (int index = previousDirection + 1; index < previousDirection + 7; index++) {
			Cell tryCell = currentCell.GetNeighbourCell(index);
			if (tryCell != null && tryCell.creature == creature) { // We don't want to trace around connected creatures
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

	public void OnRecycle() {
		Clear();
	}
}