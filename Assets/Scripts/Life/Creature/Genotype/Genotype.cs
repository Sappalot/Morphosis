using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Genotype : MonoBehaviour {
    public JawCell jawCellPrefab;
    public LeafCell leafCellPrefab;
    public MuscleCell muscleCellPrefab;
    public VeinCell veinCellPrefab;
    public Transform cells;


    public static int root = 0;
    public static int genomeLength = 21;
    private Gene[] genes = new Gene[genomeLength];
    private CellMap cellMap = new CellMap();
    private List<Cell> cellList = new List<Cell>();

    public void GenerateJellyfish() {
        CreateEmptyGenome();
        CreateJellyfish();
    }

    public void GenerateString() {
        CreateEmptyGenome();
        CreateString();
    }

    private void CreateEmptyGenome() {
        for (int index = 0; index < genomeLength; index++) {
            genes[index] = new Gene(index);
        }
        for (int index = 0; index < genomeLength; index++) {
            genes[index].SetDefault(genes);
        }
    }

    public Gene GetGeneAt(int index) {
        return genes[index];
    }

    private void CreateJellyfish() {
        //Clear();

        ////Simple Jellyfish (FPS Reference creature, Don't ever change!!)
        //genes[0].type = CellTypeEnum.Vein;
        //genes[0].setReferenceDeprecated(3, 10);
        //genes[0].setReferenceDeprecated(5, 20);
        //genes[0].setReferenceDeprecated(4, 1);

        //genes[1].type = CellTypeEnum.Muscle;

        //genes[10].type = CellTypeEnum.Leaf;
        //genes[10].setReferenceDeprecated(1, 10);
        //genes[10].setReferenceDeprecated(2, 1);

        //genes[20].type = CellTypeEnum.Leaf;
        //genes[20].setReferenceDeprecated(1, 20);
        //genes[20].setReferenceDeprecated(0, 1);

        //New Jellyfish using Arrangements
        genes[0].type = CellTypeEnum.Vein;
        genes[0].arrangements[0].isEnabled = true;
        genes[0].arrangements[0].type = ArrangementTypeEnum.Mirror;
        genes[0].arrangements[0].referenceCount = 2;
        genes[0].arrangements[0].gap = 3;
        genes[0].arrangements[0].referenceGene = genes[1];
        genes[0].arrangements[0].arrowIndex = 0;
        genes[0].arrangements[1].isEnabled = true;
        genes[0].arrangements[1].type = ArrangementTypeEnum.Side;
        genes[0].arrangements[1].referenceCount = 1;
        genes[0].arrangements[1].arrowIndex = 6;
        genes[0].arrangements[1].referenceGene = genes[2];

        genes[1].type = CellTypeEnum.Leaf;
        genes[1].arrangements[0].isEnabled = true;
        genes[1].arrangements[0].type = ArrangementTypeEnum.Side;
        genes[1].arrangements[0].referenceCount = 1;
        genes[1].arrangements[0].referenceGene = genes[2];
        genes[1].arrangements[0].arrowIndex = -2;
        genes[1].arrangements[1].isEnabled = true;
        genes[1].arrangements[1].type = ArrangementTypeEnum.Side;
        genes[1].arrangements[1].referenceCount = 1;
        genes[1].arrangements[1].referenceGene = genes[1];
        genes[1].arrangements[1].arrowIndex = 0;

        genes[2].type = CellTypeEnum.Muscle;
    }

    private void CreateString() {
        //Clear();

        //string
        genes[0].type = CellTypeEnum.Vein;
        //genes[0].setReferenceDeprecated(1, 1);
        genes[0].arrangements[0].isEnabled = true;
        genes[0].arrangements[0].referenceGene = genes[1];
        genes[0].arrangements[0].referenceCount = 4;
        genes[0].arrangements[0].arrowIndex = 0;

        genes[1].type = CellTypeEnum.Muscle;



        //genes[0].type = CellTypeEnum.Vein;
        //genes[0].setReferenceDeprecated(1, 1);

        //genes[1].type = CellTypeEnum.Leaf;
        //genes[1].setReferenceDeprecated(1, 2);

        //genes[2].type = CellTypeEnum.Leaf;
        //genes[2].setReferenceDeprecated(1, 3);

        //genes[3].type = CellTypeEnum.Leaf;
        //genes[3].setReferenceDeprecated(1, 4);

        //genes[4].type = CellTypeEnum.Leaf;
        //genes[4].setReferenceDeprecated(1, 5);

        //genes[5].type = CellTypeEnum.Leaf;
        //genes[5].setReferenceDeprecated(1, 6);

        //genes[6].type = CellTypeEnum.Leaf;
        //genes[6].setReferenceDeprecated(1, 7);

        //genes[7].type = CellTypeEnum.Leaf;

        //genes[10].type = CellTypeEnum.Mouth;
    }

    // No references, type = vein
    //public void Clear() {
    //    for (int index = 0; index < genes.Length; index++) {
    //        genes[index].ClearDeprecated();
    //    }
    //}


    public void Generate(Creature creature) {
        Clear();
        

        List <Cell> spawningFromCells = new List<Cell>();
        spawningFromCells.Add(SpawnCell(GetGeneAt(0), new Vector2i(), 0, AngleUtil.ToCardinalDirectionIndex(CardinalDirectionEnum.north), FlipSideEnum.BlackWhite, creature)); //root

        List<Cell> nextSpawningFromCells = new List<Cell>();
        for (int buildOrderIndex = 1; spawningFromCells.Count != 0 && buildOrderIndex < 4; buildOrderIndex++) {
            for (int index = 0; index < spawningFromCells.Count; index++) {
                Cell spawningFromCell = spawningFromCells[index];
                for (int referenceCardinalIndex = 0; referenceCardinalIndex < 6; referenceCardinalIndex++) {
                    GeneReference geneReference = spawningFromCell.gene.GetFlippableReference(referenceCardinalIndex, spawningFromCell.flipSide);
                    if (geneReference != null) {
                        int referenceBindHeading = (spawningFromCell.bindHeading + referenceCardinalIndex + 5) % 6; //!!
                        Gene referenceGene = geneReference.gene;
                        Vector2i referenceCellMapPosition = cellMap.GetGridNeighbourGridPosition(spawningFromCell.mapPosition, referenceBindHeading);

                        if (cellMap.IsLegalPosition(referenceCellMapPosition)) {
                            Cell residentCell = cellMap.GetCell(referenceCellMapPosition);
                            if (residentCell == null) {
                                //only time we spawn a cell if there is a vacant spot
                                Cell newCell = SpawnCell(referenceGene, referenceCellMapPosition, buildOrderIndex, referenceBindHeading, geneReference.flipSide, creature);
                                nextSpawningFromCells.Add(newCell);
                                cellList.Add(spawningFromCell);
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

        //transform.rotation = Quaternion.Euler(0f, 0f, creature.phenotype.rootCell.heading - 90f);
        cells.localPosition = creature.phenotype.rootCell.transform.position;
        cells.Rotate(0f, 0f, creature.phenotype.rootCell.heading - 90f);
        //cells.rotat



        //ConnectCells();
        //edges.GenerateWings(cellList);
        //UpdateSpringsFrequenze();
    }

    // 1 Spawn cell from prefab
    // 2 Setup its properties according to parameters
    // 3 Add cell to list and CellMap
    private Cell SpawnCell(Gene gene, Vector2i mapPosition, int buildOrderIndex, int bindHeading, FlipSideEnum flipSide, Creature creature) {
        Cell cell = null;

        if (gene.type == CellTypeEnum.Jaw) {
            cell = (Instantiate(jawCellPrefab, cellMap.ToPosition(mapPosition), Quaternion.identity) as Cell);
        } else if (gene.type == CellTypeEnum.Leaf) {
            cell = (Instantiate(leafCellPrefab, cellMap.ToPosition(mapPosition), Quaternion.identity) as Cell);
        } else if (gene.type == CellTypeEnum.Muscle) {
            cell = (Instantiate(muscleCellPrefab, cellMap.ToPosition(mapPosition), Quaternion.identity) as Cell);
        } else if (gene.type == CellTypeEnum.Vein) {
            cell = (Instantiate(veinCellPrefab, cellMap.ToPosition(mapPosition), Quaternion.identity) as Cell);
        }

        if (cell == null) {
            throw new System.Exception("Could not create Cell out of type defined in gene");
        }
        cell.transform.parent = cells;
        cell.mapPosition = mapPosition;
        cell.buildOrderIndex = buildOrderIndex;
        cell.gene = gene;
        cell.bindHeading = bindHeading;
        cell.flipSide = flipSide;
        cell.creature = creature;

        cellMap.SetCell(mapPosition, cell);
        cellList.Add(cell);



        return cell;
    }

    private void Clear() {
        for (int index = 0; index < cellList.Count; index++) {
            Destroy(cellList[index].gameObject);
        }
        cellList.Clear();
        cellMap.Clear();

        cells.localPosition = Vector3.zero;
        cells.localRotation = Quaternion.identity;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
