using UnityEngine;

public abstract class GeneSurroundingSensorChannel : GeneSignalUnit {
	protected IGenotypeDirtyfy genotypeDirtyfy;

	public abstract void Defaultify();
	public abstract void Mutate(float strength);
	public abstract void Randomize();

}
