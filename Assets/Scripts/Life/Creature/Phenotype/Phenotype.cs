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

	//-----------------------------------

	public int model = 0;
	//public Cell cellPrefab;

	public JawCell jawCellPrefab;
	public LeafCell leafCellPrefab;
	public MuscleCell muscleCellPrefab;
	public VeinCell veinCellPrefab;

	public bool isDirty = true;
	public float timeOffset;

	public GameObject cells;
	//public Wings wings;
	public Edges edges;

	private Vector3 velocity = new Vector3();
	private List<Cell> cellList = new List<Cell>();
	private Genotype genotype;
	private Vector3 spawnPositionRoot;
	private float headingAngleRoot;
	private Creature creature;
	private CellMap cellMap = new CellMap();

	public int cellCount
	{
		get
		{
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
		if (isDirty) {
			Setup(creature, rootCell.transform.position, rootCell.heading);
			TryGrowFully();
			isDirty = false;
		}
	}

	public void GenerateCells(Creature creature, Vector3 position) {
		if (isDirty) {
			Setup(creature, position, 90f);

			TryGrowFully();
			isDirty = false;
		}
	}

	//Create cellMap so that 
	//SpawnPosition is the position where the center of the root cell wil appear in word space
	private void Setup(Creature creature, Vector3 spawnPosition, float spawnAngle) {
		timeOffset = Random.Range(0f, 7f); //TODO: Remove

		Clear();

		this.creature = creature;
		genotype = creature.genotype;
		spawnPositionRoot = spawnPosition;
		headingAngleRoot = spawnAngle;
	}

	public void TryGrowFully() {
		TryGrow(genotype.geneCellCount);
	}

	public void TryGrow(int cellCount) {
		int growCellCount = 0;
		if (cellCount < 1 || this.cellCount >= genotype.geneCellCount) {
			return;
		}
		if (cellList.Count == 0) {
			SpawnCell(genotype.GetGeneAt(0), new Vector2i(), 0, AngleUtil.ToCardinalDirectionIndex(CardinalDirectionEnum.north), FlipSideEnum.BlackWhite, creature, spawnPositionRoot, true);

			EvoFixedUpdate(creature, 0f);
			rootCell.heading = headingAngleRoot;
			rootCell.angleDiffFromBindpose = headingAngleRoot - 90f;
			rootCell.triangleTransform.localRotation = Quaternion.Euler(0f, 0f, rootCell.heading); // Just updating graphics
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
						float meFromNeightbourBindPose = AngleUtil.ToAngle(indexToMe);
						float meFromNeighbour = (neighbour.angleDiffFromBindpose + meFromNeightbourBindPose) % 360f;
						float distance = geneCell.radius + neighbour.radius;
						averagePosition += neighbour.transform.position + new Vector3(distance * Mathf.Cos(meFromNeighbour * Mathf.Deg2Rad), distance * Mathf.Sin(meFromNeighbour * Mathf.Deg2Rad), 0f);
						positionCount++;
					}
				}
				Cell newCell = SpawnCell(geneCell.gene, geneCell.mapPosition, geneCell.buildOrderIndex, geneCell.bindCardinalIndex, geneCell.flipSide, creature, averagePosition / positionCount, false);
				ConnectCells(false, false); //We need to know our neighbours in order to update vectors correctly 
				newCell.UpdateNeighbourVectors(); //We need to update vectors to our neighbours, so that we can find our direction 
				newCell.UpdateRotation(); //Rotation is needed in order to place subsequent cells right
				newCell.UpdateFlipSide(); // Just graphics
				growCellCount++;
			}
		}

		ConnectCells(true, true);
		edges.GenerateWings(cellList);
		UpdateSpringsFrequenze();
		ShowCellsSelected(false);
		ShowSelectedCreature(CreatureSelectionPanel.instance.IsSelected(creature));
		ShowShadow(false);
		ShowTriangle(false);
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
		edges.GenerateWings(cellList);
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
		if (PhenotypePanel.instance.cell == cell) {
			PhenotypePanel.instance.cell = null;
			PhenotypePanel.instance.UpdateRepresentation();
		}
	}

	private Cell SpawnCell(Gene gene, Vector2i mapPosition, int buildOrderIndex, int bindHeading, FlipSideEnum flipSide, Creature creature, Vector3 position, bool modelSpace) {
		Cell cell = InstantiateCell(gene.type, mapPosition);
		Vector3 spawnPosition = (modelSpace ? genotype.geneCellMap.ToPosition(mapPosition) : Vector3.zero) + position;
		cell.transform.position = spawnPosition;

		cell.mapPosition = mapPosition;
		cell.buildOrderIndex = buildOrderIndex;
		cell.gene = gene;
		cell.bindCardinalIndex = bindHeading;
		cell.flipSide = flipSide;
		cell.timeOffset = timeOffset;
		cell.creature = creature;

		return cell;
	}

	private Cell InstantiateCell(CellTypeEnum type, Vector2i mapPosition) {
		Cell cell = null;
		if (type == CellTypeEnum.Jaw) {
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
					cell.SetNeighbourCell(AngleUtil.ToCardinalDirection(direction), cellMap.GetGridNeighbourCell(center, direction) /*grid[gridNeighbourPos.x, gridNeighbourPos.y].transform.GetComponent<Cell>()*/);
				} else {
					cell.SetNeighbourCell(AngleUtil.ToCardinalDirection(direction), null);
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

	public void ShowTriangle(bool on) {
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

	//data

	private PhenotypeData phenotypeData = new PhenotypeData();
	public PhenotypeData UpdateData() {
		phenotypeData.timeOffset = timeOffset;
		phenotypeData.cellDataList.Clear();
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			phenotypeData.cellDataList.Add(cell.UpdateData());
		}
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
		edges.GenerateWings(cellList);
		UpdateSpringsFrequenze();
		ShowCellsSelected(false);
		ShowShadow(false);
		ShowTriangle(false);

		isDirty = false; //prevent regeneration on genotype -> Phenotype switch
	}
}