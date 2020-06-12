using UnityEngine;
using System.Collections.Generic;
using Boo.Lang.Runtime;

// The physical creature defined by all its cells
public class Phenotype : MonoBehaviour {
	// Effects
	public ParticlesCellScatter particlesCellScatterPrefab;
	public ParticlesCellBleed particlesCellBleedPrefab;

	public Transform cellsTransform;
	public Edges edges; //AKA Wings
	public Veins veins;

	public NerveArrows nerveArrows;

	// ... Signal ...
	private void UpdateBrain(Genotype genotype) {
		// area tables
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdateSensorAreaTablesPhenotype();
		}

		// clear
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].PreUpdateNervesPhenotype();
		}

		// clone genotype ==> phenotype
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].CloneNervesFromGenotypeToPhenotype(genotype.GetCellAtMapPosition(cellList[index].mapPosition), this);
		}

		// root them
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdateConnectionsNervesGenotypePhenotype(false); // <== false: don't add output nerves since they were all added when cloning
		}

		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdateRootable(genotype.GetCellAtMapPosition(cellList[index].mapPosition));
		}

	}

	public List<Nerve> GetAllExternalNerves() {
		List<Nerve> nervesExternal = new List<Nerve>();
		foreach (Cell cell in cellList) {
			nervesExternal.AddRange(cell.GetAllExternalNervesGenotypePhenotype());
		}
		return nervesExternal;
	}

	// ^ Signal ^

	[HideInInspector]
	private bool isCellPatternDiffererentFromGenomeDirty = true;
	public void MakeCellPaternDifferentFromGenotypeDirty () {
		isCellPatternDiffererentFromGenomeDirty = true; // Will regrow creature from genotype, as far as possible (might be stuff in the way)
	}

	[HideInInspector]
	public bool isInterCellDirty { get; private set; }
	public void MakeInterCellDirty() {
		isInterCellDirty = true;
	}


	public int visualTelepoke { get; private set; }
	public void Telepoke(Creature creature, Vector2 impulse) {
		visualTelepoke = GlobalSettings.instance.quality.portalTeleportTickPeriod;
		AddImpulse(impulse);

		// make crature slide as well
		foreach (Creature c in creature.creaturesInCluster) {
			c.phenotype.SetFrictionSliding();
			c.phenotype.kickTickStamp = World.instance.worldTicks;
			c.phenotype.slideDurationTicks = (ulong)GlobalSettings.instance.quality.portalTeleportTickPeriod;
		}
	}

	private void AddImpulse(Vector2 impulse) {
		foreach (Cell c in cellList) {
			c.theRigidBody.AddForce(impulse, ForceMode2D.Impulse);
		}
	}

	[HideInInspector]
	public Color individualColor = Color.black;
	[HideInInspector]
	public Color outlineClusterColor = Color.black;

	public void SetCellLablesEnabled(bool enabled) {
		foreach (Cell cell in cellList) {
			cell.SetLabelEnabled(enabled);
		}
	}

	public Bounds AABB {
		get {
			Bounds aabb = new Bounds(float.MaxValue, float.MinValue, float.MaxValue, float.MinValue);
			foreach (Cell cell in cellList) {
				aabb.xMin = Mathf.Min(cell.position.x, aabb.xMin);
				aabb.xMax = Mathf.Max(cell.position.x, aabb.xMax);
				aabb.yMin = Mathf.Min(cell.position.y, aabb.yMin);
				aabb.yMax = Mathf.Max(cell.position.y, aabb.yMax);
			}
			aabb.xMin -= 0.5f;
			aabb.xMax += 0.5f;
			aabb.yMin -= 0.5f;
			aabb.yMax += 0.5f;
			return aabb;
		}
	}

	[HideInInspector]
	public Cell originCell {
		get {
			return cellList[0];
		}
	}

	public bool hasOriginCell {
		get {
			return cellList.Count > 0;
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

	public float energyPerCell {
		get {
			if (cellCount > 0) {
				return energy / cellCount;
			} else {
				return 0;
			}
		}
	}

	public float energyFullness {
		get {
			return energyPerCell / GlobalSettings.instance.phenotype.cellMaxEnergy;
		}
	}

	// ---------------- EFFECT -----------------------------

	public float Effect(EffectMeassureEnum effectMeassure) {
		switch (effectMeassure) {
			case EffectMeassureEnum.Total:
				return Effect(true, true, true, true);
			case EffectMeassureEnum.Production:
				return Effect(true, false, false, false);
			case EffectMeassureEnum.External:
				return Effect(false, true, false, false);
			case EffectMeassureEnum.Flux:
				return Effect(false, false, true, true);

		}
		return -6666666;
	}

	public float Effect(bool production, bool external, bool fluxSelf, bool fluxAttached) {
		float effect = 0;
		foreach (Cell cell in cellList) {
			effect += cell.Effect(production, external, fluxSelf, fluxAttached);
		}
		return effect;
	}

	public float EffectDown(bool production, bool external, bool fluxSelf, bool fluxAttached) {
		float effect = 0;
		foreach (Cell cell in cellList) {
			effect += cell.EffectDown(production, external, fluxSelf, fluxAttached);
		}
		return effect;
	}

	public float EffectUp(bool production, bool fluxSelf, bool fluxAttached) {
		float effect = 0;
		foreach (Cell cell in cellList) {
			effect += cell.EffectUp(production, fluxSelf, fluxAttached);
		}
		return effect;
	}

	public float EffectPerCell(EffectMeassureEnum effectMeassure) {
		switch (effectMeassure) {
			case EffectMeassureEnum.Total:
				return Effect(true, true, false, true) / cellCount;
			case EffectMeassureEnum.Production:
				return Effect(true, false, false, false) / cellCount;
			case EffectMeassureEnum.External:
				return Effect(false, true, false, false) / cellCount;
			case EffectMeassureEnum.Flux:
				return Effect(false, false, false, true) / cellCount;
		}
		return -6666666;
	}

	public float EffectPerCell(bool production, bool external, bool fluxAttached) {
		return Effect(production, external, false, fluxAttached) / cellCount;
	}

	public float EffectDownPerCell(bool production, bool external, bool fluxAttached) {
		return EffectDown(production, external, false, fluxAttached) / cellCount;
	}

	public float EffectUpPerCell(bool production, bool fluxAttached) {
		return EffectUp(production, false, fluxAttached) / cellCount;
	}

	// --------------- ^ EFFECT ^ ------------------------

	public int VeinsConnectedToCellCount(Cell cell) {
		return veins.VeinsConnectedToCellCount(cell);
	}

	public int PlacentaVeinsConnectedToCellCount(Cell cell) {
		return veins.PlacentaVeinsConnectedToCellCount(cell);
	}

	public int NonPlacentaVeinsConnectedToCellCount(Cell cell) {
		return veins.NonPlacentaVeinsConnectedToCellCount(cell);
	}

	public bool hasPlacentaSpringsToMother {
		get {
			return originCell.hasPlacentaSprings;
		}
	}

	public float speed { get; private set; }

	public bool isAlive = true;
	public bool hasError = false; // just to be able to print error in history as an event

	[HideInInspector]
	public Dictionary<Cell, Vector2> detatchmentKick;

	[HideInInspector]
	public bool isGrabbed { get; private set; }

	[HideInInspector]
	public bool hasDirtyPosition = false;

	[HideInInspector]
	public List<Cell> cellList = new List<Cell>();

	private Vector2 velocity = new Vector2();
	private Vector2 spawnPosition;
	private float spawnHeading;
	public CellMap cellMap = new CellMap(); //Containing only built cells
	private bool isDirtyCollider = true;
	private bool areBudsDirty = true;

	public void UpdateArmour() {
		foreach (Cell cell in cellList) {
			cell.UpdateArmour();
		}
	}

	public void MakeBudsDirty() {
		areBudsDirty = true;
	}

	public void DisablePhysicsComponents() {
		foreach (Cell c in cellList) {
			c.DisablePhysicsComponents();
		}
	}

	public void EnablePhysicsComponents() {
		foreach (Cell c in cellList) {
			c.EnablePhysicsComponents();
		}
	}


	//Grown cells
	public int cellCount {
		get {
			return cellMap.cellCount;
		}
	}

	public int leafCellCount {
		get {
			int count = 0;
			foreach (Cell c in cellList) {
				if (c.GetCellType() == CellTypeEnum.Leaf) {
					count++;
				}
			}
			return count;
		}
	}

	public int GetCellOfTypeCount(CellTypeEnum type) {
		int count = 0;
		foreach (Cell c in cellList) {
			if (c.GetCellType() == type) {
				count++;
			}
		}
		return count;
	}

	//public int GetShellCellOfMaterialCount(ShellCell.ShellMaterial material) {
	//	int count = 0;
	//	foreach (Cell c in cellList) {
	//		if (c.GetCellType() == CellTypeEnum.Shell && (c as ShellCell).material == material) {
	//			count++;
	//		}
	//	}
	//	return count;
	//}

	public float longestEdge {
		get {
			return edges.longestEdge;
		}
	}

	public bool IsPartlyInside(Rect area) {
		float cellRadius = 0.5f;
		float top = area.y + area.height / 2f + cellRadius;
		float bottom = area.y - area.height / 2f - cellRadius;
		float left = area.x - area.width / 2f - cellRadius;
		float right = area.x + area.width / 2f + cellRadius;
		foreach (Cell cell in cellList) {
			if (cell.position.x < right && cell.position.x > left && cell.position.y < top && cell.position.y > bottom) {
				return true;
			}
		}
		return false;
	}

	public bool IsCompletelyInside(Rect area) {
		float cellRadius = 0.5f;
		float top = area.y + area.height / 2f - cellRadius;
		float bottom = area.y - area.height / 2f + cellRadius;
		float left = area.x - area.width / 2f + cellRadius;
		float right = area.x + area.width / 2f - cellRadius;
		foreach (Cell cell in cellList) {
			if (cell.position.x > right || cell.position.x < left || cell.position.y > top || cell.position.y < bottom) {
				return false;
			}
		}
		return true;
	}

	public void InitiateEmbryo(Creature creature, Vector2 position, float heading) {
		Setup(position, heading);
		NoGrowthReason reason;
		TryGrow(creature, false, true, 1, true, false, 0, true, true, out reason);

		isCellPatternDiffererentFromGenomeDirty = false;
	}

	public bool TryRegrowCellPattern(Creature creature, Vector2 position, float heading) {
		if (isCellPatternDiffererentFromGenomeDirty) {
			Debug.Log("Update Creature TryUpdateCellPattern");

			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Creature TryUpdateCellPattern");
			}
			Setup(position, heading);
			TryGrowFully(creature, true);

			MakeInterCellDirty();

			isCellPatternDiffererentFromGenomeDirty = false;
			return true;
		}
		return false;
	}

	//SpawnPosition is the position where the center of the origin cell will appear in word space
	private void Setup(Vector2 spawnPosition, float spawnHeading) {
		Clear();

		isAlive = true;
		hasError = false;
		this.spawnPosition = spawnPosition;
		this.spawnHeading = spawnHeading;

		individualColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

		Color outline = ColorScheme.instance.outlineCluster;
		float brightnessDiff = 0f;
		float redGreenDiff = 0f;
		outlineClusterColor = new Color(outline.r + brightnessDiff + redGreenDiff, outline.g + brightnessDiff - redGreenDiff, outline.b + brightnessDiff);
	}

	public int TryGrowFully(Creature creature, bool allowOvergrowAttached) {
		NoGrowthReason reason;
		return TryGrow(creature, false, allowOvergrowAttached, creature.genotype.geneCellCount, true, false, 0, true, true, out reason);
	}

	private int failedToGrowBuds = 0;

	public int TryGrow(Creature creature, bool highestPriorityFirst, bool allowOvergrow, int cellGrowTargetCount, bool buildWithoutCost, bool tryPlayFx, ulong worldTick, bool enableInstantRegrowth, bool growOtherIfBudsBlocked, out NoGrowthReason noGrowthReason) {
		noGrowthReason = new NoGrowthReason();
		Genotype genotype = creature.genotype;

		// If fully embryo grown => return
		if (creature.IsAttachedToMotherAlive() && cellCount == creature.CellCountAtCompleteness(genotype.originCell.gene.originEmbryoMaxSizeCompleteness)) {
			noGrowthReason.fullyGrownEmbryo = true;
			return 0;
		}

		// If fully detatched grown => return
		int growCellCount = 0;
		if (cellGrowTargetCount < 1 || this.cellCount >= genotype.geneCellCount) {
			noGrowthReason.fullyGrownDetatched = true;
			return 0;
		}

		// If first cell => grow origin
		if (cellList.Count == 0) {
			SpawnCell(creature, genotype.GetGeneAt(0), new Vector2i(), 0, AngleUtil.CardinalEnumToCardinalIndex(CardinalDirectionEnum.north), FlipSideEnum.BlackWhite, spawnPosition, true, 30f);
			if (originCell.GetCellType() == CellTypeEnum.Muscle) {
				((MuscleCell)originCell).UpdateMasterAxon(); // master axon will be me in this case
			}

			//EvoFixedUpdate(creature, 0f); //Do we really need to do this here?
			originCell.heading = spawnHeading;
			originCell.rotatedRoot.rotation = Quaternion.Euler(0f, 0f, originCell.heading); // Just updating graphics
			growCellCount++;
		}
		
		List<Cell> buildList = null;
		if (highestPriorityFirst) {
			buildList = genotype.geneCellListPrioritySorted; // sort gene cell list: low number (high prio) -> hight number (low number)
		} else {
			buildList = genotype.geneCellListIndexSorted;
		}

		float? highestPriority = null; // highestPriority = lowest number

		foreach (Cell buildGeneCell in buildList) {
			// If grown enough
			if (growCellCount >= cellGrowTargetCount) {
				break;
			}

			// Is buildGeneCell an unbuilt cell with built neighbour(s)?
			if (!IsCellBuiltForGeneCell(buildGeneCell) && IsCellBuiltAtNeighbourPosition(buildGeneCell.mapPosition)) {

				// Bail out on higher priority 'layer' if this (at lest one) cell had a lame excuse not to be build
				// Use this bail out when growing cells one by one, only
				if (highestPriorityFirst) {
					if (highestPriority == null) {
						highestPriority = buildGeneCell.buildPriority;
					}
					if (buildGeneCell.buildPriority > highestPriority) { // highestPriority = lowest number
						// We are here since NO 'cell to be' in previous priority 'layer' could be built

						if (noGrowthReason.notEnoughNeighbourEnergy || noGrowthReason.waitingForRespawnCooldown || noGrowthReason.tooFarAwayFromNeighbours) {
							// if the excuse, for at least one 'cell to be', was a lame one (lame = it should be able to build shortly) than don't try to build lower priority cells at all, but just wait for problem to be solved
							break; // don't grow anything this time
						}
						// there was a reasonable excuse for all cells is previos build 'layer' (reasonable = there was something in the way), so we coutin building in the next lower level 'layer'
						// but first wait a try a bit more before we give up and build
						failedToGrowBuds++;
						bool isNoNormalBlockingJustMotherOfChildBlocking = !noGrowthReason.spaceIsOccupied && (noGrowthReason.spaceIsOccupiedByMotherPlacenta || noGrowthReason.spaceIsOccupiedByChildOrigin);
						if (growOtherIfBudsBlocked || isNoNormalBlockingJustMotherOfChildBlocking || failedToGrowBuds > Mathf.FloorToInt(originCell.gene.originGrowPriorityCellPersistance / (GlobalSettings.instance.quality.growTickPeriod * Time.fixedDeltaTime))) {
							//failedToGrowBuds = 0;
							highestPriority = buildGeneCell.buildPriority; //step up highestPriority 'layer' a notch, and give all cells at this priority 'layer' a chance
						} else {
							break; // don't grow anything this time
						}
					}
				}

				//NOTE block below: We dont do need to make this check even if we check if position is occupied later
				// Once in a blue moon the creature is bent so that a neighbour spot is free in worldspace, though it is allready built ==> building it would make cell have 2 neighbours in the same direction ==> world of shit
				// This check is there to make sure that a neighbour is not built where (a mother placenta cell) OR (a child origin cell) is allready built, even if that place is unoccupied in world (due to creature bending severely leaving it free)

				// This one was written then removed (thinking that the world space check below was enough) then reintroduced 2019-08-30
				// When removed: 2 cells may be built in the same direction. This will cause trouble when generating edges (...maybee causing other problems too down the road)
				// In Cell.GetDirectionOfOwnNeighbourCell we would run into trouble as we try to find direction of a previous cell "P" from current cell "C" (checking neighbours around "C" for "P"). C is in my body and so is P.
				// Problem is that from "C" an old cell "O" has allready been build in the position in which "P" was built later. "O" wasnt found as occupied as it was the origin of creatures child ==> not presant in cellMap. 
				// "P" is not built on top but allowed to be be built on the side, though in the same neighbour direction, as "O". Building on the side was allowed due to severe bending of creature during telepoke.
				// So... in Cell.GetDirectionOfOwnNeighbourCell "C" would not find "P" as expected when looking for previous cell, but only "O" in its direction. We cant continue building our periphery loop :(
				// The solution then was to simply crash the creature, du to this error, worked but not pretty
				// This fix was tested on a case where a creature would go into edge error during telepoke. The fix would in this case cause the "C" to avoid building "P" in the neighbour spot of "O"

				// Is the cell map position is free to grow on (regarding children and mother)?
				//
				//if (!allowOvergrow && (IsMotherPlacentaLocation(creature, buildGeneCell.mapPosition) || IsChildOriginLocation(creature, buildGeneCell.mapPosition)) ) {
				if (! allowOvergrow) {
					if (IsMotherPlacentaLocation(creature, buildGeneCell.mapPosition)) {
						noGrowthReason.spaceIsOccupiedByMotherPlacenta = true;
						continue;
					} else if (IsChildOriginLocation(creature, buildGeneCell.mapPosition)) {
						noGrowthReason.spaceIsOccupiedByChildOrigin = true;
						continue;
					}
				}

				Vector3 averagePosition = Vector3.zero;
				int positionCount = 0;

				// Find neighbours around cell to build
				List<Cell> builderCells = new List<Cell>();
				for (int neighbourIndex = 0; neighbourIndex < 6; neighbourIndex++) {
					Cell gridNeighbourBuilder = cellMap.GetGridNeighbourCell(buildGeneCell.mapPosition, neighbourIndex);
					if (gridNeighbourBuilder != null) {
						builderCells.Add(gridNeighbourBuilder);
						int indexToMe = CardinaIndexToNeighbour(gridNeighbourBuilder, buildGeneCell);
						float meFromNeightbourBindPose = AngleUtil.CardinalIndexToAngle(indexToMe);
						float meFromNeighbour = (gridNeighbourBuilder.angleDiffFromBindpose + meFromNeightbourBindPose) % 360f;
						float distance = buildGeneCell.radius + gridNeighbourBuilder.radius;
						averagePosition += gridNeighbourBuilder.transform.position + new Vector3(distance * Mathf.Cos(meFromNeighbour * Mathf.Deg2Rad), distance * Mathf.Sin(meFromNeighbour * Mathf.Deg2Rad), 0f);
						positionCount++;
					}
				}

				// Has long enough time has passed since this cell was killed?
				if (!enableInstantRegrowth && cellMap.HasKilledTimeStamp(buildGeneCell.mapPosition)) {
					if (worldTick < cellMap.KilledTimeStamp(buildGeneCell.mapPosition) + GlobalSettings.instance.phenotype.cellRebuildCooldown / Time.fixedDeltaTime) {
						noGrowthReason.waitingForRespawnCooldown = true;
						continue;
					} else {
						cellMap.RemoveTimeStamp(buildGeneCell.mapPosition);
					}
				}

				// Is the cell map position is free to grow on (regarding any cell)?
				// TODO: Check obstacles in addition to cells! Otherwise cell can be built inside terrain ==> creatures shoots away
				Vector2 spawnPosition = averagePosition / positionCount;
				if (!allowOvergrow && !CanGrowAtPosition(spawnPosition, GlobalSettings.instance.phenotype.cellBuildNeededRadius)) {
					noGrowthReason.spaceIsOccupied = true;
					continue;
				}

				// Is the cell desired spawn position close enough to ALL neighbours growing it?
				foreach (Cell builder in builderCells) {
					float distance = Vector2.Distance(spawnPosition, builder.position);
					if (distance > GlobalSettings.instance.phenotype.cellBuildMaxDistance) {
						noGrowthReason.tooFarAwayFromNeighbours = true;
						break;
					}
				}
				if (noGrowthReason.tooFarAwayFromNeighbours) {
					continue;
				}

				// Can neighbours afford to build cell, when pitching in all together?
				float newCellEnergy = 30f;
				float buildBaseEnergy = GlobalSettings.instance.phenotype.cellBuildCost;
				if (!buildWithoutCost) {
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
								builder.energy -= (builder.energy - buildBaseEnergy) * giftFactor; // neighbour donating energy
							}
						}
						newCellEnergy = buildBaseEnergy * GlobalSettings.instance.phenotype.cellNewlyBuiltKeepFactor;
					} else {
						noGrowthReason.notEnoughNeighbourEnergy = true;
						continue;
					}
				}

				// Is the cell too far away from root? Does this ever happen???
				if (Vector2.Distance(spawnPosition, originCell.position) > Creature.maxRadiusCircle) {
					Debug.Log("Building too far far away!!!!");
				}

				// Spawn cell according to gene cells instructions!
				Cell newCell = SpawnCell(creature, buildGeneCell.gene, buildGeneCell.mapPosition, buildGeneCell.buildIndex, buildGeneCell.bindCardinalIndex, buildGeneCell.flipSide, spawnPosition, false, newCellEnergy);
				UpdateNeighbourReferencesIntraBody(); //We need to know our neighbours in order to update vectors correctly 
				newCell.UpdateNeighbourVectors(); //We need to update vectors to our neighbours, so that we can find our direction 
				newCell.UpdateHeading(); // otation is needed in order to place subsequent cells right
				newCell.UpdateFlipSide(); // Just graphics
				
				if (newCell.GetCellType() == CellTypeEnum.Muscle) {
					((MuscleCell)newCell).UpdateMasterAxon();
				}
				growCellCount++;

				// Play Fx
				if (tryPlayFx) {
					bool hasAudio; float audioVolume; bool hasParticles;
					SpatialUtil.FxGrade(newCell.position, false, out hasAudio, out audioVolume, out hasParticles);
					if (hasAudio) {
						Audio.instance.CellBirth(audioVolume * 0.25f);
					}
					if (hasParticles) {
						SpawnParticlesCellBirth(newCell);
					}
				}
			}
		} // ^ for each (geneCell in geneCellList) ^

		if (growCellCount > 0) {
			if (IsSliding(World.instance.worldTicks)) {
				SetFrictionSliding();
			} else {
				SetFrictionNormal();
			}

			failedToGrowBuds = 0; // Reset 'wait for a moment to grow if blocked' 

			PhenotypePanel.instance.MakeDirty();
			MakeDirtyCollider();
			MakeInterCellDirty();
		}
		return growCellCount;
	}

	public bool CanGrowMore(Creature creature) {
		Genotype genotype = creature.genotype;
		if (cellCount >= genotype.geneCellCount || creature.IsAttachedToMotherAlive() && cellCount >= creature.CellCountAtCompleteness(genotype.originCell.gene.originEmbryoMaxSizeCompleteness)) {
			// max size as embryo or detatched reached
			return false;
		}

		List<Cell> buildList = genotype.geneCellListIndexSorted;
		foreach (Cell buildGeneCell in buildList) {
			// Is buildGeneCell an unbuilt cell with built neighbour(s)?
			if (!IsCellBuiltForGeneCell(buildGeneCell) && IsCellBuiltAtNeighbourPosition(buildGeneCell.mapPosition)) {
				if (IsMotherPlacentaLocation(creature, buildGeneCell.mapPosition)) {
					// We are never going to grow on mother placenta
					continue;
				} else if (IsChildOriginLocation(creature, buildGeneCell.mapPosition)) {
					// We are here since an egg has been fertilized which is no the origin of our attached little embry origin. We are never going to grow on child embryo origin
					continue;
				}

				Vector3 averagePosition = Vector3.zero;
				int positionCount = 0;

				// Find neighbours around cell to build

				for (int neighbourIndex = 0; neighbourIndex < 6; neighbourIndex++) {
					Cell gridNeighbourBuilder = cellMap.GetGridNeighbourCell(buildGeneCell.mapPosition, neighbourIndex);
					if (gridNeighbourBuilder != null) {
						int indexToMe = CardinaIndexToNeighbour(gridNeighbourBuilder, buildGeneCell);
						float meFromNeightbourBindPose = AngleUtil.CardinalIndexToAngle(indexToMe);
						float meFromNeighbour = (gridNeighbourBuilder.angleDiffFromBindpose + meFromNeightbourBindPose) % 360f;
						float distance = buildGeneCell.radius + gridNeighbourBuilder.radius;
						averagePosition += gridNeighbourBuilder.transform.position + new Vector3(distance * Mathf.Cos(meFromNeighbour * Mathf.Deg2Rad), distance * Mathf.Sin(meFromNeighbour * Mathf.Deg2Rad), 0f);
						positionCount++;
					}
				}

				// Is the cell map position is free to grow on (regarding any cell)?
				// TODO: Check obstacles (AKA terrain) in addition to cells! Otherwise cell can be built inside terrain ==> creatures shoots away
				Vector2 spawnPosition = averagePosition / positionCount;
				if (CanGrowAtPosition(spawnPosition, GlobalSettings.instance.phenotype.cellBuildNeededRadius)) {
					// We are happy, since there was a place where neighbours could grow an ungrown cell
					return true;
				}
			}
		} // ^ for each (geneCell in geneCellList) ^

		// Sorry, we have been through all unbuilt cells and none of them could be grown
		return false;
	}

	public void MakeDirtyCollider() {
		isDirtyCollider = true;
	}

	public bool IsChildOriginLocation(Creature creature, Vector2i mapPosition) {
		//My(placenta) <====> Child(origin)
		foreach (Creature child in creature.GetChildrenAlive()) {
			if (creature.IsAttachedToChildAlive(child.id) && creature.ChildOriginMapPosition(child.id) == mapPosition) {
				return true;
			}
		}
		return false;
	}

	public bool IsMotherPlacentaLocation(Creature creature, Vector2i mapPosition) {
		//My(origin) <====> Mohther(placenta)
		if (creature.IsAttachedToMotherAlive()) {
			Creature creatureMother = creature.GetMotherAlive();
			foreach (Creature child in creatureMother.GetChildrenAlive()) {
				if (child.IsAttachedToMotherAlive() && child.id == creature.id) {
					//We are talking about mothers view of me
					for (int index = 0; index < creatureMother.genotype.geneCellListIndexSorted.Count; index++) {
						Cell placentaCell = creatureMother.genotype.geneCellListIndexSorted[index];

						for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
							Vector2i neighbourMapPosition = CellMap.GetGridNeighbourGridPosition(placentaCell.mapPosition, cardinalIndex);
							if (neighbourMapPosition == creature.GetMotherAlive().ChildOriginMapPosition(child.id)) {
								int childSoulOriginCardinalBindIndex = creature.GetMotherAlive().ChildOriginBindCardinalIndex(child.id);
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
		foreach (Creature creature in World.instance.life.creatures) {
			if (Vector2.SqrMagnitude(creature.GetOriginPosition(PhenoGenoEnum.Phenotype) - originCell.position ) < Mathf.Pow(Creature.maxRadiusCircle * 2f, 2f)) {
				cells.AddRange(creature.phenotype.cellList);
			}
		}

		foreach (Cell cell in cells) {
			if (GeometryUtil.AreCirclesIntersecting(cell.position, cell.radius, tryPosition, tryRadius)) {
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
	// Update Signal connections
	// 
	// TODO: At the moment we are updating whole body every time a new cell is grown. This is probably costy. Just update the cells affected by the change made (dirtymark per cell)
	public bool TryUpdateInterCells(Creature creature, string motherId) {
		if (isInterCellDirty) {
			Debug.Log("TryUpdateInterCells");

			UpdateNeighbourReferencesInterBody(creature);

			//Springs
			RepairBrokenSprings();
			UpdateSpringsConnections();
			UpdatePlacentaSpringConnections(creature);
			foreach (Creature child in creature.GetAttachedChildrenAlive()) {
				child.phenotype.UpdatePlacentaSpringConnections(child); // make child reconnect its placenta springs to me as my placenta cells to child might have changed
			}

			//Groups
			UpdateGroupsInterBody(motherId);

			//Wings
			try {
				edges.GenerateWings(creature, cellMap); // Wings are ONLY generated from here
			} catch (RuntimeException e) {
				Debug.Log("Error: " + e);
				hasError = true;
				isAlive = false;
				return false;
			}

			// Friction
			if (IsSliding(World.instance.worldTicks)) {
				SetFrictionSliding();
			} else {
				SetFrictionNormal();
			}

			//Veins
			veins.Generate(creature);

			UpdateSpringsFrequenze();
			UpdateSpringsBreakingForce();

			//test with no muscel collider
			EnableCollider(true);

			// Update buds in graphics
			MakeBudsDirty();

			// Signal
			UpdateBrain(creature.genotype);

			nerveArrows.GeneratePhenotype(this);

			//Armour
			UpdateArmour();

			isInterCellDirty = false;
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
		if (creature.IsAttachedToMotherAlive()) {
			Creature creatureMother = creature.GetMotherAlive();
			foreach (Creature child in creatureMother.GetChildrenAlive()) {
				if (child.IsAttachedToMotherAlive() && child.id == creature.id) {
					//We are talking about mothers view of me
					for (int index = 0; index < creatureMother.phenotype.cellList.Count; index++) {
						Cell placentaCell = creatureMother.phenotype.cellList[index];
						for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
							Vector2i neighbourMapPosition = CellMap.GetGridNeighbourGridPosition(placentaCell.mapPosition, cardinalIndex);
							if (neighbourMapPosition == creature.GetMotherAlive().ChildOriginMapPosition(child.id)) {
								// My placenta to childs origin
								placentaCell.SetNeighbourCell(cardinalIndex, originCell);
								//Debug.Log("Me(origin)" + creature.id + " <==neighbour== Mother(placenta)" + creatureMother.id);

								//childs origin to my placenta
								originCell.SetNeighbourCell(AngleUtil.CardinalIndexRawToSafe(cardinalIndex - child.GetMotherAlive().ChildOriginBindCardinalIndex(child.id) + 1 + 3), placentaCell);
								//Debug.Log("Me(origin)" + creature.id + " ==neighbour==> Mother(placenta)" + creatureMother.id);
							}
						}
					}
				}
			}
		}

		//My(placenta) <====> Child(origin)
		foreach (Creature child in creature.GetChildrenAlive()) {
			if (creature.IsAttachedToChildAlive(child.id)) {
				for (int index = 0; index < cellList.Count; index++) {
					Cell placentaCell = cellList[index];
					for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
						Vector2i neighbourMapPosition = CellMap.GetGridNeighbourGridPosition(placentaCell.mapPosition, cardinalIndex);
						if (neighbourMapPosition == creature.ChildOriginMapPosition(child.id)) {
							// My placenta to childs origin
							placentaCell.SetNeighbourCell(cardinalIndex, child.phenotype.originCell);
							//Debug.Log("Me: " + creature.id + ", my Child :" + child.id + " Me(placenta) ==neighbour==> Child(origin)");

							//childs origin to my placenta
							child.phenotype.originCell.SetNeighbourCell(AngleUtil.CardinalIndexRawToSafe(cardinalIndex - creature.ChildOriginBindCardinalIndex(child.id) + 1 + 3), placentaCell);
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

	private void RepairBrokenSprings() {
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			cell.RepairBrokenSprings();
		}
	}

	private void UpdateSpringsConnections() {
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			cell.UpdateSpringConnectionsIntra();
		}
	}

	//Called from mother upon me as child to reconnect placenta springs
	public void UpdatePlacentaSpringConnections(Creature creature) {
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			cell.UpdatePlacentaSpringConnections(creature);
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
		cellList.Sort((emp1, emp2) => emp1.buildIndex.CompareTo(emp2.buildIndex)); // lets stick to this... shrink by buildIndex (not buildPriority)
		for (int count = 0; count < cellCount; count++) {
			if (this.cellCount == 1) {
				return;
			}
			if (CellPanel.instance.selectedCell == cellList[cellList.Count - 1]) {
				CellPanel.instance.selectedCell = null;
			}

			KillCell(cellList[cellList.Count - 1], true, true, 0);
			shrinkCellCount++;
		}
		
	}

	public void ChangeEnergy(float amount) {
		for (int count = 0; count < cellCount; count++) {
			cellList[count].energy = Mathf.Clamp(cellList[count].energy + amount, -25f, GlobalSettings.instance.phenotype.cellMaxEnergy);
		}
	}

	public void SetEnergy(float amount) {
		for (int count = 0; count < cellCount; count++) {
			cellList[count].energy = Mathf.Clamp(amount, 0f, GlobalSettings.instance.phenotype.cellMaxEnergy);
		}
	}

	public void ChangeEnergyFull() {
		for (int count = 0; count < cellCount; count++) {
			cellList[count].energy = GlobalSettings.instance.phenotype.cellMaxEnergy;
		}
	}

	public void DistributeEnergy(float amount) {
		ChangeEnergy(amount / cellCount);
	}

	public bool IsOriginNeighbouringMothersPlacenta(Creature creature) {
		Debug.Assert(creature.HasMotherAlive());

		List<Cell> neighbours = originCell.GetNeighbourCells();
		foreach(Cell motherMai in neighbours) {
			//if (motherMai.creature == creature.motherSoul.creature) {
			if (motherMai.creature.id == creature.GetMotherAlive().id) {
				return true;
			}
		}
		return false;
	}

	public void OnNeighbourCellRecycled(Creature creature, Cell deletedCell, Cell disturbedCell) {
		disturbedCell.RemoveNeighbourCell(deletedCell);

		//Check if mothers placenta is gone
		if (creature.HasMotherAlive() && disturbedCell.isOrigin && !deletedCell.IsSameCreature(disturbedCell)) {
			if (!IsOriginNeighbouringMothersPlacenta(creature)) {
				DetatchFromMother(creature, false, false);
			}
		}

		MakeInterCellDirty();
	}

	public void SetAllCellStatesToDefault() {
		foreach (Cell c in cellList) {
			c.SetDefaultState();
		}
	}

	//This will make origin inaccessible
	
	public void KillAllCells(bool tryPlayFx) {
		List<Cell> allCells = new List<Cell>(cellList);

		for (int i = 0; i < allCells.Count; i++) {
			KillCell(allCells[i], false, tryPlayFx, 0);
		}
	}

	//This is the one and only final place where cell is removed
	// fixedTime = 0 ==> no mar will be set to when this cell can be regrown again
	public void KillCell(Cell deleteCell, bool deleteDebris, bool tryPlayFx, ulong worldTicks) {
		//remove all of cells bleed particles (if it has any)
		deleteCell.DetatchParticles();

		if (tryPlayFx) {
			bool hasAudio; float audioVolume; bool hasParticles;
			SpatialUtil.FxGrade(deleteCell.position, false, out hasAudio, out audioVolume, out hasParticles);

			if (hasAudio) {
				Audio.instance.CellDeath(audioVolume * 0.25f);
			}
			if (hasParticles) {
				SpawnParticlesCellScatter(deleteCell.position, deleteCell.GetColor());
				SpawnCellBloodFromNeighboursFx(deleteCell);
			}
		}

		//Clean up neighbours
		List<Cell> neightbourCells = deleteCell.GetNeighbourCells();
		foreach (Cell neighbourCell in neightbourCells) {
			deleteCell.RemoveNeighbourCell(neighbourCell);
			neighbourCell.creature.phenotype.OnNeighbourCellRecycled(neighbourCell.creature, deleteCell, neighbourCell);
		}

		if (deleteCell.isOrigin) {
			isAlive = false; //Hack, to avoid problems with origin missing everywhere, this creature will be safely recycled soon, by Life.UpdatePhysics() 
		}

		cellMap.RemoveCellAtGridPosition(deleteCell.mapPosition);
		cellList.Remove(deleteCell);
		if (CellPanel.instance.selectedCell == deleteCell) {
			CellPanel.instance.selectedCell = null;
		}

		// Clean up cell: Has vereybodey forgotten about me? Should be right, mate!
		Morphosis.instance.cellPool.Recycle(deleteCell);

		if (deleteDebris) {
			float deletedBranchEnergy = DeleteDebris();
			if (GlobalSettings.instance.phenotype.reclaimCutBranchEnergy) {
				DistributeEnergy(deletedBranchEnergy);
			}
		}

		//Mark cell as destroyed so we don't try to build it back emediatly
		if (worldTicks != 0 && !cellMap.HasKilledTimeStamp(deleteCell.mapPosition)) {
			cellMap.AddKilledTimeStamp(deleteCell.mapPosition, worldTicks);
		}

		SetFrictionNormal();

		PhenotypePanel.instance.MakeDirty(); // Update cell text with fewer cells

		CellPanel.instance.MakeDirty();
		MakeInterCellDirty();

		CreatureSelectionPanel.instance.MakeDirty();
		CreatureSelectionPanel.instance.UpdateSelectionCluster();
	}

	private float DeleteDebris() {
		List<Vector2i> keepers = cellMap.IsConnectedTo(originCell.mapPosition);
		List<Cell> debris = new List<Cell>();
		foreach (Cell cell in cellList) {
			if (keepers.Find(p => p == cell.mapPosition) == null) {
				debris.Add(cell);
			}
		}
		float deletedEnergy = 0f;
		foreach (Cell cell in debris) {
			deletedEnergy += cell.energy; 
			KillCell(cell, false, true, 0);
		}
		return deletedEnergy;
	}

	public void SpawnCellBloodFromNeighboursFx(Cell deleteCell) {
		for (int i = 0; i < 6; i++) {
			if (deleteCell.HasNeighbourCell(i)) {
				Cell neighbourCell = deleteCell.GetNeighbourCell(i);
				//neighbour looks back
				for (int neighbourCardinalIndex = 0; neighbourCardinalIndex < 6; neighbourCardinalIndex++) {
					if (neighbourCell.GetNeighbourCell(neighbourCardinalIndex) == deleteCell) {
						float angle = AngleUtil.CardinalIndexToAngle(neighbourCardinalIndex) + neighbourCell.angleDiffFromBindpose; //YEY :D
						neighbourCell.creature.phenotype.SpawnParticlesCellBleed(neighbourCell, angle);
					}
				}
			}
		}
	}

	public void SpawnParticlesCellBirth(Cell cell) {
		ParticlesCellBirth birth = ParticlePool.instance.Borrow(ParticleTypeEnum.cellBirth) as ParticlesCellBirth;
		birth.transform.position = cell.position;
		birth.Play(cell.GetColor());
		birth.transform.parent = cell.transform;

		cell.ShowOutline(false);
	}

	public void SpawnParticlesCellBleed(Cell cell, float heading) {
		ParticlesCellBleed bleed = ParticlePool.instance.Borrow(ParticleTypeEnum.cellBleed) as ParticlesCellBleed;
		bleed.transform.position = cell.position;
		bleed.transform.rotation = Quaternion.Euler(0f, 0f, heading);
		bleed.Play(Color.red);
		bleed.transform.parent = cell.transform;
	}

	public void SpawnParticlesCellScatter(Vector2 position, Color color) {
		//ParticlesCellScatter scatter = Instantiate(particlesCellScatterPrefab);
		ParticlesCellScatter scatter = ParticlePool.instance.Borrow(ParticleTypeEnum.cellScatter) as ParticlesCellScatter;

		scatter.transform.parent = Morphosis.instance.transform;
		scatter.transform.position = position;
		scatter.transform.rotation = Quaternion.identity;
		scatter.Play(color);

	}

	public void SpawnCellDetatchBloodEffect(Cell detatchCell) {
		for (int i = 0; i < 6; i++) {
			Cell neighbourCell = detatchCell.GetNeighbourCell(i);
			if (neighbourCell != null && neighbourCell.creature.id == detatchCell.creature.GetMotherAlive().id) {
				// mother neighbour looks back...
				for (int neighbourCardinalIndex = 0; neighbourCardinalIndex < 6; neighbourCardinalIndex++) {
					if (neighbourCell.GetNeighbourCell(neighbourCardinalIndex) == detatchCell) {
						//... at me
						float angle = AngleUtil.CardinalIndexToAngle(neighbourCardinalIndex) + neighbourCell.angleDiffFromBindpose; //YEY :D
						neighbourCell.creature.phenotype.SpawnParticlesCellBleed(neighbourCell, angle);
					}
				}

				//me
				float a = AngleUtil.CardinalIndexToAngle(i) + detatchCell.angleDiffFromBindpose;
				SpawnParticlesCellBleed(detatchCell, a);
			}
		}
	}

	public bool DetatchFromMother(Creature creature, bool applyKick, bool tryPlayFx) {
		if (creature.IsAttachedToMotherAlive()) {
			if (tryPlayFx) {
				Cell originCell = creature.phenotype.originCell;

				bool hasAudio; float audioVolume; bool hasParticles; bool hasMarker;
				SpatialUtil.FxGrade(originCell.position, false, out hasAudio, out audioVolume, out hasParticles, out hasMarker);

				if (hasAudio) {
					Audio.instance.CreatureDetatch(audioVolume);
				}
				if (hasParticles) {
					SpawnCellDetatchBloodEffect(originCell);
				}
				if (hasMarker) {
					float angle = originCell.heading - 90f;
					EventSymbolPlayer.instance.Play(EventSymbolEnum.CreatureDetatch, originCell.position, angle, SpatialUtil.MarkerScale());
				}
			}

			//Kick separation
			if (applyKick) {
				Creature mother = creature.GetMotherAlive();
				Cell childOrigin = creature.phenotype.originCell;
				int placentaCount = 0;
				for (int i = 0; i < 6; i++) {
					if (childOrigin.HasNeighbourCell(i)
						&& childOrigin.GetNeighbourCell(i).creature.id != creature.id) {
						childOrigin.GetNeighbourCell(i).isPlacenta = false;
						placentaCount++;
					}
				}
				if (placentaCount > 0) {
					mother.phenotype.detatchmentKick = new Dictionary<Cell, Vector2>();
					detatchmentKick = new Dictionary<Cell, Vector2>();
					Vector2 offspringForce = Vector2.zero;
					// The size of mother ==> child kick magnitude
					// The size of child  ==> mother kick magnitude
					float kickFactorMother = GlobalSettings.instance.phenotype.detatchmentKickAtCellCount.Evaluate(mother.phenotype.cellCount);
					float kickFactorChild = GlobalSettings.instance.phenotype.detatchmentKickAtCellCount.Evaluate(cellCount);

					// Impulses are negated, but may be of different magnitude, poor Newton!
					// That way big creatures slides roughly the length of small ones
					for (int i = 0; i < 6; i++) {
						if (childOrigin.HasNeighbourCell(i) && childOrigin.GetNeighbourCell(i).creature.id != creature.id) {
							Vector2 placentaForce = (childOrigin.GetNeighbourCell(i).position - childOrigin.position).normalized / placentaCount;
							mother.phenotype.detatchmentKick.Add(childOrigin.GetNeighbourCell(i), placentaForce * kickFactorMother); //kickFactorMother
							offspringForce -= placentaForce; //same magnitude but negated
						}
					}
					detatchmentKick.Add(childOrigin, offspringForce * kickFactorChild); //kickFactorChild
				}
			}

			//me
			veins.Clear(); // Is this really nessesary as we clear them uppon regeneration?
			creature.SetAttachedToMotherAlive(false);
			MakeInterCellDirty();
			originCell.effectFluxFromMotherAttached = 0f;

			//mother
			creature.GetMotherAlive().phenotype.veins.Clear(); // Is this really nessesary as we clear them uppon regeneration?
			creature.GetMotherAlive().phenotype.MakeInterCellDirty();
			foreach (Cell cell in creature.GetMotherAlive().phenotype.cellList) {
				cell.effectFluxToChildrenAttached = 0f;
			}

			// async muscle ticks (for the sake of veins, so that sync will not propagate throug generations)
			muscleAndFluxCellTick = Random.Range(0, GlobalSettings.instance.quality.muscleCellTickPeriod);

			CreatureSelectionPanel.instance.MakeDirty();
			CreatureSelectionPanel.instance.UpdateSelectionCluster();
			return true;
		}
		return false;
	}

	// Detatchment kick is applied to child (origin cell) as well as this phenotypes placenta cells
	private void ApplyDetatchKick() {
		foreach (Cell cell in detatchmentKick.Keys) {
			if (cell != null) {
				cell.theRigidBody.AddForce(detatchmentKick[cell], ForceMode2D.Impulse);
			}
		}
		detatchmentKick = null;
	}

	private ulong kickTickStamp = 0;
	private ulong slideDurationTicks = 10;

	public bool IsSliding(float worldTicks) {
		return kickTickStamp > 0 && worldTicks < kickTickStamp + slideDurationTicks;
	}

	public void SetFrictionNormal() {
		foreach (Cell cell in cellList) {
			cell.SetFrictionNormal();
		}
	}

	private void SetFrictionSliding() {
		foreach (Cell cell in cellList) {
			cell.SetFrictionSliding();
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
		cell.buildIndex = buildOrderIndex;
		cell.SetGene(gene);
		cell.bindCardinalIndex = bindCardinalIndex;
		cell.flipSide = flipSide;
		cell.creature = creature;
		cell.energy = spawnEnergy; 

		cell.SetFrictionNormal();

		return cell;
	}

	//private List<Cell> cellsToReActivate = new List<Cell>();
	private Cell InstantiateCell(CellTypeEnum type, Vector2i mapPosition) {
		Cell cell = null;

		cell = Morphosis.instance.cellPool.Borrow(type);

		//haxzor workaround, may caus phisics to explode
		//Cell is activated in Update instead of here
		//Updating it here will cause: Assertion failed: Invalid SortingGroup index set in Renderer
		//isDirty = true;
		//cellsToReActivate.Add(cell);

		cellMap.SetCell(mapPosition, cell);
		cellList.Add(cell);
		cell.transform.parent = cellsTransform.transform;

		return cell;
	}

	private void Clear() {
		//Cells
		KillAllCells(false); //Kill Cell (when origin) will set isAlive = false;
		cellList.Clear();
		cellMap.Clear();
		
		detatchmentKick = null;
		kickTickStamp = 0;

		//Perifery edges
		edges.OnRecycle();

		veins.OnRecycle();

		nerveArrows.OnRecycle();
	}

	public int GetCellCount() {
		return cellList.Count;
	}

	public void UpdateSpringsFrequenze() {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdateSpringFrequenzyAndDampingratio();
		}
	}

	private void UpdateSpringsBreakingForce() {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdateSpringsBreakingForce();
		}
	}

	private void UpdateMasterAxons() {
		for (int index = 0; index < cellList.Count; index++) {
			if (cellList[index].GetCellType() == CellTypeEnum.Muscle) {
				((MuscleCell)cellList[index]).UpdateMasterAxon();
			}
		}
	}

	public void ShowOutline(bool show) {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].ShowOutline(show);
		}
	}

	public void UpdateTriangleOutlineAndBones(Creature creature, bool isSelected, bool isClusterSelected) {
		for (int index = 0; index < cellList.Count; index++) {
			if (isSelected) {
				cellList[index].ShowOutline(true);
				cellList[index].SetOutlineColor(ColorScheme.instance.outlineSelected);
			} else if (isClusterSelected) {
				cellList[index].ShowOutline(true);
				cellList[index].SetOutlineColor(creature.phenotype.outlineClusterColor);
			} else {
				cellList[index].ShowOutline(false);
			}

			//fix properly
			cellList[index].ShowOnTop(false);
		}

		// update flip triangle
		for (int index = 0; index < cellList.Count; index++) {
			if (cellList[index].isOrigin) {
				cellList[index].ShowTriangle(true);
				if (!creature.HasMotherDeadOrAlive()) {
					if (!creature.HasChildrenDeadOrAlive()) {
						// No mother, No children
						cellList[index].SetTriangleColor(ColorScheme.instance.noRelativesArrow); // green
					} else {
						// No mother, Yes children
						cellList[index].SetTriangleColor(ColorScheme.instance.noMotherArrow);
					}
				} else if (creature.IsAttachedToMotherAlive()) {
					// Yes mother, attached
					cellList[index].SetTriangleColor(ColorScheme.instance.motherAttachedArrow);
				} else {
					// Yes mother, not attached
					cellList[index].SetTriangleColor(ColorScheme.instance.noMotherAttachedArrow);
				}
			} else {
				cellList[index].ShowTriangle(false);
			}
		}

		// skelleton bone
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].ShowSkelletonBone(!cellList[index].isOrigin && GlobalPanel.instance.graphicsSkelletonBoneToggle.isOn);
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

	public void Show(bool on) {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].Show(on);
		}
		nerveArrows.Show(on);
	}
	
	public void MoveOriginToOrigo() {
		Vector3 originCellPosition = originCell.position;
		foreach (Cell cell in cellList) {
			cell.transform.position -= originCellPosition;
		}
	}

	public void Halt() {
		foreach (Cell cell in cellList) {
			cell.theRigidBody.velocity = Vector2.zero;
		}
	}

	public void EnableCollider(bool collider) {
		foreach (Cell cell in cellList) {
			cell.GetComponent<Collider2D>().enabled = collider;
		}
	}

	public void Grab() {
		isGrabbed = true;
		foreach (Cell cell in cellList) {
			cell.theRigidBody.isKinematic = true;
			cell.theRigidBody.velocity = Vector2.zero;
			cell.GetComponent<Collider2D>().enabled = false;
		}
		MoveOriginToOrigo();
	}

	public void Release(Creature creature) {
		isGrabbed = false;
		foreach (Cell cell in cellList) {
			cell.theRigidBody.isKinematic = false;
			cell.GetComponent<Collider2D>().enabled = true;
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
			cell.transform.parent = cellsTransform.transform;
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

	public void MoveTo(Vector2 position) {
		Move(position - originCell.position);
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

	public Cell GetCellAtMapPosition(Vector2i mapPosition) {
		return cellMap.GetCell(mapPosition);
	}

	public Cell GetCellAtPosition(Vector2 position) {
		if (IsInsideBoundingCircle(position)) {
			foreach (Cell cell in cellList) {
				if (GeometryUtil.IsPointInsideCircle(position, cell.position, cell.radius)) {
					return cell;
				}
			}
		}
		return null;
	}

	public bool IsInsideBoundingCircle(Vector2 position) {
		if (hasOriginCell) {
			return Vector2.SqrMagnitude(originCell.position - position) < Mathf.Pow(Creature.maxRadiusCircle * 2f, 2f);
		}
		return false;
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
			isDirtyCollider = true;
		}
	}

	// only for graphics
	private void UpdatePriorityBuds(Creature creature) {
		float? highestPriorityValue = 0f; // highest priority = lowest number
		List<Cell> highestPriorityGeneCells = new List<Cell>(); // an unbuilt cell
		List<Cell> remainingGeneCells = new List<Cell>(creature.genotype.geneCellListPrioritySorted);

		bool hasHighestPriorityNormalCell = false;
		for (int safety = 0; safety < 100 && (highestPriorityValue != null && !hasHighestPriorityNormalCell && remainingGeneCells.Count > 0); safety++) {

			if (safety > 90) {
				Debug.Log("Ooooops!");
			}
			// one or more highest priority cells ==> highestPriorityGeneCells
			highestPriorityValue = null;
			foreach (Cell buildGeneCell in remainingGeneCells) {
				if (!IsCellBuiltForGeneCell(buildGeneCell) && IsCellBuiltAtNeighbourPosition(buildGeneCell.mapPosition)) {
					// Empty spot to build on with build neighbours
					if (highestPriorityValue == null || buildGeneCell.buildPriority <= highestPriorityValue) {
						highestPriorityGeneCells.Add(buildGeneCell);
						highestPriorityValue = buildGeneCell.buildPriority;
					} 
				}
			}

			hasHighestPriorityNormalCell = false;
			foreach (Cell buildGeneCell in highestPriorityGeneCells) {
				if (IsChildOriginLocation(creature, buildGeneCell.mapPosition) || IsMotherPlacentaLocation(creature, buildGeneCell.mapPosition)) {
					remainingGeneCells.Remove(buildGeneCell);
				} else {
					// A normal place to build a highest priority cell was found
					hasHighestPriorityNormalCell = true;
				}
			}
		}

		foreach (Cell buildGeneCell in creature.genotype.geneCellListPrioritySorted) {
			for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
				Cell builtNeighbourToUnbuilt = cellMap.GetCell(CellMap.GetGridNeighbourGridPosition(buildGeneCell.mapPosition, cardinalIndex));
				if (builtNeighbourToUnbuilt != null) {
					// for the neighbours of the unbuilt set (neighbour in unbuilts direction (that is +180 degrees)) isPriorityBudSatus
					CellNeighbour n = builtNeighbourToUnbuilt.GetNeighbour(AngleUtil.CardinalIndexRawToSafe(cardinalIndex + 3));
					if (n != null) {
						n.isPriorityBud = highestPriorityGeneCells.Contains(buildGeneCell);
						if (IsChildOriginLocation(creature, buildGeneCell.mapPosition) || IsMotherPlacentaLocation(creature, buildGeneCell.mapPosition)) {
							n.isPriorityBudOnAttachedCreature = true;
						} else {
							n.isPriorityBudOnAttachedCreature = false;
						}
					}
				}
			}
		}
	}

	// Update
	public void UpdateGraphics(Creature creature) {
		bool isSelected = CreatureSelectionPanel.instance.IsSelected(creature);

		//TODO: Update cells flip triangles here
		//Rotate cells


		// TODO: don't nag on every frame -> every creature -> every edge/vein -> that they have to be disabled (disable them once and call the creatures edges non dirty instead) 
		// Slightly faster framerate if disabling the 3 below
		edges.UpdateGraphics(GlobalPanel.instance.graphicsPeripheryToggle.isOn && creature.isInsideFrustum && !(PhenotypeGraphicsPanel.instance.isGraphicsCellEnergyRelated && isSelected) && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype);
		// TODO: let veins the objects that move energy from cell to cell stay here, but move the graphical representation out of here as we are only showing a couple of creatures at a time there can be a global vein renderer with a pool
		veins.UpdateGraphics(PhenotypeGraphicsPanel.instance.isGraphicsCellEnergyRelated && isSelected && creature.isInsideFrustum && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype);
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdateGraphics(isSelected);
		}

		nerveArrows.UpdateGraphics(isSelected);

		// Warning:  So we are more restrictive with these updates now, make sure colliders are updated as they should
		if (isDirtyCollider) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update Creature Phenotype");

			SetCollider(hasCollider);
			isDirtyCollider = false;
		}

		if (areBudsDirty) {
			// Buds
			UpdatePriorityBuds(creature);
			for (int index = 0; index < cellList.Count; index++) {
				cellList[index].UpdateBuds();
			}
			areBudsDirty = false;
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

	//time...
	//     metabolism...
	private int eggCellTick;
	private int fungalCellTick;
	private int jawCellTick;
	private int leafCellTick;
	public int muscleAndFluxCellTick;
	private int rootCellTick;
	private int shellCellTick;
	private int veinCellTick;
	//   ^ metabolism ^

	//     signal
	private int signalTick;
	//   ^ signal ^

	//time ^

	public void UpdatePhysics(Creature creature, ulong worldTick) {
		if (isGrabbed) {
			return;
		}
		UpdateWorkAndMetabolism(creature, worldTick);
		UpdateSignal(creature, worldTick);
	}

	private void UpdateSignal(Creature creature, ulong worldTick) {
		signalTick++;
		if (signalTick >= GlobalSettings.instance.quality.signalTickPeriod) {
			signalTick = 0;
		}

		for (int index = 0; index < cellList.Count; index++) {
			if (signalTick == 0) {
				cellList[index].FeedSignal();
			}
		}

		for (int index = 0; index < cellList.Count; index++) {
			if (signalTick == 0) {
				cellList[index].ComputeSignalOutputs(GlobalSettings.instance.quality.signalTickPeriod);
			}
		}
	}

	private void UpdateWorkAndMetabolism(Creature creature, ulong worldTick) {

		//time
		fungalCellTick++;
		if (fungalCellTick >= GlobalSettings.instance.quality.fungalCellTickPeriod) {
			fungalCellTick = 0;
		}

		jawCellTick++;
		if (jawCellTick >= GlobalSettings.instance.quality.jawCellTickPeriod) {
			jawCellTick = 0;
		}

		leafCellTick++;
		if (leafCellTick >= (int)GlobalSettings.instance.quality.leafCellTickPeriod) {
			leafCellTick = 0;
		}

		muscleAndFluxCellTick++;
		if (muscleAndFluxCellTick >= GlobalSettings.instance.quality.muscleCellTickPeriod) {
			muscleAndFluxCellTick = 0;
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

		//time ^
		TryInitiateDetatchemntSlide(creature, worldTick);
		TryFinalizeDetatchmentSlide(creature, worldTick);

		// Whole body


		// We are applying force only if mussceles are set to contract
		// Edges, let edge-wings apply proper forces to neighbouring cells, caused by muscle edges swiming through ether

		if (originCell.theRigidBody.IsAwake()) {
			if (PhenotypePhysicsPanel.instance.frictionWater.isOn) {
				edges.UpdatePhysics(creature, worldTick);
			}
			if (PhenotypePhysicsPanel.instance.hingeMomentum.isOn) {
				UpdateRotation();
			}
			

			Vector2 velocitySum = new Vector3();
			for (int index = 0; index < cellList.Count; index++) {
				velocitySum += cellList[index].velocity;
			}
			velocity = (cellList.Count > 0f) ? velocity = velocitySum / cellList.Count : new Vector2();
			speed = velocity.magnitude;
		} else {
			speed = 0f;
		}

		if (!IsSliding(worldTick)) {
			originCell.UpdatePulse(); // only origin
		}

		//Metabolism
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];

			if (originCell.originPulseTick == 0 && cell.GetCellType() == CellTypeEnum.Egg) {
				cell.UpdateCellWork(GlobalSettings.instance.quality.eggCellTickPeriod, worldTick);
			} else if (fungalCellTick == 0 && cell.GetCellType() == CellTypeEnum.Fungal) {
				cell.UpdateCellWork(GlobalSettings.instance.quality.fungalCellTickPeriod, worldTick);
			} else if (jawCellTick == 0 && cell.GetCellType() == CellTypeEnum.Jaw) {
				cell.UpdateCellWork(GlobalSettings.instance.quality.jawCellTickPeriod, worldTick);
			} else if (leafCellTick == 0 && cell.GetCellType() == CellTypeEnum.Leaf) {
				cell.UpdateCellWork((int)GlobalSettings.instance.quality.leafCellTickPeriod, worldTick);
			} else if (muscleAndFluxCellTick == 0 && cell.GetCellType() == CellTypeEnum.Muscle) {
				cell.UpdateCellWork(GlobalSettings.instance.quality.muscleCellTickPeriod, worldTick);
			} else if (rootCellTick == 0 && cell.GetCellType() == CellTypeEnum.Root) {
				cell.UpdateCellWork(GlobalSettings.instance.quality.rootCellTickPeriod, worldTick);
			} else if (shellCellTick == 0 && cell.GetCellType() == CellTypeEnum.Shell) {
				cell.UpdateCellWork(GlobalSettings.instance.quality.shellCellTickPeriod, worldTick);
			} else if (veinCellTick == 0 && cell.GetCellType() == CellTypeEnum.Vein) {
				cell.UpdateCellWork(GlobalSettings.instance.quality.veinCellTickPeriod, worldTick);
			}
		}

		if (muscleAndFluxCellTick == 0) {
			if (PhenotypePhysicsPanel.instance.flux.isOn) {
				// clear all flux effect
				if (PhenotypePhysicsPanel.instance.flux.isOn) {
					foreach (Cell c in cellList) {
						c.effectFluxFromSelf = 0f;
						c.effectFluxToSelf = 0f;
					}
				}
				veins.UpdateEffect(GlobalSettings.instance.quality.muscleCellTickPeriod);
				veins.UpdateCellsPlacentaEffects();
			}

			for (int index = 0; index < cellList.Count; index++) {
				Cell cell = cellList[index];
				cell.UpdateEnergy(GlobalSettings.instance.quality.muscleCellTickPeriod, true);
			}
		}

		//Visual
		if (visualTelepoke > 0) {
			visualTelepoke--;
		}

		if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.update) {
			UpdateDidUpdateThisFrame();
		}
	}

	public void UpdateSpringLengths() {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdateSpringLengths();
		}
	}

	public void UpdateRotation() {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].MakeAllNeighbourAnglesDirty();
		}

		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdateTwistAndTurn();
		}
	}

	public void UpdateDidUpdateThisFrame() {
		for (int index = 0; index < cellList.Count; index++) {
			cellList[index].UpdateDidUpdateThisFrame();
		}
	}

	private void TryInitiateDetatchemntSlide(Creature creature, ulong worldTick) {
		// Detatchment Kick
		if (detatchmentKick != null) {
			ApplyDetatchKick();
			ulong durationTicks = (ulong)(GlobalSettings.instance.phenotype.detatchSlideDurationTicks - GlobalSettings.instance.phenotype.detatchSlideDurationTicksRandomDiff + GlobalSettings.instance.phenotype.detatchSlideDurationTicksRandomDiff * Random.value + GlobalSettings.instance.phenotype.detatchSlideDurationTicksRandomDiff * Random.value);
			foreach (Creature c in creature.creaturesInCluster) {
				c.phenotype.SetFrictionSliding();
				c.phenotype.kickTickStamp = worldTick;
				c.phenotype.slideDurationTicks = durationTicks;
				c.phenotype.originCell.originPulseTick = originCell.originPulseTick;
			}
		}
	}

	private void TryFinalizeDetatchmentSlide(Creature creature, ulong worldTick) {
		if (kickTickStamp > 0 && worldTick > kickTickStamp + slideDurationTicks) {
			SetFrictionNormal(); //make slow
			kickTickStamp = 0;
		}
	}

	// Pooling
	public void OnRecycle() {
		Clear();
		isAlive = false;
	}
	// ^ pooling ^

	//... Load / Save...

	public void OnLoaded(Creature creature) {
		UpdateSpringLengths();

		// Update signals
		for (int index = 0; index < cellList.Count; index++) {
			if (signalTick == 0) {
				cellList[index].ComputeSignalOutputs(GlobalSettings.instance.quality.signalTickPeriod);
			}
		}

		//Turn arrrows right
		UpdateRotation();
		UpdateMasterAxons();

		MakeBudsDirty(); // so that the priority bud arrows graphics will be updated
	}

	// Save
	private PhenotypeData phenotypeData = new PhenotypeData();
	public PhenotypeData UpdateData() {
		phenotypeData.cellDataList.Clear();
		for (int index = 0; index < cellList.Count; index++) {
			Cell cell = cellList[index];
			phenotypeData.cellDataList.Add(cell.UpdateData());
		}
		phenotypeData.isCellPatternDirty = isCellPatternDiffererentFromGenomeDirty;

		phenotypeData.fungalCellTick = fungalCellTick;
		phenotypeData.jawCellTick = jawCellTick;
		phenotypeData.leafCellTick = leafCellTick;
		phenotypeData.muscleCellTick = muscleAndFluxCellTick;
		phenotypeData.rootCellTick = rootCellTick;
		phenotypeData.shellCellTick = shellCellTick;
		phenotypeData.veinCellTick = veinCellTick;

		phenotypeData.signalTick = signalTick;

		return phenotypeData;
	}

	// Load
	public void ApplyData(PhenotypeData phenotypeData, Creature creature) {
		Setup(phenotypeData.cellDataList[0].position, phenotypeData.cellDataList[0].heading);
		for (int index = 0; index < phenotypeData.cellDataList.Count; index++) {
			CellData cellData = phenotypeData.cellDataList[index];
			Cell cell = InstantiateCell(creature.genotype.genes[cellData.geneIndex].type, cellData.mapPosition);
			cell.ApplyData(cellData, creature);
		}
		isCellPatternDiffererentFromGenomeDirty = false; //This work is done
		MakeInterCellDirty(); //We need to connect mothers with children

		fungalCellTick = phenotypeData.fungalCellTick;
		jawCellTick = phenotypeData.jawCellTick;
		leafCellTick = phenotypeData.leafCellTick;
		muscleAndFluxCellTick = phenotypeData.muscleCellTick;
		rootCellTick = phenotypeData.rootCellTick;
		shellCellTick = phenotypeData.shellCellTick;
		veinCellTick = phenotypeData.veinCellTick;

		signalTick = phenotypeData.signalTick;

	}

	// ^ Load / Save ^
}