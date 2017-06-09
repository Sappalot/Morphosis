﻿using UnityEngine;
using System.Collections;

public class Genotype : MonoBehaviour {
    public static int root = 0;
    public static int genomeLength = 100;
    public Gene[] genome = new Gene[genomeLength];

    public void GenerateJellyfish() {
        CreateEmptyGenome();
        CreateJellyfish();
    }

    public void GenerateString() {
        CreateEmptyGenome();
        CreateString();
    }

    private void CreateEmptyGenome() {
        for (int g = 0; g < genomeLength; g++) {
            genome[g] = new Gene();
        }
    }

    public Gene GetGeneAt(int index) {
        return genome[index];
    }

    private void CreateJellyfish() {
        Clear();

        //Simple Jellyfish (FPS Reference creature, Don't change!!)
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
    }

    private void CreateString() {
        Clear();

        //string
        genome[0].type = CellType.Leaf;
        genome[0].setReference(1, 1);

        genome[1].type = CellType.Leaf;
        genome[1].setReference(1, 2);

        genome[2].type = CellType.Leaf;
        genome[2].setReference(1, 3);

        genome[3].type = CellType.Leaf;
        genome[3].setReference(1, 4);

        genome[4].type = CellType.Leaf;
        genome[4].setReference(1, 5);

        genome[5].type = CellType.Leaf;
        genome[5].setReference(1, 6);

        genome[6].type = CellType.Leaf;
        genome[6].setReference(1, 7);

        genome[7].type = CellType.Leaf;
    }

    // No references, type = vein
    public void Clear() {
        for (int index = 0; index < genome.Length; index++) {
            genome[index].Clear();
        }
    }
	
    // Update is called once per frame
	void Update () {
	
	}
}
