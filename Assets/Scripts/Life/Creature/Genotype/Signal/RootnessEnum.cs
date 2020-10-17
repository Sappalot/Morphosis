using System;

[Serializable]
public enum RootnessEnum {
	Unrooted, // Genotype: will never be used | Phenotype: will never be used
	Rooted, // Genotype: is connected to something that matters | Phenotype: is connected to something that matters, all cells are built to reach this root
	Rootable, // Genotype: THE ENUM IS NOT USED (since stuff are allways rooted when rootable, 'unbuilt' makes no sense here ) | Phenotype: will be connected to something that matters, once the required cells are built
}