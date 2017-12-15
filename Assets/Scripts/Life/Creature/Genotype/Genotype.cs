using UnityEngine;
using System.Collections.Generic;

public class Genotype : MonoBehaviour {
	public Gene[] genome = new Gene[genomeLength]; //One gene can give rise to many geneCells

	public EggCell eggCellPrefab;
	public FungalCell fungalCellPrefab;
	public JawCell jawCellPrefab;
	public LeafCell leafCellPrefab;
	public MuscleCell muscleCellPrefab;
	public RootCell rootCellPrefab;
	public ShellCell shellCellPrefab;
	public VeinCell veinCellPrefab;
	public Transform cellsTransform;

	public static int origin = 0;
	public static int genomeLength = 21;
	private CellMap geneCellMap = new CellMap();
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
	public Cell originCell {
		get {
			return geneCellList[0];
		}
	}

	//Hack
	public Gene[] GetMutatedClone(float strength) {
		Gene[] temporary = new Gene[genomeLength];
		for (int index = 0; index < genomeLength; index++) {
			temporary[index] = genome[index].GetClone();
			temporary[index].Mutate(strength);
		}
		return temporary;
	}

	public void GenomeMutate(float strength) {
		for (int index = 0; index < genomeLength; index++) {
			genome[index].Mutate(strength);
		}
		SetReferenceGenesFromReferenceGeneIndices();
		geneCellsDiffersFromGenome = true;
	}

	public void GenomeScramble() {
		for (int chance = 0; chance < 3; chance++) {
			for (int index = 0; index < genomeLength; index++) {
				genome[index].Scramble();
			}
			if (genome[0].arrangements[0].isEnabled || genome[0].arrangements[1].isEnabled || genome[0].arrangements[2].isEnabled) {
				break;
			}
		}
		SetReferenceGenesFromReferenceGeneIndices();
		geneCellsDiffersFromGenome = true;
	}

	public void GenomeEmpty() {
		for (int index = 0; index < genomeLength; index++) {
			genome[index] = new Gene(index);
		}
		for (int index = 0; index < genomeLength; index++) {
			genome[index].SetDefault(genome);
		}
		geneCellsDiffersFromGenome = true;
	}

	public void GenomeSet(Gene[] genome) {
		for (int index = 0; index < genomeLength; index++) {
			this.genome[index] = genome[index].GetClone();
		}
		SetReferenceGenesFromReferenceGeneIndices();
		geneCellsDiffersFromGenome = true;
	}

