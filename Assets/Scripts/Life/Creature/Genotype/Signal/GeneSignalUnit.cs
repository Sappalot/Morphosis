using System.Collections.Generic;

public abstract class GeneSignalUnit {
	public SignalUnitEnum signalUnit;
	public bool isLocked;

	// TODO: move to web, we don't care about rooted or not
	public bool isRooted; // is some nerve listening to what i have to say and that is leading all the way towards some decition/action

	// TODO: move to web, we don't care about rooted or not
	public virtual void MarkThisAndChildrenAsRooted(Genotype genotype, Cell geneCell, SignalUnitEnum signalUnit) {
		isRooted = true;
	}

	// returns all genes that stretches out of geneCell
	public virtual List<GeneNerve> GetExternalGeneNerves() {
		return null;
	}
}
