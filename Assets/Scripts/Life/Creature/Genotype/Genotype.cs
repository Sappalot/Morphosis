using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Genotype : MonoBehaviour {
	public Gene[] genes = new Gene[genomeLength]; //One gene can give rise to many geneCells

	public EggCell eggCellPrefab;
	public JawCell jawCellPrefab;
	public LeafCell leafCellPrefab;
	public MuscleCell muscleCellPrefab;
	public VeinCell veinCellPrefab;
	public Transform cellsTransform;

	public static int root = 0;
	public static int genomeLength = 21;
	public CellMap geneCellMap = new CellMap();
	public List<Cell> geneCellList = new List<Cell>();

	public bool geneCellsDiffersFromGenome = true; // Cell List and Cell Map needs to be updates

	public bool isGrabbed { get; private set; }

	private bool isDirty = true;

	public int geneCellCount {
		get {
			return geneCellList.Count;
		}
	}

	[HideInInspector]
	public Cell rootCell {
		get {
			return geneCellList[0];
		}
	}

	public void GenomeMutate(float strength) {
		for (int index = 0; index < genomeLength; index++) {
			genes[index].Mutate(strength);
		}
		SetReferenceGenesFromReferenceGeneIndices();
		geneCellsDiffersFromGenome = true;
	}

	public void GenomeScramble() {
		for (int chance = 0; chance < 3; chance++) {
			for (int index = 0; index < genomeLength; index++) {
				genes[index].Scramble();
			}
			if (genes[0].arrangements[0].isEnabled || genes[0].arrangements[1].isEnabled || genes[0].arrangements[2].isEnabled) {
				break;
			}
		}
		SetReferenceGenesFromReferenceGeneIndices();
		geneCellsDiffersFromGenome = true;
	}

	public void GenomeEmpty() {
		for (int index = 0; index < genomeLength; index++) {
			genes[index] = new Gene(index);
		}
		for (int index = 0; index < genomeLength; index++) {
			genes[index].SetDefault(genes);
		}
		geneCellsDiffersFromGenome = true;
	}

	public void GenomeSet(Gene[] genome) {
		for (int index = 0; index < genomeLength; index++) {
			genes[index] = genome[index].GetClone();
		}
		SetReferenceGenesFromReferenceGeneIndices();
		geneCellsDiffersFromGenome = true;
	}

	public void SetReferenceGenesFromReferenceGeneIndices() {
		for (int index = 0; index < genomeLength; index++) {
			genes[index].SetReferenceGeneFromReferenceGeneIndex(genes);
		}
	}

	public bool IsInside(Rect area) {
		float cellRadius = 0.5f;
		float top = area.y + cellRadius + area.height / 2f;
		float bottom = area.y - cellRadius - area.height / 2f;
		float left = area.x - cellRadius - area.width / 2f;
		float right = area.x + cellRadius + area.width / 2f;
		foreach (Cell cell in geneCellList) {
			if (cell.position.x < right + cell.radius && cell.position.x > left - cell.radius && cell.position.y < top + cell.radius && cell.position.y > bottom - cell.radius) {
				return true;
			}
		}
		return false;
	}

	public bool IsGeneReferencedTo(Gene gene) {
		foreach (Cell geneCell in geneCellList) {
			if (geneCell.gene == gene)
				return true;
		}
		return false;
	}

	public void GenerateGenomeEdgeFailure() {
		GenomeEmpty();

		//Simple Jellyfish (FPS Reference creature, Don't ever change!!)
		//New Jellyfish using Arrangements
		genes[0].type = CellTypeEnum.Jaw;
		genes[0].arrangements[0].isEnabled = true;
		genes[0].arrangements[0].type = ArrangementTypeEnum.Side;
		genes[0].arrangements[0].referenceCount = 1;
		genes[0].arrangements[0].referenceGene = genes[1];
		genes[0].arrangements[0].arrowIndex = 4;

		genes[0].arrangements[1].isEnabled = true;
		genes[0].arrangements[1].type = ArrangementTypeEnum.Side;
		genes[0].arrangements[1].referenceCount = 1;
		genes[0].arrangements[1].referenceGene = genes[2];
		genes[0].arrangements[1].arrowIndex = 6;

		genes[0].arrangements[2].isEnabled = true;
		genes[0].arrangements[2].type = ArrangementTypeEnum.Side;
		genes[0].arrangements[2].referenceCount = 1;
		genes[0].arrangements[2].referenceGene = genes[3];
		genes[0].arrangements[2].arrowIndex = -4;

		genes[1].type = CellTypeEnum.Vein;
		genes[2].type = CellTypeEnum.Leaf;
		genes[3].type = CellTypeEnum.Muscle;
	}

	public void GenerateGenomeJellyfish() {
		GenomeEmpty();

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

	public bool UpdateGeneCellsFromGenome(Creature creature, Vector2 position, float heading) { // heading 90 ==> root is pointing north
		if (geneCellsDiffersFromGenome) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update Creature UpdateGeneCellsFromGenome");
			const int maxSize = 6;
			Clear();

			List<Cell> spawningFromCells = new List<Cell>();
			Cell root = SpawnGeneCell(creature, GetGeneAt(0), new Vector2i(), 0, AngleUtil.CardinalEnumToCardinalIndex(CardinalEnum.north), FlipSideEnum.BlackWhite);
			root.heading = 90f;
			spawningFromCells.Add(root);

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
									Cell newCell = SpawnGeneCell(creature, referenceGene, referenceCellMapPosition, buildOrderIndex, referenceBindHeading, geneReference.flipSide);
									nextSpawningFromCells.Add(newCell);
									//geneCellList.Add(spawningFromCell); //Why was this line typed, Removed 2017-08-23??
								} else {
									Debug.Assert(residentCell.buildOrderIndex <= buildOrderIndex, "Trying to spawn a cell at a location where a cell of higher build order are allready present.");
									if (residentCell.buildOrderIndex == buildOrderIndex) {
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

			TurnTo(heading); //is at 90 allready
			MoveTo(position);
			geneCellsDiffersFromGenome = false;
			creature.MakeDirty();
			return true;
		}
		return false;
	}

	public void UpdateFlipSides() {
		for (int index = 0; index < geneCellList.Count; index++) {
			geneCellList[index].UpdateFlipSide();
		}
	}

	// 1 Spawn cell from prefab
	// 2 Setup its properties according to parameters
	// 3 Add cell to list and CellMap
	private Cell SpawnGeneCell(Creature creature, Gene gene, Vector2i mapPosition, int buildOrderIndex, int bindHeading, FlipSideEnum flipSide) {
		Cell cell = null;

		if (gene.type == CellTypeEnum.Egg) {
			cell = (Instantiate(eggCellPrefab, geneCellMap.ToModelSpacePosition(mapPosition), Quaternion.identity) as Cell);
		} else if (gene.type == CellTypeEnum.Jaw) {
			cell = (Instantiate(jawCellPrefab, geneCellMap.ToModelSpacePosition(mapPosition), Quaternion.identity) as Cell);
		} else if (gene.type == CellTypeEnum.Leaf) {
			cell = (Instantiate(leafCellPrefab, geneCellMap.ToModelSpacePosition(mapPosition), Quaternion.identity) as Cell);
		} else if (gene.type == CellTypeEnum.Muscle) {
			cell = (Instantiate(muscleCellPrefab, geneCellMap.ToModelSpacePosition(mapPosition), Quaternion.identity) as Cell);
		} else if (gene.type == CellTypeEnum.Vein) {
			cell = (Instantiate(veinCellPrefab, geneCellMap.ToModelSpacePosition(mapPosition), Quaternion.identity) as Cell);
		}

		cell.RemovePhysicsComponents();


		if (cell == null) {
			throw new System.Exception("Could not create Cell out of type defined in gene");
		}
		cell.transform.parent = cellsTransform;
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

	public void MoveToPhenotype(Creature creature) {
		MoveTo(creature.phenotype.rootCell.position);
		TurnTo(creature.phenotype.rootCell.heading);
	}

	//Make root cell point in this direction while the rest of the cells tags along
	//Angle = 0 ==> root cell pointing east
	//Angle = 90 ==> root cell pointing north
	private void TurnTo(float targetAngle) {
		float deltaAngle = targetAngle - rootCell.heading;
		foreach (Cell cell in geneCellList) {
			Vector3 rootToCell = cell.transform.position - (Vector3)rootCell.position;
			Vector3 turnedVector = Quaternion.Euler(0, 0, deltaAngle) * rootToCell;
			cell.transform.position = (Vector2)rootCell.position + (Vector2)turnedVector;
			float heading = AngleUtil.CardinalIndexToAngle(cell.bindCardinalIndex) + targetAngle - 90f;
			cell.heading = heading;
			cell.SetTringleHeadingAngle(heading);
		}
	}

	public void Move(Vector2 vector) {
		foreach (Cell cell in geneCellList) {
			cell.transform.position += (Vector3)vector;
		}
	}

	public void MoveTo(Vector2 vector) {
		Move(vector - rootCell.position);
	}

	public Gene GetGeneAt(int index) {
		return genes[index];
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

	public void ShowGeneCellsSelectedWithGene(Gene gene, bool on) {
		List<Cell> geneCellsWithGene = GetGeneCellsWithGene(gene);
		foreach (Cell cell in geneCellsWithGene) {
			cell.ShowCellSelected(on);
		}
	}

	public void ShowCreatureSelected(bool on) {
		for (int index = 0; index < geneCellList.Count; index++) {
			geneCellList[index].ShowCreatureSelected(on);
			geneCellList[index].ShowTriangle(on);
			geneCellList[index].ShowShadow(on);
		}

		//On top
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, on ? -6f : 0f);
	}

	public void ShowGeneCellsSelected(bool on) {
		for (int index = 0; index < geneCellList.Count; index++) {
			geneCellList[index].ShowCellSelected(on);
		}
	}

	public void ShowTriangles(bool on) {
		for (int index = 0; index < geneCellList.Count; index++) {
			geneCellList[index].ShowTriangle(on);
		}
	}

	private void Clear() {
		for (int index = 0; index < geneCellList.Count; index++) {
			Destroy(geneCellList[index].gameObject);
		}
		geneCellList.Clear();
		geneCellMap.Clear();

		cellsTransform.localPosition = Vector3.zero;
		cellsTransform.localRotation = Quaternion.identity;

		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}

	public void MoveRootToOrigo() {
		Vector3 rootCellPosition = rootCell.position;
		foreach (Cell cell in geneCellList) {
			cell.transform.position -= rootCellPosition;
		}
	}

	public void Grab() {
		isGrabbed = true;
		foreach (Cell cell in geneCellList) {
			cell.GetComponent<Collider2D>().enabled = false;
		}
		MoveRootToOrigo();
	}

	public void Release(Creature creature) {
		isGrabbed = false;
		foreach (Cell cell in geneCellList) {
			cell.GetComponent<Collider2D>().enabled = true;
		}

		foreach (Cell cell in geneCellList) {
			cell.transform.parent = null;
		}
		creature.transform.position = Vector3.zero;
		creature.transform.rotation = Quaternion.identity;
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		foreach (Cell cell in geneCellList) {
			cell.transform.parent = cellsTransform.transform;
		}

		creature.phenotype.MoveToGenotype(creature);
	}

	//data

	private GenotypeData genotypeData = new GenotypeData();
	public GenotypeData UpdateData() { // Save: We have all genes and their data allready
		for (int index = 0; index < genes.Length; index++) {
			genotypeData.geneData[index] = genes[index].UpdateData();
		}
		genotypeData.rootPosition = rootCell.position;
		genotypeData.rootHeading = rootCell.heading;
		return genotypeData;
	}

	public void ApplyData(GenotypeData genotypeData) {
		for (int index = 0; index < genomeLength; index++) {
			genes[index] = new Gene(index);
			genes[index].ApplyData(genotypeData.geneData[index]);
		}
		SetReferenceGenesFromReferenceGeneIndices();
		geneCellsDiffersFromGenome = true;
	}

	//------

	private void SetCollider(bool on) {
		foreach (Cell cell in geneCellList) {
			cell.GetComponent<Collider2D>().enabled = on;
		}
	}

	private bool m_hasCollider = false;
	public bool hasCollider {
		get {
			return m_hasCollider;
		}
		set {
			m_hasCollider = value;
			isDirty = true;
		}
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update Creature Genotype");

			SetCollider(hasCollider);
			isDirty = false;
		}
	}
}
