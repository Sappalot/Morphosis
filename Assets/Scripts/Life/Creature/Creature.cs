using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Boo.Lang.Runtime;

// The container of genotype(genes) and phenotype(body)
// Holds information that does not fit into genes or body 
public class Creature : MonoBehaviour, IGenotypeDirtyfy {
	public static float maxRadiusCircle = -666;
		
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
	public int canNotGrowMoreTicks { get; private set; } // blocked by myself, child/mother/other creature, or terrain(?)

	public void Initialize() {
		maxRadiusCircle = GlobalSettings.instance.phenotype.creatureHexagonMaxRadius + 0.5f; // used for culling and check so that we don't build too far away from origin 
		genotype.Initialize(this);
	}

	public string sceneGraphName {
		get {
			if (creation == CreatureCreationEnum.Frozen) {
				return "F-Creature " + id;
			} else {
				return "Creature " + id;
			}
		}
	}

	private void ReforgeBase() {
		bornTick = World.instance.worldTicks; // born now
		creation = CreatureCreationEnum.Forged;
		generation = 1;
		detatch = false;
		growTicks = 0;
		canNotGrowMoreTicks = 0;

		Debug.Assert(mother == null, "Trying to reforge creature with mother");
		Debug.Assert(children.Count == 0, "Trying to reforge creature with children");
		// TODO: Change name of spieces
		// TODO: Change name of individual
	}

	//  regenerate new geneCell pattern from new genome, regenterate new nerves, regrow cellStructure according to old genome, update inter cell stuff
	public void ReforgeGeneCellPatternAndForward() {
		ReforgeBase();
		genotype.MakeGeneCellPatternDirty(); // will make Everything else dirty
	}

	// regenerate new nerves, regrow cellStructure according to old genome, update inter cell stuff
	
	public void ReforgeInterGeneCellAndForward() {
		ReforgeBase();
		genotype.MakeInterGeneCellDirty();
		phenotype.MakeCellPaternDifferentFromGenotypeDirty();
	}

	// regrow cellStructure according to old genome, update inter cell stuff
	// this one is called as we do gene changes that are not leading to cell pattern being changed
	// We regrow crature fully anyway, as it is reforged due to that minor change
	public void ReforgeCellPatternAndForward() {
		ReforgeBase();
		phenotype.MakeCellPaternDifferentFromGenotypeDirty(); // force regrowth
	}

	public void OnFreeze() {
		creation = CreatureCreationEnum.Frozen;
		generation = 1;
		phenotype.DetatchFromMother(this, false, false);
		phenotype.SetAllCellStatesToDefault();
		
		ClearMotherAndChildrenReferences();
		//--
		phenotype.MakeCellPaternDifferentFromGenotypeDirty();
		phenotype.TryRegrowCellPattern(this, GetOriginPosition(PhenoGenoEnum.Phenotype), GetOriginHeading(PhenoGenoEnum.Phenotype));
		phenotype.DisablePhysicsComponents();
		//--
		
	}

	public void OnDefrost() {
		creation = CreatureCreationEnum.Cloned; //TODO: mark it as defrosted?
		generation = 1;
		phenotype.DetatchFromMother(this, false, false); // should not be connected
		ClearMotherAndChildrenReferences(); // should not have any
		phenotype.SetAllCellStatesToDefault();
		phenotype.EnablePhysicsComponents();
		phenotype.MakeCellPaternDifferentFromGenotypeDirty();
		bornTick = World.instance.worldTicks;
	}

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

