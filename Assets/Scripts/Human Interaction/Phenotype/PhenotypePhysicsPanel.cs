using UnityEngine;
using UnityEngine.UI;

public class PhenotypePhysicsPanel : MonoSingleton<PhenotypePhysicsPanel> {

	public Toggle teleport;
	public Toggle telepoke;
	public Toggle killEscaping;
	public Toggle killOld;
	public Toggle grow;
	public Toggle detatch;
	public Toggle flux;
	public Toggle frictionWater;
	public Toggle hingeMomentum;

	//Fuction: what cell is doing (and its production), Effect: its cost
	public Toggle functionEgg;
	public Toggle functionJaw;
	public Toggle functionLeaf;
	public Toggle functionMuscle;

	public void OnChangedWaterReactiveForce() {
		foreach (Creature c in World.instance.life.creatures) {
			c.phenotype.SetFrictionNormal();
		}
	}

}