using UnityEngine;

public abstract class GeneSurroundingSensorChannel : GeneSignalUnit {
	public abstract void Defaultify();
	public abstract void Mutate(float strength);
	public abstract void Randomize();

}
