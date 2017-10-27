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

	[Range(0f, 10f)]
	public float muscleSpeed = 1.55f;

	public Genotype genotype;
	public Phenotype phenotype;

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

	public Soul GetChildSoul(string id) {
		return soul.GetChildSoul(id);
	}

	public Creature GetChild(string id) {
		return soul.GetChild(id);
	}

	public bool HasChild(string id) {
		return soul.HasChild(id);
	}

	// ^ Relatives ^

	public void DetatchFromMother() {
		if (!hasMotherSoul) {
			Debug.LogError("Creature can't detatch from mother, becaus it is motherless!");
		} else {
			//if (!mother.isConnected) {
			if (!soul.isConnectedWithMotherSoul) {
				Debug.LogError("Creature can't detatch from mother, becaus it is not connected!");
				return;
			}

			//me
			//mother.isConnected = false;
			soul.SetConnectedWithMotherSoul(false);
			phenotype.connectionsDiffersFromCells = true;

			//mother
			//mother.creature.children.Find(c => c.id == id).isConnected = false;
			motherSoul.creature.phenotype.connectionsDiffersFromCells = true;

			CreatureSelectionPanel.instance.MakeDirty();
		}
	}

	//NOTE: Each creature has a soul with references to mother and child souls, which are not shared among creatures
	//public void SetMother(string id, bool isConnected) {
	//	Debug.Assert(mother == null, "Creature has allready a mother");
	//	soul.mother = new Soul(id);
	//	mother.isConnected = isConnected;
	//}

	////NOTE: Each creature has a soul with references to mother and child souls, which are not shared among creatures
	//public void SetChild(string id, Vector2i rootMapPosition, int rootBindCardinalIndex, bool isConnected) {
	//	Debug.Assert(children.Find(c => c.id == id) == null, "Creature has allready a child with that id");
	//	Soul newChild = new Soul(id);
	//	newChild.childRootMapPosition = rootMapPosition;
	//	newChild.childRootBindCardinalIndex = rootBindCardinalIndex;
	//	newChild.isConnected = isConnected;
	//	soul.childReferences.Add(newChild);
	//}


	public void TryChangeRelativesId(string oldId, string newId) {
		if (motherSoul != null && motherSoul.id == oldId) {
			motherSoul.id = newId;
		}
		foreach (Soul child in childSouls) {
			if (child.id == oldId) {
				child.id = newId;
			}
		}
	}

	// ^ Relatives ^

	private bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
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

	public Vector2 GetRootPosition(PhenoGenoEnum type) {
		if (type == PhenoGenoEnum.Phenotype) {
			return phenotype.rootCell.position;
		} else {
			return genotype.rootCell.position;
		}
	}

	public float GetRootHeading(PhenoGenoEnum type) {
		if (type == PhenoGenoEnum.Phenotype) {
			return phenotype.rootCell.heading;
		} else {
			return genotype.rootCell.heading;
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
		isDirty = true;
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
		//we need to update them allready in order to have rootCell. Root cell is needed for position and heading when updating
		phenotype.cellsDiffersFromGeneCells = genotype.UpdateGeneCellsFromGenome(this, position, heading); // Generating genotype here caused Unity freeze ;/
		phenotype.connectionsDiffersFromCells = phenotype.UpdateCellsFromGeneCells(this, position, heading);
		isDirty = true;
		EvoUpdate();
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
	public void TryGrow(int cellCount = 1) {
		phenotype.TryGrow(this, cellCount);
		isDirty = true;
	}

	public void TryShrink(int cellCount = 1) {
		phenotype.TryShrink(cellCount);
		isDirty = true;
	}

	// Will cut branch as well
	public void DeleteCell(Cell cell) {
		if (!cell.isRoot) {
			phenotype.DeleteCell(cell, true);
		} else {
			Debug.LogError("You are not allowed to Delete root cell");
		}
	}

	public void ShowCellSelected(Cell cell, bool on) {
		phenotype.ShowCellSelected(cell, on);
	}

	public Cell GetCellAt(Vector2 position) {
		return phenotype.GetCellAt(position);
	}

	public void Grab(PhenoGenoEnum type) {
		if (type == PhenoGenoEnum.Phenotype) {
			Vector2 rootCellPosition = phenotype.rootCell.position;

			phenotype.Grab();
			hasPhenotypeCollider = false;

			transform.parent = null;
			transform.position = rootCellPosition;
			transform.parent = Life.instance.transform;
		} else if (type == PhenoGenoEnum.Genotype) {
			phenotype.hasDirtyPosition = true;

			Vector2 rootCellPosition = genotype.rootCell.position;

			genotype.Grab();
			hasGenotypeCollider = false;

			phenotype.Halt();

			transform.parent = null;
			transform.position = rootCellPosition;
			transform.parent = Life.instance.transform;
		}
		isDirty = true;
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
		isDirty = true;
	}

	public void ShowMarkers(bool show) {
		creturePosition.enabled =			show;
		phenotypePosition.enabled =			show;
		phenotypeCellsPosition.enabled =	show;
		genotypePosition.enabled =			show;
		genotypeCellsPosition.enabled =		show;
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

	// Load / Save

	private CreatureData creatureData = new CreatureData();

	public void StoreState() {
		BringOtherGenoPhenoPositionAndRotationToCurrent();
		UpdateData();
	}

	public void RestoreState() {
		genotype.ApplyData(creatureData.genotypeData);
		isDirty = true;
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

		return creatureData;
	}

	// Load
	public void ApplyData(CreatureData creatureData) {
		//me
		nickname = creatureData.nickname;
		id = creatureData.id;

		genotype.ApplyData(creatureData.genotypeData);
		Vector2 position = creatureData.genotypeData.rootPosition;
		float heading = creatureData.genotypeData.rootHeading;
		genotype.UpdateGeneCellsFromGenome(this, position, heading); // Generating genotype here caused Unity freeze ;/

		phenotype.ApplyData(creatureData.phenotypeData, this);
	}

	// ^ Load / Save ^

	// Update
	public void EvoFixedUpdate(float fixedTime) {
		phenotype.EvoFixedUpdate(this, fixedTime);
	}

	public void EvoUpdate() {
		if (!hasSoul) {
			if (Life.instance.HasSoul(id)) {
				soul = Life.instance.GetSoul(id);
			} else {
				Debug.LogError("Creature could not find is soul!!");
				return;
			}
		}

		genotype.EvoUpdate();
		phenotype.EvoUpdate();

		bool geneCelleWasUpdated = genotype.UpdateGeneCellsFromGenome(this, genotype.rootCell.position, genotype.rootCell.heading);

		phenotype.cellsDiffersFromGeneCells |= geneCelleWasUpdated;
		bool cellsWereUpdatedFromGeneCells = phenotype.UpdateCellsFromGeneCells(this, genotype.rootCell.position, genotype.rootCell.heading);

		phenotype.connectionsDiffersFromCells |= cellsWereUpdatedFromGeneCells;
		bool connectionsWereUpdatedFromCells = phenotype.UpdateConnectionsFromCellsBody(this);

		isDirty = isDirty || geneCelleWasUpdated || cellsWereUpdatedFromGeneCells || connectionsWereUpdatedFromCells;

		if (isDirty) {
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
			isDirty = false;
		}
	}
}

////mother
//if (motherSoul != null) {
//	creatureData.motherId = motherSoul.id;
//	//creatureData.isMotherConnected = mother.isConnected;
//	creatureData.isMotherConnected = soul.isConnectedWithMotherSoul;
//} else {
//	creatureData.motherId = string.Empty;
//}

////children
//creatureData.childrenId = new string[childSouls.Count];
//creatureData.childrenRootMapPosition = new Vector2i[childSouls.Count];
//creatureData.isChildrenConnected = new bool[childSouls.Count];
//creatureData.rootBindCardinalIndex = new int[childSouls.Count];
//for (int i = 0; i < creatureData.childrenId.Length; i++) {
//	creatureData.childrenId[i] = childSouls[i].id;

//	//creatureData.childrenRootMapPosition[i] = children[i].childRootMapPosition;
//	//creatureData.rootBindCardinalIndex[i] = children[i].childRootBindCardinalIndex;
//	//creatureData.isChildrenConnected[i] = children[i].isConnected;

//	creatureData.childrenRootMapPosition[i] = soul.childSoulRootMapPosition(childSouls[i].id);
//	creatureData.rootBindCardinalIndex[i] = soul.childSoulRootBindCardinalIndex(childSouls[i].id);
//	creatureData.isChildrenConnected[i] = soul.isConnectedWithChildSoul(childSouls[i].id);
//}

	//-------

//soul.id = creatureData.id;

////mother
//if (creatureData.motherId != string.Empty) {
//	//SetMother(creatureData.motherId, creatureData.isMotherConnected);
//	soul.SetMotherSoul(creatureData.motherId);
//}

////children
//for (int i = 0; i < creatureData.childrenId.Length; i++) {
//	//SetChild(creatureData.childrenId[i], creatureData.childrenRootMapPosition[i], creatureData.rootBindCardinalIndex[i], creatureData.isChildrenConnected[i]);
//	soul.AddChildSoul(creatureData.childrenId[i], creatureData.childrenRootMapPosition[i], creatureData.rootBindCardinalIndex[i], creatureData.isChildrenConnected[i]);
//}