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
	public Text worldSaveDirerectory;

	//Physics
	public bool isRunPhysics;
	public Image pausePhysicsImage;
	public Image runPhysicsImage;
	public Image physicsGrayOut;

	private bool m_isRunPhysicsGrayOut;
	public bool isRunPhysicsGrayOut {
		private get {
			return m_isRunPhysicsGrayOut;
		}
		set {
			m_isRunPhysicsGrayOut = value;
			physicsGrayOut.gameObject.SetActive(value);
		}
	}
	//--
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

	private void UpdateFpsSliderText() {
		fpsGuardText.text = "Fps: " + World.instance.terrain.pidCircle.fpsGoal;
	}
	//ignoreSliderMoved =treue;
	//fertilizeSlider.value =               GenePanel.instance.selectedGene.eggCellFertilizeThreshold;
	//ignoreSliderMoved = false;
	//---

	public Toggle physicsTeleport;
	public Toggle physicsTelepoke;
	public Toggle physicsKillFugitive;
	public Toggle physicsKillSterile;
	public Toggle physicsGrow;
	public Toggle physicsDetatch;
	public Toggle physicsFlux;

	public Toggle physicsEgg;
	public Toggle physicsFungal;
	public Toggle physicsJaw;
	public Toggle physicsLeaf;
	public Toggle physicsMuscle; public Toggle physicsMuscleEffect;
	public Toggle physicsRoot;
	public Toggle physicsShell;
	public Toggle physicsVein;

	public float physicsUpdatesPerSecond { get; private set; }

	public RectTransform physicsAimFillBar;

	
	

	// Sound
	public Toggle soundCreatures;

	public override void Init() {
		OnRunPhysicsClicked();
	}

	// History
	public InputField historyGraphNote;

	private void Update() {
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
			pps.text = CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && timeCount > 0f ? string.Format("PPS: {0:F1}", physicsUpdateCount / timeCount) : "PPS: ---";
			physicsUpdateCount = 0;
			timeCount = 0;

			// memoryUsage
			memoryUsage.text = "Heap size: " + (GC.GetTotalMemory(true) / 1000) + " K";

			if (World.instance.life == null) {
				return;
			}

			//creatures
			creatureAliveCount.text = "Alive: " + World.instance.life.creatureAliveCount + ", Cells: " + World.instance.life.cellAliveCount;
			creatureDeadCount.text = "Dead: " + World.instance.life.creatureDeadCount;
			runnersKilledCount.text = "Runners Killed: " + PrisonWall.instance.runnersKilledCount;
			sterileKilledCount.text = "Sterile Killed: " + World.instance.life.sterileKilledCount;

			//Creature Pool
			creaturePoolCount.text = "Creatures: " + CreaturePool.instance.storedCount + " + " + CreaturePool.instance.loanedCount + " = " + (CreaturePool.instance.storedCount + CreaturePool.instance.loanedCount);

			//Cell Pool
			cellPoolEggCount.text = "E: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Egg) + " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Egg) + " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Egg) + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Egg));
			cellPoolFungalCount.text = "F: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Fungal) + " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Fungal) + " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Fungal) + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Fungal));
			cellPoolJawCount.text = "J: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Jaw) + " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Jaw) + " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Jaw) + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Jaw));
			cellPoolLeafCount.text = "L: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Leaf) + " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Leaf) + " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Leaf) + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Leaf));
			cellPoolMuscleCount.text = "M: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Muscle) + " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Muscle) + " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Muscle) + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Muscle));
			cellPoolRootCount.text = "R: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Root) + " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Root) + " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Root) + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Root));
			cellPoolShellCount.text = "S: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Shell) + " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Shell) + " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Shell) + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Shell));
			cellPoolVeinCount.text = "V: " + World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Vein) + " + " + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Vein) + " = " + (World.instance.life.cellPool.GetStoredCellCount(CellTypeEnum.Vein) + World.instance.life.cellPool.GetLoanedCellCount(CellTypeEnum.Vein));

			//Cell Pool
			geneCellPoolEggCount.text = "E: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Egg) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Egg) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Egg) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Egg));
			geneCellPoolFungalCount.text = "F: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Fungal) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Fungal) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Fungal) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Fungal));
			geneCellPoolJawCount.text = "J: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Jaw) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Jaw) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Jaw) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Jaw));
			geneCellPoolLeafCount.text = "L: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Leaf) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Leaf) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Leaf) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Leaf));
			geneCellPoolMuscleCount.text = "M: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Muscle) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Muscle) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Muscle) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Muscle));
			geneCellPoolRootCount.text = "R: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Root) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Root) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Root) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Root));
			geneCellPoolShellCount.text = "S: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Shell) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Shell) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Shell) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Shell));
			geneCellPoolVeinCount.text = "V: " + World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Vein) + " + " + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Vein) + " = " + (World.instance.life.geneCellPool.GetStoredCellCount(CellTypeEnum.Vein) + World.instance.life.geneCellPool.GetLoanedCellCount(CellTypeEnum.Vein));

			deletedCellCount.text = "Deleted Cells: " + World.instance.life.deletedCellCount;
			if (World.instance.life.deletedCellCount == 0) {
				deletedCellCount.color = Color.gray;
			} else {
				deletedCellCount.color = Color.red;
			}

			edgePoolCount.text = "Edges: " + World.instance.life.edgePool.storedCount + " + " + World.instance.life.edgePool.loanedCount + " = " + (World.instance.life.edgePool.storedCount + World.instance.life.edgePool.loanedCount);

			veinPoolCount.text = "Veins: " + World.instance.life.veinPool.storedCount + " + " + World.instance.life.veinPool.loanedCount + " = " + (World.instance.life.veinPool.storedCount + World.instance.life.veinPool.loanedCount);

			UpdateFpsSliderText();
		}
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
		World.instance.Save();
	}

	public void OnPausePhysicsClicked() {
		SelectPausePhysics();
	}

	public void SelectPausePhysics() {
		if (!isRunPhysicsGrayOut) {
			pausePhysicsImage.color = ColorScheme.instance.selectedButton;
			runPhysicsImage.color = ColorScheme.instance.notSelectedButton;
			isRunPhysics = false;
		}
	}

	public void OnRunPhysicsClicked() {
		SelectRunPhysics();
	}

	public void SelectRunPhysics() {
		if (!isRunPhysicsGrayOut) {
			pausePhysicsImage.color = ColorScheme.instance.notSelectedButton;
			runPhysicsImage.color = ColorScheme.instance.selectedButton;
			isRunPhysics = true;
		}
	}

	public void OnAddHistoryNoteClicked() {
		World.instance.AddHistoryEvent(new HistoryEvent(historyGraphNote.text, false, new Color(0.5f, 0.5f, 0f)));
	}

	public bool isWritingHistoryNote {
		get {
			return historyGraphNote.isFocused;
		}
	}
}