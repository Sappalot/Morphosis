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

	public float energy {
		get {
			float energy = 0;
			foreach (Cell cell in cellList) {
				energy += cell.energy;
			}
			return energy;
		}
	}

	public float effectProduction {
		get {
			float effect = 0;
			foreach (Cell cell in cellList) {
				effect += cell.effectProduction;
			}
			return effect;
		}
	}

	public float effectConsumption {
		get {
			float effect = 0;
			foreach (Cell cell in cellList) {
				effect += cell.effectConsumption;
			}
			return effect;
		}
	}

	public float effect {
		get {
			return effectProduction - effectConsumption;
		}
	}


	[HideInInspector]
	public bool isAlive = true; // Are we going to use this approach?

	public Transform cellsTransform;

	public CellDeath cellDeathPrefab;
	public CellDetatch cellDetatchPrefab; 

	public EggCell eggCellPrefab;
	public JawCell jawCellPrefab;
	public LeafCell leafCellPrefab;
	public MuscleCell muscleCellPrefab;
	public VeinCell veinCellPrefab;

	public bool cellsDiffersFromGeneCells = true;
	public bool connectionsDiffersFromCells = true;

	public float timeOffset;

	public GameObject cells;
	public Edges edges; //Wings
	public Veins veins;

	public bool isGrabbed { get; private set; }
	[HideInInspector]
	public bool hasDirtyPosition = false;

	private Vector3 velocity = new Vector3();
	public List<Cell> cellList = new List<Cell>();
	private Vector2 spawnPosition;
	private float spawnHeading;
	private CellMap cellMap = new CellMap();

	private bool isDirty = true;

	public CircleCollider2D probe;

	//Grown cells
	public int cellCount {
		get {
			return cellMap.cellCount;
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

	public void ShuffleCellUpdateOrder() {
		ListUtils.Shuffle(cellList);
	}

	public void InitiateEmbryo(Creature creature, Vector2 position, float heading) {
		Setup(creature, position, heading);
		TryGrow(creature, true, 1, true, false);
		cellsDiffersFromGeneCells = false;
	}

	public bool UpdateCellsFromGeneCells(Creature creature, Vector2 position, float heading) {
		if (cellsDiffersFromGeneCells) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Creature UpdateCellsFromGeneCells");
			}
			Setup(creature, position, heading);
			TryGrowFully(creature, true);
			cellsDiffersFromGeneCells = false;
			return true;
		}
		return false;
	}

	//SpawnPosition is the position where the center of the root cell will appear in word space
	private void Setup(Creature creature, Vector2 spawnPosition, float spawnHeading) {
		timeOffset = Random.Range(0f, 7f); //TODO: Remove

		Clear();
		this.spawnPosition = spawnPosition;
		this.spawnHeading = spawnHeading;
	}

	public int TryGrowFully(Creature creature, bool forceGrow) {
		return TryGrow(creature, forceGrow, creature.genotype.geneCellCount, true, false);
	}

	public int TryGrow(Creature creature, bool allowOvergrowth, int cellCount, bool free, bool playEffects) {
		////Fail safe ... to be removed
		for (int index = 0; index < Life.instance.soulList.Count; index++) {
			Life.instance.soulList[index].UpdateReferences();
		}

		int growCellCount = 0;
		Genotype genotype  = creature.genotype;
		if (cellCount < 1 || this.cellCount >= genotype.geneCellCount) {
			return 0;
		}

		if (cellList.Count == 0) {
			SpawnCell(creature, genotype.GetGeneAt(0), new Vector2i(), 0, AngleUtil.CardinalEnumToCardinalIndex(CardinalEnum.north), FlipSideEnum.BlackWhite, spawnPosition, true, 30f);

			//EvoFixedUpdate(creature, 0f); //Do we really need to do this here?
			rootCell.heading = spawnHeading;
			rootCell.triangleTransform.rotation = Quaternion.Euler(0f, 0f, rootCell.heading); // Just updating graphics
			growCellCount++;
		}
		genotype.geneCellList.Sort((emp1, emp2) => emp1.buildOrderIndex.CompareTo(emp2.buildOrderIndex));

		foreach (Cell geneCell in genotype.geneCellList) {
			if (growCellCount >= cellCount) {
				break;
			}

			if (!IsCellBuiltForGeneCell(geneCell)
				&& IsCellBuiltAtNeighbourPosition(geneCell.mapPosition)
				&& (allowOvergrowth || !(IsMotherPlacentaLocation(creature, geneCell.mapPosition) || IsChildRootLocation(creature, geneCell.mapPosition)))) {
				Vector3 averagePosition = Vector3.zero;
				int positionCount = 0;

				//find neighbours around cell to build
				List<Cell> builderCells = new List<Cell>();
				for (int neighbourIndex = 0; neighbourIndex < 6; neighbourIndex++) {
					Cell gridNeighbourBuilder = cellMap.GetGridNeighbourCell(geneCell.mapPosition, neighbourIndex);
					if (gridNeighbourBuilder != null) {
						builderCells.Add(gridNeighbourBuilder);
						int indexToMe = CardinaIndexToNeighbour(gridNeighbourBuilder, geneCell);
						float meFromNeightbourBindPose = AngleUtil.CardinalIndexToAngle(indexToMe);
						float meFromNeighbour = (gridNeighbourBuilder.angleDiffFromBindpose + meFromNeightbourBindPose) % 360f;
						float distance = geneCell.radius + gridNeighbourBuilder.radius;
						averagePosition += gridNeighbourBuilder.transform.position + new Vector3(distance * Mathf.Cos(meFromNeighbour * Mathf.Deg2Rad), distance * Mathf.Sin(meFromNeighbour * Mathf.Deg2Rad), 0f);
						positionCount++;
					}
				}

				// test if the position is free to grow on
				Vector2 spawnPosition = averagePosition / positionCount;
				if (!allowOvergrowth && !CanGrowAtPosition(spawnPosition, 0.33f)) {
					continue;
				}

				// test if neighbours can afford to build cell
				float newCellEnergy = 30f;
				const float buildBaseEnergy = 10f;
				if (!free) {
					float sumExtraEnergy = 0f;
					foreach (Cell builder in builderCells) {
						if (builder.energy > buildBaseEnergy) {
							sumExtraEnergy += (builder.energy - buildBaseEnergy);
						}
					}
					if (sumExtraEnergy >= buildBaseEnergy) {
						float giftFactor = buildBaseEnergy / sumExtraEnergy;
						foreach (Cell builder in builderCells) {
							if (builder.energy > buildBaseEnergy) {
								builder.energy -= (builder.energy - buildBaseEnergy) * giftFactor;
							}
						}
						newCellEnergy = buildBaseEnergy;
					} else {
						continue;
					}
				}

				Cell newCell = SpawnCell(creature, geneCell.gene, geneCell.mapPosition, geneCell.buildOrderIndex, geneCell.bindCardinalIndex, geneCell.flipSide, spawnPosition, false, newCellEnergy);
				UpdateNeighbourReferencesIntraBody(); //We need to know our neighbours in order to update vectors correctly 
				newCell.UpdateNeighbourVectors(); //We need to update vectors to our neighbours, so that we can find our direction 
				newCell.UpdateRotation(); //Rotation is needed in order to place subsequent cells right
				newCell.UpdateFlipSide(); // Just graphics
				growCellCount++;
			}
		}
		if (growCellCount > 0) {
			//if (playEffects) {
			//	Audio.instance.CellBirth();
			//}

			PhenotypePanel.instance.MakeDirty();
			connectionsDiffersFromCells = true;
		}
		return growCellCount;
	}

	private bool IsChildRootLocation(Creature creature, Vector2i mapPosition) {
		//My(placenta) <====> Child(root)
		foreach (Soul child in creature.childSouls) {
			if (creature.soul.isConnectedWithChildSoul(child.id) && creature.soul.childSoulRootMapPosition(child.id) == mapPosition) {
				return true;
			}
		}
		return false;
	}

	private bool IsMotherPlacentaLocation(Creature creature, Vector2i mapPosition) {
		//My(root) <====> Mohther(placenta)
		if (creature.soul != null && creature.soul.motherSoulReference.id != string.Empty && creature.soul.isConnectedWithMotherSoul) {
			Creature creatureMother = creature.mother;
			foreach (Soul child in creatureMother.childSouls) {
				if (child.isConnectedWithMotherSoul && child.id == creature.id) {
					//We are talking about mothers view of me
					for (int index = 0; index < creatureMother.genotype.geneCellList.Count; index++) {
						Cell placentaCell = creatureMother.genotype.geneCellList[index];

						for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
							Vector2i neighbourMapPosition = CellMap.GetGridNeighbourGridPosition(placentaCell.mapPosition, cardinalIndex);
							if (neighbourMapPosition == creature.motherSoul.childSoulRootMapPosition(child.id)) {
								int childSoulRootCardinalBindIndex = creature.motherSoul.childSoulRootBindCardinalIndex(child.id);
								// One of mothers cells found a neighbour, which is its childSoulRootMapPosition 
								Vector2i placentaCellPositionInChildSpace = CellMap.GetGridNeighbourGridPosition(new Vector2i(0, 0), AngleUtil.CardinalIndexRawToSafe(cardinalIndex + 3 - childSoulRootCardinalBindIndex + 1));
								if (placentaCellPositionInChildSpace == mapPosition) {
									return true;
								}
							}
						}
					}
				}
			}
		}
		return false;
	}

	private bool IsCellBuiltAtConnectedMother(Cell geneCell) {

		return false;
	}

	private bool CanGrowAtPosition(Vector2 tryPosition, float tryRadius) {
		//Collider2D c = Physics2D.OverlapCircle(tryPosition, tryRadius);
		//return c == null || !(c is CircleCollider2D); //the isCircleCollider2D test is there to avoid count collision with the big world touch square collider

		List<Cell> cells = new List<Cell>();
		//All creatures in cluster
		foreach (Creature clusterPart in Life.instance.creatures) { // creature.creaturesInCluster
			cells.AddRange(clusterPart.phenotype.cellList);
		}

		foreach (Cell cell in cells) {
			if (GeometryUtils.AreCirclesIntersecting(cell.position, cell.radius, tryPosition, tryRadius)) {
				return false;
			}
		}
		return true;
	}

	//Only used by grow above (Not taking mother and children into account)
	private void UpdateNeighbourReferencesIntraBody() {
		//Debug.Log("Updating intER creature neighbours!!");
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			Vector2i center = cell.mapPosition;
			for (int direction = 0; direction < 6; direction++) {
				Vector2i gridNeighbourPos = CellMap.GetGridNeighbourGridPosition(center, direction); // GetGridNeighbour(center, CardinalDirectionHelper.ToCardinalDirection(direction));
				Debug.Assert(gridNeighbourPos != null, "Why would this happen?");
				cell.SetNeighbourCell(direction, cellMap.GetGridNeighbourCell(center, direction) /*grid[gridNeighbourPos.x, gridNeighbourPos.y].transform.GetComponent<Cell>()*/);
			}
		}
	}

	// Update neighbours, even in mother and children
	// Connect springs in between all connected (special case for mother and child)
	// Update groups
	// Update wings
	public bool UpdateConnectionsFromCellsBody(Creature creature, string motherId) {
		if (connectionsDiffersFromCells) {

			////Fail safe ... to be removed
			for (int index = 0; index < Life.instance.soulList.Count; index++) {
				Life.instance.soulList[index].UpdateReferences();
			}

			UpdateNeighbourReferencesInterBody(creature);

			//Springs
			UpdateSpringsInterBody(creature);

			//Groups
			UpdateGroupsInterBody(motherId);

			//Wings
			edges.GenerateWings(creature, cellMap); // Wings are only generated from here

			//Veins
			veins.GenerateVeins(creature, cellMap);

			//Debug
			UpdateSpringsFrequenze(); //testing only

			//Clean
			connectionsDiffersFromCells = false;
			return true;
		}
		return false;
	}

	private void UpdateNeighbourReferencesInterBody(Creature creature) {
		// All cells within body
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			Vector2i center = cell.mapPosition;
			for (int direction = 0; direction < 6; direction++) {
				Vector2i gridNeighbourPos = CellMap.GetGridNeighbourGridPosition(center, direction); 
				Debug.Assert(gridNeighbourPos != null, "Why would this happen?");
				
				cell.SetNeighbourCell(direction, cellMap.GetGridNeighbourCell(center, direction)); // Watch out for child clearing off references to mother
			}
		}

		//My(root) <====> Mohther(placenta)
		if (creature.hasMotherSoul && creature.soul.isConnectedWithMotherSoul) {
			Creature creatureMother = creature.mother;
			foreach (Soul child in creatureMother.childSouls) {
				if (child.isConnectedWithMotherSoul && child.id == creature.id) {
					//We are talking about mothers view of me
					for (int index = 0; index < creatureMother.phenotype.cellList.Count; index++) {
						Cell placentaCell = creatureMother.phenotype.cellList[index];
						for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
							Vector2i neighbourMapPosition = CellMap.GetGridNeighbourGridPosition(placentaCell.mapPosition, cardinalIndex);
							if (neighbourMapPosition == creature.motherSoul.childSoulRootMapPosition(child.id)) {
								// My placenta to childs root
								placentaCell.SetNeighbourCell(cardinalIndex, rootCell);
								//Debug.Log("Me(root)" + creature.id + " <==neighbour== Mother(placenta)" + creatureMother.id);

								//childs root to my placenta
								rootCell.SetNeighbourCell(AngleUtil.CardinalIndexRawToSafe(cardinalIndex - child.creature.motherSoul.childSoulRootBindCardinalIndex(child.id) + 1 + 3), placentaCell);
								//Debug.Log("Me(root)" + creature.id + " ==neighbour==> Mother(placenta)" + creatureMother.id);
							}
						}
					}
				}
			}
		}

		//My(placenta) <====> Child(root)
		foreach (Soul child in creature.childSouls) {
			if (creature.soul.isConnectedWithChildSoul(child.id)) {
				for (int index = 0; index < cellList.Count; index++) {
					Cell placentaCell = cellList[index];
					for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
						Vector2i neighbourMapPosition = CellMap.GetGridNeighbourGridPosition(placentaCell.mapPosition, cardinalIndex);
						if (neighbourMapPosition == creature.soul.childSoulRootMapPosition(child.id)) {
							// My placenta to childs root
							placentaCell.SetNeighbourCell(cardinalIndex, child.creature.phenotype.rootCell);
							//Debug.Log("Me: " + creature.id + ", my Child :" + child.id + " Me(placenta) ==neighbour==> Child(root)");

							//childs root to my placenta
							child.creature.phenotype.rootCell.SetNeighbourCell(AngleUtil.CardinalIndexRawToSafe(cardinalIndex - creature.soul.childSoulRootBindCardinalIndex(child.id) + 1 + 3), placentaCell);
							//Debug.Log("Me: " + creature.id + ", my Child :" + child.id + " Me(placenta) <==neighbour== Child(root)");
						}
					}
				}
			}
		}
	}

	private void UpdateGroupsInterBody(string motherId) {
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			cell.UpdateGroups(motherId);
		}
	}

	private void UpdateSpringsInterBody(Creature creature) {
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			cell.UpdateSpringConnectionsIntra();
			cell.UpdateSpringConnectionsInter(creature);
		}
	}

	private int CardinaIndexToNeighbour(Cell from, Cell to) {
		for (int index = 0; index < 6; index++) {
			Vector2i neighbourPosition = CellMap.GetGridNeighbourGridPosition(from.mapPosition, index);
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
			if (CellPanel.instance.selectedCell == cellList[cellList.Count - 1]) {
				CellPanel.instance.selectedCell = null;
			}

			DeleteCell(cellList[cellList.Count - 1], false, true);
			shrinkCellCount++;
		}
		
	}

	public void ChangeEnergy(float amount) {
		for (int count = 0; count < cellCount; count++) {
			cellList[count].energy = Mathf.Min(cellList[count].energy + amount, Cell.maxEnergy);
		}
	}

	public bool IsRootNeighbouringMothersPlacenta(Creature creature) {
		Debug.Assert(creature.hasMotherSoul);

		List<Cell> neighbours = rootCell.GetNeighbourCells();
		foreach(Cell motherMai in neighbours) {
			if (motherMai.creature == creature.motherSoul.creature) {
				return true;
			}
		}
		return false;
	}

	public void OnNeighbourDeleted(Creature creature, Cell deletedCell, Cell disturbedCell) {
		disturbedCell.RemoveNeighbourCell(deletedCell);

		//Check if mothers placenta is gone
		if (creature.hasMotherSoul && disturbedCell.isRoot && !deletedCell.IsSameCreature(disturbedCell)) {
			if (!IsRootNeighbouringMothersPlacenta(creature)) {
				DetatchFromMother(creature, true);
			}
		}

		connectionsDiffersFromCells = true;
	}

	//This will make root inaccessible
	public void DeleteAllCells() {
		List<Cell> allCells = new List<Cell>(cellList);

		for (int i = 0; i < allCells.Count; i++) {
			DeleteCell(allCells[i], false, true);
		}
	}

	//This is the one and only final place where cell is removed
	public void DeleteCell(Cell deleteCell, bool deleteDebris, bool playEffects = false) {
		if (playEffects) {
			Audio.instance.CellDeath();
			if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype) {
				SpawnCellDeathEffect(deleteCell.position, Color.red);
				SpawnCellDeleteBloodEffect(deleteCell);
			}
		}

		List<Cell> disturbedCells = deleteCell.GetNeighbourCells();
		foreach (Cell disturbedCell in disturbedCells) {
			deleteCell.RemoveNeighbourCell(disturbedCell);
			disturbedCell.creature.phenotype.OnNeighbourDeleted(disturbedCell.creature, deleteCell, disturbedCell);
		}

		if (!deleteCell.isRoot) {
			cellMap.RemoveCellAtGridPosition(deleteCell.mapPosition);
			cellList.Remove(deleteCell);
			Destroy(deleteCell.gameObject);
			if (CellPanel.instance.selectedCell == deleteCell) {
				CellPanel.instance.selectedCell = null;
			}
		} else {
			isAlive = false; //Hack
		}

		if (deleteDebris) {
			DeleteDebris();
		}

		PhenotypePanel.instance.MakeDirty(); // Update cell text with fewer cells
		CreatureSelectionPanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();
		connectionsDiffersFromCells = true;
	}

	private void DeleteDebris() {
		List<Vector2i> keepers = cellMap.IsConnectedTo(rootCell.mapPosition);
		List<Cell> debris = new List<Cell>();
		foreach (Cell c in cellList) {
			if (keepers.Find(p => p == c.mapPosition) == null) {
				debris.Add(c);
			}
		}
		for (int i = 0; i < debris.Count; i++) {
			DeleteCell(debris[i], false, true);
		}
	}

	public void SpawnCellDeathEffect(Vector2 position, Color color) {
		CellDeath death = Instantiate(cellDeathPrefab, position, Quaternion.identity);
		death.Prime(color);
	}

	public void SpawnCellDeleteBloodEffect(Cell deleteCell) {
		for (int i = 0; i < 6; i++) {
			if (deleteCell.HasNeighbourCell(i)) {
				Cell neighbourCell = deleteCell.GetNeighbourCell(i);
				//neighbour looks back
				for (int neighbourCardinalIndex = 0; neighbourCardinalIndex < 6; neighbourCardinalIndex++) {
					if (neighbourCell.GetNeighbourCell(neighbourCardinalIndex) == deleteCell) {
						float angle = AngleUtil.CardinalIndexToAngle(neighbourCardinalIndex) + neighbourCell.angleDiffFromBindpose; //YEY :D
						neighbourCell.creature.phenotype.SpawnCellBlood(neighbourCell, angle);
					}
				}
			}
		}
	}

	public void SpawnCellBlood(Cell cell, float heading) {
		CellDetatch blood = Instantiate(cellDetatchPrefab, cell.position, Quaternion.Euler(0f, 0f, heading)); //Quaternion.Euler(0f, 0f, heading)
		blood.Prime(Color.red);
		blood.transform.parent = cell.transform;
	}

	public void SpawnCellDetatchBloodEffect(Cell detatchCell) {
		for (int i = 0; i < 6; i++) {
			Cell neighbourCell = detatchCell.GetNeighbourCell(i);
			if (neighbourCell != null && neighbourCell.creature.id == detatchCell.creature.soul.motherSoulReference.id) {
				// mother neighbour looks back...
				for (int neighbourCardinalIndex = 0; neighbourCardinalIndex < 6; neighbourCardinalIndex++) {
					if (neighbourCell.GetNeighbourCell(neighbourCardinalIndex) == detatchCell) {
						//... at me
						float angle = AngleUtil.CardinalIndexToAngle(neighbourCardinalIndex) + neighbourCell.angleDiffFromBindpose; //YEY :D
						neighbourCell.creature.phenotype.SpawnCellBlood(neighbourCell, angle);
					}
				}

				//me
				float a = AngleUtil.CardinalIndexToAngle(i) + detatchCell.angleDiffFromBindpose;
				SpawnCellBlood(detatchCell, a);
			}
		}
	}

	public bool DetatchFromMother(Creature creature, bool playEffects = false) {
		if (creature.hasMotherSoul && creature.soul.isConnectedWithMotherSoul) {
			if (playEffects) {
				Audio.instance.CreatureDetatch();
				Cell rootCell = creature.phenotype.rootCell;
				SpawnCellDetatchBloodEffect(rootCell);
			}

			//GeometryUtils.GetVector(rootCell.heading + 180f, rootCell.radius)

			//me
			creature.soul.SetConnectedWithMotherSoul(false);
			connectionsDiffersFromCells = true;

			//mother
			creature.motherSoul.creature.phenotype.connectionsDiffersFromCells = true;

			CreatureSelectionPanel.instance.MakeDirty();
			return true;
		}
		return false;
	}

	private bool IsCellBuiltForGeneCell(Cell geneCell) {
		return cellMap.HasCell(geneCell.mapPosition);
	}

	private List<Cell> GetBuiltNeighbourCells(Vector2i gridPosition) {
		List<Cell> neighbours = new List<Cell>();
		for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
			Cell neighbour = cellMap.GetCell(CellMap.GetGridNeighbourGridPosition(gridPosition, cardinalIndex));
			if (neighbour != null) {
				neighbours.Add(neighbour);
			}
		}
		return neighbours;
	}

	private bool IsCellBuiltAtNeighbourPosition(Vector2i gridPosition) {
		for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
			if (cellMap.HasCell(CellMap.GetGridNeighbourGridPosition(gridPosition, cardinalIndex))) {
				return true;
			}
		}
		return false;
	}

	private Cell SpawnCell(Creature creature, Gene gene, Vector2i mapPosition, int buildOrderIndex, int bindCardinalIndex, FlipSideEnum flipSide, Vector2 position, bool modelSpace, float spawnEnergy) {
		Cell cell = InstantiateCell(gene.type, mapPosition);
		Vector2 spawnPosition = (modelSpace ? CellMap.ToModelSpacePosition(mapPosition) : Vector2.zero) + position;
		cell.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0f);

		cell.mapPosition = mapPosition;
		cell.buildOrderIndex = buildOrderIndex;
		cell.gene = gene;
		cell.bindCardinalIndex = bindCardinalIndex;
		cell.flipSide = flipSide;
		cell.timeOffset = timeOffset;
		cell.creature = creature;

		cell.energy = spawnEnergy;

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

	private void Clear() {
		for (int index = 0; index < cellList.Count; index++) {
			Destroy(cellList[index].gameObject);
		}
		cellList.Clear();
		cellMap.Clear();
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
			cellList[index].ShowTriangle(false); // Debug
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

	//Allways false for phenotype
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

	private void SetCollider(bool on) {
		foreach (Cell cell in cellList) {
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

	// Load / Save

	private PhenotypeData phenotypeData = new PhenotypeData();
	public PhenotypeData UpdateData() {
		phenotypeData.timeOffset = timeOffset;
		phenotypeData.cellDataList.Clear();
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			phenotypeData.cellDataList.Add(cell.UpdateData());
		}
		phenotypeData.differsFromGenotype = cellsDiffersFromGeneCells;
		return phenotypeData;
	}

	public void ApplyData(PhenotypeData phenotypeData, Creature creature) {
		timeOffset = phenotypeData.timeOffset;

		Setup(creature, phenotypeData.cellDataList[0].position, phenotypeData.cellDataList[0].heading);
		for (int index = 0; index < phenotypeData.cellDataList.Count; index++) {
			CellData cellData = phenotypeData.cellDataList[index];
			Cell cell = InstantiateCell(creature.genotype.genome[cellData.geneIndex].type, cellData.mapPosition);
			cell.ApplyData(cellData, creature);
		}
		cellsDiffersFromGeneCells = false; //This work is done
		connectionsDiffersFromCells = true; //We need to connect mothers with children
	}

	// ^ Load / Save ^
	// Update

	public void UpdateGraphics() {
		//TODO: Update cells flip triangles here

		edges.UpdateGraphics();
		veins.UpdateGraphics();

		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update Creature Phenotype");

			SetCollider(hasCollider);
			isDirty = false;
		}

		//TODO Check if dirty
		//Todo: only if creature inside frustum && should be shown
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdateGraphics();
		}
	}

	public bool UpdateKillWeakCells() {
		if (rootCell.energy <= 0f) {
			return true;
		}
		for (int index = 1; index < cellList.Count; index++) {
			if (cellList[index].energy <= 0f) {
				DeleteCell(cellList[index], true, true);
			}
		}
		return false;
	}

	public void UpdatePhysics(Creature creature, float fixedTime, float deltaTickTime, bool isTick) {
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
		//	cellList[index].TurnNeighboursInPlace();
		//}

		// Edges, let edge-wings apply proper forces to neighbouring cells
		edges.UpdatePhysics(velocity, creature);

		veins.UpdatePhysics(deltaTickTime);

		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdatePhysics(fixedTime, deltaTickTime, isTick);
		}
	}
}