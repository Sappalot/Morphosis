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

	public void SwitchToPhenotype() {
		ShowType(PhenoGenoEnum.Phenotype);
		phenotype.MoveToGenotype(this);
		phenotype.GenerateCells(this);
	}

	public void SwitchToGenotype() {
		ShowType(PhenoGenoEnum.Genotype);
		genotype.MoveToPhenotype(this);
		genotype.UpdateGraphics(this);
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
		genotype.GenerateGenomeEmpty();
		GenerateGenotypeAndPhenotype(position, heading);
		ShowType(showType);
	}

	public void GenerateMergling(List<Gene[]> genomes, Vector3 position, float heading, PhenoGenoEnum showType) {
		genotype.SetGenome(GenotypeUtil.Combine(genomes));
		GenerateGenotypeAndPhenotype(position, heading);
		ShowType(showType);
	}

	public void GenerateCopy(Creature creature, PhenoGenoEnum showType) {
		genotype.GenerateGenomeEmpty();
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

	public void TryGrow(int cellCount = 1) {
		phenotype.TryGrow(this, cellCount);
	}

	public void TryShrink(int cellCount = 1) {
		phenotype.TryShrink(cellCount);
	}

	public void EvoUpdate() {
		phenotype.EvoUpdate();
	}

	public void EvoFixedUpdate(float fixedTime) {
		phenotype.EvoFixedUpdate(this, fixedTime);
	}

	private bool isShowCreatureSelected = false;
	public void ShowSelected(bool on, bool force = false) {
		if (on != isShowCreatureSelected || force) {
			phenotype.ShowSelectedCreature(on);
			genotype.ShowCreatureSelected(on);
			isShowCreatureSelected = on;
		}
	}

	public void ShowCellsAndGeneCellsSelected(bool on) {
		phenotype.ShowCellsSelected(on);
		genotype.ShowGeneCellsSelected(on);
	}

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

	public void Clone(Creature original) {
		ApplyData(original.UpdateData());
	} 

	//data
	private CreatureData creatureData = new CreatureData();

	public CreatureData UpdateData() {
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
}