	public void SetReferenceGenesFromReferenceGeneIndices() {
		for (int index = 0; index < genomeLength; index++) {
			genome[index].SetReferenceGeneFromReferenceGeneIndex(genome);
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
		genome[0].type = CellTypeEnum.Jaw;
		genome[0].arrangements[0].isEnabled = true;
		genome[0].arrangements[0].type = ArrangementTypeEnum.Side;
		genome[0].arrangements[0].referenceCount = 1;
		genome[0].arrangements[0].referenceGene = genome[1];
		genome[0].arrangements[0].arrowIndex = 4;

		genome[0].arrangements[1].isEnabled = true;
		genome[0].arrangements[1].type = ArrangementTypeEnum.Side;
		genome[0].arrangements[1].referenceCount = 1;
		genome[0].arrangements[1].referenceGene = genome[2];
		genome[0].arrangements[1].arrowIndex = 6;

		genome[0].arrangements[2].isEnabled = true;
		genome[0].arrangements[2].type = ArrangementTypeEnum.Side;
		genome[0].arrangements[2].referenceCount = 1;
		genome[0].arrangements[2].referenceGene = genome[3];
		genome[0].arrangements[2].arrowIndex = -4;

		genome[1].type = CellTypeEnum.Vein;
		genome[2].type = CellTypeEnum.Leaf;
		genome[3].type = CellTypeEnum.Muscle;
	}

	public void GenerateGenomeJellyfish() {
		GenomeEmpty();

		//Simple Jellyfish (FPS Reference creature, Don't ever change!!)
		//New Jellyfish using Arrangements
		genome[0].type = CellTypeEnum.Vein;
		genome[0].arrangements[0].isEnabled = true;
		genome[0].arrangements[0].type = ArrangementTypeEnum.Mirror;
		genome[0].arrangements[0].referenceCount = 2;
		genome[0].arrangements[0].gap = 3;
		genome[0].arrangements[0].referenceGene = genome[1];
		genome[0].arrangements[0].arrowIndex = 0;
		genome[0].arrangements[1].isEnabled = true;
		genome[0].arrangements[1].type = ArrangementTypeEnum.Side;
		genome[0].arrangements[1].referenceCount = 1;
		genome[0].arrangements[1].arrowIndex = 6;
		genome[0].arrangements[1].referenceGene = genome[2];

		genome[1].type = CellTypeEnum.Leaf;
		genome[1].arrangements[0].isEnabled = true;
		genome[1].arrangements[0].type = ArrangementTypeEnum.Side;
		genome[1].arrangements[0].referenceCount = 1;
		genome[1].arrangements[0].referenceGene = genome[2];
		genome[1].arrangements[0].arrowIndex = -2;
		genome[1].arrangements[1].isEnabled = true;
		genome[1].arrangements[1].type = ArrangementTypeEnum.Side;
		genome[1].arrangements[1].referenceCount = 1;
		genome[1].arrangements[1].referenceGene = genome[1];
		genome[1].arrangements[1].arrowIndex = 0;

		genome[2].type = CellTypeEnum.Muscle;
	}

	public bool UpdateGeneCellsFromGenome(Creature creature, Vector2 position, float heading) { // heading 90 ==> origin is pointing north
		if (geneCellsDiffersFromGenome) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update Creature UpdateGeneCellsFromGenome");
			const int maxSize = 6;
			Clear();

			List<Cell> spawningFromCells = new List<Cell>();
			Cell origin = SpawnGeneCell(creature, GetGeneAt(0), new Vector2i(), 0, AngleUtil.CardinalEnumToCardinalIndex(CardinalEnum.north), FlipSideEnum.BlackWhite);
			origin.heading = 90f;
			spawningFromCells.Add(origin);

			List<Cell> nextSpawningFromCells = new List<Cell>();
			for (int buildOrderIndex = 1; spawningFromCells.Count != 0 && buildOrderIndex < maxSize; buildOrderIndex++) {
				for (int index = 0; index < spawningFromCells.Count; index++) {
					Cell spawningFromCell = spawningFromCells[index];
					for (int referenceCardinalIndex = 0; referenceCardinalIndex < 6; referenceCardinalIndex++) {
						GeneReference geneReference = spawningFromCell.gene.GetFlippableReference(referenceCardinalIndex, spawningFromCell.flipSide);
						if (geneReference != null) {
							int referenceBindHeading = (spawningFromCell.bindCardinalIndex + referenceCardinalIndex + 5) % 6; //!!
							Gene referenceGene = geneReference.gene;
							Vector2i referenceCellMapPosition = CellMap.GetGridNeighbourGridPosition(spawningFromCell.mapPosition, referenceBindHeading);

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
			creature.MakeDirtyGraphics();
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
			cell = (Instantiate(eggCellPrefab, CellMap.ToModelSpacePosition(mapPosition), Quaternion.identity) as Cell);
		} else if (gene.type == CellTypeEnum.Fungal) {
			cell = (Instantiate(fungalCellPrefab, CellMap.ToModelSpacePosition(mapPosition), Quaternion.identity) as Cell);
		} else if (gene.type == CellTypeEnum.Jaw) {
			cell = (Instantiate(jawCellPrefab, CellMap.ToModelSpacePosition(mapPosition), Quaternion.identity) as Cell);
		} else if (gene.type == CellTypeEnum.Leaf) {
			cell = (Instantiate(leafCellPrefab, CellMap.ToModelSpacePosition(mapPosition), Quaternion.identity) as Cell);
		} else if (gene.type == CellTypeEnum.Muscle) {
			cell = (Instantiate(muscleCellPrefab, CellMap.ToModelSpacePosition(mapPosition), Quaternion.identity) as Cell);
		} else if (gene.type == CellTypeEnum.Root) {
			cell = (Instantiate(rootCellPrefab, CellMap.ToModelSpacePosition(mapPosition), Quaternion.identity) as Cell);
		} else if (gene.type == CellTypeEnum.Shell) {
			cell = (Instantiate(shellCellPrefab, CellMap.ToModelSpacePosition(mapPosition), Quaternion.identity) as Cell);
		} else if (gene.type == CellTypeEnum.Vein) {
			cell = (Instantiate(veinCellPrefab, CellMap.ToModelSpacePosition(mapPosition), Quaternion.identity) as Cell);
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
		MoveTo(creature.phenotype.originCell.position);
		TurnTo(creature.phenotype.originCell.heading);
	}

	//Make origin cell point in this direction while the rest of the cells tags along
	//Angle = 0 ==> origin cell pointing east
	//Angle = 90 ==> origin cell pointing north
	private void TurnTo(float targetAngle) {
		float deltaAngle = targetAngle - originCell.heading;
		foreach (Cell cell in geneCellList) {
			Vector3 originToCell = cell.transform.position - (Vector3)originCell.position;
			Vector3 turnedVector = Quaternion.Euler(0, 0, deltaAngle) * originToCell;
			cell.transform.position = (Vector2)originCell.position + (Vector2)turnedVector;
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
		Move(vector - originCell.position);
	}

	public Gene GetGeneAt(int index) {
		return genome[index];
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
			geneCellList[index].ShowTriangle(true);
			geneCellList[index].ShowShadow(on);

			geneCellList[index].SetOrderInLayer(on ? 1 : 0);
		}
		isElevated = on;
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

	public void MoveOriginToOrigo() {
		Vector3 originCellPosition = originCell.position;
		foreach (Cell cell in geneCellList) {
			cell.transform.position -= originCellPosition;
		}
	}

	public void Grab() {
		isGrabbed = true;
		foreach (Cell cell in geneCellList) {
			cell.GetComponent<Collider2D>().enabled = false;
		}
		MoveOriginToOrigo();
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

	private void SetElevated(bool on) {
		foreach (Cell cell in geneCellList) {
			cellsTransform.localPosition = new Vector3(0f, 0f, on ? -1f : 0f);
		}
	}

	private bool m_isElevated = false;
	public bool isElevated {
		get {
			return m_isElevated;
		}
		set {
			m_isElevated = value;
			isDirty = true;
		}
	}

	// Load / Save

	private GenotypeData genotypeData = new GenotypeData();
	public GenotypeData UpdateData() { // Save: We have all genes and their data allready
		for (int index = 0; index < genome.Length; index++) {
			genotypeData.geneData[index] = genome[index].UpdateData();
		}
		genotypeData.originPosition = originCell.position;
		genotypeData.originHeading = originCell.heading;
		return genotypeData;
	}

	public void ApplyData(GenotypeData genotypeData) {
		for (int index = 0; index < genomeLength; index++) {
			genome[index] = new Gene(index);
			genome[index].ApplyData(genotypeData.geneData[index]);
		}
		SetReferenceGenesFromReferenceGeneIndices();
		geneCellsDiffersFromGenome = true;
	}

	// ^ Load / Save ^

	// Update

	public void UpdateGraphics() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update Creature Genotype");

			SetCollider(hasCollider);
			SetElevated(isElevated);
			isDirty = false;
		}
	}

	//private void EvoUpdateCells() {
	//	//TODO: only if creature inside frustum && should be shown
	//	for (int index = 0; index < geneCellList.Count; index++) {
	//		geneCellList[index].UpdateGraphics();
	//	}
	//}
}