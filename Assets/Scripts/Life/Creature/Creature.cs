using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// The container of genotype(genes) and phenotype(body)
// Holds information that does not fit into genes or body 
public class Creature : MonoBehaviour {
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

	public SpriteRenderer creturePosition;
	public SpriteRenderer phenotypePosition;
	public SpriteRenderer phenotypeCellsPosition;
	public SpriteRenderer genotypePosition;
	public SpriteRenderer genotypeCellsPosition;

	[HideInInspector]
	public string id;
	[HideInInspector]
	public string nickname;
	[HideInInspector]
	public CreatureCreationEnum creation = CreatureCreationEnum.Unset;
	[HideInInspector]
	public int generation = 1;

	// Relations
	//Dont keep any direct reference to avoid keeping references to killed relatives (which are then recycled)
	private Mother mother;
	private Dictionary<string, Child> children = new Dictionary<string, Child>(); // each entry contains both string and child (child is allways a reference ID, which may or may not be found in life)

	//time
	private int growTicks;
	//time ^

	private bool detatch = false;
	private int cantGrowMore = 0;

	public void ClearMotherAndChildrenReferences() {
		ClearMotherReference();
		ClearChildrenReferences();
	}

	public void ClearMotherReference() {
		mother = null;
	}

	public void ClearChildrenReferences() {
		children.Clear();
	}

	// Relations -> mother	

	//do this when creature is born, copied or loaded
	public void SetMotherReference(string motherId) {
		mother = new Mother(motherId);
	}

	public bool HasMotherAlive() {
		Debug.Assert(GetRelationToMother() != FamilyMemberState.Error || GetRelationToMother() != FamilyMemberState.Unexisting);
		return GetRelationToMother() == FamilyMemberState.AliveAndAttached || GetRelationToMother() == FamilyMemberState.AliveAndDetatched;
	}

	public Creature GetMotherAlive() {
		if (!HasMotherAlive()) {
			return null;
		}
		if (World.instance.life.HasCreature(mother.id)) {
			return World.instance.life.GetCreature(mother.id);
		}
		return null;
	}

	public bool IsAttachedToMotherAlive() {
		return GetRelationToMother() == FamilyMemberState.AliveAndAttached;
	}

	public bool IsDetatchedWithMotherAlive() {
		return GetRelationToMother() == FamilyMemberState.AliveAndDetatched;
	}

	public void SetAttachedToChild(string childId, bool attached) {
		if (!children.ContainsKey(childId)) {
			return;
		}
		children[childId].isConnectedToMother = attached;
	}

	public bool HasMotherDeadOrAlive() {
		return GetRelationToMother() == FamilyMemberState.AliveAndAttached || GetRelationToMother() == FamilyMemberState.AliveAndDetatched || GetRelationToMother() == FamilyMemberState.Dead;
	}

	public string GetMotherIdDeadOrAlive() {
		if (mother != null) {
			return mother.id;
		}
		return null;
	}

	private FamilyMemberState GetRelationToMother() {
		if (mother == null) {
			return FamilyMemberState.Unexisting;
		} else {
			Creature myMother = World.instance.life.GetCreature(mother.id);
			if (myMother != null) {
				if (myMother.IsAttachedToChildAlive(id)) {
					return FamilyMemberState.AliveAndAttached;
				} else {
					return FamilyMemberState.AliveAndDetatched;
				}
			} else {
				return FamilyMemberState.Dead;
			}
		}
	}
	// ^ Relations -> mother ^
	// Relations -> children
	public void AddChildReference(ChildData childData) {
		children.Add(childData.id, new Child(childData.id, childData.isConnectedToMother, childData.originMapPosition, childData.originBindCardinalIndex));
	}

	public bool HasChildrenAlive() {
		return ChildrenCountAlive() > 0;
	}

	public Creature GetChildAlive(string id) {
		if (!children.ContainsKey(id)) {
			return null;
		}
		if (World.instance.life.HasCreature(id)) {
			return World.instance.life.GetCreature(id); // Was mother.id ??
		}
		return null;
	}

	public bool IsAttachedToChildAlive(string id) {
		if (!children.ContainsKey(id)) {
			return false;
		}
		return children[id].isConnectedToMother;
	}

	// Thll mother to see me as connected
	public void SetAttachedToMotherAlive(bool attached) {
		Creature m = GetMotherAlive();
		if (m != null) {
			m.SetAttachedToChild(id, attached);
		}
	}

	public Vector2i ChildOriginMapPosition(string id) {
		return children[id].originMapPosition;
	}

