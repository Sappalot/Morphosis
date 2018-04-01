using System.Collections.Generic;
using UnityEngine;

// The container of genotype(genes) and phenotype(body)
// Holds information that does not fit into genes or body 
public class Creature : MonoBehaviour {
	[HideInInspector]
	public Soul soul; //Born soul-less the soul will find the creature 

	[HideInInspector]
	public string id;
	[HideInInspector]
	public string nickname;

	//time
	[HideInInspector]
	public ulong bornTick;
	[HideInInspector]
	public ulong deadTick;

	public ulong GetAgeTicks(ulong worldTicks) {
		return worldTicks - bornTick;
	}

	public float GetAge(ulong worldTicks) {
		return (worldTicks - bornTick) * Time.fixedDeltaTime;
	}

	//debug
	public SpriteRenderer creturePosition;
	public SpriteRenderer phenotypePosition;
	public SpriteRenderer phenotypeCellsPosition;
	public SpriteRenderer genotypePosition;
	public SpriteRenderer genotypeCellsPosition;

	//wing force
	[Range(0f, 1f)]
	public float wingDrag = 0.1f;

	[Range(0f, 1f)]
	public float f1 = 0.1f;

	[Range(0f, 5f)]
	public float wingF2 = 0.15f;

	[Range(0f, 40f)]
	public float wingPow = 2f;

	[Range(0f, 100f)]
	public float wingMax = 5f;

	//muscle
	[Range(0f, 0.5f)]
	public float muscleRadiusDiff = 0.1f;

	[Range(-1f, 1f)]
	public float muscleContractRetract = -0.14f;

	[Range(0f, 20f)]
	public float muscleSpeed = 6.55f;

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

	public int cellCount {
		get {
			return phenotype.cellCount;
		}
	}

	public int clusterCellCount {
		get {
			int cellCount = 0;
			foreach (Creature c in creaturesInCluster) {
				cellCount += c.cellCount;
			}
			return cellCount;
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
		if (creature.soul.motherSoulReference.isReferenceUpdated && creature.hasMother && !allreadyInList.Contains(creature.mother)) {
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

		if (from.hasMotherSoul && from == to && from.soul.isConnectedWithMotherSoul) {
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

	public void DetatchFromMother(bool applyKick, bool playEffects) {
		phenotype.DetatchFromMother(this, applyKick, playEffects);
	}

	public bool isAttachedToMother {
		get {
			Debug.Assert(soul != null, "isAttachedToMother? on soulless creature");
			return soul.isConnectedWithMotherSoul;
		}
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

	public bool IsPhenotypePartlyInside(Rect area) {
		return phenotype.IsPartlyInside(area);
	}

	public bool IsPhenotypeCompletelyInside(Rect area) {
		return phenotype.IsCompletelyInside(area);
	}

	public bool IsGenotypePartlyInside(Rect area) {
		return genotype.IsPartlyInside(area);
	}

	public void GenerateEmbryo(Gene[] genome, Vector3 position, float heading) {
		genotype.GenomeSet(genome);
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

	//OK
	public void KillCell(Cell cell, bool playEffects, ulong worldTicks) {
		phenotype.KillCell(cell, true, playEffects, worldTicks);
	}

	public void KillAllCells(bool effects) {
		phenotype.KillAllCells(effects);
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

		creatureData.bornTick = bornTick;
		creatureData.deadTick = deadTick;
		//todo: spieces

		creatureData.genotypeData = genotype.UpdateData();
		creatureData.phenotypeData = phenotype.UpdateData();

		creatureData.growTicks = growTicks;

		return creatureData;
	}

	// Load
	public void ApplyData(CreatureData creatureData) {
		//me
		nickname = creatureData.nickname;
		id = creatureData.id;

		bornTick = creatureData.bornTick;
		deadTick = creatureData.deadTick;

		genotype.ApplyData(creatureData.genotypeData);
		Vector2 position = creatureData.genotypeData.originPosition;
		float heading = creatureData.genotypeData.originHeading;
		genotype.UpdateGeneCellsFromGenome(this, position, heading); // Generating genotype here caused Unity freeze ;/

		growTicks = creatureData.growTicks;

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
				//phenotype.ShowShadow(false);
				phenotype.UpdateOutline(this, CreatureSelectionPanel.instance.IsSelected(this), CreatureSelectionPanel.instance.IsSelectedCluster(this));

				//Show selected or not
				phenotype.ShowCellsSelected(false);
				if (CreatureSelectionPanel.instance.soloSelected == this) {
					phenotype.ShowCellSelected(CellPanel.instance.selectedCell, true);
				}
			} else if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
				//Show selected or not
				//phenotype.ShowOutline(false);
				genotype.UpdateOutline(this, CreatureSelectionPanel.instance.IsSelected(this));
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
	//time ^

	private bool detatch = false;
	private int cantGrowMore = 0;

	//Returns true if creature grew
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

		//if (isAttachedToMother) {
		//	detatchTicks++;
		//	if (detatchTicks >= GlobalSettings.instance.quality.detatchTickPeriod) {
		//		detatchTicks = 0;
		//	}
		//}
		//time ^

		phenotype.UpdatePhysics(this, worldTicks);

		int didGrowCount = 0;
		if (GlobalPanel.instance.physicsUpdateMetabolism.isOn) {

			if (detatch) { // At this point we are sure that our origin cell had time to get to know its neighbours (including mother placenta)
				DetatchFromMother(true, true);
				PhenotypePanel.instance.MakeDirty();
				CellPanel.instance.MakeDirty();
				cantGrowMore = 0;

				detatch = false;
			}

			if (growTicks == 0) {
				NoGrowthReason reason;
				didGrowCount = phenotype.TryGrow(this, false, 1, false, true, worldTicks, false, out reason);
				if (didGrowCount > 0) {
					PhenotypePanel.instance.MakeDirty();
					CellPanel.instance.MakeDirty();
					cantGrowMore = 0;
				} else if (reason.fullyGrown) {
					cantGrowMore = int.MaxValue;
				} else if ((reason.roomBound && !reason.energyBound && !reason.respawnTimeBound)  ) {
					cantGrowMore++; // wait a while before giving up on finding a spot to grow another cell
				}

				// ☠ ꕕ Haha, make use of these
				//Debug.Log(" Id: " + id + ", CGM: " + cantGrowMore + ", roomBound: " + reason.roomBound + ", energyBound: " + reason.energyBound + ", respawnTimeBound: " + reason.respawnTimeBound + ", fullyGrown: " + reason.fullyGrown);

				if (isAttachedToMother) {
					if ((phenotype.originCell.originDetatchMode == ChildDetatchModeEnum.Size && phenotype.cellCount >= phenotype.originCell.originDetatchSizeThreshold) ||
						(phenotype.originCell.originDetatchMode == ChildDetatchModeEnum.Energy && phenotype.originCell.energy >= phenotype.originCell.originDetatchEnergyThreshold && cantGrowMore >= GlobalSettings.instance.phenotype.detatchCompletionPersistance)) {
						detatch = true; // Make sure we go one loop and reach UpdateStructure() before detatching from mother. Otherwise: if we just grew, originCell wouldn't know about placenta in mother and kick wouldn't be made properly
					}
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
				Life.instance.FertilizeCreature(fertilizeCell, true, worldTicks, false);
				PhenotypePanel.instance.MakeDirty();
				CellPanel.instance.MakeDirty();
			}
		}
	}

	public void OnReturnToPool() {
		//genotype.OnReturnToPool
		//phenotype.OnReturnToPool();

		//clear all information
	}
}