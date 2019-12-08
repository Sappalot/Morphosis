using UnityEngine;
using System.Collections.Generic;
using System.IO;
using SerializerFree;
using SerializerFree.Serializers;

// A blueprint of the final creature
// A way to know how cells are positioned relative to each other in the final creature
// Used by both genotype and phenotype

public class CellMap {
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
	private static List<List<Vector2i>> gridPositionsWithinRadiusList = new List<List<Vector2i>>();

	// hexaRadius = 0; just gridPosition Cell
	public static bool IsInsideMaximumHexagon(Vector2i gridPosition) {
		return ManhexanDistanceFromOrigin(gridPosition) <= Creature.maxRadiusHexagon;
	}

	public static List<Vector2i> GetGridPositionsWithinHexagon(int hexaRadius) {
		return gridPositionsWithinRadiusList[hexaRadius];
	}

	public static int ManhexanDistance(Vector2i positionA, Vector2i positionB) {
		return ManhexanDistanceFromOrigin(HexagonalMinus(positionB, positionA));
	}

	public static Vector2i HexagonalMinus(Vector2i vectorA, Vector2i vectorB) {
		Vector2i transformed = vectorA - vectorB;
		if (vectorA.x % 2 == 0 && vectorB.x % 2 == 1) {
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

	// Takes some time at startup save this shit
	public static void Init() {
		//CellMapData cellMapData = LoadCellMapData();
		//if (cellMapData != null) {
		//	//Apply old
		//	ApplyData(cellMapData);
		//} else {

		// Generate
		// takes just ~0.02 s ==> no need for this save load shit :/ , note check time cost to generate first before creating some fancy solution!!!!!!!!!!!!!!!! 
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

		for (int radius = 0; radius <= 15; radius++) {
			for (int y = -radius; y <= radius; y++) {
				for (int x = -radius; x <= radius; x++) {
					if (IsInsideHexagon(new Vector2i(x, y), radius)) {
						gridPositionsWithinRadiusList[radius].Add(new Vector2i(x, y));
					}
				}
			}
		}

		for (int radius = 0; radius <= 15; radius++) {
			for (int cellIndex = 0; cellIndex < gridPositionsWithinRadiusList[radius].Count; cellIndex++) {
				if (!radiusAtGridPosition.ContainsKey(gridPositionsWithinRadiusList[radius][cellIndex])) {
					radiusAtGridPosition.Add(gridPositionsWithinRadiusList[radius][cellIndex], radius);
				}
			}
		}

		//SaveCellMapData();
		//}
	}

	// Save
	public static CellMapData UpdateData() {
		cellMapData.radiusAtGridPositionKey.Clear();
		cellMapData.radiusAtGridPositionValue.Clear();
		foreach (KeyValuePair<Vector2i, int> pair in radiusAtGridPosition) {
			cellMapData.radiusAtGridPositionKey.Add(pair.Key);
			cellMapData.radiusAtGridPositionValue.Add(pair.Value);
		}

		cellMapData.gridPositionsWithinRadius0.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius0) {
			cellMapData.gridPositionsWithinRadius0.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius1.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius1) {
			cellMapData.gridPositionsWithinRadius1.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius2.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius2) {
			cellMapData.gridPositionsWithinRadius2.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius3.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius3) {
			cellMapData.gridPositionsWithinRadius3.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius4.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius4) {
			cellMapData.gridPositionsWithinRadius4.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius5.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius5) {
			cellMapData.gridPositionsWithinRadius5.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius6.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius6) {
			cellMapData.gridPositionsWithinRadius6.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius7.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius7) {
			cellMapData.gridPositionsWithinRadius7.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius8.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius8) {
			cellMapData.gridPositionsWithinRadius8.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius9.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius9) {
			cellMapData.gridPositionsWithinRadius9.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius10.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius10) {
			cellMapData.gridPositionsWithinRadius10.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius11.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius11) {
			cellMapData.gridPositionsWithinRadius11.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius12.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius12) {
			cellMapData.gridPositionsWithinRadius12.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius13.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius13) {
			cellMapData.gridPositionsWithinRadius13.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius14.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius14) {
			cellMapData.gridPositionsWithinRadius14.Add(positin);
		}

		cellMapData.gridPositionsWithinRadius15.Clear();
		foreach (Vector2i positin in gridPositionsWithinRadius15) {
			cellMapData.gridPositionsWithinRadius15.Add(positin);
		}

		return cellMapData;
	}

	// Load
	public static void ApplyData(CellMapData cellMapData) {

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

		radiusAtGridPosition.Clear();
		for (int index = 0; index < cellMapData.radiusAtGridPositionKey.Count; index++) {
			radiusAtGridPosition.Add(cellMapData.radiusAtGridPositionKey[index], cellMapData.radiusAtGridPositionValue[index]);
		}

		gridPositionsWithinRadius0.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius0) {
			gridPositionsWithinRadius0.Add(positin);
		}

		gridPositionsWithinRadius1.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius1) {
			gridPositionsWithinRadius1.Add(positin);
		}

		gridPositionsWithinRadius2.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius2) {
			gridPositionsWithinRadius2.Add(positin);
		}

		gridPositionsWithinRadius3.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius3) {
			gridPositionsWithinRadius3.Add(positin);
		}

		gridPositionsWithinRadius4.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius4) {
			gridPositionsWithinRadius4.Add(positin);
		}

		gridPositionsWithinRadius5.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius5) {
			gridPositionsWithinRadius5.Add(positin);
		}

		gridPositionsWithinRadius6.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius6) {
			gridPositionsWithinRadius6.Add(positin);
		}

		gridPositionsWithinRadius7.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius7) {
			gridPositionsWithinRadius7.Add(positin);
		}

		gridPositionsWithinRadius8.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius8) {
			gridPositionsWithinRadius8.Add(positin);
		}

		gridPositionsWithinRadius9.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius9) {
			gridPositionsWithinRadius9.Add(positin);
		}

		gridPositionsWithinRadius10.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius10) {
			gridPositionsWithinRadius10.Add(positin);
		}

		gridPositionsWithinRadius11.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius11) {
			gridPositionsWithinRadius11.Add(positin);
		}

		gridPositionsWithinRadius12.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius12) {
			gridPositionsWithinRadius12.Add(positin);
		}

		gridPositionsWithinRadius13.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius13) {
			gridPositionsWithinRadius13.Add(positin);
		}

		gridPositionsWithinRadius14.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius14) {
			gridPositionsWithinRadius14.Add(positin);
		}

		gridPositionsWithinRadius15.Clear();
		foreach (Vector2i positin in cellMapData.gridPositionsWithinRadius15) {
			gridPositionsWithinRadius15.Add(positin);
		}
	}

	private static CellMapData cellMapData = new CellMapData();

	private static CellMapData LoadCellMapData() {
		string filename = "CellMapHexagon.txt";

		string path = Morphosis.savePath;
		if (File.Exists(path + filename)) {
			string serializedString = File.ReadAllText(path + filename);
			return Serializer.Deserialize<CellMapData>(serializedString, new UnityJsonSerializer());
		}

		return null;
	}

	public static void SaveCellMapData() {
		cellMapData = UpdateData();
		string filename = "cellMapHexagon.txt";
		string path = Morphosis.savePath;

		string cellMapToSave = Serializer.Serialize(cellMapData, new UnityJsonSerializer());
		if (!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}

		File.WriteAllText(path + filename, cellMapToSave);
	}
}