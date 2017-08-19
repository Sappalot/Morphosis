using UnityEngine;
using System.Collections.Generic;

// The physical creature defined by all its cells

public class Phenotype : MonoBehaviour {
    public int model = 0;
    //public Cell cellPrefab;

    public JawCell jawCellPrefab;
    public LeafCell leafCellPrefab;
    public MuscleCell muscleCellPrefab;
    public VeinCell veinCellPrefab;

    public float timeOffset;

    public GameObject cells;
    //public Wings wings;
    public Edges edges;

    [HideInInspector]
    public Cell rootCell;

    private Vector3 velocity = new Vector3();
    private List<Cell> cellList = new List<Cell>();
    private Genotype genotype;
    private Vector3 spawnPositionRoot;
    private Creature creature;
    private CellMap cellMap = new CellMap();

    private int update = 0;

    public int cellCount {
        get {
            return cellList.Count;
        }
    }

    public void EvoUpdate() {
        EvoUpdateCells();
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
            cellList[index].EvoFixedUpdate(fixedTime, true);
        }
  
    }

    public void ShuffleCellUpdateOrder() {
        ListUtils.Shuffle(cellList);
    }

    //Create cellMap so that 
    //SpawnPosition is the position where the center of the root cell wil appear in word space
    public void Setup(Creature creature, Vector3 spawnPosition) {
        timeOffset = Random.Range(0f, 7f); //TODO: Remove

        Clear();        

        this.creature = creature;
        genotype = creature.genotype; 
        spawnPositionRoot = spawnPosition;
    }

    public void TryGrow(int cellCount) {
        int growCellCount = 0;
        if (cellCount < 1 || this.cellCount >= genotype.geneCellCount) {
            return;
        }
        if (cellList.Count == 0) {
            rootCell = SpawnCell(genotype.GetGeneAt(0), new Vector2i(), 0, AngleUtil.ToCardinalDirectionIndex(CardinalDirectionEnum.north), FlipSideEnum.BlackWhite, creature, spawnPositionRoot, Quaternion.identity);
            growCellCount++;
        }
        genotype.geneCellList.Sort((emp1, emp2) => emp1.buildOrderIndex.CompareTo(emp2.buildOrderIndex));


        foreach (Cell geneCell in genotype.geneCellList) {
            if (growCellCount >= cellCount) {
                break;
            }
            if (!IsCellBuiltForGene(geneCell) && IsCellBuiltAtNeighbourPosition(geneCell.mapPosition)) {
                Cell newCell = SpawnCell(geneCell.gene, geneCell.mapPosition, geneCell.buildOrderIndex, geneCell.bindHeading, geneCell.flipSide, creature, rootCell.transform.position, rootCell.transform.localRotation);
                growCellCount++;
            }
        }

        ConnectCells();
        edges.GenerateWings(cellList);
        UpdateSpringsFrequenze();
    }

    public void TryShrink(int cellCount) {
        int shrinkCellCount = 0;
        if (this.cellCount == 1) {
            return;
        }
        cellList.Sort((emp1, emp2) => emp1.buildOrderIndex.CompareTo(emp2.buildOrderIndex));

        DeleteCell(cellList[cellList.Count - 1]);
        shrinkCellCount++;

        ConnectCells();
        edges.GenerateWings(cellList);
        UpdateSpringsFrequenze();
    }

    private bool IsCellBuiltForGene(Cell gene) {
        return cellMap.HasCell(gene.mapPosition);
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

        ConnectCells();
        edges.GenerateWings(cellList);
    }

    // 1 Spawn cell from prefab
    // 2 Setup its properties according to parameters
    // 3 Add cell to list and CellMap
    private Cell SpawnCell(Gene gene, Vector2i mapPosition, int buildOrderIndex, int bindHeading, FlipSideEnum flipSide, Creature creature, Vector3 position, Quaternion rotation) {
        Cell cell = null;

        if (gene.type == CellTypeEnum.Jaw) {
            cell = (Instantiate(jawCellPrefab, genotype.geneCellMap.ToPosition(mapPosition) + position, rotation) as Cell);
        } else if (gene.type == CellTypeEnum.Leaf) {
            cell = (Instantiate(leafCellPrefab, genotype.geneCellMap.ToPosition(mapPosition) + position, rotation) as Cell);
        } else if (gene.type == CellTypeEnum.Muscle) {
            cell = (Instantiate(muscleCellPrefab, genotype.geneCellMap.ToPosition(mapPosition) + position, rotation) as Cell);
        } else if (gene.type == CellTypeEnum.Vein) {
            cell = (Instantiate(veinCellPrefab, genotype.geneCellMap.ToPosition(mapPosition) + position, rotation) as Cell);
        }

        if (cell == null) {
            throw new System.Exception("Could not create Cell out of type defined in gene");
        }
        cell.transform.parent = cells.transform;
        cell.mapPosition = mapPosition;
        cell.buildOrderIndex = buildOrderIndex;
        cell.gene = gene;
        cell.bindHeading = bindHeading;
        cell.flipSide = flipSide;
        cell.timeOffset = this.timeOffset;
        cell.creature = creature;

        cellMap.SetCell(mapPosition, cell);
        cellList.Add(cell);

        return cell;
    }

    private void ConnectCells() {
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
            cell.UpdateSpringConnections();
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

    public void SetHighlite(bool on) {
        for (int index = 0; index < cellList.Count; index++) {
            cellList[index].ShowSelection(on);
        }
    }
}
