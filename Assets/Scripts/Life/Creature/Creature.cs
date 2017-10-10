using System.Collections.Generic;
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

	public bool isDirty = false;

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

	public void GeneratePhenotypeCells() {
		phenotype.GenerateCells(this);
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

	public void GenerateEdgeFailure(Vector3 position, float heading) {
		genotype.GenerateGenomeEdgeFailure();
		GenerateGenotypeAndPhenotype(position, heading);
	}

	public void GenerateJellyfish(Vector2 position, float heading) {
		genotype.GenerateGenomeJellyfish();
		GenerateGenotypeAndPhenotype(position, heading);
	}

	public void GenerateEmbryo(Vector3 position, float heading) {
		genotype.GenomeEmpty();
		GenerateGenotypeAndPhenotype(position, heading);
	}

	public void GenerateFreak(Vector3 position, float heading) {
		genotype.GenomeEmpty();
		genotype.GenomeScramble();
		GenerateGenotypeAndPhenotype(position, heading);
	}

	public void GenerateMergling(List<Gene[]> genomes, Vector3 position, float heading) {
		genotype.GenomeSet(GenotypeUtil.CombineGenomeFine(genomes));
		GenerateGenotypeAndPhenotype(position, heading);
	}

	private void GenerateGenotypeAndPhenotype(Vector2 position, float heading) {
		genotype.differsFromGenome = true;
		genotype.GenerateGeneCells(this, position, heading); // Generating genotype here caused Unity freeze ;/

		phenotype.differsFromGeneCells = true;
		phenotype.GenerateCells(this, position, heading);
		isDirty = true;
	}

	// Apply on genotype ==> Phenotype
	public void Clear() {
		genotype.GenomeEmpty();
		genotype.GenerateGeneCells(this, genotype.rootCell.position, genotype.rootCell.heading);
		phenotype.differsFromGeneCells = true;
	}

	public void MutateAbsolute(float strength) {
		RestoreState();
		MutateCummulative(strength);
	}

	public void MutateCummulative(float strength) {
		genotype.GenomeMutate(strength);
		genotype.GenerateGeneCells(this, genotype.rootCell.position, genotype.rootCell.heading);
		phenotype.differsFromGeneCells = true;
	}

	public void Scramble() {
		genotype.GenomeScramble();
		genotype.GenerateGeneCells(this, genotype.rootCell.position, genotype.rootCell.heading);
		phenotype.differsFromGeneCells = true;
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

	// --
	public void EvoUpdate() {
		phenotype.EvoUpdate();
	}

	public void EvoFixedUpdate(float fixedTime) {
		phenotype.EvoFixedUpdate(this, fixedTime);
	}

	private bool isShowCreatureSelected = false;

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

	public void StoreState() {
		BringOtherGenoPhenoPositionAndRotationToCurrent();
		UpdateData();
	}

	public void RestoreState() {
		ApplyData(creatureData);
		ShowCurrentGenoPhenoAndHideOther();
		isDirty = true;
	}

	public void Clone(Creature original) {
		ApplyData(original.UpdateData());
	} 

	//data
	private CreatureData creatureData = new CreatureData();

	public CreatureData UpdateData() {
		BringOtherGenoPhenoPositionAndRotationToCurrent();

		creatureData.id = id;
		creatureData.nickname = nickname;
		//todo: spieces

		creatureData.genotypeData = genotype.UpdateData();
		creatureData.phenotypeData = phenotype.UpdateData();

		return creatureData;
	}

	public void ApplyData(CreatureData creatureData) {
		nickname = creatureData.nickname;

		genotype.ApplyData(creatureData.genotypeData);
		genotype.differsFromGenome = true;
		Vector2 position = creatureData.genotypeData.rootPosition;
		float heading = creatureData.genotypeData.rootHeading;
		genotype.GenerateGeneCells(this, position, heading); // Generating genotype here caused Unity freeze ;/

		phenotype.ApplyData(creatureData.phenotypeData, this);
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

	// if we update as creature copy is born we'll run into copy creture offset problems
	private void Update() {
		if (isDirty) {
			if (genotype.differsFromGenome) {
				genotype.GenerateGeneCells(this, genotype.rootCell.position, genotype.rootCell.heading);
			}

			if (phenotype.differsFromGeneCells) {
				phenotype.GenerateCells(this, genotype.rootCell.position, genotype.rootCell.heading);
			}

			ShowCurrentGenoPhenoAndHideOther();
			EnableCurrentGenoPhenoColliderAndDisableOther();

			if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype) {
				// Update selection
				phenotype.ShowSelectedCreature(CreatureSelectionPanel.instance.IsSelected(this));

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
			}
			isDirty = false;
		}
	}
}