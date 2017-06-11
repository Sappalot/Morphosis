using UnityEngine;
using System.Collections;

public class Genotype : MonoBehaviour {
    public static int root = 0;
    public static int genomeLength = 21;
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
        for (int index = 0; index < genomeLength; index++) {
            genome[index] = new Gene(index);
        }
        for (int index = 0; index < genomeLength; index++) {
            genome[index].SetDefaultReferenceGene(genome);
        }
    }

    public Gene GetGeneAt(int index) {
        return genome[index];
    }

    private void CreateJellyfish() {
        Clear();

        //Simple Jellyfish (FPS Reference creature, Don't change!!)
        genome[0].type = CellTypeEnum.Vein;
        genome[0].setReference(3, 10);
        genome[0].setReference(5, 20);
        genome[0].setReference(4, 1);

        genome[1].type = CellTypeEnum.Muscle;

        genome[10].type = CellTypeEnum.Leaf;
        genome[10].setReference(1, 10);
        genome[10].setReference(2, 1);

        genome[20].type = CellTypeEnum.Leaf;
        genome[20].setReference(1, 20);
        genome[20].setReference(0, 1);
    }

    private void CreateString() {
        Clear();

        //string
        genome[0].type = CellTypeEnum.Leaf;
        genome[0].setReference(1, 1);

        genome[1].type = CellTypeEnum.Leaf;
        genome[1].setReference(1, 2);

        genome[2].type = CellTypeEnum.Leaf;
        genome[2].setReference(1, 3);

        genome[3].type = CellTypeEnum.Leaf;
        genome[3].setReference(1, 4);

        genome[4].type = CellTypeEnum.Leaf;
        genome[4].setReference(1, 5);

        genome[5].type = CellTypeEnum.Leaf;
        genome[5].setReference(1, 6);

        genome[6].type = CellTypeEnum.Leaf;
        genome[6].setReference(1, 7);

        genome[7].type = CellTypeEnum.Leaf;

        genome[10].type = CellTypeEnum.Mouth;
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
