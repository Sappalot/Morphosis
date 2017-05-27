using UnityEngine;
using System.Collections;

public class Genotype : MonoBehaviour {
    public static int root = 0;
    public static int genomeLength = 100;
    Gene[] genome = new Gene[genomeLength];

    public void Generate() {
        for (int g = 0; g < genomeLength; g++) {
            genome[g] = new Gene();
        }
        CreateDummy();
    }


    public Gene GetGeneAt(int index) {
        return genome[index];
    }

    private void CreateDummy() {
        Clear();

        //Simple Jellyfish
        genome[0].type = CellType.Vein;
        genome[0].setReference(3, 10);
        genome[0].setReference(5, 20);
        genome[0].setReference(4, 1);

        genome[1].type = CellType.Muscle;

        genome[10].type = CellType.Leaf;
        genome[10].setReference(1, 10);
        genome[10].setReference(2, 1);

        genome[20].type = CellType.Leaf;
        genome[20].setReference(1, 20);
        genome[20].setReference(0, 1);

        ////simplistic motor experiment
        //genome[0].type = CellType.Muscle;
        //genome[0].setReference(1, 1);
        //genome[0].setReference(2, 2);
        //genome[0].setReference(3, 2);

        //genome[1].type = CellType.Muscle;

        //genome[2].type = CellType.Muscle;
        //genome[2].setReference(2, 3);

        //genome[3].type = CellType.Muscle;
    }

    // No references, type = vein
    public void Clear() {
        foreach (Gene gene in genome) {
            gene.Clear();
        }
    }
	
    // Update is called once per frame
	void Update () {
	
	}
}
