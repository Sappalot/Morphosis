using System.Collections.Generic;
using UnityEngine;

// The container of genotype(genes) and phenotype(body)
// Holds information that does not fit into genes or body 

public class Creature : MonoBehaviour {

    public string id;

    //wing force
    [Range(0f, 1f)]
    public float wingDrag = 1f;

    [Range(0f, 1f)]
    public float f1 = 0.03f;

    [Range(0f, 5f)]
    public float wingF2 = 1f;

    [Range(0f, 40f)]
    public float wingPow = 10f;

    [Range(0f, 10f)]
    public float wingMax = 0.1f;

    //muscle
    [Range(0f, 0.5f)]
    public float muscleRadiusDiff = 0.2f;

    [Range(-1f, 1f)]
    public float muscleContractRetract = -0.5f;

    [Range(0f, 10f)]
    public float muscleSpeed = 1.55f;

    //private string spieces;
    //private long age;
    //private Creature directMother;
    //private Creature directFather;
    //private List<Creature> offspring = new List<Creature>();
    //private Creature partsInward;
    //private List<Creature> partsOutward = new List<Creature>();

    public Genotype genotype;
    public Phenotype phenotype;
    

    void Awake() {
        genotype.Generate();
        phenotype.Generate(genotype, this);
    }

    //void FixedUpdate() {
    //    phenotype.UpdatePhysics(this);
    //}

    //private void RegeneratePhenotype() {
    //    phenotype.Generate(genotype);
    //}

    public void UpdateGrowth(float time) {
        //phenotype.Grow(time);
    }

    public void UpdatePhyscis() {
        phenotype.UpdatePhysics(this);
    }

    public void UpdateAction(float time) {

    }

    public void UpdateGraphics() {
        phenotype.UpdateGraphics();
    }
}

