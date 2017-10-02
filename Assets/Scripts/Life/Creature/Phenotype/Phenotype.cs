using UnityEngine;
using System.Collections.Generic;

// The physical creature defined by all its cells

public class Phenotype : MonoBehaviour {
	[HideInInspector]
	public Cell rootCell {
		get {
			return cellList[0];
		}
	}

	public Transform cellsTransform;

	public int model = 0;

	public EggCell eggCellPrefab;
	public JawCell jawCellPrefab;
	public LeafCell leafCellPrefab;
	public MuscleCell muscleCellPrefab;
	public VeinCell veinCellPrefab;

	public bool differsFromGenotype = true;
	public float timeOffset;

	public GameObject cells;
	public Edges edges;

	private Vector3 velocity = new Vector3();
	private List<Cell> cellList = new List<Cell>();
	private Vector2 spawnPosition;
	private float spawnHeading;
	private CellMap cellMap = new CellMap();

	private bool isGrabbed;
	public bool hasDirtyPosition = false;

	public int cellCount {
		get {
			return cellList.Count;
		}
	}

	public bool IsInside(Rect area) {
		float cellRadius = 0.5f;
		float top = area.y + cellRadius + area.height / 2f;
		float bottom = area.y - cellRadius - area.height / 2f;
		float left = area.x - cellRadius - area.width / 2f;
		float right = area.x + cellRadius + area.width / 2f;
		foreach (Cell cell in cellList) {
			if (cell.position.x < right + cell.radius && cell.position.x > left - cell.radius && cell.position.y < top + cell.radius && cell.position.y > bottom - cell.radius) {
				return true;
			}
		}
		return false;
	}

	public void EvoUpdate() {
		//EvoUpdateCells();
		edges.EvoUpdate();
	}

