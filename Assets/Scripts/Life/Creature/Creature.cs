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

	public void GenerateEmbryo(Vector3 position, float heading, PhenoGenoEnum showType) {
		genotype.GenomeEmpty();
		GenerateGenotypeAndPhenotype(position, heading);
		ShowType(showType);
	}

	public void GenerateFreak(Vector3 position, float heading, PhenoGenoEnum showType) {
		genotype.GenomeEmpty();
		genotype.GenomeScramble();
		GenerateGenotypeAndPhenotype(position, heading);
		ShowType(showType);
	}

	public void GenerateMergling(List<Gene[]> genomes, Vector3 position, float heading, PhenoGenoEnum showType) {
		genotype.GenomeSet(GenotypeUtil.CombineGenomeFine(genomes));
		GenerateGenotypeAndPhenotype(position, heading);
		ShowType(showType);
	}

	public void GenerateCopy(Creature creature, PhenoGenoEnum showType) {
		genotype.GenomeEmpty();
		GenerateGenotypeAndPhenotype(creature.GetRootPosition(showType), creature.GetRootHeading(showType));
		ShowType(showType);
	}

	private void GenerateGenotypeAndPhenotype(Vector2 position, float heading) {
		genotype.hasDirtyGenes = true;
		genotype.GenerateGeneCells(this, position, heading); // Generating genotype here caused Unity freeze ;/

		phenotype.differsFromGenotype = true;
		phenotype.GenerateCells(this, position, heading);

		ShowSelected(false, true);
	}

	//Make show type a member ??
	public void ShowType(PhenoGenoEnum showType) {
		phenotype.Show(showType == PhenoGenoEnum.Phenotype); //Don't use SetActive() since it clears rigigdBody velocity
		genotype.gameObject.SetActive(showType == PhenoGenoEnum.Genotype);
	}

	public void ShowType() {
		phenotype.Show(CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype); //Don't use SetActive() since it clears rigigdBody velocity
		genotype.gameObject.SetActive(CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype);
	}

	// Apply on genotype ==> Phenotype
	public void Clear() {
		genotype.GenomeEmpty();
		genotype.GenerateGeneCells(this, genotype.rootCell.position, genotype.rootCell.heading);
		phenotype.differsFromGenotype = true;
	}

	public void MutateAbsolute(float strength) {
		RestoreState();
		MutateCummulative(strength);
	}

	public void MutateCummulative(float strength) {
		genotype.GenomeMutate(strength);
		genotype.GenerateGeneCells(this, genotype.rootCell.position, genotype.rootCell.heading);
		phenotype.differsFromGenotype = true;
	}

	public void Scramble() {
		genotype.GenomeScramble();
		genotype.GenerateGeneCells(this, genotype.rootCell.position, genotype.rootCell.heading);
		phenotype.differsFromGenotype = true;
	}

	// Apply on Phenotype
	public void TryGrow(int cellCount = 1) {
		phenotype.TryGrow(this, cellCount);
	}

	public void TryShrink(int cellCount = 1) {
		phenotype.TryShrink(cellCount);
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
		if (type ==PhenoGenoEnum.Phenotype) {
			Vector2 rootCellPosition = phenotype.rootCell.position;
			phenotype.Grab();

			transform.parent = null;
			transform.position = rootCellPosition;
			transform.parent = World.instance.life.transform;
		} else if (type == PhenoGenoEnum.Genotype) {
			phenotype.hasDirtyPosition = true;

			Vector2 rootCellPosition = genotype.rootCell.position;
			genotype.Grab();
			phenotype.Halt();

			transform.parent = null;
			transform.position = rootCellPosition;
			transform.parent = World.instance.life.transform;
		}
	}

	public void Release(PhenoGenoEnum type) {
		if (type == PhenoGenoEnum.Phenotype) {
			phenotype.Release(this);
		} else if (type == PhenoGenoEnum.Genotype) {
			genotype.Release(this);
		}
	}

	public void ShowMarkers(bool show) {
		creturePosition.enabled =			show;
		phenotypePosition.enabled =			show;
		phenotypeCellsPosition.enabled =	show;
		genotypePosition.enabled =			show;
		genotypeCellsPosition.enabled =		show;
	}

	public void StoreState() {
		SyncGenoPhenoSpatial();
		UpdateData();
	}

	public void RestoreState() {
		ApplyData(creatureData);
		ShowType();
	}

	public void Clone(Creature original) {

		ApplyData(original.UpdateData());
	} 

	private void SyncGenoPhenoSpatial() {
		if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype) {
			genotype.MoveToPhenotype(this);
		} else if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
			phenotype.MoveToGenotype(this);
		}
	}

	//data
	private CreatureData creatureData = new CreatureData();

	public CreatureData UpdateData() {
		SyncGenoPhenoSpatial();

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
		genotype.hasDirtyGenes = true;
		Vector2 position = creatureData.genotypeData.rootPosition;
		float heading = creatureData.genotypeData.rootHeading;
		genotype.GenerateGeneCells(this, position, heading); // Generating genotype here caused Unity freeze ;/

		phenotype.ApplyData(creatureData.phenotypeData, this);
	}

	//--
	public void ShowCellsAndGeneCellsSelected(bool on) {
		phenotype.ShowCellsSelected(on);
		genotype.ShowGeneCellsSelected(on);
	}

	public void ShowSelected(bool on, bool force = false) {
		if (on != isShowCreatureSelected || force) {
			phenotype.ShowSelectedCreature(on);
			genotype.ShowCreatureSelected(on);
			isShowCreatureSelected = on;
		}
	}


	public void SwitchToPhenotype() {
		ShowType(PhenoGenoEnum.Phenotype);
		phenotype.MoveToGenotype(this);
		phenotype.GenerateCells(this);
		isDirty = true;
	}

	public void SwitchToGenotype() {
		ShowType(PhenoGenoEnum.Genotype);
		genotype.MoveToPhenotype(this);
		isDirty = true;
	}

	//-------------------------------
	private bool m_isDirty;
	public bool isDirty {
		get {
			return m_isDirty;
		}
		set {
			m_isDirty = value;
		}
	}
	
	private PhenoGenoEnum showingType = PhenoGenoEnum.Void;

	private void Update() {
		if (isDirty) {
			bool isCreatureSelected = CreatureSelectionPanel.instance.IsSelected(this);
			if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype) {
				// Switch show phenotype ==> genotype
				if (showingType != PhenoGenoEnum.Phenotype) {
					ShowType(PhenoGenoEnum.Phenotype);
					phenotype.MoveToGenotype(this);
					phenotype.GenerateCells(this);
					showingType = PhenoGenoEnum.Phenotype;
				}

				// Update selection
				phenotype.ShowSelectedCreature(isCreatureSelected);
				phenotype.ShowCellsSelected(false);
				if (CreatureSelectionPanel.instance.soloSelected == this) {
					phenotype.ShowCellSelected(PhenotypePanel.instance.selectedCell, true);
				}
			} else if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
				// Switch show genotype ==> phenotype
				if (showingType != PhenoGenoEnum.Genotype) {
					ShowType(PhenoGenoEnum.Genotype);
					genotype.MoveToPhenotype(this);
					showingType = PhenoGenoEnum.Genotype;
				}

				// Update selection
				genotype.ShowCreatureSelected(CreatureSelectionPanel.instance.IsSelected(this));
				genotype.ShowGeneCellsSelected(false);
				if (CreatureSelectionPanel.instance.soloSelected == this) {
					genotype.ShowGeneCellsSelectedWithGene(GenePanel.instance.selectedGene, true);
				}
			}
			isDirty = false;
		}
	}
}