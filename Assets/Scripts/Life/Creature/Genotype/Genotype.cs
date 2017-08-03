using UnityEngine;
using System.Collections;

public class Genotype : MonoBehaviour {
    public static int root = 0;
    public static int genomeLength = 21;
    private Gene[] genes = new Gene[genomeLength];

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
            genes[index] = new Gene(index);
        }
        for (int index = 0; index < genomeLength; index++) {
            genes[index].SetDefault(genes);
        }
    }

    public Gene GetGeneAt(int index) {
        return genes[index];
    }

    private void CreateJellyfish() {
        //Clear();

        ////Simple Jellyfish (FPS Reference creature, Don't ever change!!)
        //genes[0].type = CellTypeEnum.Vein;
        //genes[0].setReferenceDeprecated(3, 10);
        //genes[0].setReferenceDeprecated(5, 20);
        //genes[0].setReferenceDeprecated(4, 1);

        //genes[1].type = CellTypeEnum.Muscle;

        //genes[10].type = CellTypeEnum.Leaf;
        //genes[10].setReferenceDeprecated(1, 10);
        //genes[10].setReferenceDeprecated(2, 1);

        //genes[20].type = CellTypeEnum.Leaf;
        //genes[20].setReferenceDeprecated(1, 20);
        //genes[20].setReferenceDeprecated(0, 1);

        //New Jellyfish using Arrangements
        genes[0].type = CellTypeEnum.Vein;
        genes[0].arrangements[0].isEnabled = true;
        genes[0].arrangements[0].type = ArrangementTypeEnum.Mirror;
        genes[0].arrangements[0].referenceCount = 2;
        genes[0].arrangements[0].gap = 3;
        genes[0].arrangements[0].referenceGene = genes[1];
        genes[0].arrangements[0].arrowIndex = 0;
        genes[0].arrangements[1].isEnabled = true;
        genes[0].arrangements[1].type = ArrangementTypeEnum.Side;
        genes[0].arrangements[1].referenceCount = 1;
        genes[0].arrangements[1].arrowIndex = 6;
        genes[0].arrangements[1].referenceGene = genes[2];

        genes[1].type = CellTypeEnum.Leaf;
        genes[1].arrangements[0].isEnabled = true;
        genes[1].arrangements[0].type = ArrangementTypeEnum.Side;
        genes[1].arrangements[0].referenceCount = 1;
        genes[1].arrangements[0].referenceGene = genes[2];
        genes[1].arrangements[0].arrowIndex = -2;
        genes[1].arrangements[1].isEnabled = true;
        genes[1].arrangements[1].type = ArrangementTypeEnum.Side;
        genes[1].arrangements[1].referenceCount = 1;
        genes[1].arrangements[1].referenceGene = genes[1];
        genes[1].arrangements[1].arrowIndex = 0;

        genes[2].type = CellTypeEnum.Muscle;
    }

    private void CreateString() {
        //Clear();

        //string
        genes[0].type = CellTypeEnum.Vein;
        //genes[0].setReferenceDeprecated(1, 1);
        genes[0].arrangements[0].isEnabled = true;
        genes[0].arrangements[0].referenceGene = genes[1];
        genes[0].arrangements[0].referenceCount = 4;
        genes[0].arrangements[0].arrowIndex = 0;

        genes[1].type = CellTypeEnum.Muscle;



        //genes[0].type = CellTypeEnum.Vein;
        //genes[0].setReferenceDeprecated(1, 1);

        //genes[1].type = CellTypeEnum.Leaf;
        //genes[1].setReferenceDeprecated(1, 2);

        //genes[2].type = CellTypeEnum.Leaf;
        //genes[2].setReferenceDeprecated(1, 3);

        //genes[3].type = CellTypeEnum.Leaf;
        //genes[3].setReferenceDeprecated(1, 4);

        //genes[4].type = CellTypeEnum.Leaf;
        //genes[4].setReferenceDeprecated(1, 5);

        //genes[5].type = CellTypeEnum.Leaf;
        //genes[5].setReferenceDeprecated(1, 6);

        //genes[6].type = CellTypeEnum.Leaf;
        //genes[6].setReferenceDeprecated(1, 7);

        //genes[7].type = CellTypeEnum.Leaf;

        //genes[10].type = CellTypeEnum.Mouth;
    }

    // No references, type = vein
    //public void Clear() {
    //    for (int index = 0; index < genes.Length; index++) {
    //        genes[index].ClearDeprecated();
    //    }
    //}
	
    // Update is called once per frame
	void Update () {
	
	}
}