	public void EvoFixedUpdate(Creature creature, float fixedTime) {
		if (isGrabbed) {
			return;
		}
		
		//if (update % 50 == 0) {
		//    edges.ShuffleEdgeUpdateOrder();
		//    ShuffleCellUpdateOrder();
		//}
		//update++;

		// Creature
		Vector3 averageVelocity = new Vector3();
		for (int index = 0; index < cellList.Count; index++) {
			averageVelocity += cellList[index].velocity;
		}
		velocity = (cellList.Count > 0f) ? velocity = averageVelocity / cellList.Count : new Vector3();

		//// Cells, turn strings of cells straight
		//for (int index = 0; index < cellList.Count; index++) {
		//    cellList[index].TurnNeighboursInPlace();
		//}

		// Edges, let edge-wings apply proper forces to neighbouring cells
		edges.EvoFixedUpdate(velocity, creature);

		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].EvoFixedUpdate(fixedTime);
		}
	}

	public void ShuffleCellUpdateOrder() {
		ListUtils.Shuffle(cellList);
	}

	public void GenerateCells(Creature creature) {
		if (differsFromGenotype) {
			Setup(creature, rootCell.transform.position, rootCell.heading);
			TryGrowFully(creature);
			differsFromGenotype = false;
		}
	}

	public void GenerateCells(Creature creature, Vector2 position, float heading) {
		if (differsFromGenotype) {
			Setup(creature, position, heading);

			TryGrowFully(creature);
			differsFromGenotype = false;
		}
	}

	//SpawnPosition is the position where the center of the root cell will appear in word space
	private void Setup(Creature creature, Vector2 spawnPosition, float spawnHeading) {
		timeOffset = Random.Range(0f, 7f); //TODO: Remove

		Clear();
		this.spawnPosition = spawnPosition;
		this.spawnHeading = spawnHeading;
	}

	public void TryGrowFully(Creature creature) {
		TryGrow(creature, creature.genotype.geneCellCount);
	}

	public void TryGrow(Creature creature, int cellCount) {
		int growCellCount = 0;
		Genotype genotype  = creature.genotype;
		if (cellCount < 1 || this.cellCount >= genotype.geneCellCount) {
			return;
		}
		if (cellList.Count == 0) {
			SpawnCell(creature, genotype.GetGeneAt(0), new Vector2i(), 0, AngleUtil.CardinalEnumToCardinalIndex(CardinalEnum.north), FlipSideEnum.BlackWhite, spawnPosition, true);

			EvoFixedUpdate(creature, 0f);
			rootCell.heading = spawnHeading;
			rootCell.triangleTransform.rotation = Quaternion.Euler(0f, 0f, rootCell.heading); // Just updating graphics
			growCellCount++;
		}
		genotype.geneCellList.Sort((emp1, emp2) => emp1.buildOrderIndex.CompareTo(emp2.buildOrderIndex));

		foreach (Cell geneCell in genotype.geneCellList) {
			if (growCellCount >= cellCount) {
				break;
			}
			if (!IsCellBuiltForGene(geneCell) && IsCellBuiltAtNeighbourPosition(geneCell.mapPosition)) {
				Vector3 averagePosition = Vector3.zero;
				int positionCount = 0;
				for (int neighbourIndex = 0; neighbourIndex < 6; neighbourIndex++) {
					Cell neighbour = cellMap.GetGridNeighbourCell(geneCell.mapPosition, neighbourIndex);
					if (neighbour != null) {
						int indexToMe = CardinaIndexToNeighbour(neighbour, geneCell);
						float meFromNeightbourBindPose = AngleUtil.CardinalIndexToAngle(indexToMe);
						float meFromNeighbour = (neighbour.angleDiffFromBindpose + meFromNeightbourBindPose) % 360f;
						float distance = geneCell.radius + neighbour.radius;
						averagePosition += neighbour.transform.position + new Vector3(distance * Mathf.Cos(meFromNeighbour * Mathf.Deg2Rad), distance * Mathf.Sin(meFromNeighbour * Mathf.Deg2Rad), 0f);
						positionCount++;
					}
				}
				Cell newCell = SpawnCell(creature, geneCell.gene, geneCell.mapPosition, geneCell.buildOrderIndex, geneCell.bindCardinalIndex, geneCell.flipSide, averagePosition / positionCount, false);
				ConnectCells(false, false); //We need to know our neighbours in order to update vectors correctly 
				newCell.UpdateNeighbourVectors(); //We need to update vectors to our neighbours, so that we can find our direction 
				newCell.UpdateRotation(); //Rotation is needed in order to place subsequent cells right
				newCell.UpdateFlipSide(); // Just graphics
				growCellCount++;
			}
		}

		ConnectCells(true, true);
		
		UpdateSpringsFrequenze();
		ShowCellsSelected(false);
		ShowSelectedCreature(CreatureSelectionPanel.instance.IsSelected(creature));
		ShowShadow(false);
		ShowTriangles(false);

		edges.GenerateWings(cellMap);
	}

	private int CardinaIndexToNeighbour(Cell from, Cell to) {
		for (int index = 0; index < 6; index++) {
			Vector2i neighbourPosition = cellMap.GetGridNeighbourGridPosition(from.mapPosition, index);
			if (neighbourPosition.x == to.mapPosition.x && neighbourPosition.y == to.mapPosition.y) {
				return index;
			}
		}
		return -1;
	}

	public void TryShrink(int cellCount) {
		int shrinkCellCount = 0;
		cellList.Sort((emp1, emp2) => emp1.buildOrderIndex.CompareTo(emp2.buildOrderIndex));
		for (int count = 0; count < cellCount; count++) {
			if (this.cellCount == 1) {
				return;
			}
			DeleteCell(cellList[cellList.Count - 1]);
			shrinkCellCount++;
		}
		ConnectCells(true, true);
		edges.GenerateWings(cellMap);
		UpdateSpringsFrequenze();
	}

	private bool IsCellBuiltForGene(Cell gene) {
		return cellMap.HasCell(gene.mapPosition);
	}

	private List<Cell> GetBuiltNeighbourCells(Vector2i gridPosition) {
		List<Cell> neighbours = new List<Cell>();
		for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
			Cell neighbour = cellMap.GetCell(cellMap.GetGridNeighbourGridPosition(gridPosition, cardinalIndex));
			if (neighbour != null) {
				neighbours.Add(neighbour);
			}
		}
		return neighbours;
	}

	private bool IsCellBuiltAtNeighbourPosition(Vector2i gridPosition) {
		for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
			if (cellMap.HasCell(cellMap.GetGridNeighbourGridPosition(gridPosition, cardinalIndex))) {
				return true;
			}
		}
		return false;
	}

	private void DeleteCell(Cell cell) {
		cellMap.RemoveCellAtGridPosition(cell.mapPosition);
		cellList.Remove(cell);
		Destroy(cell.gameObject);
		if (PhenotypePanel.instance.selectedCell == cell) {
			PhenotypePanel.instance.selectedCell = null;
			PhenotypePanel.instance.UpdateRepresentation();
		}
	}

	private Cell SpawnCell(Creature creature, Gene gene, Vector2i mapPosition, int buildOrderIndex, int bindCardinalIndex, FlipSideEnum flipSide, Vector2 position, bool modelSpace) {
		Cell cell = InstantiateCell(gene.type, mapPosition);
		Vector2 spawnPosition = (modelSpace ? creature.genotype.geneCellMap.ToModelSpacePosition(mapPosition) : Vector2.zero) + position;
		cell.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0f);

		cell.mapPosition = mapPosition;
		cell.buildOrderIndex = buildOrderIndex;
		cell.gene = gene;
		cell.bindCardinalIndex = bindCardinalIndex;
		cell.flipSide = flipSide;
		cell.timeOffset = timeOffset;
		cell.creature = creature;

		return cell;
	}

	private Cell InstantiateCell(CellTypeEnum type, Vector2i mapPosition) {
		Cell cell = null;
		if (type == CellTypeEnum.Egg) {
			cell = (Instantiate(eggCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Jaw) {
			cell = (Instantiate(jawCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Leaf) {
			cell = (Instantiate(leafCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Muscle) {
			cell = (Instantiate(muscleCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Vein) {
			cell = (Instantiate(veinCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		}
		if (cell == null) {
			throw new System.Exception("Could not create Cell out of type defined in gene");
		}
		cellMap.SetCell(mapPosition, cell);
		cellList.Add(cell);
		cell.transform.parent = cells.transform;

		return cell;
	}

	private void ConnectCells(bool connectSprings, bool updateGroups) {
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			Vector2i center = cell.mapPosition;
			for (int direction = 0; direction < 6; direction++) {
				Vector2i gridNeighbourPos = cellMap.GetGridNeighbourGridPosition(center, direction); // GetGridNeighbour(center, CardinalDirectionHelper.ToCardinalDirection(direction));
				if (gridNeighbourPos != null) {
					cell.SetNeighbourCell(AngleUtil.CardinalIndexToCardinalEnum(direction), cellMap.GetGridNeighbourCell(center, direction) /*grid[gridNeighbourPos.x, gridNeighbourPos.y].transform.GetComponent<Cell>()*/);
				} else {
					cell.SetNeighbourCell(AngleUtil.CardinalIndexToCardinalEnum(direction), null);
				}
			}
			if (connectSprings)
				cell.UpdateSpringConnections();
			if (updateGroups)
				cell.UpdateGroups();
		}
	}

	private void Clear() {
		for (int index = 0; index < cellList.Count; index++) {
			Destroy(cellList[index].gameObject);
		}
		cellList.Clear();
		cellMap.Clear();
	}

	private void EvoUpdateCells() {
		//Todo: only if creature inside frustum && should be shown
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].EvoUpdate();
		}
	}

	public int GetCellCount() {
		return cellList.Count;
	}

	private void UpdateSpringsFrequenze() {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdateSpringFrequenzy();
		}
	}

	public void ShowSelectedCreature(bool on) {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].ShowCreatureSelected(on);
			cellList[index].ShowTriangle(on); // Debug
		}
	}

	public void ShowCellsSelected(bool on) {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].ShowCellSelected(on);
		}
	}

	public void ShowCellSelected(Cell cell, bool on) {
		for (int index = 0; index < cellList.Count; index++) {
			if (cellList[index] == cell) {
				cellList[index].ShowCellSelected(on);
			}
		}
	}

	public void ShowTriangles(bool on) {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].ShowTriangle(on);
		}
	}

	public void ShowShadow(bool on) {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].ShowShadow(on);
		}
	}

	public void Show(bool on) {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].Show(on);
		}
	}
	
	public void MoveRootToOrigo() {
		Vector3 rootCellPosition = rootCell.position;
		foreach (Cell cell in cellList) {
			cell.transform.position -= rootCellPosition;
		}
	}

	public void Halt() {
		foreach (Cell cell in cellList) {
			cell.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		}
	}

	public void Grab() {
		isGrabbed = true;
		foreach (Cell cell in cellList) {
			cell.GetComponent<Rigidbody2D>().isKinematic = true;
			cell.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			//cell.GetComponent<Collider2D>().enabled = false;
		}
		MoveRootToOrigo();
	}

	public void Release(Creature creature) {
		isGrabbed = false;
		foreach (Cell cell in cellList) {
			cell.GetComponent<Rigidbody2D>().isKinematic = false;
			//cell.GetComponent<Collider2D>().enabled = true;
		}
		foreach (Cell cell in cellList) {
			cell.transform.parent = null;
		}
		creature.transform.position = Vector3.zero;
		creature.transform.rotation = Quaternion.identity;
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		Halt();
		foreach (Cell cell in cellList) {
			cell.transform.parent = cells.transform;
		}
	}

	public void MoveToGenotype(Creature creature) {
		if (hasDirtyPosition) {
			MoveTo(creature.genotype.rootCell.position);
			TurnTo(creature.genotype.rootCell.heading);
			hasDirtyPosition = false;
		}
	}

	public void Move(Vector2 vector) {
		foreach (Cell cell in cellList) {
			cell.transform.position += (Vector3)vector;
		}
	}

	public void MoveTo(Vector2 vector) {
		Move(vector - rootCell.position);
	}

	//Make root cell point in this direction while the rest of the cells tags along
	//Angle = 0 ==> root cell pointing east
	//Angle = 90 ==> root cell pointing north
	private void TurnTo(float targetAngle) {
		float deltaAngle = targetAngle - rootCell.heading;
		foreach (Cell cell in cellList) {
			Vector3 rootToCell = cell.transform.position - (Vector3)rootCell.position;
			Vector3 turnedVector = Quaternion.Euler(0, 0, deltaAngle) * rootToCell;
			cell.transform.position = (Vector2)rootCell.position + (Vector2)turnedVector;
			float heading = AngleUtil.CardinalIndexToAngle(cell.bindCardinalIndex) + targetAngle - 90f;
			cell.heading = heading;
			cell.SetTringleHeadingAngle(heading);
		}
	}

	//data

	private PhenotypeData phenotypeData = new PhenotypeData();
	public PhenotypeData UpdateData() {
		phenotypeData.timeOffset = timeOffset;
		phenotypeData.cellDataList.Clear();
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			phenotypeData.cellDataList.Add(cell.UpdateData());
		}
		phenotypeData.differsFromGenotype = differsFromGenotype;
		//phenotypeData.rootCellPosition = rootCell.position;
		return phenotypeData;
	}

	public void ApplyData(PhenotypeData phenotypeData, Creature creature) {
		timeOffset = phenotypeData.timeOffset;
		
		Setup(creature, phenotypeData.cellDataList[0].position, phenotypeData.cellDataList[0].heading);
		for (int index = 0; index < phenotypeData.cellDataList.Count; index++) {
			CellData cellData = phenotypeData.cellDataList[index];
			Cell cell = InstantiateCell(creature.genotype.genes[cellData.geneIndex].type, cellData.mapPosition);
			cell.ApplyData(cellData, creature);
		}

		ConnectCells(true, true);
		edges.GenerateWings(cellMap);
		UpdateSpringsFrequenze();
		ShowCellsSelected(false);
		ShowShadow(false);
		ShowTriangles(true);
		differsFromGenotype = phenotypeData.differsFromGenotype; //prevent regeneration on genotype -> Phenotype switch

	}

	//TODO: Remove
	//private void Update() {

	//}
	public Cell GetCellAt(Vector2 position) {
		foreach (Cell cell in cellList) {
			if (IsPointInsideCircle(position, cell.position, cell.radius + 0.2f)) {
				return cell;
			}
		}
		return null;
	}

	private bool IsPointInsideCircle(Vector2 point, Vector2 center, float radius) {
		return Mathf.Pow((point.x - center.x), 2) + Mathf.Pow((point.y - center.y), 2) < Mathf.Pow(radius, 2);
	}



	//--------
	public void SetCollider(bool on) {
		foreach (Cell cell in cellList) {
			cell.GetComponent<Collider2D>().enabled = on;
		}
	}

	public bool isDirty = true;
	private void Update() {
		if (isDirty) {

			isDirty = false;
		}
	}
}