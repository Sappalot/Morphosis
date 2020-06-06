// 
public enum NerveStatusEnum {
	Void, // this nerve is not used, probably blocked

	InputLocal, // nerve head at owner's signal unit input | nerve tail at signal unit output in same gene cell
	InputExternal, // nerve head at owner's signal unit input | nerve tail at signal unit output in another gene cell
	OutputLocal, // nerve head at singnal unit input in same gene cell | nerve tail at owner's signal unit output
	OutputExternal, // nerve head at singnal unit input in another gene cell | nerve tail at owner's signal unit output
}
