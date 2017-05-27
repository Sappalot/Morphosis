using UnityEngine;
using System.Collections.Generic;

// The physical creature defined by all its cells

public class Phenotype : MonoBehaviour {
    public int model = 0;
    //public Cell cellPrefab;

    public VeinCell veinCellPrefab;
    public MuscleCell muscleCellPrefab;
    public LeafCell leafCellPrefab;

    public float timeOffset;

    public GameObject cells; //folder
    //public Wings wings;
    public Edges edges;

    private Vector3 velocity = new Vector3();
    private List<Cell> cellList = new List<Cell>();
    private CellMap cellMap = new CellMap();

    public void Generate(Genotype genotype, Creature creature) {
        timeOffset = Random.Range(0f, 7f);

        Clear();

        List<Cell> spawningFromCells = new List<Cell>();
        spawningFromCells.Add(SpawnCell(genotype.GetGeneAt(0), new Vector2i(), 0, CardinalDirectionHelper.ToIndex(CardinalDirection.north), creature)); //root

        List<Cell> nextSpawningFromCells = new List<Cell>();
        for (int buildOrderIndex = 1;  spawningFromCells.Count != 0 && buildOrderIndex < 4; buildOrderIndex++) {
            foreach (Cell cell in spawningFromCells) {
                for (int referenceDirection = 0; referenceDirection < 6; referenceDirection++) {
                    if (cell.gene.getReference(referenceDirection) != null) {
                        int referenceHeading = (cell.heading + referenceDirection + 5) % 6; //!!
                        Gene referenceGene = genotype.GetGeneAt((int)cell.gene.getReference(referenceDirection));
                        Vector2i referenceCellMapPosition = cellMap.GetGridNeighbourGridPosition(cell.mapPosition, referenceHeading);

                        if (cellMap.IsLegalPosition(referenceCellMapPosition)) {
                            Cell residentCell = cellMap.GetCell(referenceCellMapPosition);
                            if (residentCell == null) {
                                //only time we spawn a cell if there is a vacant spot
                                Cell newCell = SpawnCell(referenceGene, referenceCellMapPosition, buildOrderIndex, referenceHeading, creature);
                                nextSpawningFromCells.Add(newCell);
                                cellList.Add(cell);
                            }
                            else {
                                if (residentCell.buildOrderIndex > buildOrderIndex) {
                                    throw new System.Exception("Trying to spawn a cell at a location where a cell of higher build order are allready present.");
                                }
                                else if (residentCell.buildOrderIndex == buildOrderIndex) {
                                    //trying to spawn a cell where ther is one allready with the same buildOrderIndex, in fight over this place bothe cwlls will loose, so the resident will be removed
                                    GameObject.Destroy(residentCell.gameObject);
                                    cellList.Remove(residentCell);
                                    cellMap.removeCellAtGridPosition(residentCell.mapPosition);
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
    private Cell SpawnCell(Gene gene, Vector2i mapPosition, int buildOrderIndex, int direction, Creature creature) {
        Cell cell = null;
        if (gene.type == CellType.Leaf) {
            cell = (Instantiate(leafCellPrefab, transform.position + cellMap.toPosition(mapPosition), Quaternion.identity) as Cell);
        }
        else if (gene.type == CellType.Muscle) {
            cell = (Instantiate(muscleCellPrefab, transform.position + cellMap.toPosition(mapPosition), Quaternion.identity) as Cell);
        }
        else if (gene.type == CellType.Vein) {
            cell = (Instantiate(veinCellPrefab, transform.position + cellMap.toPosition(mapPosition), Quaternion.identity) as Cell);
        }

        if (cell == null) {
            throw new System.Exception("Could not create Cell out of type defined in gene");
        }
        cell.transform.parent = cells.transform;
        cell.mapPosition = mapPosition;
        cell.buildOrderIndex = buildOrderIndex;
        cell.gene = gene;
        cell.heading = direction;
        cell.timeOffset = this.timeOffset;
        cell.creature = creature;

        cellMap.SetCell(mapPosition, cell);
        cellList.Add(cell);

        return cell;
    }

    void ConnectCells() {
        foreach (Cell cell in cellList) {
            Vector2i center = cell.mapPosition;
            for (int direction = 0; direction < 6; direction++) {
                Vector2i gridNeighbourPos = cellMap.GetGridNeighbourGridPosition(center, direction); // GetGridNeighbour(center, CardinalDirectionHelper.ToCardinalDirection(direction));
                if (gridNeighbourPos != null) {
                    cell.SetNeighbourCell(CardinalDirectionHelper.ToCardinalDirection(direction), cellMap.GetGridNeighbourCell(center, direction) /*grid[gridNeighbourPos.x, gridNeighbourPos.y].transform.GetComponent<Cell>()*/);
                }
                else {
                    cell.SetNeighbourCell(CardinalDirectionHelper.ToCardinalDirection(direction), null);
                }
            }
            cell.UpdateSpringConnections();
            cell.UpdateGroups();
        }
    }

    private void Clear() {
        foreach (Cell cell in cellList) {
            GameObject.Destroy(cell);
        }
        cellList.Clear();
        edges.Clear();
        cellMap.Clear();
    }

    //private bool grownUp = false;

    /*public void Grow(float time) {
        if (!grownUp) {
            if (model == 0)
                cellType = model0;
            else if (model == 1)
                cellType = model1;
            else if (model == 2)
                cellType = model2;
            else if (model == 3)
                cellType = model3;
            else if (model == 4)
                cellType = model4;
            else if (model == 5)
                cellType = model5;
            else if (model == 6)
                cellType = model6;

            //cells
            //§SpawnCells();
            //ConnectCells();

            //wings
            //edges.GenerateWings(cellList);

            //springs
            //UpdateSpringsFrequenze();

            grownUp = true;
        }
    }*/

    int update = 0;
    public void UpdatePhysics(Creature creature) {
        update++;
        if (true || update % 10 == 0) { 
            // Creature
            UpdateVelocity();

            // Cells
            TurnNeighboursInPlace();

            // Wings
            edges.UpdateWingForces(velocity, creature);

            UpdateCellPhysics();
        }
    }

    public void UpdateGraphics() {
        UpdateCellGraphics();
    }


    private void UpdateCellGraphics() {
        foreach (Cell cell in cellList) {
            cell.UpdateGraphics();
        }
    }

    private void UpdateCellPhysics() {
        foreach (Cell cell in cellList) {
            cell.UpdatePhysics();
        }
    }

    public void UpdateVelocity() {
        Vector3 averageVelocity = new Vector3();
        foreach (Cell cell in cellList) {
            averageVelocity += cell.velocity;
        }
        velocity = (cellList.Count > 0f) ? velocity = averageVelocity / cellList.Count : new Vector3();
    }

    public int GetCellCount() {
        return cellList.Count;
    }

    private void UpdateSpringsFrequenze() {
        foreach (Cell cell in cellList) {
            cell.UpdateSpringFrequenzy();
        }
    }

    /*private void UpdateRadius(float time) {
        foreach (Cell cell in cellList) {
            cell.UpdateRadius(time);
        }
    }

   private void UpdateSpringLengths() {
        foreach (Cell cell in cellList) {
            cell.UpdateSpringLengths();
        }
    }
   */
    private void TurnNeighboursInPlace() {
        foreach (Cell cell in cellList) {
            cell.TurnNeighboursInPlace();
        }
    } 

    /*public void UpdateGraphics() {
        foreach (Cell cell in cellList) {
            cell.UpdateGraphics();
        }
    }*/

    /*void SpawnCells() {
        float cellRadius = 0.5f;
        float xStride = Mathf.Sqrt(Mathf.Pow(cellRadius * 2, 2) - Mathf.Pow(cellRadius, 2));
        float yStride = cellRadius * 2;
        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                if (cellType[x, y] != 0) {
                    float displace = (x % 2 == 0) ? 0f : cellRadius;
                    Vector3 position = transform.position + new Vector3(xStride * x, yStride * y + displace, 0f);

                    Cell cell = null;
                    if (cellType[x, y] == 1) {
                        cell = (GameObject.Instantiate(leafCellPrefab, position, Quaternion.identity) as Cell);
                    }
                    else if (cellType[x, y] == 2) {
                        cell = (GameObject.Instantiate(muscleCellPrefab, position, Quaternion.identity) as Cell);
                    }

                    cell.transform.parent = cells.transform;

                    grid[x, y] = cell;
                    cellList.Add(cell);
                }
            }
        }
    }*/



   /* Vector2i GetGridNeighbour(Vector2i pos, CardinalDirection direction) {
        Vector2i neighbour = null;
        int even = (pos.x % 2 == 0) ? 1 : 0;
        int odd = (pos.x % 2 == 0) ? 0 : 1;
        if (direction == CardinalDirection.northEast) {
            neighbour = new Vector2i(pos.x + 1, pos.y + odd);
        }
        if (direction == CardinalDirection.north) {
            neighbour = new Vector2i(pos.x, pos.y + 1);
        }
        if (direction == CardinalDirection.northWest) {
            neighbour = new Vector2i(pos.x - 1, pos.y + odd);
        }
        if (direction == CardinalDirection.southWest) {
            neighbour = new Vector2i(pos.x - 1, pos.y - even);
        }
        if (direction == CardinalDirection.south) {
            neighbour = new Vector2i(pos.x, pos.y - 1);
        }
        if (direction == CardinalDirection.southEast) {
            neighbour = new Vector2i(pos.x + 1, pos.y - even);
        }

        if (neighbour != null && neighbour.x >= 0 && neighbour.x < sizeX && neighbour.y >= 0 && neighbour.y < sizeY && cellType[neighbour.x, neighbour.y] != 0) {
            return neighbour;
        }
        return null;
    }*/



    //--------------------------------------------------------------
   /* const int sizeX = 6;
    const int sizeY = 10;

    Cell[,] grid = new Cell[sizeX, sizeY];
    int[,] model0 = new int[sizeX, sizeY] { { 0,  1,  1,  1,  1,  1,  1,  1,  1,  1 },
                                              { 1,  1,  2,  2,  2,  2,  2,  2,  1,  1 },
                                            { 0,  0,  0,  0,  0,  1,  0,  0,  0,  0 },
                                              { 0,  0,  0,  0,  1,  1,  0,  0,  0,  0 },
                                            { 0,  1,  1,  1,  1,  1,  1,  1,  1,  1 },
                                              { 1,  1,  2,  2,  2,  2,  2,  2,  1,  1 } };

    int[,] model1 = new int[sizeX, sizeY] { { 0,  1,  1,  1,  1,  1,  1,  1,  1,  1 },
                                              { 1,  1,  1,  2,  2,  2,  2,  1,  1,  1 },
                                            { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                              { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                            { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                              { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 } };

    int[,] model2 = new int[sizeX, sizeY] { { 0,  1,  1,  1,  1,  1,  1,  1,  1,  1 },
                                              { 1,  1,  2,  2,  2,  2,  2,  2,  1,  1 },
                                            { 0,  0,  0,  0,  0,  1,  0,  0,  0,  0 },
                                              { 0,  0,  0,  0,  1,  1,  0,  0,  0,  0 },
                                            { 0,  0,  0,  0,  1,  1,  1,  0,  0,  0 },
                                              { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 } };

    int[,] model3 = new int[sizeX, sizeY] { { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                              { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                            { 0,  0,  0,  0,  0,  4,  4,  4,  4,  4 },
                                              { 1,  1,  1,  1,  1,  1,  1,  1,  1,  1 },
                                            { 0,  0,  0,  0,  0,  3,  3,  3,  3,  3 },
                                              { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 } };

    int[,] model4 = new int[sizeX, sizeY] { { 0,  1,  1,  1,  1,  1,  1,  1,  1,  1 },
                                              { 2,  2,  2,  2,  2,  2,  2,  2,  2,  2 },
                                            { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                              { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                            { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                              { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 } };


    int[,] model5 = new int[sizeX, sizeY] { { 0,  0,  1,  1,  1,  1,  1,  1,  1,  0 },
                                              { 0,  1,  2,  2,  2,  2,  2,  2,  1,  0 },
                                            { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                              { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                            { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                              { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 } };

    int[,] model6 = new int[sizeX, sizeY] { { 0,  0,  0,  1,  1,  1,  1,  1,  0,  0 },
                                              { 0,  0,  1,  2,  2,  2,  2,  1,  0,  0 },
                                            { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                              { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                            { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                                              { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 } };

    int[,] cellType = new int[sizeX, sizeY]; */

}