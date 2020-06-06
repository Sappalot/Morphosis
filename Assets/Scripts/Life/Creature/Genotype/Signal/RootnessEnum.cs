using System;

[Serializable]
public enum RootnessEnum {
	Unrooted, // Genotype: will never be used | Phenotype: will never be used
	Rooted, // Genotype: is connected to something that matters | Phenotype: is connected to something that matters, all cells are built to reach this root
	Rootable, // Genotype: NOT USED | Phenotype: will be connected to something that matters, once the required cells are built
}