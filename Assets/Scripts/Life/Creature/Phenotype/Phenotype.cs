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

    public GameObject cells; //folder
    //public Wings wings;
    public Edges edges;

    private Vector3 velocity = new Vector3();
    private List<Cell> cellList = new List<Cell>();
    private CellMap cellMap = new CellMap();

    private int update = 0;

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
            cellList[index].EvoFixedUpdate(fixedTime);
        }
  
    }

    public void ShuffleCellUpdateOrder() {
        ListUtils.Shuffle(cellList);
    }

    public void Generate(Genotype genotype, Creature creature) {
        timeOffset = Random.Range(0f, 7f);

        Clear();

        List<Cell> spawningFromCells = new List<Cell>();
        spawningFromCells.Add(SpawnCell(genotype.GetGeneAt(0), new Vector2i(), 0, AngleUtil.ToCardinalDirectionIndex(CardinalDirectionEnum.north), creature)); //root


        List<Cell> nextSpawningFromCells = new List<Cell>();
        for (int buildOrderIndex = 1; spawningFromCells.Count != 0 && buildOrderIndex < 4; buildOrderIndex++) {
            for (int index = 0; index < spawningFromCells.Count; index++) {
                Cell cell = spawningFromCells[index];
                for (int referenceDirection = 0; referenceDirection < 6; referenceDirection++) {
                    if (cell.gene.getReference(referenceDirection) != null) {
                        int referenceHeading = (cell.bindHeading + referenceDirection + 5) % 6; //!!
                        Gene referenceGene = genotype.GetGeneAt((int)cell.gene.getReference(referenceDirection));
                        Vector2i referenceCellMapPosition = cellMap.GetGridNeighbourGridPosition(cell.mapPosition, referenceHeading);

                        if (cellMap.IsLegalPosition(referenceCellMapPosition)) {
                            Cell residentCell = cellMap.GetCell(referenceCellMapPosition);
                            if (residentCell == null) {
                                //only time we spawn a cell if there is a vacant spot
                                Cell newCell = SpawnCell(referenceGene, referenceCellMapPosition, buildOrderIndex, referenceHeading, creature);
                                nextSpawningFromCells.Add(newCell);
                                cellList.Add(cell);
                            } else {
                                if (residentCell.buildOrderIndex > buildOrderIndex) {
                                    throw new System.Exception("Trying to spawn a cell at a location where a cell of higher build order are allready present.");
                                } else if (residentCell.buildOrderIndex == buildOrderIndex) {
                                    //trying to spawn a cell where ther is one allready with the same buildOrderIndex, in fight over this place bothe cwlls will loose, so the resident will be removed
                                    GameObject.Destroy(residentCell.gameObject);
                                    cellList.Remove(residentCell);
                                    cellMap.RemoveCellAtGridPosition(residentCell.mapPosition);
                                    nextSpawningFromCells.Remove(residentCell);
                                    cellMap.MarkAsIllegal(residentCell.mapPosition);
                                } else {
                                    // trying to spawn a cell where there is one with lowerBuildOrder index, no action needed
                                }
                            }
                        }
                    }
                }
            }
            spawningFromCells.Clear();
            spawningFromCells.AddRange(nextSpawningFromCells);
            nextSpawningFromCells.Clear();
            if (buildOrderIndex == 99) {
                throw new System.Exception("Creature generation going on for too long");
            }
        }

        ConnectCells();
        edges.GenerateWings(cellList);
        UpdateSpringsFrequenze();
    }

    // 1 Spawn cell from prefab
    // 2 Setup its properties according to parameters
    // 3 Add cell to list and CellMap
    private Cell SpawnCell(Gene gene, Vector2i mapPosition, int buildOrderIndex, int bindHeading, Creature creature) {
        Cell cell = null;

        if (gene.type == CellTypeEnum.Jaw) {
            cell = (Instantiate(jawCellPrefab, transform.position + cellMap.ToPosition(mapPosition), Quaternion.identity) as Cell);
        }
        else if (gene.type == CellTypeEnum.Leaf) {
            cell = (Instantiate(leafCellPrefab, transform.position + cellMap.ToPosition(mapPosition), Quaternion.identity) as Cell);
        }
        else if (gene.type == CellTypeEnum.Muscle) {
            cell = (Instantiate(muscleCellPrefab, transform.position + cellMap.ToPosition(mapPosition), Quaternion.identity) as Cell);
        }
        else if (gene.type == CellTypeEnum.Vein) {
            cell = (Instantiate(veinCellPrefab, transform.position + cellMap.ToPosition(mapPosition), Quaternion.identity) as Cell);
        }

        if (cell == null) {
            throw new System.Exception("Could not create Cell out of type defined in gene");
        }
        cell.transform.parent = cells.transform;
        cell.mapPosition = mapPosition;
        cell.buildOrderIndex = buildOrderIndex;
        cell.gene = gene;
        cell.bindHeading = bindHeading;
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
                }
                else {
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
        edges.Clear();
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