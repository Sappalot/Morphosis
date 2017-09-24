﻿using UnityEngine;
using System.Collections.Generic;

// A blueprint of the final creature
// A way to know how cells are positioned relative to each other in the final creature

public class CellMap {
	private Dictionary<GridPosition, Cell> grid = new Dictionary<GridPosition, Cell>();
	private List<Vector2i> illegalPositions = new List<Vector2i>();
	public float cellRadius = 0.5f;

	public void Clear() {
		grid.Clear();
		illegalPositions.Clear();
	}

	public bool HasCell(Vector2i gridPosition) {
		return GetCell(gridPosition) != null;
	}

	public Cell GetCell(Vector2i gridPosition) {
		if (grid.ContainsKey(new GridPosition(gridPosition))) {
			return grid[new GridPosition(gridPosition)];
		}
		return null;
	}

	public void SetCell(Vector2i gridPosition, Cell cell) {
		grid.Add(new GridPosition(gridPosition), cell);
		cell.modelSpacePosition = ToModelSpacePosition(gridPosition);
	}

	public void RemoveCellAtGridPosition(Vector2i gridPosition) {
		grid.Remove(new GridPosition(gridPosition));
	}

	public bool IsLegalPosition(Vector2i gridPosition) {
		return !illegalPositions.Contains(gridPosition);
	}

	public void MarkAsIllegal(Vector2i gridPosition) {
		illegalPositions.Add(gridPosition);
	}

	public Cell GetGridNeighbourCell(Vector2i gridPosition, CardinalEnum direction) {
		return GetGridNeighbourCell(gridPosition, AngleUtil.CardinalEnumToCardinalIndex(direction));
	}

	public Cell GetGridNeighbourCell(Vector2i gridPosition, int direction) {
		return GetCell(GetGridNeighbourGridPosition(gridPosition, direction));
	}

	public Vector2i GetGridNeighbourGridPosition(Vector2i gridPosition, CardinalEnum direction) {
		return GetGridNeighbourGridPosition(gridPosition, AngleUtil.CardinalEnumToCardinalIndex(direction));
	}

	public Vector2i GetGridNeighbourGridPosition(Vector2i gridPosition, int direction) {
		Vector2i neighbour = null;
		int even = (gridPosition.x % 2 == 0) ? 1 : 0;
		int odd = (gridPosition.x % 2 == 0) ? 0 : 1;
		if (direction == 0) {
			neighbour = new Vector2i(gridPosition.x + 1, gridPosition.y + odd);
		}
		if (direction == 1) {
			neighbour = new Vector2i(gridPosition.x, gridPosition.y + 1);
		}
		if (direction == 2) {
			neighbour = new Vector2i(gridPosition.x - 1, gridPosition.y + odd);
		}
		if (direction == 3) {
			neighbour = new Vector2i(gridPosition.x - 1, gridPosition.y - even);
		}
		if (direction == 4) {
			neighbour = new Vector2i(gridPosition.x, gridPosition.y - 1);
		}
		if (direction == 5) {
			neighbour = new Vector2i(gridPosition.x + 1, gridPosition.y - even);
		}
		return neighbour;
	}

	//returned position is in creature space
	public Vector2 ToModelSpacePosition(Vector2i gridPosition) {
		float xStride = Mathf.Sqrt(Mathf.Pow(cellRadius * 2, 2) - Mathf.Pow(cellRadius, 2));
		float yStride = cellRadius * 2;

		float displace = (gridPosition.x % 2 == 0) ? 0f : cellRadius;
		return new Vector2(xStride * gridPosition.x, yStride * gridPosition.y + displace);
	}

	public List<Cell> GetCells() {
		Cell[] cells = new Cell[grid.Count];
		grid.Values.CopyTo(cells, 0);
		List<Cell> cellList = new List<Cell>();
		foreach (Cell c in cells) {
			cellList.Add(c);
		}
		return cellList;
	}

	public Cell GetRightmostCell() {
		List<Cell> cells = GetCells();
		Cell record = cells[0];
		foreach (Cell cell in cells) {
			if (cell.modelSpacePosition.x > record.modelSpacePosition.x) {
				record = cell;
			}
		}
		return record;
	}

	public Vector2i ToGridPositionTODO(Vector3 position) {
		// TODO
		return new Vector2i();
	}

	public struct GridPosition {
		public readonly int x;
		public readonly int y;
		public GridPosition(Vector2i gridPosition) {
			x = gridPosition.x;
			y = gridPosition.y;
		}

		public override bool Equals(object obj) {
			// If parameter is null return false.
			if (obj == null) {
				return false;
			}

			// If parameter cannot be cast to Point return false.
			GridPosition p = (GridPosition)obj;
			if ((object)p == null) {
				return false;
			}

			// Return true if the fields match:
			return (x == p.x) && (y == p.y);
		}

		public override int GetHashCode() {
			int hash = 13;
			hash = (hash * 7) + x.GetHashCode();
			hash = (hash * 7) + y.GetHashCode();
			return hash;
		}
	}
}
