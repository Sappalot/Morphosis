using System;
using UnityEngine;
using UnityEngine.UI;

public class GlobalPanel : MonoSingleton<GlobalPanel> {

	//Debug
	public Text worldNameAndTimeText;
	public Text fps;
	public Text pps;

	public Text creatureCount;
	public Text soulsDirtyCount;
	public Text soulsCleanCount;
	public Text soulsDeadButUsedCount;
	public Text soulsLostCount;
	public Text runnersKilledCount;
	public Text sterileKilledCount;

	private int frameCount;
	private float timeCount;
	private float updateTimeCount;
	private float updatePeriod = 1f;

	private int physicsUpdateCount;

	//Debug -> Cell Pool Count
	public Text cellPoolEggCount;
	public Text cellPoolFungalCount;
	public Text cellPoolJawCount;
	public Text cellPoolLeafCount;
	public Text cellPoolMuscleCount;
	public Text cellPoolRootCount;
	public Text cellPoolShellCount;
	public Text cellPoolVeinCount;

	//World
	public void UpdateWorldNameAndTime(string worldName, ulong worldTicks) {
		worldNameAndTimeText.text = worldName + " " + TimeUtil.GetTimeString((ulong)(worldTicks * Time.fixedDeltaTime));
	}

	//Physics
	public Toggle physicsUpdateMetabolism;
	public Toggle physicsApplyWingForce;
	public Toggle physicsTeleport;
	public Toggle physicsJawRam;

	public Slider timeSpeedSilder;
	public Text physicsTimeSpeedText;
	public float physicsUpdatesPerSecond;
	public RectTransform physicsSliderFillBar;


	//Graphics
	public Toggle graphicsCreatures;
	public Toggle graphicsRenderWings;
	public Dropdown graphicsCellDropdown;
	public enum CellGraphicsEnum {
		type,
		energy,
		effect,
		effectCreature,
		leafExposure,
		childCountCreature,
		predatorPray,
		typeAndPredatorPray,
		update,
	}
	[HideInInspector]
	public CellGraphicsEnum graphicsCell {
		get {
			return (CellGraphicsEnum)graphicsCellDropdown.value;
		}
	}

	// Effects
	public Toggle effectsPlaySound;
	public Toggle effectsShowParticles;

	private void Update () {
		frameCount++;
		timeCount += Time.unscaledDeltaTime;
		updateTimeCount += Time.unscaledDeltaTime;
		if (updateTimeCount > updatePeriod) {
			updateTimeCount = 0;

			//fps
			fps.text = timeCount > 0f ? string.Format("FPS: {0:F1}", frameCount / timeCount) : "FPS: ---";
			frameCount = 0;

			//pps
			physicsUpdatesPerSecond = physicsUpdateCount / timeCount;
			pps.text = CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype && timeCount > 0f ? string.Format("PPS: {0:F1}", physicsUpdateCount / timeCount) : "PPS: ---";
			physicsUpdateCount = 0;
			float width = Mathf.Clamp(physicsUpdatesPerSecond * Time.fixedDeltaTime * 9f, 0f, 180f);
			physicsSliderFillBar.sizeDelta = new Vector2(width, physicsSliderFillBar.sizeDelta.y);

			timeCount = 0;

			//creatures & souls
			creatureCount.text =         "Creatures: "   + Life.instance.creatureCount;
			soulsDirtyCount.text =       "Alive & dirty: " + Life.instance.soulUnupdatedCount;
			soulsCleanCount.text =       "Alive & clean: " + (Life.instance.soulUpdatedCount - Life.instance.soulsDeadButUsedCount);
			soulsDeadButUsedCount.text = "Dead & used: "   + Life.instance.soulsDeadButUsedCount;
			soulsLostCount.text =        "Dead & lost: "   + Life.instance.soulsLostCount;
			runnersKilledCount.text =    "Runners Killed: " + PrisonWall.instance.runnersKilledCount;
			sterileKilledCount.text =    "Sterile Killed: " + Life.instance.sterileKilledCount;

			//Cell Pool
			cellPoolEggCount.text =    "E: " + CellPool.instance.GetStoredCellCount(CellTypeEnum.Egg) +    " + " + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Egg) +    " = " + (CellPool.instance.GetStoredCellCount(CellTypeEnum.Egg)    + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Egg));
			cellPoolFungalCount.text = "F: " + CellPool.instance.GetStoredCellCount(CellTypeEnum.Fungal) + " + " + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Fungal) + " = " + (CellPool.instance.GetStoredCellCount(CellTypeEnum.Fungal) + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Fungal));
			cellPoolJawCount.text =    "J: " + CellPool.instance.GetStoredCellCount(CellTypeEnum.Jaw) +    " + " + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Jaw) +    " = " + (CellPool.instance.GetStoredCellCount(CellTypeEnum.Jaw)    + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Jaw));
			cellPoolLeafCount.text =   "L: " + CellPool.instance.GetStoredCellCount(CellTypeEnum.Leaf) +   " + " + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Leaf) +   " = " + (CellPool.instance.GetStoredCellCount(CellTypeEnum.Leaf)   + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Leaf));
			cellPoolMuscleCount.text = "M: " + CellPool.instance.GetStoredCellCount(CellTypeEnum.Muscle) + " + " + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Muscle) + " = " + (CellPool.instance.GetStoredCellCount(CellTypeEnum.Muscle) + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Muscle));
			cellPoolRootCount.text =   "R: " + CellPool.instance.GetStoredCellCount(CellTypeEnum.Root) +   " + " + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Root) +   " = " + (CellPool.instance.GetStoredCellCount(CellTypeEnum.Root)   + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Root));
			cellPoolShellCount.text =  "S: " + CellPool.instance.GetStoredCellCount(CellTypeEnum.Shell) +  " + " + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Shell) +  " = " + (CellPool.instance.GetStoredCellCount(CellTypeEnum.Shell)  + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Shell));
			cellPoolVeinCount.text =   "V: " + CellPool.instance.GetStoredCellCount(CellTypeEnum.Vein) +   " + " + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Vein) +   " = " + (CellPool.instance.GetStoredCellCount(CellTypeEnum.Vein)   + CellPool.instance.GetLoanedCellCount(CellTypeEnum.Vein));
		}
	}

	private void FixedUpdate() {
		physicsUpdateCount++;
	}

	public void OnRestartClicked() {
		World.instance.Restart();
	}

	public void OnLoadClicked() {
		World.instance.Load();
	}

	public void OnSaveClicked() {
		World.instance.Save();
	}
}