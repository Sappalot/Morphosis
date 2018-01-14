﻿using System.Collections.Generic;
using UnityEngine;

// The container of genotype(genes) and phenotype(body)
// Holds information that does not fit into genes or body 
public class Creature : MonoBehaviour {
	[HideInInspector]
	public Soul soul; //Born soul-less the soul will find the creature 

	public string id;
	public string nickname;

	//debug
	public SpriteRenderer creturePosition;
	public SpriteRenderer phenotypePosition;
	public SpriteRenderer phenotypeCellsPosition;
	public SpriteRenderer genotypePosition;
	public SpriteRenderer genotypeCellsPosition;

	//wing force
	[Range(0f, 1f)]
	public float wingDrag = 1f;

	[Range(0f, 1f)]
	public float f1 = 0.03f;

	[Range(0f, 5f)]
	public float wingF2 = 1f;

	[Range(0f, 40f)]
	public float wingPow = 10f;

	[Range(0f, 100f)]
	public float wingMax = 0.1f;

	//muscle
	[Range(0f, 0.5f)]
	public float muscleRadiusDiff = 0.2f;

	[Range(-1f, 1f)]
	public float muscleContractRetract = -0.5f;

	[Range(0f, 20f)]
	public float muscleSpeed = 1.55f;

	public Genotype genotype;
	public Phenotype phenotype;

	public float energy {
		get {
			return phenotype.energy;
		}
	}

	public bool hasSoul {
		get {
			return soul != null;
		}
	}

	public Soul motherSoul {
		get {
			return soul.motherSoul;
		}
	}

	public List<Creature> children {
		get {
			return soul.children;
		}
	}

	public List<Soul> childSouls {
		get {
			return soul.childSouls;
		}
	}

	public int childSoulCount {
		get {
			return soul.childSoulsCount;
		}
	}

	public bool hasMotherSoul {
		get {
			return soul.hasMotherSoul;
		}
	}

	public Creature mother {
		get {
			return soul.mother;
		}
	}

	public bool hasMother {
		get {
			return soul.hasMother;
		}
	}

	public bool hasChildSoul {
		get {
			return soul.hasChildSoul;
		}
	}

	public List<Creature> creaturesInCluster {
		get {
			List<Creature> allreadyInList = new List<Creature>();
			AddConnectedCreatures(this, null, allreadyInList);
			return allreadyInList;
		}
	}

	private List<Creature> AddConnectedCreatures(Creature creature, Creature callCreature, List<Creature> allreadyInList) {
		//me
		if (allreadyInList.Contains(creature) || (!IsConnected(creature, callCreature) && callCreature != null)) {
			return allreadyInList;
		} else {
			allreadyInList.Add(creature);
		}

		// mother
		if (creature.hasMother && !allreadyInList.Contains(creature.mother)) {
			AddConnectedCreatures(creature.mother, creature, allreadyInList);
		}

		//children
		foreach (Creature child in creature.children) {
			AddConnectedCreatures(child, creature, allreadyInList);
		}

		return allreadyInList;
	}

	public static bool IsConnected(Creature alpha, Creature beta) {
		return IsConnectedHelper(alpha, beta) || IsConnectedHelper(beta, alpha);
	}

	//PANIC!! clear up
	private static bool IsConnectedHelper(Creature from, Creature to) {
		if (from == null || from.soul == null && to == null) {
			return false;
		}

		if (from.hasMother && from.mother == to && from.soul.isConnectedWithMotherSoul) {
			return true;
		}

		foreach (Creature child in from.children) {
			if (child != null && from != null && from.soul != null && to != null && from.soul.areAllReferencesUpdated && from.soul.isConnectedWithChildSoul(to.id)) {
				return true;
			}
		}
		return false;
	}

	public Soul GetChildSoul(string id) {
		return soul.GetChildSoul(id);
	}

	public Creature GetChild(string id) {
		return soul.GetChild(id);
	}

	public bool HasChild(string id) {
		return soul.HasChild(id);
	}

	public void DetatchFromMother(bool playEffects = false) {
		phenotype.DetatchFromMother(this, playEffects);
	}

	// ^ Relatives ^

	private bool isDirtyGraphics = false;
	public void MakeDirtyGraphics() {
		isDirtyGraphics = true;
	}

	public int cellsCountFullyGrown {
		get {
			return genotype.geneCellCount;
		}
	}

	public int cellsCount {
		get {
			return phenotype.cellCount;
		}
	}