	public int ChildOriginBindCardinalIndex(string id) {
		return children[id].originBindCardinalIndex;
	}

	//if we have no children we leave a list full of null
	public List<Creature> GetChildrenAlive() {
		List<Creature> childrenAlive = new List<Creature>();
		foreach (string id in children.Keys) {
			Creature found = World.instance.life.GetCreature(id);
			if (found != null) {
				childrenAlive.Add(found);
			}
		}
		return childrenAlive;
	}

	public List<Creature> GetAttachedChildrenAlive() {
		List<Creature> childrenAliveAndAttached = new List<Creature>();
		foreach (string id in children.Keys) {
			Creature found = World.instance.life.GetCreature(id);
			if (found != null && GetRelationToChild(found.id) == FamilyMemberState.AliveAndAttached) {
				childrenAliveAndAttached.Add(found);
			}
		}
		return childrenAliveAndAttached;
	}

	public int GetAttachedChildrenAliveCount() {
		return GetAttachedChildrenAlive().Count();
	}

	public List<Creature> GetDetatchedChildrenAlive() {
		List<Creature> childrenAliveAndDetatched = new List<Creature>();
		foreach (string id in children.Keys) {
			Creature found = World.instance.life.GetCreature(id);
			if (found != null && GetRelationToChild(found.id) == FamilyMemberState.AliveAndDetatched) {
				childrenAliveAndDetatched.Add(found);
			}
		}
		return childrenAliveAndDetatched;
	}

	public List<string> GetChildrenIdDeadOrAlive() {
		List<string> ids = new List<string>();
		foreach (string id in children.Keys) {
			ids.Add(id);
		}
		return ids;
	}

	public int ChildrenCountAlive() {
		int count = 0;
		foreach (Child c in children.Values) {
			if (GetRelationToChild(c.id) == FamilyMemberState.AliveAndAttached || GetRelationToChild(c.id) == FamilyMemberState.AliveAndDetatched) {
				count++;
			}
		}
		return count;
	}

	public bool HasChildrenDeadOrAlive() {
		return children.Count > 0;
	}

	public int ChildrenCountDeadOrAlive() {
		return children.Count;
	}

	public int ChildrenCountDead() {
		return ChildrenCountDeadOrAlive() - ChildrenCountAlive();
	}

	private FamilyMemberState GetRelationToChild(string id) {
		if (children.ContainsKey(id)) {
			if (World.instance.life.HasCreature(id)) {
				if (children[id].isConnectedToMother) {
					return FamilyMemberState.AliveAndAttached;
				} else {
					return FamilyMemberState.AliveAndDetatched;
				}
			} else {
				return FamilyMemberState.Dead;
			}
		} else {
			return FamilyMemberState.Unexisting;
		}
	}

	private void AddChild(Child child) {
		children.Add(child.id, child);
	}

	//  ^ Relations - children ^
	//  ^ Relations ^


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

	public float energy {
		get {
			return phenotype.energy;
		}
	}

	//Dead or alive counts
	public bool allowedToChangeGenome {
		get {
			return !(HasMotherDeadOrAlive() || HasChildrenDeadOrAlive());
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
		if (creature.HasMotherAlive() && !allreadyInList.Contains(creature.GetMotherAlive())) {
			AddConnectedCreatures(creature.GetMotherAlive(), creature, allreadyInList);
		}

		//children
		foreach (Creature child in creature.GetChildrenAlive()) {
			AddConnectedCreatures(child, creature, allreadyInList);
		}

		return allreadyInList;
	}

	public static bool IsConnected(Creature alpha, Creature beta) {
		return IsConnectedHelper(alpha, beta) || IsConnectedHelper(beta, alpha);
	}

	//PANIC!! clear up
	private static bool IsConnectedHelper(Creature from, Creature to) {
		if (from == null || to == null) { //used to be &&
			return false;
		}

		if (from.HasMotherAlive() && from == to && from.IsAttachedToMotherAlive()) {
			return true;
		}

		foreach (Creature child in from.GetChildrenAlive()) {
			if (child != null && from != null && to != null && from.IsAttachedToChildAlive(to.id)) {
				return true;
			}
		}
		return false;
	}

	public void DetatchFromMother(bool applyKick, bool playEffects) {
		phenotype.DetatchFromMother(this, applyKick, playEffects);
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

	public int cellCount {
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
		id = "";
		nickname = "";
		generation = 0;
		creation = CreatureCreationEnum.Unset;

		detatch = false;
		cantGrowMore = 0;
		growTicks = 0;

		ClearMotherAndChildrenReferences();
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
			transform.parent = World.instance.life.transform;
		} else if (type == PhenoGenoEnum.Genotype) {
			phenotype.hasDirtyPosition = true;

			Vector2 originCellPosition = genotype.originCell.position;

			genotype.Grab();
			hasGenotypeCollider = false;

			phenotype.Halt();

			transform.parent = null;
			transform.position = originCellPosition;
			transform.parent = World.instance.life.transform;
		}
		isDirtyGraphics = true;
	}

	public void Release(PhenoGenoEnum type) {
		if (type == PhenoGenoEnum.Phenotype) {
			phenotype.Release(this);
			genotype.MoveToPhenotype(this);
			hasPhenotypeCollider = true;
			phenotype.UpdateRotation();
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
		if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			phenotype.MoveToGenotype(this);
		} else if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			genotype.MoveToPhenotype(this);
		}
	}

