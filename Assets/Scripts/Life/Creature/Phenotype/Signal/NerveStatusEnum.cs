// 
public enum NerveStatusEnum {
	Void, // this nerve is not used, probably blocked

	Input_GenotypeLocal, // nerve head at owner's signal unit input | nerve tail at signal unit output in same gene cell
	Input_GenotypeExternal, // nerve head at owner's signal unit input | nerve tail at signal unit output in another gene cell
	Input_GenotypeExternalVoid, // nerve head at owner's signal unit input | nerve tail at (signal unit output) in the void

	Output_GenotypeLocal, // nerve head at singnal unit input in same gene cell | nerve tail at owner's signal unit output
	Output_GenotypeExternal, // nerve head at singnal unit input in another gene cell | nerve tail at owner's signal unit output

	Input_PhenotypeIsBlockedByValve,
	Input_PhenotypeListensToTarget,
	Input_PhenotypeListensToVoid,
	Output_PhenotypeSpeakingToTarget,
	Output_PhenotypeSpeakingToUnbuiltOrUnrooted, // we are rooted in genotype, but not in phenotype
}
