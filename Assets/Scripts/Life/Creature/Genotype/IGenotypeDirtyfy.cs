public interface IGenotypeDirtyfy {
	//  regenerate new geneCell pattern from new genome, regenterate new nerves, regrow cellStructure according to old genome, update inter cell stuff
	void ReforgeGeneCellPatternAndForward();


	// regenerate new nerves, regrow cellStructure according to old genome, update inter cell stuff
	void ReforgeInterGeneCellAndForward();


	// regrow cellStructure according to old genome, update inter cell stuff
	void ReforgeCellPatternAndForward();
}
