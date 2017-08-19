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
    
    private static int number = 0;

    public void SwitchToPhenotype() {
        phenotype.gameObject.SetActive(true);
        genotype.gameObject.SetActive(false);
        RegeneratePhenotype();
    }

    public void SwitchToGenotype() {
        phenotype.gameObject.SetActive(false);
        genotype.Generate(this);
        genotype.UpdateGraphics(this);
        genotype.gameObject.SetActive(true);
    }

    public void GenerateGenotypeAndPhenotype(Vector3 offset) {
        //float rnd = Random.Range(0f, 2f);
        genotype.GenerateJellyfish(); // TODO: Load from disc
        //if (number > 0) {
        //    genotype.GenerateJellyfish();
        //} else {
        //    genotype.GenerateString();
        //}

        genotype.Generate(this); // Generating genotype here caused Unity freeze ;/
                                 //Debug.Log("genotype: " + this.ToString());
                                 //genotype.UpdateGraphics(this);

        //phenotype.Generate(genotype, this, offset); // TODO: Load from disc
        phenotype.Setup(this, offset);
        phenotype.TryGrow(40);

        number++;
    }

    public void RegeneratePhenotype() {
        phenotype.Setup(this, phenotype.rootCell.transform.position);
        phenotype.TryGrow(40);
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

    public void SetHighlite(bool on) {
        phenotype.SetHighlite(on);
        genotype.SetHighlite(on);
    }
}

