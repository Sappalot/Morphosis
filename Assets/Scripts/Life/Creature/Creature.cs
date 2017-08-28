using System.Collections.Generic;
using UnityEngine;

// The container of genotype(genes) and phenotype(body)
// Holds information that does not fit into genes or body 

public class Creature : MonoBehaviour {
    public string id;
    public string nickname;

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

    public void SwitchToPhenotype() {
        phenotype.Show(true); //Don't use SetActive() since it clears rigigdBody velocity
        genotype.gameObject.SetActive(false);
        phenotype.GenerateCells(this);
    }

    public void SwitchToGenotype() {
        phenotype.Show(false); //Don't use SetActive() since it clears rigigdBody velocity
        genotype.UpdateTransformAndHighlite(this);
        genotype.gameObject.SetActive(true);
    }

    public void GenerateJellyfish(Vector3 position) {
        genotype.GenerateGenomeJellyfish();
        genotype.isDirty = true;
        genotype.GenerateGeneCells(this); // Generating genotype here caused Unity freeze ;/

        phenotype.isDirty = true;
        phenotype.GenerateCells(this, position);
    }

    public void GenerateMinimalistic(Vector3 position) {
        genotype.GenerateGenomeEmpty();
        genotype.isDirty = true;
        genotype.GenerateGeneCells(this); // Generating genotype here caused Unity freeze ;/

        phenotype.isDirty = true;
        phenotype.GenerateCells(this, position);
    }

    public void TryGrow(int cellCount = 1) {
        phenotype.TryGrow(cellCount);
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

    public void ShowCreatureSelected(bool on) {
        phenotype.ShowSelectedCreature(on);
        genotype.ShowCreatureSelected(on);
    }

    public void ShowCellsSelected(bool on) {
        phenotype.ShowCellsSelected(on);
        genotype.ShowGeneCellsSelected(on);
    }

    public void ShowCellSelected(Cell cell, bool on) {
        phenotype.ShowCellSelected(cell, on);
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
        genotype.isDirty = true;
        genotype.GenerateGeneCells(this); // Generating genotype here caused Unity freeze ;/

        phenotype.ApplyData(creatureData.phenotypeData, this);
    }
}

