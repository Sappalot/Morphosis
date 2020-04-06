public abstract class GeneSignalUnit {
	public SignalUnitEnum signalUnit;
	public bool isLocked;

	public bool isUsedInternal; // is nome nerve listening to what i have to say and that is leading all the way towards some decition/action

	public virtual void MarkThisAndChildrenAsUsedInternal(Gene gene) {
		isUsedInternal = true;
	}
}
