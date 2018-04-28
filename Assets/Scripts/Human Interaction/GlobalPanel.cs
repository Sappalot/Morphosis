using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalPanel : MonoSingleton<GlobalPanel> {

	public float frameRate { get; private set; }

	//Debug
	public Text worldNameAndTimeText;
	public Text fps;
	public Text pps;
	public Text memoryUsage;

	public Text creatureAliveCount;
	public Text creatureDeadCount;
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

	// Operation
	public Toggle fpsGuardToggle;
	public Slider fpsGuardSlider;
	public Text fpsGuardText;
	public void OnFpsGuardSliderMoved() {
		fpsGuardText.text = "FPS Guard: " + fpsGuardSlider.value;
	}

	public Toggle physicsTeleport;
	public Toggle physicsTelefrag;
	public Toggle physicsKillFugitive;
	public Toggle physicsKillSterile;
	public Toggle physicsGrow;
	public Toggle physicsDetatch;
	public Toggle physicsOsmosis;

	public Toggle physicsEgg;
	public Toggle physicsFungal;
	public Toggle physicsJaw;
	public Toggle physicsLeaf;
	public Toggle physicsMuscle; public Toggle physicsMuscleEffect;
	public Toggle physicsRoot;
	public Toggle physicsShell;
	public Toggle physicsVein;


	public Slider physicsTimeScaleSilder;
	public Text physicsTimeScaleIsText;
	public Text physicsTimeScaleWantText;
	public RectTransform physicsSliderFillBar;
	public float physicsUpdatesPerSecond;

	public RectTransform physicsAimFillBar;

	//Graphics
	public Toggle graphicsCreatures;
	public Toggle graphicsPeriphery;
	public Toggle graphicsEffects;
	public Toggle graphicsMuscleForces;
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

	// Sound
	public Toggle soundCreatures;

	// Operation
	public Slider garbageCollectPeriodSlider; // time in seconds
	public Text garbageCollectText;
	public Text garbageCollectPeriodText;
	public void OnGarbageColectPeriodSliderMoved() {
		garbageCollectPeriodText.text = garbageCollectPeriodSlider.value == 0 ? "Period: --" : ("Period: " + garbageCollectPeriodSlider.value + " s");
	}


	private void Update () {
		frameCount++;
		timeCount += Time.unscaledDeltaTime;
		updateTimeCount += Time.unscaledDeltaTime;
		if (updateTimeCount > updatePeriod) {
			updateTimeCount = 0;

			//fps
			if (timeCount > 0f) {
				frameRate = frameCount / timeCount;
			}
			fps.text = timeCount > 0f ? string.Format("FPS: {0:F1}", frameRate) : "FPS: ---";
			frameCount = 0;

			//pps
			physicsUpdatesPerSecond = physicsUpdateCount / timeCount;
			pps.text = CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype && timeCount > 0f ? string.Format("PPS: {0:F1}", physicsUpdateCount / timeCount) : "PPS: ---";
			physicsUpdateCount = 0;
			float width = Mathf.Clamp(physicsUpdatesPerSecond * Time.fixedDeltaTime * 9f, 0f, 180f);
			physicsSliderFillBar.sizeDelta = new Vector2(width, physicsSliderFillBar.sizeDelta.y);

			timeCount = 0;

			width = Mathf.Lerp(0f, 180f, Mathf.InverseLerp(0f, 20, World.instance.aimSpeedLowPass));
			physicsAimFillBar.sizeDelta = new Vector2(width, physicsAimFillBar.sizeDelta.y);

			// memoryUsage
			memoryUsage.text = "Heap size: " + (GC.GetTotalMemory(true) / 1000) + " K";

			if (World.instance.life == null) {
				return;
			}

			//creatures
			creatureAliveCount.text =    "Alive: "   + World.instance.life.creatureAliveCount + ", Cells: " + World.instance.life.cellAliveCount;
			creatureDeadCount.text =     "Dead: " + World.instance.life.creatureDeadCount;
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

		//TryRecreateWorld();
	}

	private void FixedUpdate() {
		physicsUpdateCount++;
	}

	public void OnRestartClicked() {
		World.instance.Restart();
	}

	public void OnLoadClicked() {
		World.instance.Load("save.txt");
	}

	public void OnSaveClicked() {
		World.instance.Save("save.txt");
	}


	public void OnClickRecreateScene() {
		World.instance.Save("temp.txt");
		StartCoroutine("load");
		ActivateScene();
		System.GC.Collect();
	}

	private bool shouldRecreate = true;
	private void TryRecreateWorld() {
		if (shouldRecreate) {
			World.instance.Load("temp.txt");
			shouldRecreate = false;
		}
	}

	public void OnClickLoadBack() {
		World.instance.Load("temp.txt");
	}



	private AsyncOperation async;

	private IEnumerator load() {
		async = SceneManager.LoadSceneAsync("MainScene");
		async.allowSceneActivation = false;
		while (!async.isDone) {
			yield return null;
		}
		Debug.Log("Done loading scene");
		
	}

	private void ActivateScene() {
		async.allowSceneActivation = true;
	}
}