	public bool IsAttachedToChildAlive() {
		return GetAttachedChildrenAliveCount() > 0;
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

	public bool HasRelativeWithId(string id) {
		if (GetMotherIdDeadOrAlive() == id) {
			return true;
		}
		if (HasChildrenDeadOrAlive()) {
			List<string> childrenIds = GetChildrenIdDeadOrAlive();
			foreach (string childId in childrenIds) {
				if (childId == id) {
					return true;
				}
			}
		}
		return false;
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

	public float GetAge(ulong worldTicks) { // Age in seconds
		return (worldTicks - bornTick) * Time.fixedDeltaTime;
	}

	public float GetAgeNormalized(ulong worldTicks) { // Age in seconds
		return ((worldTicks - bornTick) * Time.fixedDeltaTime) / GlobalSettings.instance.phenotype.maxAge;
	}

	public float energy {
		get {
			return phenotype.energy;
		}
	}

	//Dead or alive counts
	public bool allowedToChangeGenome {
		get {
			return !(HasMotherDeadOrAlive() || HasChildrenDeadOrAlive()) && creation != CreatureCreationEnum.Frozen;
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

	// Costy. Cache this one and update it as creatures in cluster are detatching or dying or fertilizing egg cell
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

	public int CellCountAtCompleteness(float completeness) {
		return genotype.CompletenessCellCount(completeness);
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


	// any genome with only 1 cell grown
	public void GenerateEmbryo(Gene[] genome, Vector3 position, float heading, IGenotypeDirtyfy genotypeDirtyfy) {
		genotype.SetGenome(genome, genotypeDirtyfy);
		genotype.TryUpdateGeneCellPattern(this, position, heading);
		phenotype.InitiateEmbryo(this, position, heading);
		isDirtyGraphics = true;
	}

	public void GenerateSimple(Vector3 position, float heading) {
		genotype.SetDefault();
		UpdateCellsAndGeneCells(position, heading);
	}

	public void GenerateFreak(Vector3 position, float heading) {
		genotype.SetDefault();
		genotype.SetScrambled();
		UpdateCellsAndGeneCells(position, heading);
	}

	public void GenerateMergling(List<Gene[]> genomes, Vector3 position, float heading, IGenotypeDirtyfy genotypeDirtyfy) {
		genotype.SetGenome(GenotypeUtil.CombineGenomeFine(genomes, genotypeDirtyfy), genotypeDirtyfy);
		UpdateCellsAndGeneCells(position, heading);
	}

	public void GenerateJellyfish(Vector2 position, float heading) {
		genotype.GenerateGenomeJellyfish();
		UpdateCellsAndGeneCells(position, heading);
	}

	private void UpdateCellsAndGeneCells(Vector2 position, float heading) {
		//we need to update them allready in order to have originCell. Origin cell is needed for position and heading when updating
		genotype.TryUpdateGeneCellPattern(this, position, heading); // Generating genotype here caused Unity freeze ;/
		phenotype.TryRegrowCellPattern(this, position, heading);
		
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
		canNotGrowMoreTicks = 0;
		growTicks = 0;

		ClearMotherAndChildrenReferences();
		genotype.SetDefault();
	}

	// Button ==> Apply on Phenotype
	public void TryGrow(bool allowOvergrowthOfAttached, int cellCount, bool playEffects) {
		NoGrowthReason reason;
		phenotype.TryGrow(this, true, allowOvergrowthOfAttached, cellCount, true, playEffects, 0, true, true, out reason);
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

	public void KillAllCells(bool tryPlayFx) {
		phenotype.KillAllCells(tryPlayFx);
	}

	public void ShowCellSelected(Cell cell, bool on) {
		phenotype.ShowCellSelected(cell, on);
	}

	public Cell GetCellAtPosition(Vector2 position) {
		return phenotype.GetCellAtPosition(position);
	}

	public Cell GetGeneCellAtPosition(Vector2 position) {
		return genotype.GetCellAtWorldPosition(position);
	}

	public void Grab(PhenoGenoEnum type) {
		if (type == PhenoGenoEnum.Phenotype) {
			Vector2 originCellPosition = phenotype.originCell.position;

			phenotype.Grab();
			hasPhenotypeCollider = false;

			transform.parent = null;
			transform.position = originCellPosition;
			if (creation == CreatureCreationEnum.Frozen) {
				transform.parent = Freezer.instance.transform;
			} else {
				transform.parent = World.instance.life.transform;
			}
		} else if (type == PhenoGenoEnum.Genotype) {
			phenotype.hasDirtyPosition = true;

			Vector2 originCellPosition = genotype.originCell.position;

			genotype.Grab();

			phenotype.Halt();

			transform.parent = null;
			transform.position = originCellPosition;
			if (creation == CreatureCreationEnum.Frozen) {
				transform.parent = Freezer.instance.transform;
			} else {
				transform.parent = World.instance.life.transform;
			}
		}
		isDirtyGraphics = true;
		phenotype.MakeBudsDirty();
	}

	public void Release(PhenoGenoEnum type) {
		if (type == PhenoGenoEnum.Phenotype) {
			phenotype.Release(this);
			genotype.MoveToPhenotype(this);
			hasPhenotypeCollider = true;
			phenotype.UpdateRotation();
		} else if (type == PhenoGenoEnum.Genotype) {
			genotype.Release(this);
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
		phenotype.Show(CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype, CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && !PhenotypeGraphicsPanel.instance.isGraphicsCellEnergyRelated && CreatureSelectionPanel.instance.IsSelectedCluster(this)); //Don't use SetActive() since it clears rigigdBody velocity
		genotype.gameObject.SetActive(CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype);
	}

	// Update
	public bool isInsideFrustum { get; private set; }

	public void UpdateGraphics() {
		if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			if (phenotype.isAlive && (!phenotype.hasOriginCell || SpatialUtil.IsInsideFrustum(phenotype.originCell.position))) {
				// inside frustum
				isInsideFrustum = true;
				phenotype.UpdateGraphics(this);
			} else if (isInsideFrustum) {
				// Leaving frustum
				isInsideFrustum = false;
				phenotype.UpdateGraphics(this);
			}

			//phenotype.UpdateGraphics(this);
		} else if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			if (SpatialUtil.IsInsideFrustum(genotype.originCell.position)) {
				isInsideFrustum = true;
				genotype.UpdateGraphics(CreatureSelectionPanel.instance.IsSelected(this));
				phenotype.edges.UpdateGraphics(false); // must be removed
			} else if (isInsideFrustum) {
				isInsideFrustum = false;
				genotype.UpdateGraphics(CreatureSelectionPanel.instance.IsSelected(this));
				phenotype.UpdateGraphics(this);
				phenotype.edges.UpdateGraphics(false); // must be removed
			}
		}

		if (isDirtyGraphics) {
			if (GlobalSettings.instance.debug.debugLogMenuUpdate)
				Debug.Log("Update Creature (due to user input)");

			ShowCurrentGenoPhenoAndHideOther();
			phenotype.hasCollider = CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && !phenotype.isGrabbed;

			if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
				//phenotype.ShowShadow(false);
				phenotype.UpdateTriangleOutlineAndBones(this, CreatureSelectionPanel.instance.IsSelected(this), CreatureSelectionPanel.instance.IsSelectedCluster(this));

				//Show selected or not
				phenotype.ShowCellsSelected(false);
				if (CreatureSelectionPanel.instance.soloSelected == this) {
					phenotype.ShowCellSelected(CellPanel.instance.selectedCell, true);
				}
			} else if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
				//Show selected or not
				//phenotype.ShowOutline(false);
				genotype.UpdateOutlineTriangleAndBones(this, CreatureSelectionPanel.instance.IsSelected(this));
				genotype.ShowGeneCellsSelected(false);
				if (CreatureSelectionPanel.instance.soloSelected == this) {
					genotype.ShowGeneCellsSelectedWithGene(GenePanel.instance.selectedGene, true);
				}
			}
			isDirtyGraphics = false;
		}
	}

	public void UpdateStructure() {
		if (!genotype.hasOriginCell) {
			return;
		}

		isDirtyGraphics |= genotype.TryUpdateGeneCellPattern(this, genotype.originCell.position, genotype.originCell.heading);

		isDirtyGraphics |= genotype.TryUpdateInterGeneCells();

		isDirtyGraphics |= phenotype.TryRegrowCellPattern(this, genotype.originCell.position, genotype.originCell.heading); // Will regrow fully

		isDirtyGraphics |= phenotype.TryUpdateInterCells(this, HasMotherAlive() ? GetMotherAlive().id : "no mother"); // Oupate as soon 
	}

	public bool UpdateKillWeakCells(ulong worldTicks) {
		return phenotype.UpdateKillWeakCells(worldTicks);
	}

	//Returns true if creature grew
	public void UpdatePhysics(ulong worldTicks) {
		//phenotype.UpdateSpringsFrequenze(); // While testing


		//time
		growTicks++;
		if (growTicks >= phenotype.growTickPeriod) { // GlobalSettings.instance.quality.growTickPeriod
			growTicks = 0;
		}

		phenotype.UpdatePhysics(this, worldTicks);

		int didGrowCount = 0;

		// Execute detatch
		if (PhenotypePhysicsPanel.instance.detatch.isOn && detatch) { // At this point we are sure that our origin cell had time to get to know its neighbours (including mother placenta)
			DetatchFromMother(true, true);

			PhenotypePanel.instance.MakeDirty();
			CellPanel.instance.MakeDirty();
			canNotGrowMoreTicks = 0;
			detatch = false;
		}

		if (growTicks == 0) {
			if (PhenotypePhysicsPanel.instance.grow.isOn) {
				NoGrowthReason reason;
				didGrowCount = phenotype.TryGrow(this, true, false, 1, false, true, worldTicks, false, false, out reason);
				if (didGrowCount > 0) {
					PhenotypePanel.instance.MakeDirty();
					CellPanel.instance.MakeDirty();
					canNotGrowMoreTicks = 0; // obviously there was room
				} else if (phenotype.CanGrowMore(this)) {
					canNotGrowMoreTicks = 0;
				} else {
					// so, i didn't grow and i cant grow more
					canNotGrowMoreTicks += phenotype.growTickPeriod; //GlobalSettings.instance.quality.growTickPeriod;
				}
			}
			// ☠ ꕕ Haha, make use of these
			//Debug.Log(" Id: " + id + ", CGM: " + cantGrowMore + ", roomBound: " + reason.roomBound + ", energyBound: " + reason.energyBound + ", respawnTimeBound: " + reason.respawnTimeBound + ", fullyGrown: " + reason.fullyGrown);

			// Detatch child from mother
			if (PhenotypePhysicsPanel.instance.detatch.isOn && IsAttachedToMotherAlive() && phenotype.originCell.originDetatchLogicBox.GetOutput(SignalUnitSlotEnum.outputEarlyA)) {
				detatch = true; // Make sure we go one loop and reach UpdateStructure() before detatching from mother. Otherwise: if we just grew, originCell wouldn't know about placenta in mother and kick wouldn't be made properly
			}
		}
	}

	public void UpdateFertilize(ulong worldTicks) {
		// Execute pending Egg Fertilize
		// If Egg was disabled there is no point checking here either
		if (PhenotypePhysicsPanel.instance.functionEgg.isOn) {
			if (phenotype.originCell.originPulseTick == 0) { // Allways fertilize at start of pulse to make mother and child locomote together
				Cell fertilizeCell = null;
				foreach (Cell c in phenotype.cellList) {
					if (c is EggCell) {
						EggCell eggCell = c as EggCell;
						if (eggCell.fertilizeLogicBox.GetOutput(SignalUnitSlotEnum.outputEarlyA)) {
							fertilizeCell = eggCell;
							break;
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
	}

	public void OnRecycle() {
		Clear();
		genotype.OnRecycle();
		phenotype.OnRecycle();
	}

	public void OnBorrowToWorld() {
		Clear();
	}

	public void OnLoaded() {
		phenotype.OnLoaded(this);
	}

	// Load / Save

	private CreatureData creatureData = new CreatureData();

	//Everything is deep cloned even the id. Change this not to have trouble
	public void Clone(Creature original) {
		ApplyData(original.UpdateData());
		OnLoaded();
	}

	// Save
	public CreatureData UpdateData() {
		BringOtherGenoPhenoPositionAndRotationToCurrent(); //Do we really need this one??

		creatureData.genotypeData = genotype.UpdateData();
		creatureData.phenotypeData = phenotype.UpdateData();


		creatureData.id = id;
		creatureData.nickname = nickname;
		creatureData.creation = creation;
		creatureData.generation = generation;
		creatureData.bornTick = bornTick;
		creatureData.deadTick = deadTick;

		creatureData.growTicks = growTicks;
		creatureData.canNotGrowMoreTicks = canNotGrowMoreTicks;
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
		genotype.ApplyData(creatureData.genotypeData, this);
		Vector2 position = creatureData.genotypeData.originPosition;
		float heading = creatureData.genotypeData.originHeading;
		genotype.TryUpdateGeneCellPattern(this, position, heading); // Generating genotype here caused Unity freeze ;/ (? still so 2020-04-12)
		phenotype.ApplyData(creatureData.phenotypeData, this);

		// Set these properties after changing gene stuff to prevent them from being erased (during reforge)
		nickname = creatureData.nickname;
		id = creatureData.id;
		creation = creatureData.creation;
		generation = creatureData.generation;
		bornTick = creatureData.bornTick;
		deadTick = creatureData.deadTick;

		growTicks = creatureData.growTicks;
		canNotGrowMoreTicks = creatureData.canNotGrowMoreTicks;
		detatch = creatureData.detatch;

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
			phenotype.MakeInterCellDirty();
		}
	}

	// ^ Load / Save ^
}