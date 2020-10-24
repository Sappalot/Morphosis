using UnityEngine;
using System.Collections.Generic;
using System.IO;
using SerializerFree;
using SerializerFree.Serializers;

// A blueprint of the final creature
// A way to know how cells are positioned relative to each other in the final creature
// Used by both genotype and phenotype

public class CellMap {

	// How the grid positions will be represented in the world:

	//                v              v           v 

	//-------       -------  0: 2 -------       -------       
	//       ------- -1: 1 -------  1: 1 -------       -------
	//------- -2: 1 -------  0: 1 -------       -------       
	//       ------- -1: 0 -------  1: 0 -------       -------
	//------- -2: 0 -------  0: 0 -------       -------       
	//       ------- -1:-1 -------  1:-1 -------       -------
	//------- -2:-1 -------  0:-1 -------       -------       
	//       ------- -1:-2 -------  1:-2 -------       -------

	//         ^              ^            ^


	// Note: vector + and - can't be done like in a cartezian system

	private Dictionary<GridPosition, Cell> grid = new Dictionary<GridPosition, Cell>();
	private List<Vector2i> illegalPositions = new List<Vector2i>(); // cell position allready occupied with another cell of lower build order

	private Dictionary<Vector2i, float?> positionKilledTimeStamp = new Dictionary<Vector2i, float?>(); // phenotype
	
	private static Dictionary<Vector2i, int> radiusAtGridPosition = new Dictionary<Vector2i, int>();
	private static List<Vector2i> gridPositionsWithinRadius0 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius1 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius2 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius3 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius4 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius5 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius6 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius7 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius8 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius9 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius10 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius11 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius12 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius13 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius14 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius15 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius16 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius17 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius18 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius19 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius20 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius21 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius22 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius23 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius24 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius25 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius26 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius27 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius28 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius29 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius30 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius31 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius32 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius33 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius34 = new List<Vector2i>();
	private static List<Vector2i> gridPositionsWithinRadius35 = new List<Vector2i>();
	private static List<List<Vector2i>> gridPositionsWithinRadiusList = new List<List<Vector2i>>();


	// if i am at key take me to value and call it a rotation
	private static Dictionary<Vector2i, Vector2i> rotationMap0 = new Dictionary<Vector2i, Vector2i>(); // just for the symetry in code
	private static Dictionary<Vector2i, Vector2i> rotationMap60 = new Dictionary<Vector2i, Vector2i>();
	private static Dictionary<Vector2i, Vector2i> rotationMap120 = new Dictionary<Vector2i, Vector2i>();
	private static Dictionary<Vector2i, Vector2i> rotationMap180 = new Dictionary<Vector2i, Vector2i>();
	private static Dictionary<Vector2i, Vector2i> rotationMap240 = new Dictionary<Vector2i, Vector2i>();
	private static Dictionary<Vector2i, Vector2i> rotationMap300 = new Dictionary<Vector2i, Vector2i>();
	private static List<Dictionary<Vector2i, Vector2i>> rotationMaps = new List<Dictionary<Vector2i, Vector2i>>();

	// hexaRadius = 0; just gridPosition Cell
	public static bool IsInsideMaximumHexagon(Vector2i gridPosition) {
		return ManhexanDistanceFromOrigin(gridPosition) <= GlobalSettings.instance.phenotype.creatureHexagonMaxRadius;
	}

	public static List<Vector2i> GetGridPositionsWithinHexagon(int hexaRadius) {
		return gridPositionsWithinRadiusList[hexaRadius];
	}

	public static int ManhexanDistance(Vector2i positionA, Vector2i positionB) {
		return ManhexanDistanceFromOrigin(HexagonalMinus(positionB, positionA));
	}


	// Returns a vector with tail in origo and head at (A - B)
	public static Vector2i HexagonalMinus(Vector2i vectorA, Vector2i vectorB) {
		Vector2i transformed = vectorA - vectorB;
		if (Mathf.Abs(vectorA.x) % 2 == 0 && Mathf.Abs(vectorB.x) % 2 == 1) {
			transformed.y--;
		}
		return transformed;
	}

	public static Vector2i HexagonalPlus(Vector2i vectorA, Vector2i vectorB) {
		Vector2i transformed = vectorA + vectorB;
		if (vectorA.x % 2 != 0 && vectorB.x % 2 != 0) {
			transformed.y++;
		}
		return transformed;
	}

