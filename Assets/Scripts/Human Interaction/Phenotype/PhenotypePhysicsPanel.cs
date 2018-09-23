using UnityEngine;
using UnityEngine.UI;

public class PhenotypePhysicsPanel : MonoSingleton<PhenotypePhysicsPanel> {
	public Toggle fpsGuardToggle;
	public Text fpsGuardText;
	public Slider fpsGuardSlider;
	private bool ignoreSliderMoved;

	//public 
	public void OnFpsGuardSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		World.instance.terrain.pidCircle.fpsGoal = fpsGuardSlider.value;
		UpdateFpsSliderText();
	}

	public void UpdateSliderAndToggleValue() {
		ignoreSliderMoved = true;
		fpsGuardSlider.value = World.instance.terrain.pidCircle.fpsGoal;
		fpsGuardToggle.isOn = World.instance.terrain.pidCircle.isOn;
		ignoreSliderMoved = false;
		UpdateFpsSliderText();
	}

	public void OnFpsGuardToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}
		World.instance.terrain.pidCircle.isOn = fpsGuardToggle.isOn;
	}

	public void UpdateFpsSliderText() {
		fpsGuardText.text = "Fps: " + World.instance.terrain.pidCircle.fpsGoal;
	}

	public Toggle teleport;
	public Toggle telepoke;
	public Toggle killFugitive;
	public Toggle killSterile;
	public Toggle grow;
	public Toggle detatch;
	public Toggle flux;

	//Fuction: what cell is doing (and its production), Effect: its cost
	public Toggle functionEgg;
	public Toggle functionFungal;
	public Toggle functionJaw;
	public Toggle functionLeaf;
	public Toggle functionMuscle; public Toggle effectMuscle; //figure out how to separate function and production
	public Toggle functionRoot;
	public Toggle functionShell;
	public Toggle functionVein;
}