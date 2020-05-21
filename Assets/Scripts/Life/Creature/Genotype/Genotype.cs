using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Genotype : MonoBehaviour, IGenotypeDirtyfy {
	public EggCell eggCellPrefab;
	public FungalCell fungalCellPrefab;
	public JawCell jawCellPrefab;
	public LeafCell leafCellPrefab;
	public MuscleCell muscleCellPrefab;
	public RootCell rootCellPrefab;
	public ShellCell shellCellPrefab;
	public VeinCell veinCellPrefab;
	public Transform geneCellsTransform;

	private bool isGeneCellPatternDirty = true; // Cell List and Cell Map needs to be updates, needs to be done as genes cause changes in cell structure
	public void MakeGeneCellPatternDirty() { // LAZY: Used even if only nerves are changed since we need to regenerate phenotype as anything is changed "new forge"
		isGeneCellPatternDirty = true;
	}

	public NerveArrows nerveArrows;

	private bool isInterGeneCellDirty = true;

	public static int genomeLength = 21;
	[HideInInspector]
	public Gene[] genes = new Gene[genomeLength]; //One gene can give rise to many geneCells

	// GeneCellLists...
	[HideInInspector]
	public List<Cell> geneCellListIndexSorted = new List<Cell>();
	//[HideInInspector]
	public List<Cell> geneCellListPrioritySorted {
		get {
			List<Cell> prioritySorted = new List<Cell>(geneCellListIndexSorted);
			prioritySorted.Sort((emp1, emp2) => emp1.buildPriority.CompareTo(emp2.buildPriority));
			return prioritySorted;
		}
	}

	// ... Signal ...
	private void UpdateNerves() {
		// clear
		for (int index = 0; index < geneCellListIndexSorted.Count; index++) {
			geneCellListIndexSorted[index].PreUpdateNervesGenotype();
		}

		// reach out
		for (int index = 0; index < geneCellListIndexSorted.Count; index++) {
			geneCellListIndexSorted[index].UpdateInputNervesGenotype(this);
		}

		// root them
		for (int index = 0; index < geneCellListIndexSorted.Count; index++) {
			geneCellListIndexSorted[index].UpdateConnectionsNervesGenotype(this);
		}

		//
	}

	// ^ Signal ^

	public void Initialize() {
		// This is the only place where the genes are made
		// When we want to change the creature, we hchange its genes 
		for (int index = 0; index < genomeLength; index++) {
			genes[index] = new Gene(index, this);
		}
		int a = 1;
	}

	public void AddCellToGeneCellLists(Cell cell) {
		geneCellListIndexSorted.Add(cell);
	}

	public void ClearGeneCellLists() {
		geneCellListIndexSorted.Clear();
	}
	// ^ geneCellLists ^

	public bool isGrabbed { get; private set; }

	private CellMap geneCellMap = new CellMap(); // contains blueprint of final creature, all cells

	public bool hasGenes {
		get {
			return geneCellListIndexSorted != null && geneCellListIndexSorted.Count > 0;
		}
	}

	// ... Signal ...

	// ^ Signal ^


	public Cell GetCellAtMapPosition(Vector2i mapPosition) {
		return geneCellMap.GetCell(mapPosition);
	}

	public Cell GetCellAtWorldPosition(Vector2 position) {
		if (IsInsideBoundingCircle(position)) {
			foreach (Cell geneCell in geneCellListIndexSorted) {
				if (GeometryUtil.IsPointInsideCircle(position, geneCell.position, geneCell.radius)) {
					return geneCell;
				}
			}
		}
		return null;
	}

	public bool IsInsideBoundingCircle(Vector2 position) {
		return Vector2.SqrMagnitude(originCell.position - position) < Mathf.Pow(Creature.maxRadiusCircle * 2f, 2f);
	}

	public bool IsCompletelyInside(Rect area) {
		float cellRadius = 0.5f;
		float top = area.y + area.height / 2f - cellRadius;
		float bottom = area.y - area.height / 2f + cellRadius;
		float left = area.x - area.width / 2f + cellRadius;
		float right = area.x + area.width / 2f - cellRadius;
		foreach (Cell cell in geneCellListIndexSorted) {
			if (cell.position.x > right || cell.position.x < left || cell.position.y > top || cell.position.y < bottom) {
				return false;
			}
		}
		return true;
	}

	//Full size of creature
	public int geneCellCount {
		get {
			return geneCellListIndexSorted.Count;
		}
	}

	public int CompletenessCellCount(float completeness) {
		return Mathf.Max(1, Mathf.RoundToInt(completeness * geneCellCount));
	}

	public int GetGeneCellOfTypeCount(CellTypeEnum type) {
		int count = 0;
		foreach (Cell c in geneCellListIndexSorted) {
			if (c.GetCellType() == type) {
				count++;
			}
		}
		return count;
	}

	public Cell originCell {
		get {
			return geneCellListIndexSorted[0];
		}
	}

	public bool hasOriginCell {
		get {
			return geneCellListIndexSorted.Count > 0;
		}
	}

	private const int stepsToRootAtMost = 100;

	public Cell GetClosestAxonGeneCellUpBranch(Vector2i position) {
		Vector2i currentPosition = position;

		for (int rep = 0; rep < stepsToRootAtMost; rep++) {
			Cell cellAtPosition = geneCellMap.GetCell(currentPosition);
			if (cellAtPosition.isAxonEnabled) {
				return cellAtPosition;
			}
			// there should be an axone at origin, since it is forced to be there
			if (currentPosition.x == 0 && currentPosition.y == 0) {
				Debug.LogError("We are at root now but no axone was found");
			}
			currentPosition = geneCellMap.GetCellGridPositionUpBranch(currentPosition);
		}
		Debug.LogError("We should have been climbing all the way up the branc to the origin, by now!");
		return null;
	}

	public int? GetDistanceToClosestAxonGeneCellUpBranch(Vector2i position) {
		int distance = 0;
		Vector2i currentPosition = position;
		for (int rep = 0; rep < stepsToRootAtMost; rep++) {
			Cell cellAtPosition = geneCellMap.GetCell(currentPosition);
			if (cellAtPosition.isAxonEnabled) {
				return distance;
			}
			if (currentPosition.x == 0 && currentPosition.y == 0) {
				Debug.LogError("We are at root now but no axone was found");
				return null;
			}
			currentPosition = geneCellMap.GetCellGridPositionUpBranch(currentPosition);
			distance++;
		}
		Debug.LogError("We should have been climbing all the way to the root by now!");
		return null;
	}

	// Hack
	// The child needs to be the dirty one as anything happens to its genes
	public Gene[] GetMutatedClone(IGenotypeDirtyfy genotypeDirtyfy, float strength) {
		Gene[] temporary = new Gene[genomeLength];
		for (int index = 0; index < genomeLength; index++) {
			temporary[index] = genes[index].GetClone(genotypeDirtyfy);
			temporary[index].Mutate(strength);
		}
		return temporary;
	}

	public void SetDefault() {
		Defaultify(false);
	}
	public void Defaultify(bool junkOnly) {
		for (int index = 0; index < genomeLength; index++) {
			if (!junkOnly || !IsGeneReferencedTo(genes[index])) {
				genes[index].Defaultify();
			}
		}
		SetReferenceGenesFromReferenceGeneIndices();

		MakeGeneCellPatternDirty();
	}

	public void SetScrambled() {
		Randomize(false);
	}
	public void Randomize(bool junkOnly) {
		for (int chance = 0; chance < 3; chance++) {
			for (int index = 0; index < genomeLength; index++) {
				if (!junkOnly || !IsGeneReferencedTo(genes[index])) {
					genes[index].Randomize();
				}
			}
			if (genes[0].arrangements[0].isEnabled || genes[0].arrangements[1].isEnabled || genes[0].arrangements[2].isEnabled) {
				break;
			}
		}
		SetReferenceGenesFromReferenceGeneIndices();
		MakeGeneCellPatternDirty();
	}

	public void Mutate(float strength) {
		Mutate(strength, false);
	}
	public void Mutate(float strength, bool junkOnly) {
		for (int index = 0; index < genomeLength; index++) {
			if (!junkOnly || !IsGeneReferencedTo(genes[index])) {
				genes[index].Mutate(strength);
			}
		}
		SetReferenceGenesFromReferenceGeneIndices();
		MakeGeneCellPatternDirty();
	}

	public void SetGenome(Gene[] genome) {
		for (int index = 0; index < genomeLength; index++) {
			this.genes[index] = genome[index].GetClone(this);
		}
		SetReferenceGenesFromReferenceGeneIndices();
		MakeGeneCellPatternDirty();
	}

	public void SetReferenceGenesFromReferenceGeneIndices() {
		for (int index = 0; index < genomeLength; index++) {
			genes[index].SetReferenceGeneFromReferenceGeneIndex(genes);
		}
	}

	public bool IsPartlyInside(Rect area) {
		float cellRadius = 0.5f;
		float top = area.y + cellRadius + area.height / 2f;
		float bottom = area.y - cellRadius - area.height / 2f;
		float left = area.x - cellRadius - area.width / 2f;
		float right = area.x + cellRadius + area.width / 2f;
		foreach (Cell cell in geneCellListIndexSorted) {
			if (cell.position.x < right && cell.position.x > left && cell.position.y < top && cell.position.y > bottom) {
				return true;
			}
		}
		return false;
	}

	public bool IsGeneReferencedTo(Gene gene) {
		foreach (Cell geneCell in geneCellListIndexSorted) {
			if (geneCell.gene == gene)
				return true;
		}
		return false;
	}

	public void GenerateGenomeEdgeFailure() {
		SetDefault();

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
		SetDefault();

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

	public bool TryUpdateGeneCellPattern(Creature creature, Vector2 position, float heading) { // heading 90 ==> origin is pointing north
		if (isGeneCellPatternDirty) {

			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Creature TryUpdateGeneCellPattern");
			}

			Clear();

			List<Cell> spawningFromCells = new List<Cell>();
			Cell origin = SpawnGeneCell(creature, GetGeneAt(0), new Vector2i(), 0, AngleUtil.CardinalEnumToCardinalIndex(CardinalDirectionEnum.north), FlipSideEnum.BlackWhite);
			origin.heading = 90f; // North
			spawningFromCells.Add(origin);

			const int sanityLimit = 100;

			List<Cell> nextSpawningFromCells = new List<Cell>();
			for (int buildOrderIndex = 1; spawningFromCells.Count != 0 && buildOrderIndex < sanityLimit; buildOrderIndex++) {

				for (int index = 0; index < spawningFromCells.Count; index++) {
					Cell spawningFromCell = spawningFromCells[index];
					spawningFromCell.failBlueprintNeighboursDueToAreaOrCount = false; // assume we will not be stopped
					spawningFromCell.failBlueprintNeighboursDueToConcurentBuild = false; // assume we will not be stopped

					for (int referenceCardinalIndex = 0; referenceCardinalIndex < 6; referenceCardinalIndex++) {
						GeneReference geneReference = spawningFromCell.gene.GetFlippableReference(referenceCardinalIndex, spawningFromCell.flipSide);
						if (geneReference != null) {
							int referenceBindHeading = (spawningFromCell.bindCardinalIndex + referenceCardinalIndex + 5) % 6; //!!
							Gene referenceGene = geneReference.gene;
							Vector2i referenceCellMapPosition = CellMap.GetGridNeighbourGridPosition(spawningFromCell.mapPosition, referenceBindHeading);

							//if (!CellMap.IsInsideHexagon(referenceCellMapPosition, Creature.maxRadiusHexagon)) {
							if (!CellMap.IsInsideMaximumHexagon(referenceCellMapPosition)) {
								spawningFromCell.failBlueprintNeighboursDueToAreaOrCount = true;
								continue;
							}

							if (geneCellMap.IsLegalPosition(referenceCellMapPosition)) {
								Cell residentCell = geneCellMap.GetCell(referenceCellMapPosition);
								if (residentCell == null) {
									//only time we spawn a cell if there is a vacant spot
									Cell newCell = SpawnGeneCell(creature, referenceGene, referenceCellMapPosition, buildOrderIndex, referenceBindHeading, geneReference.flipSide);
									nextSpawningFromCells.Add(newCell);

								} else {
									Debug.Assert(residentCell.buildIndex <= buildOrderIndex, "Trying to spawn a cell at a location where a cell of lower build index is allready present.");
									if (residentCell.buildIndex == buildOrderIndex) {
										//trying to spawn a cell where there is one allready with the same buildOrderIndex, in fight over this place bothe cells will loose, so the resident will be removed

										//mark the cell that spawned the resident cell
										geneCellMap.GetCell(geneCellMap.GetCellGridPositionUpBranch(residentCell.mapPosition)).failBlueprintNeighboursDueToConcurentBuild = true;
										//...and the current cell
										spawningFromCell.failBlueprintNeighboursDueToConcurentBuild = true;

										// then remove the resident cell
										Morphosis.instance.geneCellPool.Recycle(residentCell);
										geneCellListIndexSorted.Remove(residentCell);
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

				if (geneCellMap.cellCount > Creature.maxCellCount) {
					//MarkeAllGeneCellsAsCanGrowNeighbours();

					// We have more cells in the blueprint than max size, so REMOVE the last buildOrderIndex layer ALL TOGETHER and call it a creature
					foreach (Cell tooMuch in nextSpawningFromCells) { // nextSpawningFromCells contains all cells in last layer added
																	  //cell 'back' from cell, we are about to remove, could not build this cell, so its arrow will be coloured red 
						geneCellMap.GetCell(geneCellMap.GetCellGridPositionUpBranch(tooMuch.mapPosition)).failBlueprintNeighboursDueToAreaOrCount = true;

						Morphosis.instance.geneCellPool.Recycle(tooMuch);
						geneCellListIndexSorted.Remove(tooMuch);
						geneCellMap.RemoveCellAtGridPosition(tooMuch.mapPosition);
					}

					break; // Blueprint is done
				}

				spawningFromCells.Clear();
				spawningFromCells.AddRange(nextSpawningFromCells);
				nextSpawningFromCells.Clear();
				if (buildOrderIndex == sanityLimit) {
					throw new System.Exception("Creature generation going on for too long");
				}
			}

			TurnTo(heading); //is at 90 allready
			MoveTo(position);
			
			creature.MakeDirtyGraphics();

			// This is the only place where we add and remove GeneCells to geneCell List so we can safely sort it by priority here
			// Sorted by priority ==> high BuildPrio (low number) to low BuildPrio (high number) 
			// We need to have it sorted in this way and never in any other way
			//geneCellList.Sort((emp1, emp2) => emp1.buildPriority.CompareTo(emp2.buildPriority));
			geneCellListIndexSorted.Sort((emp1, emp2) => emp1.buildIndex.CompareTo(emp2.buildIndex)); // only sorted here

			isInterGeneCellDirty = true;
			creature.phenotype.MakeCellPaternDifferentFromGenotypeDirty();

			isGeneCellPatternDirty = false;
			return true;
		}
		return false;
	}

	public bool TryUpdateInterGeneCells() {
		if (isInterGeneCellDirty) {

			UpdateNerves();

			//foreach (Gene gene in genes) {
			//	gene.PreUpdateInterGeneCell();
			//}

			//foreach (Gene gene in genes) {
			//	gene.UpdateInterGeneCell(this);
			//}

			nerveArrows.GenerateGenotype(this);

			isInterGeneCellDirty = false;

			return true;
		}
		return false;
	}

	private Cell SpawnGeneCell(Creature creature, Gene gene, Vector2i mapPosition, int buildOrderIndex, int bindHeading, FlipSideEnum flipSide) {
		Cell cell = Morphosis.instance.geneCellPool.Borrow(gene.type);

		cell.transform.parent = geneCellsTransform;
		cell.transform.position = CellMap.ToModelSpacePosition(mapPosition);
		cell.RemovePhysicsComponents();
		if (cell == null) {
			throw new System.Exception("Could not create Cell out of type defined in gene");
		}
		
		cell.mapPosition = mapPosition;
		cell.buildIndex = buildOrderIndex;
		cell.SetGene(gene);
		cell.bindCardinalIndex = bindHeading;
		cell.flipSide = flipSide;
		cell.creature = creature;

		geneCellMap.SetCell(mapPosition, cell);
		AddCellToGeneCellLists(cell);
		
		return cell;
	}

	public void MoveToPhenotype(Creature creature) {
		if (creature.phenotype.isAlive) {
			MoveTo(creature.phenotype.originCell.position);
			TurnTo(creature.phenotype.originCell.heading);
		}
	}

	//Make origin cell point in this direction while the rest of the cells tags along
	//Angle = 0 ==> origin cell pointing east
	//Angle = 90 ==> origin cell pointing north
	private void TurnTo(float targetAngle) {
		float deltaAngle = targetAngle - originCell.heading;
		foreach (Cell cell in geneCellListIndexSorted) {
			Vector3 originToCell = cell.transform.position - (Vector3)originCell.position;
			Vector3 turnedVector = Quaternion.Euler(0, 0, deltaAngle) * originToCell;
			cell.transform.position = (Vector2)originCell.position + (Vector2)turnedVector;
			float heading = AngleUtil.CardinalIndexToAngle(cell.bindCardinalIndex) + targetAngle - 90f;
			cell.heading = heading;
			cell.SetTringleHeadingAngle(heading);
		}
	}

	public void Move(Vector2 vector) {
		foreach (Cell cell in geneCellListIndexSorted) {
			cell.transform.position += (Vector3)vector;
		}
	}

	public void MoveTo(Vector2 vector) {
		Move(vector - originCell.position);
	}

	public Gene GetGeneAt(int index) {
		return genes[index];
	}

	public List<Cell> GetGeneCellsWithGene(Gene gene) {
		List<Cell> cells = new List<Cell>();
		foreach (Cell cell in geneCellListIndexSorted) {
			if (cell.gene == gene) {
				cells.Add(cell);
			}
		}
		return cells;
	}

	// if there are twins, only one of them will be returned (which one is arbitrary)
	public List<Nerve> GetAllUniqueNervesGenotype(Gene gene) {
		List<Cell> geneCells = GetGeneCellsWithGene(gene);
		List<Nerve> uniqueNerves = new List<Nerve>();
		foreach (Cell geneCell in geneCells) {
			// all geneCells containing gene

			List<Nerve> allGeneCellNerves = geneCell.GetAllNervesGenotype();
			foreach (Nerve geneCellNerve in allGeneCellNerves) {
				// for all used nerves in the geneCell

				if (uniqueNerves.Find(n => Nerve.AreTwinNerves(n, geneCellNerve, true)) == null) {
					// unique nerves does not contain a twin to geneCellNerve
					uniqueNerves.Add(geneCellNerve);
				}
			}
		}
		return uniqueNerves;
	}


	// used to bundle up sets of neve twins together, who share the "same" properties (same everything, except the geneCell. Not the same geneCells, but the same genes inside them)
	// don't get anything from unrooted signal units
	public List<List<Nerve>> GetNerveTwinBundles(Gene gene, SignalUnitEnum signalUnitEnum, XputEnum xputEnum) {
		Dictionary<string, List<Nerve>> nerveBundleDictionary = new Dictionary<string, List<Nerve>>();

		List<Cell> geneCellsWithGene = GetGeneCellsWithGene(gene);
		foreach (Cell geneCell in geneCellsWithGene) {
			SignalUnit signalUnit = geneCell.GetSignalUnit(signalUnitEnum);
			if (signalUnit == null) {
				Debug.Log("null");
			}

			if (signalUnit.isRooted) {
				if (xputEnum == XputEnum.Output) {
					List<Nerve> outputNerves = signalUnit.GetOutputNervesGenotype();
					foreach (Nerve nerve in outputNerves) {
						List<Nerve> nerveTwinList;
						if (nerveBundleDictionary.TryGetValue(nerve.ToTwinString(), out nerveTwinList)) {
							nerveTwinList.Add(nerve);
						} else {
							nerveTwinList = new List<Nerve>();
							nerveTwinList.Add(nerve);
							nerveBundleDictionary.Add(nerve.ToTwinString(), nerveTwinList);
						}
					}
				} else {
					List<Nerve> inputNerves = signalUnit.GetInputNervesGenotype();
					foreach (Nerve nerve in inputNerves) {
						List<Nerve> nerveTwinList;
						if (nerveBundleDictionary.TryGetValue(nerve.ToTwinString(), out nerveTwinList)) {
							nerveTwinList.Add(nerve);
						} else {
							nerveTwinList = new List<Nerve>();
							nerveTwinList.Add(nerve);
							nerveBundleDictionary.Add(nerve.ToTwinString(), nerveTwinList);
						}
					}
				}
			}
		}
		return nerveBundleDictionary.Values.ToList();
	}

	public bool HasAllOccurancesOfThisGeneSameBuildIndex(Gene gene) {
		int? buildIndex = null;
		foreach (Cell c in geneCellListIndexSorted) {
			if (c.gene == gene) {
				if (buildIndex == null) {
					buildIndex = c.buildIndex;
				} else if (c.buildIndex != buildIndex) {
					return false;
				}
			}
		}
		return true;
	}

	public void ShowGeneCellsSelectedWithGene(Gene gene, bool on) {
		List<Cell> geneCellsWithGene = GetGeneCellsWithGene(gene);
		foreach (Cell cell in geneCellsWithGene) {
			cell.ShowCellSelected(on);
		}
	}

	public void UpdateOutlineTriangleAndBones(Creature creature, bool isSelected) {
		for (int index = 0; index < geneCellListIndexSorted.Count; index++) {
			if (isSelected) {
				geneCellListIndexSorted[index].ShowOutline(true);
				geneCellListIndexSorted[index].SetOutlineColor(ColorScheme.instance.outlineSelected);
				geneCellListIndexSorted[index].ShowOnTop(true);
			} else {
				geneCellListIndexSorted[index].ShowOutline(false);
				geneCellListIndexSorted[index].ShowOnTop(false);
			}
		}

		for (int index = 0; index < geneCellListIndexSorted.Count; index++) {
			geneCellListIndexSorted[index].ShowTriangle(true);
			if (geneCellListIndexSorted[index].isOrigin) {
				geneCellListIndexSorted[index].ShowTriangle(true);
				if (!creature.HasMotherDeadOrAlive()) {
					if (!creature.HasChildrenDeadOrAlive()) {
						geneCellListIndexSorted[index].SetTriangleColor(ColorScheme.instance.noRelativesArrow);
					} else {
						geneCellListIndexSorted[index].SetTriangleColor(ColorScheme.instance.noMotherArrow);
					}
				} else if (creature.IsAttachedToMotherAlive()) {
					geneCellListIndexSorted[index].SetTriangleColor(ColorScheme.instance.motherAttachedArrow);
				} else {
					geneCellListIndexSorted[index].SetTriangleColor(ColorScheme.instance.noMotherAttachedArrow);
				}
			} else {
				if (geneCellListIndexSorted[index].failBlueprintNeighboursDueToAreaOrCount && !geneCellListIndexSorted[index].failBlueprintNeighboursDueToConcurentBuild) {
					// We could not 'grow' one or more neighbours
					geneCellListIndexSorted[index].SetTriangleColor(ColorScheme.instance.failBlueprintNeighboursDueToAreaOrCountArrow);
				} else if (!geneCellListIndexSorted[index].failBlueprintNeighboursDueToAreaOrCount && geneCellListIndexSorted[index].failBlueprintNeighboursDueToConcurentBuild) {
					geneCellListIndexSorted[index].SetTriangleColor(ColorScheme.instance.failBlueprintNeighboursDueToConcurentBuildArrow);
				} else if (geneCellListIndexSorted[index].failBlueprintNeighboursDueToAreaOrCount && geneCellListIndexSorted[index].failBlueprintNeighboursDueToConcurentBuild) {
					geneCellListIndexSorted[index].SetTriangleColor(ColorScheme.instance.failBlueprintNeighboursDueToAreaOrCountArrow * 0.5f + ColorScheme.instance.failBlueprintNeighboursDueToConcurentBuildArrow * 0.5f);
				} else {
					geneCellListIndexSorted[index].SetTriangleColor(ColorScheme.instance.noMotherAttachedArrow);
				}
				
			}
		}

		// skelleton bone
		for (int index = 0; index < geneCellListIndexSorted.Count; index++) {
			geneCellListIndexSorted[index].ShowSkelletonBone(false);
		}
	}

	public void UpdateGraphics(bool isSelected) {
		for (int index = 0; index < geneCellListIndexSorted.Count; index++) {
			geneCellListIndexSorted[index].UpdateGraphics(isSelected);
			geneCellListIndexSorted[index].UpdateFlipSide();
			geneCellListIndexSorted[index].UpdateBuds();

			nerveArrows.UpdateGraphics();
		}
	}


	public void ShowGeneCellsSelected(bool on) {
		for (int index = 0; index < geneCellListIndexSorted.Count; index++) {
			geneCellListIndexSorted[index].ShowCellSelected(on);
		}
	}

	public void ShowTriangles(bool on) {
		for (int index = 0; index < geneCellListIndexSorted.Count; index++) {
			geneCellListIndexSorted[index].ShowTriangle(on);
		}
	}

	private void Clear() {
		for (int index = 0; index < geneCellListIndexSorted.Count; index++) {
			Morphosis.instance.geneCellPool.Recycle(geneCellListIndexSorted[index]);
		}
		ClearGeneCellLists();
		geneCellMap.Clear();

		geneCellsTransform.localPosition = Vector3.zero;
		geneCellsTransform.localRotation = Quaternion.identity;

		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;

		nerveArrows.OnRecycle();
	}

	public void MoveOriginToOrigo() {
		Vector3 originCellPosition = originCell.position;
		foreach (Cell cell in geneCellListIndexSorted) {
			cell.transform.position -= originCellPosition;
		}
	}

	public void Grab() {
		isGrabbed = true;
		MoveOriginToOrigo();
	}

	public void Release(Creature creature) {
		isGrabbed = false;
		foreach (Cell cell in geneCellListIndexSorted) {
			cell.transform.parent = null;
		}
		creature.transform.position = Vector3.zero;
		creature.transform.rotation = Quaternion.identity;
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		foreach (Cell cell in geneCellListIndexSorted) {
			cell.transform.parent = geneCellsTransform.transform;
		}

		creature.phenotype.MoveToGenotype(creature);
	}

	// Load / Save

	private GenotypeData genotypeData = new GenotypeData();
	public GenotypeData UpdateData() { // Save: We have all genes and their data allready
		for (int index = 0; index < genes.Length; index++) {
			genotypeData.geneData[index] = genes[index].UpdateData();
		}
		genotypeData.originPosition = originCell.position;
		genotypeData.originHeading = originCell.heading;
		return genotypeData;
	}

	public void ApplyData(GenotypeData genotypeData) {
		for (int index = 0; index < genomeLength; index++) {
			genes[index] = new Gene(index, this);
			genes[index].ApplyData(genotypeData.geneData[index]);
		}
		SetReferenceGenesFromReferenceGeneIndices();
		MakeGeneCellPatternDirty();
	}

	// ^ Load / Save ^

	// Update

	public void OnRecycle() {
		Clear();
	}
}