	// Rotate the vector counter clockwise (around its tail in origo) angle times 60 degrees. angle = 0 to 5
	public static Vector2i HexagonalRotate(Vector2i vector, int angle) {
		if (vector == new Vector2i()) {
			return new Vector2i(); // rotating origo
		}

		return rotationMaps[angle][vector];
	}

	public static Vector2i HexagonalFlip(Vector2i vector) {
		return new Vector2i(-vector.x, vector.y);
	}


	// Sweet name if i may say so myself :)
	public static int ManhexanDistanceFromOrigin(Vector2i gridPosition) {
		return radiusAtGridPosition[gridPosition];
	}

	public static List<Vector2i> GetGridPositionsInHexagonAroundPosition(Vector2i gridPosition, int hexaRadius) {
		return GetGridPositionsInHexagonAroundPosition(gridPosition, hexaRadius, new List<Vector2i>());
	}

	public static List<Vector2i> GetGridPositionsInHexagonAroundPosition(Vector2i gridPosition, int hexaRadius, List<Vector2i> gridPositions) {
		List<Vector2i> hexagon = GetGridPositionsWithinHexagon(hexaRadius);
		foreach (Vector2i position in hexagon) {
			//Vector2i transformedPosition = position + gridPosition;

			////fix strange transform distortion
			//if (gridPosition.x % 2 != 0 && transformedPosition.x % 2 == 0) {
			//	transformedPosition.y++;
			//}

			Vector2i transformed = HexagonalPlus(position, gridPosition);
			if (IsInsideMaximumHexagon(transformed)) {
				gridPositions.Add(transformed);
			}
		}
		return gridPositions;
	}

	public List<Cell> GetCellsInHexagonAroundPosition(Vector2i gridPosition, int hexaRadius) {
		return GetCellsInHexagonAroundPosition(gridPosition, hexaRadius, new List<Cell>());
	}

	public List<Cell> GetCellsInHexagonAroundPosition(Vector2i gridPosition, int hexaRadius, List<Cell> cells) {
		List<Vector2i> hexagon = GetGridPositionsInHexagonAroundPosition(gridPosition, hexaRadius);
		foreach (Vector2i position in hexagon) {
			if (HasCell(position)) {
				cells.Add(GetCell(position));
			}
		}
		return cells;
	}

	private static bool IsInsideHexagon(Vector2i gridPosition, int hexaradius) {
		int xStep = Mathf.Abs(gridPosition.x);
		if (xStep > hexaradius || Mathf.Abs(gridPosition.y) > hexaradius) {
			return false;
		}
		if (gridPosition.y > 0 && gridPosition.y > hexaradius - Mathf.CeilToInt(xStep * 0.5f - 0.01f)) {
			return false;
		} else if (gridPosition.y < -hexaradius + Mathf.FloorToInt(xStep * 0.5f + 0.01f)) {
			return false;
		}
		return true;
	}

	public bool HasKilledTimeStamp(Vector2i gridPosition) {
		return positionKilledTimeStamp.ContainsKey(gridPosition);
	}

	public void AddKilledTimeStamp(Vector2i gridPosition, ulong worldTicks) {
		positionKilledTimeStamp.Add(gridPosition, worldTicks);
	}

	public float? KilledTimeStamp(Vector2i gridPosition) {
		return positionKilledTimeStamp[gridPosition];
	}

	public void RemoveTimeStamp(Vector2i gridPosition) {
		positionKilledTimeStamp.Remove(gridPosition);
	}


	public float cellRadius = 0.5f;

	public int cellCount {
		get {
			return grid.Count;
		}
	}

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