	public Vector2 GetOriginPosition(PhenoGenoEnum type) {
		if (type == PhenoGenoEnum.Phenotype) {
			return phenotype.originCell.position;
		} else {
			return genotype.originCell.position;
		}
	}

	public float GetOriginHeading(PhenoGenoEnum type) {
		if (type == PhenoGenoEnum.Phenotype) {
			return phenotype.originCell.heading;
		} else {
			return genotype.originCell.heading;
		}
	}

	public bool IsPhenotypeInside(Rect area) {
		return phenotype.IsInside(area);
	}

	public bool IsGenotypeInside(Rect area) {
		return genotype.IsInside(area);
	}

	public void GenerateEmbryo(Gene[] motherGenome, Vector3 position, float heading) {
		genotype.GenomeSet(motherGenome);
		phenotype.cellsDiffersFromGeneCells = genotype.UpdateGeneCellsFromGenome(this, position, heading);
		phenotype.InitiateEmbryo(this, position, heading);
		isDirtyGraphics = true;
		//EvoUpdate(); //To avoid occational flicker (shows genotype shortly)
	}

	public void GenerateSimple(Vector3 position, float heading) {
		genotype.GenomeEmpty();
		UpdateCellsAndGeneCells(position, heading);
	}

	public void GenerateFreak(Vector3 position, float heading) {
		genotype.GenomeEmpty();
		genotype.GenomeScramble();
		UpdateCellsAndGeneCells(position, heading);
	}

	public void GenerateMergling(List<Gene[]> genomes, Vector3 position, float heading) {
		genotype.GenomeSet(GenotypeUtil.CombineGenomeFine(genomes));
		UpdateCellsAndGeneCells(position, heading);
	}

	public void GenerateJellyfish(Vector2 position, float heading) {
		genotype.GenerateGenomeJellyfish();
		UpdateCellsAndGeneCells(position, heading);
	}

	private void UpdateCellsAndGeneCells(Vector2 position, float heading) {
		//we need to update them allready in order to have originCell. Origin cell is needed for position and heading when updating
		phenotype.cellsDiffersFromGeneCells = genotype.UpdateGeneCellsFromGenome(this, position, heading); // Generating genotype here caused Unity freeze ;/
		phenotype.connectionsDiffersFromCells = phenotype.UpdateCellsFromGeneCells(this, position, heading);
		isDirtyGraphics = true;
		UpdateGraphics();
	}

	// Apply on genotype ==> Phenotype
	public void Clear() {
		genotype.GenomeEmpty();
	}

	public void MutateAbsolute(float strength) {
		RestoreState();
		genotype.GenomeMutate(strength);
	}

	public void MutateCummulative(float strength) {
		genotype.GenomeMutate(strength);
	}

	public void Scramble() {
		genotype.GenomeScramble();
	}

	// Apply on Phenotype
	public void TryGrow(bool forceGrow, int cellCount, bool playEffects) {
		NoGrowthReason reason;
		phenotype.TryGrow(this, forceGrow, cellCount, true, playEffects, 0, true, out reason);
		isDirtyGraphics = true;
	}

	public void TryShrink(int cellCount = 1) {
		phenotype.TryShrink(cellCount);
		isDirtyGraphics = true;
	}

	public void ChangeEnergy(float amount) {
		phenotype.ChangeEnergy(amount);
		isDirtyGraphics = true;
	}

	public void KillCell(Cell cell, bool playEffects, ulong worldTicks) {
		phenotype.KillCell(cell, true, playEffects, worldTicks);
	}

	public void KillAllCells() {
		phenotype.KillAllCells();
	}

	public void ShowCellSelected(Cell cell, bool on) {
		phenotype.ShowCellSelected(cell, on);
	}

	public Cell GetCellAt(Vector2 position) {
		return phenotype.GetCellAt(position);
	}

	public void Grab(PhenoGenoEnum type) {
		if (type == PhenoGenoEnum.Phenotype) {
			Vector2 originCellPosition = phenotype.originCell.position;

			phenotype.Grab();
			hasPhenotypeCollider = false;

			transform.parent = null;
			transform.position = originCellPosition;
			transform.parent = Life.instance.transform;
		} else if (type == PhenoGenoEnum.Genotype) {
			phenotype.hasDirtyPosition = true;

			Vector2 originCellPosition = genotype.originCell.position;

			genotype.Grab();
			hasGenotypeCollider = false;

			phenotype.Halt();

			transform.parent = null;
			transform.position = originCellPosition;
			transform.parent = Life.instance.transform;
		}
		isDirtyGraphics = true;
	}

