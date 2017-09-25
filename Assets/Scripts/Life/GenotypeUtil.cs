

using System.Collections.Generic;
using UnityEngine;

public static class GenotypeUtil {
	public static Gene[] CombineGenomeCaorse(List<Gene[]> genomes) {
		Gene[] combination = new Gene[Genotype.genomeLength];

		for (int index = 0; index < Genotype.genomeLength; index++) {
			int pick = Random.Range(0, genomes.Count);
			combination[index] = genomes[pick][index];
		}
		for (int index = 0; index < Genotype.genomeLength; index++) {
			combination[index].SetReferenceGeneFromReferenceGeneIndex(combination);
		}
		return combination;
	}

	public static Gene[] CombineGenomeFine(List<Gene[]> genomes) {
		Gene[] combination = new Gene[Genotype.genomeLength];

		for (int geneIndex = 0; geneIndex < Genotype.genomeLength; geneIndex++) {
			combination[geneIndex] = new Gene(geneIndex);
			int sourceGenome = Random.Range(0, genomes.Count);
			combination[geneIndex].type = genomes[sourceGenome][geneIndex].type;
			for (int arrangementIndex = 0; arrangementIndex < 3; arrangementIndex++) {
				if (Random.Range(0, 6) == 0) {
					sourceGenome = Random.Range(0, genomes.Count);
				}
				combination[geneIndex].arrangements[arrangementIndex] = genomes[sourceGenome][geneIndex].arrangements[arrangementIndex];
			}
		}
		for (int index = 0; index < Genotype.genomeLength; index++) {
			combination[index].SetReferenceGeneFromReferenceGeneIndex(combination);
		}
		return combination;
	}

}