using System;
using UnityEngine;
using UnityEngine.UI;

public class GlobalPanel : MonoSingleton<GlobalPanel> {

	//Debug
	public Text worldNameAndTimeText;
	public Text fps;
	public Text pps;

	public Text creatureCount;
	public Text runnersKilledCount;
	public Text sterileKilledCount;

	private int frameCount;
	private float timeCount;
	private float updateTimeCount;
	private float updatePeriod = 1f;

	private int physicsUpdateCount;

	public Text deletedCellCount;

	//Debug -> Creature Pool Count
	public Text creaturePoolCount;

	//Debug -> Cell Pool Count
	public Text cellPoolEggCount;
	public Text cellPoolFungalCount;
	public Text cellPoolJawCount;
	public Text cellPoolLeafCount;
	public Text cellPoolMuscleCount;
	public Text cellPoolRootCount;
	public Text cellPoolShellCount;
	public Text cellPoolVeinCount;

	//Debug -> Gene Cell Pool Count
	public Text geneCellPoolEggCount;
	public Text geneCellPoolFungalCount;
	public Text geneCellPoolJawCount;
	public Text geneCellPoolLeafCount;
	public Text geneCellPoolMuscleCount;
	public Text geneCellPoolRootCount;
	public Text geneCellPoolShellCount;
	public Text geneCellPoolVeinCount;

	//Debug -> Edge Pool Count
	public Text edgePoolCount;

	//Debug -> Edge Pool Count
	public Text veinPoolCount;

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
	public Toggle graphicsWingsForces;
	public Toggle graphicsPeriphery;
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
		creation,
		individual,
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

			if (World.instance.life == null) {
				return;
			}

			//creatures
			creatureCount.text =         "Creatures: "   + World.instance.life.creatureCount;
			runnersKilledCount.text =    "Runners Killed: " + PrisonWall.instance.runnersKilledCount;
			sterileKilledCount.text =    "Sterile Killed: " + World.instance.life.sterileKilledCount;

			//Creature Pool
			creaturePoolCount.text = "Creatures: " + CreaturePool.instance.storedCount + " + " + CreaturePool.instance.loanedCount + " = " + (CreaturePool.instance.storedCount + CreaturePool.instance.loanedCount);

			//Cell Pool
			cellPoolEggCount.text =    "E: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Egg) +    " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Egg) +    " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Egg)    + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Egg));
			cellPoolFungalCount.text = "F: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Fungal) + " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Fungal) + " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Fungal) + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Fungal));
			cellPoolJawCount.text =    "J: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Jaw) +    " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Jaw) +    " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Jaw)    + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Jaw));
			cellPoolLeafCount.text =   "L: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Leaf) +   " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Leaf) +   " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Leaf)   + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Leaf));
			cellPoolMuscleCount.text = "M: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Muscle) + " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Muscle) + " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Muscle) + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Muscle));
			cellPoolRootCount.text =   "R: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Root) +   " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Root) +   " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Root)   + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Root));
			cellPoolShellCount.text =  "S: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Shell) +  " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Shell) +  " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Shell)  + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Shell));
			cellPoolVeinCount.text =   "V: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Vein) +   " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Vein) +   " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Vein)   + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Vein));

			//Cell Pool
			geneCellPoolEggCount.text =    "E: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Egg) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Egg) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Egg) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Egg));
			geneCellPoolFungalCount.text = "F: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Fungal) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Fungal) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Fungal) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Fungal));
			geneCellPoolJawCount.text =    "J: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Jaw) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Jaw) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Jaw) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Jaw));
			geneCellPoolLeafCount.text =   "L: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Leaf) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Leaf) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Leaf) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Leaf));
			geneCellPoolMuscleCount.text = "M: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Muscle) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Muscle) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Muscle) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Muscle));
			geneCellPoolRootCount.text =   "R: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Root) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Root) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Root) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Root));
			geneCellPoolShellCount.text =  "S: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Shell) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Shell) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Shell) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Shell));
			geneCellPoolVeinCount.text =   "V: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Vein) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Vein) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Vein) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Vein));

			deletedCellCount.text = "Deleted Cells: " + World.instance.life.deletedCellCount;
			if (World.instance.life.deletedCellCount == 0) {
				deletedCellCount.color = Color.gray;
			} else {
				deletedCellCount.color = Color.red;
			}

			edgePoolCount.text = "Edges: " + World.instance.life.edgePool.storedCount + " + " + World.instance.life.edgePool.loanedCount + " = " + (World.instance.life.edgePool.storedCount + World.instance.life.edgePool.loanedCount);

			veinPoolCount.text = "Veins: " + World.instance.life.veinPool.storedCount + " + " + World.instance.life.veinPool.loanedCount + " = " + (World.instance.life.veinPool.storedCount + World.instance.life.veinPool.loanedCount);
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

	public void OnClickedStoreLife() {
		World.instance.StoreLife();
	}

	public void OnClickedRestoreLife() {
		World.instance.RestoreLife();
	}
}