	public void Release(PhenoGenoEnum type) {
		if (type == PhenoGenoEnum.Phenotype) {
			phenotype.Release(this);
			genotype.MoveToPhenotype(this);
			hasPhenotypeCollider = true;
		} else if (type == PhenoGenoEnum.Genotype) {
			genotype.Release(this);
			hasGenotypeCollider = true;
		}
		isDirtyGraphics = true;
	}

	public void ShowMarkers(bool show) {
		creturePosition.enabled = show;
		phenotypePosition.enabled = show;
		phenotypeCellsPosition.enabled = show;
		genotypePosition.enabled = show;
		genotypeCellsPosition.enabled = show;
	}

	public bool hasPhenotypeCollider {
		get {
			return phenotype.hasCollider;
		}
		set {
			phenotype.hasCollider = value;
		}
	}

	public bool hasGenotypeCollider {
		get {
			return genotype.hasCollider;
		}
		set {
			genotype.hasCollider = value; // Used for picking only
		}
	}

	// Update according to type
	public void BringCurrentGenoPhenoPositionAndRotationToOther() {
		if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype) {
			phenotype.MoveToGenotype(this);
		} else if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
			genotype.MoveToPhenotype(this);
		}
	}

	private void BringOtherGenoPhenoPositionAndRotationToCurrent() {
		if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype) {
			genotype.MoveToPhenotype(this);
		} else if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
			phenotype.MoveToGenotype(this);
		}
	}

	private void ShowCurrentGenoPhenoAndHideOther() {
		phenotype.Show(CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype); //Don't use SetActive() since it clears rigigdBody velocity
		genotype.gameObject.SetActive(CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype);
	}

	//Don't enable collider if grabbed, thou
	private void EnableCurrentGenoPhenoColliderAndDisableOther() {
		phenotype.hasCollider = CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype && !phenotype.isGrabbed;
		genotype.hasCollider = CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype && !genotype.isGrabbed;
	}

	public void FetchSoul() {
		if (!hasSoul) {
			if (Life.instance.HasSoul(id)) {
				soul = Life.instance.GetSoul(id);
			} else {
				Debug.LogError("Creature could not find is soul!!");
				return;
			}
		}
	}

	// Load / Save

	private CreatureData creatureData = new CreatureData();

	public void StoreState() {
		BringOtherGenoPhenoPositionAndRotationToCurrent();
		UpdateData();
	}

	public void RestoreState() {
		genotype.ApplyData(creatureData.genotypeData);
		isDirtyGraphics = true;
	}

	//Everything is deep cloned even the id. Change this not to have trouble
	public void Clone(Creature original) {
		ApplyData(original.UpdateData());
	}

	// Save
	public CreatureData UpdateData() {
		BringOtherGenoPhenoPositionAndRotationToCurrent(); //Do we really need this one??

		//me
		creatureData.id = id;
		creatureData.nickname = nickname;
		//todo: spieces

		creatureData.genotypeData = genotype.UpdateData();
		creatureData.phenotypeData = phenotype.UpdateData();

		creatureData.growTicks = growTicks;
		creatureData.detatchTicks = detatchTicks;

		return creatureData;
	}

	// Load
	public void ApplyData(CreatureData creatureData) {
		//me
		nickname = creatureData.nickname;
		id = creatureData.id;

		genotype.ApplyData(creatureData.genotypeData);
		Vector2 position = creatureData.genotypeData.originPosition;
		float heading = creatureData.genotypeData.originHeading;
		genotype.UpdateGeneCellsFromGenome(this, position, heading); // Generating genotype here caused Unity freeze ;/

		growTicks = creatureData.growTicks;
		detatchTicks = creatureData.detatchTicks;

		phenotype.ApplyData(creatureData.phenotypeData, this);
	}

	// ^ Load / Save ^

	// Update
	public void UpdateGraphics() {
		if (!hasSoul) {
			return;
		}

		genotype.UpdateGraphics();
		phenotype.UpdateGraphics(this);

		if (isDirtyGraphics) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update Creature (due to user input)");

			ShowCurrentGenoPhenoAndHideOther();
			EnableCurrentGenoPhenoColliderAndDisableOther();

			if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype) {

				// Update selection
				phenotype.ShowSelectedCreature(CreatureSelectionPanel.instance.IsSelected(this));

				phenotype.ShowShadow(false);

				//Show selected or not
				phenotype.ShowCellsSelected(false);
				if (CreatureSelectionPanel.instance.soloSelected == this) {
					phenotype.ShowCellSelected(CellPanel.instance.selectedCell, true);
				}

			} else if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
				// Update selection
				genotype.ShowCreatureSelected(CreatureSelectionPanel.instance.IsSelected(this));

				//Show selected or not
				genotype.ShowGeneCellsSelected(false);
				if (CreatureSelectionPanel.instance.soloSelected == this) {
					genotype.ShowGeneCellsSelectedWithGene(GenePanel.instance.selectedGene, true);
				}

				genotype.UpdateFlipSides();
			}
			isDirtyGraphics = false;
		}
	}

	public void UpdateStructure() {
		if (genotype.UpdateGeneCellsFromGenome(this, genotype.originCell.position, genotype.originCell.heading)) {
			phenotype.cellsDiffersFromGeneCells = true;
			isDirtyGraphics = true;
		}

		if (phenotype.UpdateCellsFromGeneCells(this, genotype.originCell.position, genotype.originCell.heading)) {
			phenotype.connectionsDiffersFromCells = true;
			isDirtyGraphics = true;
		}

		if (phenotype.UpdateConnectionsFromCellsBody(this, soul.motherSoulReference.id) ) {
			isDirtyGraphics = true;
		}
	}

	public bool UpdateKillWeakCells(ulong worldTicks) {
		return phenotype.UpdateKillWeakCells(worldTicks);
	}

	//time
	private int growTicks;
	private int detatchTicks;

	//time ^

	private int cantGrowMore = 0;

	public void UpdatePhysics(ulong worldTicks) {
		if (!hasSoul) {
			return;
		}

		//time
		growTicks++;
		if (growTicks >= GlobalSettings.instance.quality.growTickPeriod) {
			growTicks = 0;
		}

		//fertilizeTicks++;
		//if (fertilizeTicks >= fertilizeTickPeriod) {
		//	fertilizeTicks = 0;
		//}

		detatchTicks++;
		if (detatchTicks >= GlobalSettings.instance.quality.detatchTickPeriod) {
			detatchTicks = 0;
		}
		//time ^

		phenotype.UpdatePhysics(this, worldTicks);

		if (GlobalPanel.instance.effectsUpdateMetabolism.isOn) {

			
			if (growTicks == 0) {
				NoGrowthReason reason;
				int growCount = phenotype.TryGrow(this, false, 1, false, true, worldTicks, false, out reason);
				if (growCount > 0) {
					PhenotypePanel.instance.MakeDirty();
					CellPanel.instance.MakeDirty();
					cantGrowMore = 0;
				} else if (reason.fullyGrown) {
					cantGrowMore = int.MaxValue;
				} else if ((reason.roomBound && !reason.energyBound && !reason.respawnTimeBound)  ) {
					cantGrowMore++; // wait a while before giving up on finding a spot to grow another cell
				}

				// ☠ ꕕ Haha, make use of these
				Debug.Log(" Id: " + id + ", roomBound: " + reason.roomBound + "CGM: " + cantGrowMore + ", energyBound: " + reason.energyBound + ", respawnTimeBound: " + reason.respawnTimeBound + ", fullyGrown: " + reason.fullyGrown);
			}

			if (detatchTicks == 0) {
				if ((phenotype.originCell.originDetatchMode == ChildDetatchModeEnum.Size   && phenotype.cellCount         >= phenotype.originCell.originDetatchSizeThreshold) ||
					(phenotype.originCell.originDetatchMode == ChildDetatchModeEnum.Energy && phenotype.originCell.energy >= phenotype.originCell.originDetatchEnergyThreshold && cantGrowMore >= GlobalSettings.instance.phenotype.DetatchCompletionPersistance)) {

					DetatchFromMother(true);
					PhenotypePanel.instance.MakeDirty();
					CellPanel.instance.MakeDirty();

					cantGrowMore = 0;
				}
			}

			//if (fertilizeTicks == 0) {
			Cell fertilizeCell = null;
			foreach (Cell c in phenotype.cellList) {
				if (c is EggCell) {
					EggCell eggCell = c as EggCell;
					if (eggCell.shouldFertilize == 0) {
						fertilizeCell = eggCell;
						eggCell.shouldFertilize = -1;
						break;
					} else if (eggCell.shouldFertilize > 0) {
						eggCell.shouldFertilize--;
					}
				}
			}
			if (fertilizeCell != null) {
				Life.instance.FertilizeCreature(fertilizeCell, true, worldTicks);
				PhenotypePanel.instance.MakeDirty();
				CellPanel.instance.MakeDirty();
			}
			//}
		}
	}
}