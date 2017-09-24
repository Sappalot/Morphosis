

using System.Collections.Generic;
using UnityEngine;

public static class GenotypeUtil {
	public static Gene[] Combine(List<Gene[]> genomes) {
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
}