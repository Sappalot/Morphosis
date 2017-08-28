using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Genotype : MonoBehaviour {
    public Gene[] genes = new Gene[genomeLength]; //One gene can give rise to many geneCells

    //-------------------------
    public JawCell jawCellPrefab;
    public LeafCell leafCellPrefab;
    public MuscleCell muscleCellPrefab;
    public VeinCell veinCellPrefab;
    public Transform cells;

    public static int root = 0;
    public static int genomeLength = 21;
    public CellMap geneCellMap = new CellMap();
    public List<Cell> geneCellList = new List<Cell>();

    public bool isDirty = true;

    public bool IsGeneReferencedTo(Gene gene) {
        foreach (Cell geneCell in geneCellList) {
            if (geneCell.gene == gene)
                return true;
        }
        return false;
    }

    public int geneCellCount {
        get {
            return geneCellList.Count;
        }
    }

    public void GenerateGenomeJellyfish() {
        GenerateGenomeEmpty();

        //Simple Jellyfish (FPS Reference creature, Don't ever change!!)
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

    public void GenerateGenomeEmpty() {
        for (int index = 0; index < genomeLength; index++) {
            genes[index] = new Gene(index);
        }
        for (int index = 0; index < genomeLength; index++) {
            genes[index].SetDefault(genes);
        }
    }

    public void GenerateGeneCells(Creature creature) {
        if (isDirty) {
            const int maxSize = 6;
            Clear();

            List<Cell> spawningFromCells = new List<Cell>();
            spawningFromCells.Add(SpawnGeneCell(GetGeneAt(0), new Vector2i(), 0, AngleUtil.ToCardinalDirectionIndex(CardinalDirectionEnum.north), FlipSideEnum.BlackWhite, creature)); //root

            List<Cell> nextSpawningFromCells = new List<Cell>();
            for (int buildOrderIndex = 1; spawningFromCells.Count != 0 && buildOrderIndex < maxSize; buildOrderIndex++) {
                for (int index = 0; index < spawningFromCells.Count; index++) {
                    Cell spawningFromCell = spawningFromCells[index];
                    for (int referenceCardinalIndex = 0; referenceCardinalIndex < 6; referenceCardinalIndex++) {
                        GeneReference geneReference = spawningFromCell.gene.GetFlippableReference(referenceCardinalIndex, spawningFromCell.flipSide);
                        if (geneReference != null) {
                            int referenceBindHeading = (spawningFromCell.bindCardinalIndex + referenceCardinalIndex + 5) % 6; //!!
                            Gene referenceGene = geneReference.gene;
                            Vector2i referenceCellMapPosition = geneCellMap.GetGridNeighbourGridPosition(spawningFromCell.mapPosition, referenceBindHeading);

                            if (geneCellMap.IsLegalPosition(referenceCellMapPosition)) {
                                Cell residentCell = geneCellMap.GetCell(referenceCellMapPosition);
                                if (residentCell == null) {
                                    //only time we spawn a cell if there is a vacant spot
                                    Cell newCell = SpawnGeneCell(referenceGene, referenceCellMapPosition, buildOrderIndex, referenceBindHeading, geneReference.flipSide, creature);
                                    nextSpawningFromCells.Add(newCell);
                                    //geneCellList.Add(spawningFromCell); //Why was this line typed, Removed 2017-08-23??
                                } else {
                                    if (residentCell.buildOrderIndex > buildOrderIndex) {
                                        throw new System.Exception("Trying to spawn a cell at a location where a cell of higher build order are allready present.");
                                    } else if (residentCell.buildOrderIndex == buildOrderIndex) {
                                        //trying to spawn a cell where ther is one allready with the same buildOrderIndex, in fight over this place bothe cwlls will loose, so the resident will be removed
                                        GameObject.Destroy(residentCell.gameObject);
                                        geneCellList.Remove(residentCell);
                                        geneCellMap.RemoveCellAtGridPosition(residentCell.mapPosition);
                                        nextSpawningFromCells.Remove(residentCell);
                                        geneCellMap.MarkAsIllegal(residentCell.mapPosition);
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
            ShowGeneCellsSelected(false);
            isDirty = false;
        }
    }

    // 1 Spawn cell from prefab
    // 2 Setup its properties according to parameters
    // 3 Add cell to list and CellMap
    private Cell SpawnGeneCell(Gene gene, Vector2i mapPosition, int buildOrderIndex, int bindHeading, FlipSideEnum flipSide, Creature creature) {
        Cell cell = null;

        if (gene.type == CellTypeEnum.Jaw) {
            cell = (Instantiate(jawCellPrefab, geneCellMap.ToPosition(mapPosition), Quaternion.identity) as Cell);
        } else if (gene.type == CellTypeEnum.Leaf) {
            cell = (Instantiate(leafCellPrefab, geneCellMap.ToPosition(mapPosition), Quaternion.identity) as Cell);
        } else if (gene.type == CellTypeEnum.Muscle) {
            cell = (Instantiate(muscleCellPrefab, geneCellMap.ToPosition(mapPosition), Quaternion.identity) as Cell);
        } else if (gene.type == CellTypeEnum.Vein) {
            cell = (Instantiate(veinCellPrefab, geneCellMap.ToPosition(mapPosition), Quaternion.identity) as Cell);
        }

        cell.RemovePhysicsComponents();


        if (cell == null) {
            throw new System.Exception("Could not create Cell out of type defined in gene");
        }
        cell.transform.parent = cells;
        cell.mapPosition = mapPosition;
        cell.buildOrderIndex = buildOrderIndex;
        cell.gene = gene;
        cell.bindCardinalIndex = bindHeading;
        cell.flipSide = flipSide;
        cell.creature = creature;

        geneCellMap.SetCell(mapPosition, cell);
        geneCellList.Add(cell);

        return cell;
    }

    public void UpdateTransformAndHighlite(Creature creature) {
        cells.localPosition = creature.phenotype.rootCell.transform.position;
        cells.localRotation = Quaternion.identity;
        cells.Rotate(0f, 0f, creature.phenotype.rootCell.heading - 90f);
        ShowCreatureSelected(CreatureSelectionPanel.instance.IsSelected(creature));

        for (int index = 0; index < geneCellList.Count; index++) {
            geneCellList[index].EvoFixedUpdate(0);
        }
    }

    public Gene GetGeneAt(int index) {
        return genes[index];
    }

    public void ShowGeneCellsSelectedWithGene(Gene gene, bool on) {
        List<Cell> geneCellsWithGene = GetGeneCellsWithGene(gene);
        foreach (Cell cell in geneCellsWithGene) {
            cell.ShowCellSelected(on);
        }
    }

    public List<Cell> GetGeneCellsWithGene(Gene gene) {
        List<Cell> cells = new List<Cell>();
        foreach (Cell cell in geneCellList) {
            if (cell.gene == gene) {
                cells.Add(cell);
            }
        }
        return cells;
    }

    public void ShowCreatureSelected(bool on) {
        for (int index = 0; index < geneCellList.Count; index++) {
            geneCellList[index].ShowCreatureSelected(on);
            geneCellList[index].ShowShadow(on);
            transform.localPosition = new Vector3(0f, 0f, on ? -8f : 0f);
        }
    }

    public void ShowGeneCellsSelected(bool on) {
        for (int index = 0; index < geneCellList.Count; index++) {
            geneCellList[index].ShowCellSelected(on);
        }
    }

    private void Clear() {
        for (int index = 0; index < geneCellList.Count; index++) {
            Destroy(geneCellList[index].gameObject);
        }
        geneCellList.Clear();
        geneCellMap.Clear();

        cells.localPosition = Vector3.zero;
        cells.localRotation = Quaternion.identity;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    //data

    private GenotypeData genotypeData = new GenotypeData();
    public GenotypeData UpdateData() { // Save: We have all genes and their data allready
        for (int index = 0; index < genes.Length; index++) {
            genotypeData.geneData[index] = genes[index].UpdateData();
        }
        return genotypeData;
    }

    public void ApplyData(GenotypeData genotypeData) {
        for (int index = 0; index < genomeLength; index++) {
            genes[index] = new Gene(index);
            genes[index].ApplyData(genotypeData.geneData[index]);
        }
        for (int index = 0; index < genomeLength; index++) {
            genes[index].SetReferenceGeneFromReferenceGeneIndex(genes);
        }
    }
}
