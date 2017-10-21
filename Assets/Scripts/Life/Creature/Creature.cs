﻿using System.Collections.Generic;
using UnityEngine;

// The container of genotype(genes) and phenotype(body)
// Holds information that does not fit into genes or body 
public class Creature : MonoBehaviour {
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

	// Relatives ---------------
	private List<Creature> originals = new List<Creature>();
	private Mother mother;
	private List<Child> children = new List<Child>();

	public bool hasMother {
		get {
			return mother != null;
		}
	}

	public bool isMotherDirty {
		get {
			return hasMother && mother.isReferenceDirty;
		}
	}
	
	public bool hasChild {
		get {
			return  children.Count > 0;
		}
	}

	public bool isChildDirty {
		get {
			if (!hasChild) {
				return false;
			}
			foreach (Child c in children) {
				if (c.isReferenceDirty) {
					return true;
				}
			}
			return false;
		}
	}

	public void SetMother(string id, bool isConnected) {
		Debug.Assert(mother == null, "Creature has allready a mother");
		mother = new Mother(id);
		mother.isConnected = isConnected;
	}

	public void SetChild(string id, Vector2i placentaMapPosition, bool isConnected) {
		Debug.Assert(children.Find(c => c.id == id) == null, "Creature has allready a child with that id");
		Child newChild = new Child(id);
		newChild.placentaMapPosition = placentaMapPosition;
		newChild.isConnected = isConnected;
		children.Add(newChild);
	}

	public void TryChangeRelativesId(string oldId, string newId) {
		if (mother != null && mother.id == oldId) {
			mother.id = newId;
		}
		foreach (Child child in children) {
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

	public int cellsTotalCount {
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

	public void ConnectChild(Creature child) {

	}

	public void ReleaseChild(Creature child) {

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
		Update(); //To avoid occational flicker (shows genotype shortly)
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
		Update();
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
		if (cell.mapPosition != phenotype.rootCell.mapPosition) {
			phenotype.DeleteCell(cell, true);
		} else {
			Debug.LogError("You are not allowed to Delete root cell");
		}
	}

	//TODO: update graphics not caused by hunman interaction here
	public void EvoUpdate() {
		//if (!phenotype.isGrabbed) {
		//	phenotype.EvoUpdate();
		//}
		//if (!genotype.isGrabbed) {
		//	genotype.EvoUpdate();
		//}
	}

	public void EvoFixedUpdate(float fixedTime) {
		phenotype.EvoFixedUpdate(this, fixedTime);
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
			transform.parent = World.instance.life.transform;
		} else if (type == PhenoGenoEnum.Genotype) {
			phenotype.hasDirtyPosition = true;

			Vector2 rootCellPosition = genotype.rootCell.position;

			genotype.Grab();
			hasGenotypeCollider = false;

			phenotype.Halt();

			transform.parent = null;
			transform.position = rootCellPosition;
			transform.parent = World.instance.life.transform;
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



	//----------------------

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

	public bool isSomethingDirty {
		get {
			return genotype.geneCellsDiffersFromGenome ||
				phenotype.cellsDiffersFromGeneCells ||
				phenotype.connectionsDiffersFromCells ||
				isMotherDirty ||
				isChildDirty;
		}
	}

	private void Update() {
		bool geneCelleWasUpdated = genotype.UpdateGeneCellsFromGenome(this, genotype.rootCell.position, genotype.rootCell.heading);

		phenotype.cellsDiffersFromGeneCells |= geneCelleWasUpdated;
		bool cellsWereUpdatedFromGeneCells = phenotype.UpdateCellsFromGeneCells(this, genotype.rootCell.position, genotype.rootCell.heading);

		phenotype.connectionsDiffersFromCells |= cellsWereUpdatedFromGeneCells;
		bool connectionsWereUpdatedFromCells = phenotype.UpdateConnectionsFromCellsIntraBody();

		if (mother != null) {
			mother.UpdateCreatureFromId();
		}
		foreach (Child c in children) {
			c.UpdateCreatureFromId();
		}

		//Stitch mother and child together with springs
		if (!isSomethingDirty) {
			foreach (Child child in children) {
				if (child.isConnected && child.isConnectionDirty && !child.creature.isSomethingDirty) {
					Debug.Log("Connect: mother: " + id + " ==> " + child.id);
					phenotype.ConnectChildEmbryo(child);
					child.isConnectionDirty = false;
				}
			}
		}


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

	// Save / Load / Clone
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
		// TODO: Make clone of mother with attached child work.... trouble with ids 
		ApplyData(original.UpdateData());
	}

	public CreatureData UpdateData() {
		BringOtherGenoPhenoPositionAndRotationToCurrent(); //Do we really need this one??

		//me
		creatureData.id = id;
		creatureData.nickname = nickname;
		//todo: spieces

		//mother
		if (mother != null) {
			creatureData.motherId = mother.id;
			creatureData.isMotherConnected = mother.isConnected;
		} else {
			creatureData.motherId = string.Empty;
		}

		//children
		creatureData.childrenId = new string[children.Count];
		creatureData.childrenPlacentaMapPosition = new Vector2i[children.Count];
		creatureData.isChildrenConnected = new bool[children.Count];
		for (int i = 0; i < creatureData.childrenId.Length; i++) {
			creatureData.childrenId[i] = children[i].id;
			creatureData.childrenPlacentaMapPosition[i] = children[i].placentaMapPosition;
			creatureData.isChildrenConnected[i] = children[i].isConnected;
		}

		creatureData.genotypeData = genotype.UpdateData();
		creatureData.phenotypeData = phenotype.UpdateData();

		return creatureData;
	}

	public void ApplyData(CreatureData creatureData) {
		//me
		nickname = creatureData.nickname;
		id = creatureData.id;

		//mother
		if (creatureData.motherId != string.Empty) {
			SetMother(creatureData.motherId, creatureData.isMotherConnected);
		}

		//children
		for (int i = 0; i < creatureData.childrenId.Length; i++) {
			SetChild(creatureData.childrenId[i], creatureData.childrenPlacentaMapPosition[i], creatureData.isChildrenConnected[i]);
		}

		genotype.ApplyData(creatureData.genotypeData);
		Vector2 position = creatureData.genotypeData.rootPosition;
		float heading = creatureData.genotypeData.rootHeading;
		genotype.UpdateGeneCellsFromGenome(this, position, heading); // Generating genotype here caused Unity freeze ;/

		phenotype.ApplyData(creatureData.phenotypeData, this);
	}
}