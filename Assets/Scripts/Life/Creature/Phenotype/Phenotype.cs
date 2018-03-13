using UnityEngine;
using System.Collections.Generic;

// The physical creature defined by all its cells

public class Phenotype : MonoBehaviour {
	[HideInInspector]
	public Cell originCell {
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

	public bool isAttachedToMother {
		get {
			return originCell.isAttachedToMother;
		}
	}

	[HideInInspector]
	public bool isAlive = true; // Are we going to use this approach?

	public Transform cellsTransform;

	public CellDeath cellDeathPrefab;
	public CellDetatch cellDetatchPrefab;

	public EggCell eggCellPrefab;
	public FungalCell fungalCellPrefab;
	public JawCell jawCellPrefab;
	public LeafCell leafCellPrefab;
	public MuscleCell muscleCellPrefab;
	public RootCell rootCellPrefab;
	public ShellCell shellCellPrefab;
	public VeinCell veinCellPrefab;

	public bool cellsDiffersFromGeneCells = true;
	public bool connectionsDiffersFromCells = true;
	public Dictionary<Cell, Vector2> detatchmentKick;

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

	//time
	private ulong bornTick;
	private ulong deadTick;

	public ulong GetAgeTicks(ulong worldTicks) {
		return worldTicks - bornTick;
	}

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
		NoGrowthReason reason;
		TryGrow(creature, true, 1, true, false, 0, true, out reason);
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

	//SpawnPosition is the position where the center of the origin cell will appear in word space
	private void Setup(Creature creature, Vector2 spawnPosition, float spawnHeading) {
		timeOffset = 0f; // Random.Range(0f, 7f); //TODO: Remove

		Clear();
		this.spawnPosition = spawnPosition;
		this.spawnHeading = spawnHeading;
	}

	public int TryGrowFully(Creature creature, bool forceGrow) {
		NoGrowthReason reason;
		return TryGrow(creature, forceGrow, creature.genotype.geneCellCount, true, false, 0, true, out reason);
	}

	public int TryGrow(Creature creature, bool allowOvergrowth, int cellCount, bool free, bool playEffects, ulong worldTicks, bool enableInstantRegrowth, out NoGrowthReason noGrowthReason) {
		noGrowthReason = new NoGrowthReason();
		
		////Fail safe ... to be removed
		//Life.instance.UpdateSoulReferences();

		int growCellCount = 0;
		Genotype genotype  = creature.genotype;
		if (cellCount < 1 || this.cellCount >= genotype.geneCellCount) {
			noGrowthReason.fullyGrown = true;
			return 0;
		}

		if (cellList.Count == 0) {
			SpawnCell(creature, genotype.GetGeneAt(0), new Vector2i(), 0, AngleUtil.CardinalEnumToCardinalIndex(CardinalEnum.north), FlipSideEnum.BlackWhite, spawnPosition, true, 30f);

			//EvoFixedUpdate(creature, 0f); //Do we really need to do this here?
			originCell.heading = spawnHeading;
			originCell.triangleTransform.rotation = Quaternion.Euler(0f, 0f, originCell.heading); // Just updating graphics
			growCellCount++;
		}
		genotype.geneCellList.Sort((emp1, emp2) => emp1.buildOrderIndex.CompareTo(emp2.buildOrderIndex));

		foreach (Cell geneCell in genotype.geneCellList) {
			if (growCellCount >= cellCount) {
				break;
			}

			if (!IsCellBuiltForGeneCell(geneCell) && IsCellBuiltAtNeighbourPosition(geneCell.mapPosition)) {
				// test if the cell map position is free to grow on
				if (!allowOvergrowth && (IsMotherPlacentaLocation(creature, geneCell.mapPosition) || IsChildOriginLocation(creature, geneCell.mapPosition))) {
					noGrowthReason.roomBound = true;
					continue;
				}

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

				// test if long enough time has passed since cell was killed
				if (!enableInstantRegrowth && cellMap.HasKilledTimeStamp(geneCell.mapPosition)) {
					if (worldTicks < cellMap.KilledTimeStamp(geneCell.mapPosition) + GlobalSettings.instance.phenotype.cellRebuildCooldown / Time.fixedDeltaTime) {
						noGrowthReason.respawnTimeBound = true;
						continue;
					} else {
						cellMap.RemoveTimeStamp(geneCell.mapPosition);
					}
				}

				// test if the position is free to grow on
				Vector2 spawnPosition = averagePosition / positionCount;
				if (!allowOvergrowth && !CanGrowAtPosition(spawnPosition, 0.33f)) {
					noGrowthReason.roomBound = true;
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
						noGrowthReason.energyBound = true;
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
			if (!IsSliding((float)worldTicks)) {
				SetTrueCellDrag(); //make slow
			}
			//if (playEffects) {
			//	Audio.instance.CellBirth();
			//}

			PhenotypePanel.instance.MakeDirty();
			connectionsDiffersFromCells = true;
		}
		return growCellCount;
	}

	private bool IsChildOriginLocation(Creature creature, Vector2i mapPosition) {
		//My(placenta) <====> Child(origin)
		foreach (Soul child in creature.childSouls) {
			if (creature.soul.isConnectedWithChildSoul(child.id) && creature.soul.childSoulOriginMapPosition(child.id) == mapPosition) {
				return true;
			}
		}
		return false;
	}

	private bool IsMotherPlacentaLocation(Creature creature, Vector2i mapPosition) {
		//My(origin) <====> Mohther(placenta)
		if (creature.soul != null && creature.soul.motherSoulReference.id != string.Empty && creature.soul.isConnectedWithMotherSoul) {
			Creature creatureMother = creature.mother;
			foreach (Soul child in creatureMother.childSouls) {
				if (child.isConnectedWithMotherSoul && child.id == creature.id) {
					//We are talking about mothers view of me
					for (int index = 0; index < creatureMother.genotype.geneCellList.Count; index++) {
						Cell placentaCell = creatureMother.genotype.geneCellList[index];

						for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
							Vector2i neighbourMapPosition = CellMap.GetGridNeighbourGridPosition(placentaCell.mapPosition, cardinalIndex);
							if (neighbourMapPosition == creature.motherSoul.childSoulOriginMapPosition(child.id)) {
								int childSoulOriginCardinalBindIndex = creature.motherSoul.childSoulOriginBindCardinalIndex(child.id);
								// One of mothers cells found a neighbour, which is its childSoulOriginMapPosition 
								Vector2i placentaCellPositionInChildSpace = CellMap.GetGridNeighbourGridPosition(new Vector2i(0, 0), AngleUtil.CardinalIndexRawToSafe(cardinalIndex + 3 - childSoulOriginCardinalBindIndex + 1));
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

			////Fail safe ... to be removed .... or is it ... really?
			Life.instance.UpdateSoulReferences();

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

		//My(origin) <====> Mohther(placenta)
		if (creature.hasMotherSoul && creature.soul.isConnectedWithMotherSoul) {
			Creature creatureMother = creature.mother;
			if (creatureMother.soul != null) {
				foreach (Soul child in creatureMother.childSouls) {
					if (child.isConnectedWithMotherSoul && child.id == creature.id) {
						//We are talking about mothers view of me
						for (int index = 0; index < creatureMother.phenotype.cellList.Count; index++) {
							Cell placentaCell = creatureMother.phenotype.cellList[index];
							for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
								Vector2i neighbourMapPosition = CellMap.GetGridNeighbourGridPosition(placentaCell.mapPosition, cardinalIndex);
								if (neighbourMapPosition == creature.motherSoul.childSoulOriginMapPosition(child.id)) {
									// My placenta to childs origin
									placentaCell.SetNeighbourCell(cardinalIndex, originCell);
									//Debug.Log("Me(origin)" + creature.id + " <==neighbour== Mother(placenta)" + creatureMother.id);

									//childs origin to my placenta
									originCell.SetNeighbourCell(AngleUtil.CardinalIndexRawToSafe(cardinalIndex - child.creature.motherSoul.childSoulOriginBindCardinalIndex(child.id) + 1 + 3), placentaCell);
									//Debug.Log("Me(origin)" + creature.id + " ==neighbour==> Mother(placenta)" + creatureMother.id);
								}
							}
						}
					}
				}
			}
		}

		//My(placenta) <====> Child(origin)
		foreach (Soul child in creature.childSouls) {
			if (creature.soul.isConnectedWithChildSoul(child.id)) {
				for (int index = 0; index < cellList.Count; index++) {
					Cell placentaCell = cellList[index];
					for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
						Vector2i neighbourMapPosition = CellMap.GetGridNeighbourGridPosition(placentaCell.mapPosition, cardinalIndex);
						if (neighbourMapPosition == creature.soul.childSoulOriginMapPosition(child.id)) {
							// My placenta to childs origin
							placentaCell.SetNeighbourCell(cardinalIndex, child.creature.phenotype.originCell);
							//Debug.Log("Me: " + creature.id + ", my Child :" + child.id + " Me(placenta) ==neighbour==> Child(origin)");

							//childs origin to my placenta
							child.creature.phenotype.originCell.SetNeighbourCell(AngleUtil.CardinalIndexRawToSafe(cardinalIndex - creature.soul.childSoulOriginBindCardinalIndex(child.id) + 1 + 3), placentaCell);
							//Debug.Log("Me: " + creature.id + ", my Child :" + child.id + " Me(placenta) <==neighbour== Child(origin)");
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

			KillCell(cellList[cellList.Count - 1], false, true, 0);
			shrinkCellCount++;
		}
		
	}

	public void ChangeEnergy(float amount) {
		for (int count = 0; count < cellCount; count++) {
			cellList[count].energy = Mathf.Min(cellList[count].energy + amount, Cell.maxEnergy);
		}
	}

	public bool IsOriginNeighbouringMothersPlacenta(Creature creature) {
		Debug.Assert(creature.hasMotherSoul);

		List<Cell> neighbours = originCell.GetNeighbourCells();
		foreach(Cell motherMai in neighbours) {
			//if (motherMai.creature == creature.motherSoul.creature) {
			if (motherMai.creature.id == creature.soul.motherSoulReference.id) {
				return true;
			}
		}
		return false;
	}

	public void OnNeighbourDeleted(Creature creature, Cell deletedCell, Cell disturbedCell) {
		disturbedCell.RemoveNeighbourCell(deletedCell);

		//Check if mothers placenta is gone
		if (creature.hasMotherSoul && disturbedCell.isOrigin && !deletedCell.IsSameCreature(disturbedCell)) {
			if (!IsOriginNeighbouringMothersPlacenta(creature)) {
				DetatchFromMother(creature, true);
			}
		}

		connectionsDiffersFromCells = true;
	}

	//This will make origin inaccessible
	public void KillAllCells() {
		List<Cell> allCells = new List<Cell>(cellList);

		for (int i = 0; i < allCells.Count; i++) {
			KillCell(allCells[i], false, true, 0);
		}
	}

	//This is the one and only final place where cell is removed
	// fixedTime = 0 ==> no mar will be set to when this cell can be regrown again
	public void KillCell(Cell deleteCell, bool deleteDebris, bool playEffects, ulong worldTicks) {
		if (playEffects && (GlobalPanel.instance.effectsPlaySound.isOn || (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype && GlobalPanel.instance.effectsShowParticles.isOn))) {
			bool isObserved = CameraUtils.IsObservedLazy(deleteCell.position, GlobalSettings.instance.orthoMaxHorizonFx);

			if (GlobalPanel.instance.effectsPlaySound.isOn && isObserved) {
				Audio.instance.CellDeath(CameraUtils.GetEffectStrengthLazy());
			}
		
			if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype && GlobalPanel.instance.effectsShowParticles.isOn && isObserved) {
				SpawnCellDeathEffect(deleteCell.position, Color.red);
				SpawnCellDeleteBloodEffect(deleteCell);
			}
		}

		// Clean up
		deleteCell.OnDelete();
		List<Cell> disturbedCells = deleteCell.GetNeighbourCells();
		foreach (Cell disturbedCell in disturbedCells) {
			deleteCell.RemoveNeighbourCell(disturbedCell);
			disturbedCell.creature.phenotype.OnNeighbourDeleted(disturbedCell.creature, deleteCell, disturbedCell);
		}

		if (!deleteCell.isOrigin) {
			cellMap.RemoveCellAtGridPosition(deleteCell.mapPosition);
			cellList.Remove(deleteCell);
			Destroy(deleteCell.gameObject);
			if (CellPanel.instance.selectedCell == deleteCell) {
				CellPanel.instance.selectedCell = null;
			}
		} else {
			isAlive = false; //Hack, to avoid problems with origin missing everywhere
		}

		if (deleteDebris) {
			DeleteDebris();
		}

		//Mark cell as destroyed so we don't try to build it back emediatly
		if (worldTicks != 0 && !cellMap.HasKilledTimeStamp(deleteCell.mapPosition)) {
			cellMap.AddKilledTimeStamp(deleteCell.mapPosition, worldTicks);
		}

		PhenotypePanel.instance.MakeDirty(); // Update cell text with fewer cells
		CreatureSelectionPanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();
		connectionsDiffersFromCells = true;

		CreatureSelectionPanel.instance.UpdateSelectionCluster();
	}

	private void DeleteDebris() {
		List<Vector2i> keepers = cellMap.IsConnectedTo(originCell.mapPosition);
		List<Cell> debris = new List<Cell>();
		foreach (Cell c in cellList) {
			if (keepers.Find(p => p == c.mapPosition) == null) {
				debris.Add(c);
			}
		}
		for (int i = 0; i < debris.Count; i++) {
			KillCell(debris[i], false, true, 0);
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
			if (playEffects && CameraUtils.IsObservedLazy(creature.phenotype.originCell.position, GlobalSettings.instance.orthoMaxHorizonFx)) {
				if (GlobalPanel.instance.effectsPlaySound.isOn) {
					Audio.instance.CreatureDetatch(CameraUtils.GetEffectStrengthLazy());
				}
				if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype && GlobalPanel.instance.effectsShowParticles.isOn) {
					Cell originCell = creature.phenotype.originCell;
					SpawnCellDetatchBloodEffect(originCell);
				}
			}

			//Kick separation
			Creature mother = creature.motherSoul.creature;
			Cell childOrigin = creature.phenotype.originCell;
			int placentaCount = 0;
			for (int i = 0; i < 6; i++) {
				if (childOrigin.HasNeighbourCell(i) && childOrigin.GetNeighbourCell(i).creature.id != creature.id) {
					childOrigin.GetNeighbourCell(i).isPlacenta = false;
					placentaCount++;
				}
			}
			if (placentaCount > 0) {
				mother.phenotype.detatchmentKick = new Dictionary<Cell, Vector2>();
				detatchmentKick = new Dictionary<Cell, Vector2>();
				Vector2 offspringForce = Vector2.zero;
				float cellSum = cellCount + mother.phenotype.cellCount;
				float kickFactor = GlobalSettings.instance.phenotype.detatchmentKick * cellSum + GlobalSettings.instance.phenotype.detatchmentKickSquare * cellSum * cellSum;

				for (int i = 0; i < 6; i++) {
					if (childOrigin.HasNeighbourCell(i) && childOrigin.GetNeighbourCell(i).creature.id != creature.id) {
						Vector2 placentaForce = (childOrigin.GetNeighbourCell(i).position - childOrigin.position).normalized / placentaCount;
						mother.phenotype.detatchmentKick.Add(childOrigin.GetNeighbourCell(i), placentaForce * kickFactor);
						offspringForce -= placentaForce;
					}
				}
				detatchmentKick.Add(childOrigin, offspringForce * kickFactor);
			}

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

	private void ApplyDetatchKick() {
		foreach (Cell cell in detatchmentKick.Keys) {
			if (cell != null) {
				cell.GetComponent<Rigidbody2D>().AddForce(detatchmentKick[cell], ForceMode2D.Impulse);
			}
		}
		detatchmentKick = null;
	}

	private ulong kickTickStamp = 0;
	
	private void SetTrueCellDrag() {
		foreach (Cell cell in cellList) {
			if (cell.GetCellType() == CellTypeEnum.Leaf) {
				cell.GetComponent<Rigidbody2D>().drag = 0.2f;
			} else {
				cell.GetComponent<Rigidbody2D>().drag = 0.2f;
			}
		}
	}

	public bool IsSliding(float worldTicks) {
		return kickTickStamp > 0 && worldTicks < kickTickStamp + (ulong)(GlobalSettings.instance.phenotype.detatchSlideDuration / Time.fixedDeltaTime);
	}

	private void SetSlideCellDrag() {
		foreach (Cell cell in cellList) {
			cell.GetComponent<Rigidbody2D>().drag = 0.2f;
		}
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

		// Gene settings
		// Egg
		// form gene to eggCell
		cell.eggCellFertilizeThreshold =       gene.eggCellFertilizeThreshold;
		cell.eggCellCanFertilizeWhenAttached = gene.eggCellCanFertilizeWhenAttached;
		cell.eggCellDetatchMode =              gene.eggCellDetatchMode;
		cell.eggCellDetatchSizeThreshold =     gene.eggCellDetatchSizeThreshold; 
		cell.eggCellDetatchEnergyThreshold =   gene.eggCellDetatchEnergyThreshold;

		return cell;
	}

	private Cell InstantiateCell(CellTypeEnum type, Vector2i mapPosition) {
		Cell cell = null;
		if (type == CellTypeEnum.Egg) {
			cell = (Instantiate(eggCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Fungal) {
			cell = (Instantiate(fungalCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Jaw) {
			cell = (Instantiate(jawCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Leaf) {
			cell = (Instantiate(leafCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Muscle) {
			cell = (Instantiate(muscleCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Root) {
			cell = (Instantiate(rootCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Shell) {
			cell = (Instantiate(shellCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
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

	public void ShowOutline(bool show) {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].ShowOutline(show);
		}
	}

	public void UpdateOutline(Creature creature, bool isSelected, bool isClusterSelected) {
		for (int index = 0; index < cellList.Count; index++) {
			if (isSelected) {
				cellList[index].ShowOutline(true);
				cellList[index].SetOutlineColor(ColorScheme.instance.outlineSelected);
			} else if (isClusterSelected) {
				cellList[index].ShowOutline(true);
				cellList[index].SetOutlineColor(ColorScheme.instance.outlineCluster);
			} else {
				cellList[index].ShowOutline(false);
			}

			//fix properly
			cellList[index].ShowShadow(false);
		}

		for (int index = 0; index < cellList.Count; index++) {
			if (cellList[index].isOrigin) {
				cellList[index].ShowTriangle(true); // Debug
				if (!creature.hasMotherSoul) {
					if (!creature.hasChildSoul) {
						cellList[index].SetTriangleColor(ColorScheme.instance.noRelativesArrow);
					} else {
						cellList[index].SetTriangleColor(ColorScheme.instance.noMotherArrow);
					}
				} else if (creature.soul.isConnectedWithMotherSoul) {
					cellList[index].SetTriangleColor(ColorScheme.instance.motherAttachedArrow);
				} else {
					cellList[index].SetTriangleColor(ColorScheme.instance.noMotherAttachedArrow);
				}
			} else {
				cellList[index].ShowTriangle(false);
			}
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
	//public void ShowShadow(bool on) {
	//	for (int index = 0; index < cellList.Count; index++) {
	//		cellList[index].ShowShadow(on);
	//	}
	//}

	public void Show(bool on) {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].Show(on);
		}
	}
	
	public void MoveOriginToOrigo() {
		Vector3 originCellPosition = originCell.position;
		foreach (Cell cell in cellList) {
			cell.transform.position -= originCellPosition;
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
		MoveOriginToOrigo();
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
			MoveTo(creature.genotype.originCell.position);
			TurnTo(creature.genotype.originCell.heading);
			hasDirtyPosition = false;
		}
	}

	public void Move(Vector2 vector) {
		foreach (Cell cell in cellList) {
			cell.transform.position += (Vector3)vector;
		}
	}

	public void MoveTo(Vector2 vector) {
		Move(vector - originCell.position);
	}

	//Make origin cell point in this direction while the rest of the cells tags along
	//Angle = 0 ==> origin cell pointing east
	//Angle = 90 ==> origin cell pointing north
	private void TurnTo(float targetAngle) {
		float deltaAngle = targetAngle - originCell.heading;
		foreach (Cell cell in cellList) {
			Vector3 originToCell = cell.transform.position - (Vector3)originCell.position;
			Vector3 turnedVector = Quaternion.Euler(0, 0, deltaAngle) * originToCell;
			cell.transform.position = (Vector2)originCell.position + (Vector2)turnedVector;
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

	// Save
	private PhenotypeData phenotypeData = new PhenotypeData();
	public PhenotypeData UpdateData() {
		phenotypeData.timeOffset = timeOffset;
		phenotypeData.cellDataList.Clear();
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			phenotypeData.cellDataList.Add(cell.UpdateData());
		}
		phenotypeData.differsFromGenotype = cellsDiffersFromGeneCells;

		phenotypeData.eggCellTick =    eggCellTick;
		phenotypeData.fungalCellTick = fungalCellTick;
		phenotypeData.jawCellTick =    jawCellTick;
		phenotypeData.leafCellTick =   leafCellTick;
		phenotypeData.muscleCellTick = muscleCellTick;
		phenotypeData.rootCellTick =   rootCellTick;
		phenotypeData.shellCellTick =  shellCellTick;
		phenotypeData.veinCellTick =   veinCellTick;

		phenotypeData.veinTick =       veinTick;
		return phenotypeData;
	}

	// Load
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

		eggCellTick =    phenotypeData.eggCellTick;
		fungalCellTick = phenotypeData.fungalCellTick;
		jawCellTick =    phenotypeData.jawCellTick;
		leafCellTick =   phenotypeData.leafCellTick;
		muscleCellTick = phenotypeData.muscleCellTick;
		rootCellTick =   phenotypeData.rootCellTick;
		shellCellTick =  phenotypeData.shellCellTick;
		veinCellTick =   phenotypeData.veinCellTick;

		phenotypeData.veinTick = veinTick;

		//Turn arrrows right
		UpdateRotation();
	}

	// ^ Load / Save ^
	// Update

	public void UpdateGraphics(Creature creature) {
		//TODO: Update cells flip triangles here

		edges.UpdateGraphics();
		veins.UpdateGraphics(CreatureSelectionPanel.instance.IsSelected(creature));

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

	public bool UpdateKillWeakCells(ulong worldTicks) {
		if (originCell.energy <= 0f) {
			return true;
		}
		for (int index = 1; index < cellList.Count; index++) {
			if (cellList[index].energy <= 0f) {
				KillCell(cellList[index], true, true, worldTicks);
			}
		}
		return false;
	}

	//time
	private int eggCellTick;
	private int fungalCellTick;
	private int jawCellTick;
	private int leafCellTick;
	private int muscleCellTick;
	private int rootCellTick;
	private int shellCellTick;
	private int veinCellTick;

	private int veinTick;

	//time ^

	public void UpdatePhysics(Creature creature, ulong worldTick) {
		if (isGrabbed) {
			return;
		}

		//time
		eggCellTick++;
		if (eggCellTick >= GlobalSettings.instance.quality.eggCellTickPeriod) {
			eggCellTick = 0;
		}

		fungalCellTick++;
		if (fungalCellTick >= GlobalSettings.instance.quality.fungalCellTickPeriod) {
			fungalCellTick = 0;
		}

		jawCellTick++;
		if (jawCellTick >= GlobalSettings.instance.quality.jawCellTickPeriod) {
			jawCellTick = 0;
		}

		leafCellTick++;
		if (leafCellTick >= GlobalSettings.instance.quality.leafCellTickPeriod) {
			leafCellTick = 0;
		}

		muscleCellTick++;
		if (muscleCellTick >= GlobalSettings.instance.quality.muscleCellTickPeriod) {
			muscleCellTick = 0;
		}

		rootCellTick++;
		if (rootCellTick >= GlobalSettings.instance.quality.rootCellTickPeriod) {
			rootCellTick = 0;
		}

		shellCellTick++;
		if (shellCellTick >= GlobalSettings.instance.quality.shellCellTickPeriod) {
			shellCellTick = 0;
		}

		veinCellTick++;
		if (veinCellTick >= GlobalSettings.instance.quality.veinCellTickPeriod) {
			veinCellTick = 0;
		}

		veinTick++;
		if (veinTick >= GlobalSettings.instance.quality.veinTickPeriod) {
			veinTick = 0;
		}
		//time ^

		// Detatchment Kick
		if (detatchmentKick != null) {
			SetSlideCellDrag();
			ApplyDetatchKick();
			kickTickStamp = worldTick;
		}
		if (kickTickStamp > 0 && worldTick > kickTickStamp + (ulong)(GlobalSettings.instance.phenotype.detatchSlideDuration / Time.fixedDeltaTime)) {
			SetTrueCellDrag(); //make slow
			kickTickStamp = 0;
		}

		// Creature
		Vector3 averageVelocity = new Vector3();
		for (int index = 0; index < cellList.Count; index++) {
			averageVelocity += cellList[index].velocity;
		}
		velocity = (cellList.Count > 0f) ? velocity = averageVelocity / cellList.Count : new Vector3();

		// Edges, let edge-wings apply proper forces to neighbouring cells
		edges.UpdatePhysics(velocity, creature);

		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdatePhysics();
			//if (leafTick == 0 && GlobalPanel.instance.effectsUpdateMetabolism.isOn) {
			//	cellList[index].UpdateMetabolism(leafTickPeriod, worldTick);
			//}
		}

		//Metabolism
		if (GlobalPanel.instance.effectsUpdateMetabolism.isOn) {
			for (int index = 0; index < cellList.Count; index++) {
				Cell cell = cellList[index];
				if (eggCellTick == 0           && cell.GetCellType() == CellTypeEnum.Egg) {
					cell.UpdateMetabolism(GlobalSettings.instance.quality.eggCellTickPeriod, worldTick);
				} else if (fungalCellTick == 0 && cell.GetCellType() == CellTypeEnum.Fungal) {
					cell.UpdateMetabolism(GlobalSettings.instance.quality.fungalCellTickPeriod, worldTick);
				} else if (jawCellTick == 0    && cell.GetCellType() == CellTypeEnum.Jaw) {
					cell.UpdateMetabolism(GlobalSettings.instance.quality.jawCellTickPeriod, worldTick);
				} else if (leafCellTick == 0   && cell.GetCellType() == CellTypeEnum.Leaf) {
					cell.UpdateMetabolism(GlobalSettings.instance.quality.leafCellTickPeriod, worldTick);
				} else if (muscleCellTick == 0 && cell.GetCellType() == CellTypeEnum.Muscle) {
					cell.UpdateMetabolism(GlobalSettings.instance.quality.muscleCellTickPeriod, worldTick);
				} else if (rootCellTick == 0   && cell.GetCellType() == CellTypeEnum.Root) {
					cell.UpdateMetabolism(GlobalSettings.instance.quality.rootCellTickPeriod, worldTick);
				} else if (shellCellTick == 0  && cell.GetCellType() == CellTypeEnum.Shell) {
					cell.UpdateMetabolism(GlobalSettings.instance.quality.shellCellTickPeriod, worldTick);
				} else if (veinCellTick == 0   && cell.GetCellType() == CellTypeEnum.Vein) {
					cell.UpdateMetabolism(GlobalSettings.instance.quality.veinCellTickPeriod, worldTick);
				}
			}

			if (veinTick == 0) {
				veins.UpdateMetabolism(GlobalSettings.instance.quality.veinTickPeriod);
			}
		}
	}

	public void UpdateRotation() {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdatePhysics();
		}
	}
}