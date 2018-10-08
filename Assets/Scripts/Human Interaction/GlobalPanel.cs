using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalPanel : MonoSingleton<GlobalPanel> {
	public float frameRate { get; private set; }
	public float physicsUpdatesPerSecond { get; private set; }

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

	//Time
	[HideInInspector]
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

	//Graphics general
	public Toggle graphicsRelationsToggle;

	//phenotype graphics
	public Toggle graphicsCreaturesToggle;
	public Toggle graphicsPeripheryToggle;
	public Toggle graphicsEffectsToggle;
	public Toggle graphicsMuscleForcesToggle;


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
			runnersKilledCount.text = "Runners Killed: " + TerrainPerimeter.instance.runnersKilledCount;
			sterileKilledCount.text = "Sterile Killed: " + World.instance.life.sterileKilledCount;

			//Creature Pool
			creaturePoolCount.text = "Creatures: " + CreaturePool.instance.storedCount + " + " + CreaturePool.instance.loanedCount + " = " + (CreaturePool.instance.storedCount + CreaturePool.instance.loanedCount);

			//Cell Pool
			cellPoolEggCount.text = "E: " + Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Egg) + " + " + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Egg) + " = " + (Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Egg) + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Egg));
			cellPoolFungalCount.text = "F: " + Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Fungal) + " + " + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Fungal) + " = " + (Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Fungal) + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Fungal));
			cellPoolJawCount.text = "J: " + Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Jaw) + " + " + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Jaw) + " = " + (Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Jaw) + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Jaw));
			cellPoolLeafCount.text = "L: " + Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Leaf) + " + " + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Leaf) + " = " + (Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Leaf) + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Leaf));
			cellPoolMuscleCount.text = "M: " + Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Muscle) + " + " + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Muscle) + " = " + (Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Muscle) + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Muscle));
			cellPoolRootCount.text = "R: " + Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Root) + " + " + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Root) + " = " + (Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Root) + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Root));
			cellPoolShellCount.text = "S: " + Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Shell) + " + " + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Shell) + " = " + (Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Shell) + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Shell));
			cellPoolVeinCount.text = "V: " + Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Vein) + " + " + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Vein) + " = " + (Morphosis.instance.cellPool.GetStoredCellCount(CellTypeEnum.Vein) + Morphosis.instance.cellPool.GetLoanedCellCount(CellTypeEnum.Vein));

			//Cell Pool
			geneCellPoolEggCount.text = "E: " + Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Egg) + " + " + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Egg) + " = " + (Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Egg) + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Egg));
			geneCellPoolFungalCount.text = "F: " + Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Fungal) + " + " + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Fungal) + " = " + (Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Fungal) + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Fungal));
			geneCellPoolJawCount.text = "J: " + Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Jaw) + " + " + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Jaw) + " = " + (Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Jaw) + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Jaw));
			geneCellPoolLeafCount.text = "L: " + Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Leaf) + " + " + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Leaf) + " = " + (Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Leaf) + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Leaf));
			geneCellPoolMuscleCount.text = "M: " + Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Muscle) + " + " + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Muscle) + " = " + (Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Muscle) + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Muscle));
			geneCellPoolRootCount.text = "R: " + Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Root) + " + " + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Root) + " = " + (Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Root) + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Root));
			geneCellPoolShellCount.text = "S: " + Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Shell) + " + " + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Shell) + " = " + (Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Shell) + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Shell));
			geneCellPoolVeinCount.text = "V: " + Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Vein) + " + " + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Vein) + " = " + (Morphosis.instance.geneCellPool.GetStoredCellCount(CellTypeEnum.Vein) + Morphosis.instance.geneCellPool.GetLoanedCellCount(CellTypeEnum.Vein));

			deletedCellCount.text = "Deleted Cells: " + World.instance.life.deletedCellCount;
			if (World.instance.life.deletedCellCount == 0) {
				deletedCellCount.color = Color.gray;
			} else {
				deletedCellCount.color = Color.red;
			}

			edgePoolCount.text = "Edges: " + Morphosis.instance.edgePool.storedCount + " + " + Morphosis.instance.edgePool.loanedCount + " = " + (Morphosis.instance.edgePool.storedCount + Morphosis.instance.edgePool.loanedCount);

			veinPoolCount.text = "Veins: " + Morphosis.instance.veinPool.storedCount + " + " + Morphosis.instance.veinPool.loanedCount + " = " + (Morphosis.instance.veinPool.storedCount + Morphosis.instance.veinPool.loanedCount);

			PhenotypePhysicsPanel.instance.UpdateFpsSliderText();
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