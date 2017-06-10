using UnityEngine;
using System.Collections.Generic;

// A blueprint of the final creature
// A way to know how cells are positioned relative to each other in the final creature

public class CellMap {
    private Dictionary<Key, Cell> grid = new Dictionary<Key, Cell>();
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
        if (grid.ContainsKey(new Key(gridPosition))) {
            return grid[new Key(gridPosition)];
        }
        return null;
    }

    public void SetCell(Vector2i gridPosition, Cell cell) {
        grid.Add(new Key(gridPosition), cell);
    }

    public void RemoveCellAtGridPosition(Vector2i gridPosition) {
        grid.Remove(new Key(gridPosition));
    }

    public bool IsLegalPosition(Vector2i gridPosition) {
        return !illegalPositions.Contains(gridPosition);

    }

    public void MarkAsIllegal(Vector2i gridPosition) {
        illegalPositions.Add(gridPosition);
    }

    public Cell GetGridNeighbourCell(Vector2i gridPosition, CardinalDirectionEnum direction) {
        return GetGridNeighbourCell(gridPosition, CardinalDirectionUtil.ToIndex(direction));
    }

    public Cell GetGridNeighbourCell(Vector2i gridPosition, int direction) {
        return GetCell(GetGridNeighbourGridPosition(gridPosition, direction));
    }

    public Vector2i GetGridNeighbourGridPosition(Vector2i gridPosition, CardinalDirectionEnum direction) {
        return GetGridNeighbourGridPosition(gridPosition, CardinalDirectionUtil.ToIndex(direction));
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
    public Vector3 ToPosition(Vector2i gridPosition) {
        float xStride = Mathf.Sqrt(Mathf.Pow(cellRadius * 2, 2) - Mathf.Pow(cellRadius, 2));
        float yStride = cellRadius * 2;

        float displace = (gridPosition.x % 2 == 0) ? 0f : cellRadius;
        return new Vector3(xStride * gridPosition.x, yStride * gridPosition.y + displace, 0f);
    }

    public Vector2i ToGridPosition(Vector3 position) {
        // TODO
        return new Vector2i();
    }

    struct Key {
        public readonly int x;
        public readonly int y;
        public Key(Vector2i gridPosition) {
            x = gridPosition.x;
            y = gridPosition.y;
        }

        public override bool Equals(object obj) {
            // If parameter is null return false.
            if (obj == null) {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Key p = (Key)obj;
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