	public Vector2i GetCellGridPositionUpBranch(Vector2i gridPosition) {
		Cell here = GetCell(gridPosition);
		if (here != null) {
			int backCardinalDirection = AngleUtil.CardinalIndexRawToSafe(here.bindCardinalIndex + 3);
			return GetGridNeighbourGridPosition(gridPosition, backCardinalDirection);
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

	public Cell GetGridNeighbourCell(Vector2i gridPosition, CardinalDirectionEnum cardinalEnum) {
		return GetGridNeighbourCell(gridPosition, AngleUtil.CardinalEnumToCardinalIndex(cardinalEnum));
	}

	public Cell GetGridNeighbourCell(Vector2i gridPosition, int cardinalIndex) {
		return GetCell(GetGridNeighbourGridPosition(gridPosition, cardinalIndex));
	}

	public bool HasGridNeighbourCell(Vector2i gridPosition, int cardinalIndex) {
		return GetGridNeighbourCell(gridPosition, cardinalIndex) != null;
	}

	public List<Cell> GetGridNeighbourCell(Vector2i gridPosition) {
		List<Cell> retreivedCells = new List<Cell>();
		for (int i = 0; i < 6; i++) {
			if (HasGridNeighbourCell(gridPosition, i)) {
				retreivedCells.Add(GetGridNeighbourCell(gridPosition, i));
			}
		}
		return retreivedCells;
	}

	public static Vector2i GetGridNeighbourGridPosition(Vector2i gridPosition, CardinalDirectionEnum cardinalenum) {
		return GetGridNeighbourGridPosition(gridPosition, AngleUtil.CardinalEnumToCardinalIndex(cardinalenum));
	}

	public static Vector2i GetGridNeighbourGridPosition(Vector2i gridPosition, int cardinalIndex) {
		Vector2i neighbour = null;
		int even = (gridPosition.x % 2 == 0) ? 1 : 0;
		int odd = (gridPosition.x % 2 == 0) ? 0 : 1;
		if (cardinalIndex == 0) {
			neighbour = new Vector2i(gridPosition.x + 1, gridPosition.y + odd); // north east
		}
		if (cardinalIndex == 1) {
			neighbour = new Vector2i(gridPosition.x, gridPosition.y + 1); // north
		}
		if (cardinalIndex == 2) {
			neighbour = new Vector2i(gridPosition.x - 1, gridPosition.y + odd); // north west
		}
		if (cardinalIndex == 3) {
			neighbour = new Vector2i(gridPosition.x - 1, gridPosition.y - even); // south west
		}
		if (cardinalIndex == 4) {
			neighbour = new Vector2i(gridPosition.x, gridPosition.y - 1); // south
		}
		if (cardinalIndex == 5) {
			neighbour = new Vector2i(gridPosition.x + 1, gridPosition.y - even); // south east
		}
		return neighbour;
	}

	//returned position is in creature space
	public static Vector2 ToModelSpacePosition(Vector2i gridPosition) {
		float defaultCellRadius = 0.5f;

		float xStride = Mathf.Sqrt(Mathf.Pow(defaultCellRadius * 2, 2) - Mathf.Pow(defaultCellRadius, 2));
		float yStride = defaultCellRadius * 2;

		float displace = (gridPosition.x % 2 == 0) ? 0f : defaultCellRadius;
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

	public Cell GetRightmostCellInModelSpace() {
		List<Cell> cells = GetCells();
		Cell record = cells[0];
		foreach (Cell cell in cells) {
			if (cell.modelSpacePosition.x > record.modelSpacePosition.x) {
				record = cell;
			}
		}
		return record;
	}

	public Vector2i ToGridPositionTODO(Vector2 position) {
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

	public List<Vector2i> IsConnectedTo(Vector2i cellAtPosition) {
		hasBeenAsked.Clear();
		MarkCellAtNeighbours(cellAtPosition);
		return hasBeenAsked;
	}

	public bool IsConnected(Vector2i cellAtPositionA, Vector2i cellAtPositionB) {
		hasBeenAsked.Clear();
		return IsCellAtNeighbours(cellAtPositionA, cellAtPositionB);
	}

	private List<Vector2i> hasBeenAsked = new List<Vector2i>();

	private bool IsCellAtNeighbours(Vector2i searchFrom, Vector2i searchGoal) {
		if (searchFrom == searchGoal) {
			return true;
		}
		if (hasBeenAsked.Find(p => p == searchFrom) == null) {
			hasBeenAsked.Add(searchFrom);
		}		
		for (int direction = 0; direction < 6; direction++) {
			Vector2i neighbourPosition = GetGridNeighbourGridPosition(searchFrom, direction);
			if (HasCell(neighbourPosition)) {
				if (hasBeenAsked.Find(p => p == searchFrom) != null) {
					if (IsCellAtNeighbours(neighbourPosition, searchGoal)) {
						return true;
					}
				}
			}
		}
		return false;
	}

	private void MarkCellAtNeighbours(Vector2i searchFrom) {
		if (hasBeenAsked.Find(p => p == searchFrom) == null) {
			hasBeenAsked.Add(searchFrom);
		}
		for (int direction = 0; direction < 6; direction++) {
			Vector2i neighbourPosition = GetGridNeighbourGridPosition(searchFrom, direction);
			if (HasCell(neighbourPosition) && hasBeenAsked.Find(p => p == neighbourPosition) == null) {
				MarkCellAtNeighbours(neighbourPosition);
			}
		}
	}

	public static void Init() {


		// Generate radius tables
		// takes just ~0.02 s ==> no need for this save load shit :/ , note to self: check time cost to generate first before creating some fancy solution!!!!!!!!!!!!!!!! 
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius0);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius1);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius2);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius3);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius4);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius5);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius6);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius7);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius8);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius9);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius10);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius11);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius12);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius13);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius14);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius15);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius16);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius17);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius18);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius19);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius20);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius21);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius22);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius23);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius24);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius25);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius26);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius27);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius28);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius29);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius30);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius31);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius32);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius33);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius34);
		gridPositionsWithinRadiusList.Add(gridPositionsWithinRadius35);

		for (int radius = 0; radius <= 35; radius++) {
			for (int y = -radius; y <= radius; y++) {
				for (int x = -radius; x <= radius; x++) {
					if (IsInsideHexagon(new Vector2i(x, y), radius)) {
						gridPositionsWithinRadiusList[radius].Add(new Vector2i(x, y));
					}
				}
			}
		}

		for (int radius = 0; radius <= 35; radius++) {
			for (int cellIndex = 0; cellIndex < gridPositionsWithinRadiusList[radius].Count; cellIndex++) {
				if (!radiusAtGridPosition.ContainsKey(gridPositionsWithinRadiusList[radius][cellIndex])) {
					radiusAtGridPosition.Add(gridPositionsWithinRadiusList[radius][cellIndex], radius);
				}
			}
		}

		// Generate rotation tables, NEAT! :)
		rotationMaps.Add(rotationMap0);
		rotationMaps.Add(rotationMap60);
		rotationMaps.Add(rotationMap120);
		rotationMaps.Add(rotationMap180);
		rotationMaps.Add(rotationMap240);
		rotationMaps.Add(rotationMap300);

		const int rotationMapRadius = 6; // origo + this many locations
		for (int rotationMapIndex = 0; rotationMapIndex < 6; rotationMapIndex++) { // the rotation map we are writing to
			for (int segmentAngleIndex = 0; segmentAngleIndex < 6; segmentAngleIndex++) { // the segments we are reading from
				Vector2i trunkPosition = GetGridNeighbourGridPosition(new Vector2i(), segmentAngleIndex);
				for (int trunkIndex = 0; trunkIndex < rotationMapRadius; trunkIndex++) { // index of location as we are walking from center and out
					Vector2i branchPosition = trunkPosition;
					for (int branchIndex = 0; branchIndex < rotationMapRadius; branchIndex++) { // index of location as we are walking away from trunk diagonaaly out
						rotationMaps[rotationMapIndex].Add(branchPosition, CoordinateAt((segmentAngleIndex + rotationMapIndex) % 6, trunkIndex, branchIndex));
						branchPosition = GetGridNeighbourGridPosition(branchPosition, (segmentAngleIndex + 1) % 6); // if we are at 5 we should go 0
					}
					trunkPosition = GetGridNeighbourGridPosition(trunkPosition, segmentAngleIndex);
				}
			}
		}
	}

	private static Vector2i CoordinateAt(int cardinalDirection, int trunkDistance, int branchDistance) {
		// First anlong trunk
		Vector2i trunkPosition = GetGridNeighbourGridPosition(new Vector2i(), cardinalDirection);
		for (int trunkIndex = 0; trunkIndex < trunkDistance; trunkIndex++) { // index of location as we are walking from center and out
			trunkPosition = GetGridNeighbourGridPosition(trunkPosition, cardinalDirection); // step on along trunk
		}
		// Then along branch
		Vector2i branchPosition = trunkPosition;
		for (int branchIndex = 0; branchIndex < branchDistance; branchIndex++) { // index of location as we are walking away from trunk diagonaaly out
			branchPosition = GetGridNeighbourGridPosition(branchPosition, (cardinalDirection + 1) % 6); //step on along branch
		}
		return branchPosition;
	}
}