	private void BringOtherGenoPhenoPositionAndRotationToCurrent() {
		if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			genotype.MoveToPhenotype(this);
		} else if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			phenotype.MoveToGenotype(this);
		}
	}

	private void ShowCurrentGenoPhenoAndHideOther() {
		phenotype.Show(CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype); //Don't use SetActive() since it clears rigigdBody velocity
		genotype.gameObject.SetActive(CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype);
	}

	//Don't enable collider if grabbed, thou
	private void EnableCurrentGenoPhenoColliderAndDisableOther() {
		phenotype.hasCollider = CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && !phenotype.isGrabbed;
		genotype.hasCollider = CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype && !genotype.isGrabbed;
	}

	// Update
	public void UpdateGraphics() {
		genotype.UpdateGraphics();
		phenotype.UpdateGraphics(this);

		if (isDirtyGraphics) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update Creature (due to user input)");

			ShowCurrentGenoPhenoAndHideOther();
			EnableCurrentGenoPhenoColliderAndDisableOther();

			if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
				//phenotype.ShowShadow(false);
				phenotype.UpdateOutline(this, CreatureSelectionPanel.instance.IsSelected(this), CreatureSelectionPanel.instance.IsSelectedCluster(this));

				//Show selected or not
				phenotype.ShowCellsSelected(false);
				if (CreatureSelectionPanel.instance.soloSelected == this) {
					phenotype.ShowCellSelected(CellPanel.instance.selectedCell, true);
				}
			} else if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
				//Show selected or not
				//phenotype.ShowOutline(false);
				genotype.UpdateOutline(this, CreatureSelectionPanel.instance.IsSelected(this));
				genotype.ShowGeneCellsSelected(false);
				if (CreatureSelectionPanel.instance.soloSelected == this) {
					genotype.ShowGeneCellsSelectedWithGene(GeneNeighboursPanel.instance.selectedGene, true);
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

		if (phenotype.UpdateConnectionsFromCellsBody(this, HasMotherAlive() ? GetMotherAlive().id : "no mother")) {
			isDirtyGraphics = true;
		}
	}

	public bool UpdateKillWeakCells(ulong worldTicks) {
		return phenotype.UpdateKillWeakCells(worldTicks);
	}

	public void SetFluxEffectToZero() {
		foreach (Cell cell in phenotype.cellList) {
			cell.effectFluxFromSelf = 0f;
			cell.effectFluxToSelf = 0f;
		}
	}
	
	//Returns true if creature grew
	public void UpdatePhysics(ulong worldTicks) {
		//time
		growTicks++;
		if (growTicks >= GlobalSettings.instance.quality.growTickPeriod) {
			growTicks = 0;
		}

		phenotype.UpdatePhysics(this, worldTicks);

		int didGrowCount = 0;

		// Execute detatch
		if (GlobalPanel.instance.physicsDetatch.isOn && detatch) { // At this point we are sure that our origin cell had time to get to know its neighbours (including mother placenta)
			DetatchFromMother(true, true);

			PhenotypePanel.instance.MakeDirty();
			CellPanel.instance.MakeDirty();
			cantGrowMore = 0;
			detatch = false;
		}

		if (growTicks == 0) {
			if (GlobalPanel.instance.physicsGrow.isOn) {
				NoGrowthReason reason;
				didGrowCount = phenotype.TryGrow(this, false, 1, false, true, worldTicks, false, out reason);
				if (didGrowCount > 0) {
					PhenotypePanel.instance.MakeDirty();
					CellPanel.instance.MakeDirty();
					cantGrowMore = 0;
				} else if (reason.fullyGrown) {
					cantGrowMore = int.MaxValue;
				} else if (((reason.roomBound || reason.poseBound) && !reason.energyBound && !reason.respawnTimeBound)) {
					cantGrowMore++; // wait a while before giving up on finding a spot to grow another cell
				} else {
					cantGrowMore = 0;
				}
			}
			// ☠ ꕕ Haha, make use of these
			//Debug.Log(" Id: " + id + ", CGM: " + cantGrowMore + ", roomBound: " + reason.roomBound + ", energyBound: " + reason.energyBound + ", respawnTimeBound: " + reason.respawnTimeBound + ", fullyGrown: " + reason.fullyGrown);

			// Detatch child from mother
			if (GlobalPanel.instance.physicsDetatch.isOn && IsAttachedToMotherAlive()) {
				if ((phenotype.originCell.originDetatchMode == ChildDetatchModeEnum.Size && phenotype.originCell.originDetatchSizeThreshold < 1f && phenotype.cellCount >= Mathf.Clamp(Mathf.RoundToInt(phenotype.originCell.originDetatchSizeThreshold * genotype.geneCellCount), 1, genotype.geneCellCount)) ||
					(phenotype.originCell.originDetatchMode == ChildDetatchModeEnum.Energy && phenotype.originCell.energyFullness >= phenotype.originCell.originDetatchEnergyThreshold && cantGrowMore >= GlobalSettings.instance.phenotype.detatchAfterCompletePersistance)) {

					//) {

					detatch = true; // Make sure we go one loop and reach UpdateStructure() before detatching from mother. Otherwise: if we just grew, originCell wouldn't know about placenta in mother and kick wouldn't be made properly
				}
			}
		}

		// Execute pending Egg Fertilize
		// If Egg was disabled there is no point checking here either
		if (GlobalPanel.instance.physicsEgg.isOn) {
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
				World.instance.life.FertilizeCreature(fertilizeCell, true, worldTicks, false);
				PhenotypePanel.instance.MakeDirty();
				CellPanel.instance.MakeDirty();
			}
		}
	}

	public void OnRecycle() {
		Clear();
		genotype.OnRecycle();
		phenotype.OnRecycle();
	}

	public void OnBorrowToWorld() {
		Clear();
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
		creatureData.creation = creation;
		creatureData.generation = generation;

		creatureData.bornTick = bornTick;
		creatureData.deadTick = deadTick;
		//todo: spieces

		creatureData.genotypeData = genotype.UpdateData();
		creatureData.phenotypeData = phenotype.UpdateData();


		//time
		creatureData.growTicks = growTicks;
		creatureData.cantGrowMore = cantGrowMore;
		creatureData.detatch = detatch;

		//Relatives
		creatureData.childDataList.Clear();
		List<Child> c = children.Values.ToList();
		for (int index = 0; index < children.Values.Count; index++) {
			Child child = c[index];
			creatureData.childDataList.Add(child.UpdateData());
		}
		if (mother != null) {
			creatureData.motherData = mother.UpdateData();
		} else {
			creatureData.motherData = null;
		}

		return creatureData;
	}

	// Load
	public void ApplyData(CreatureData creatureData) {
		//me
		nickname = creatureData.nickname;
		id = creatureData.id;
		creation = creatureData.creation;
		generation = creatureData.generation;

		bornTick = creatureData.bornTick;
		deadTick = creatureData.deadTick;

		genotype.ApplyData(creatureData.genotypeData);
		Vector2 position = creatureData.genotypeData.originPosition;
		float heading = creatureData.genotypeData.originHeading;
		genotype.UpdateGeneCellsFromGenome(this, position, heading); // Generating genotype here caused Unity freeze ;/

		growTicks = creatureData.growTicks;
		cantGrowMore = creatureData.cantGrowMore;
		detatch = creatureData.detatch;

		phenotype.ApplyData(creatureData.phenotypeData, this);

		//Relatives
		ClearMotherAndChildrenReferences();
		for (int index = 0; index < creatureData.childDataList.Count; index++) {
			Child child = new Child();
			child.ApplyData(creatureData.childDataList[index]);
			AddChild(child);
		}
		if (creatureData.motherData != null && creatureData.motherData.id != "") {
			mother = new Mother();
			mother.ApplyData(creatureData.motherData);
			phenotype.connectionsDiffersFromCells = true;
		}
	}

	// ^ Load / Save ^
}