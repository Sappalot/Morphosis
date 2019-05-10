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
	public Text creatureDeadByAgeCount;
	public Text creatureDeadByBreakingCount;
	public Text creatureDeadByEscapingCount;

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

	//Debug -> event Marker Pool Count
	public Text eventSymbolPoolCount;

	//Debug -> particle Pool Count
	public Text particlePoolCellBirthCount;
	public Text particlePoolCellBleedCount;
	public Text particlePoolCellScatterCount;
	public Text particlePoolCellTeleportCount;

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
			creatureDeadByAgeCount.text = "...by age: " + World.instance.life.creatureDeadByAgeCount;
			creatureDeadByBreakingCount.text = "...by breaking: " + World.instance.life.creatureDeadByBreakingCount;
			creatureDeadByEscapingCount.text = "...by escaping: " + World.instance.life.creatureDeadByEscapingCount;
			

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
				deletedCellCount.color = ColorScheme.instance.grayedOut;
			} else {
				deletedCellCount.color = Color.red;
			}

			edgePoolCount.text = "Edges: " + Morphosis.instance.edgePool.storedCount + " + " + Morphosis.instance.edgePool.loanedCount + " = " + (Morphosis.instance.edgePool.storedCount + Morphosis.instance.edgePool.loanedCount);

			veinPoolCount.text = "Veins: " + Morphosis.instance.veinPool.storedCount + " + " + Morphosis.instance.veinPool.loanedCount + " = " + (Morphosis.instance.veinPool.storedCount + Morphosis.instance.veinPool.loanedCount);

			eventSymbolPoolCount.text = "Event Symbols: " + EventSymbolPool.instance.storedCount + " + " + EventSymbolPool.instance.loanedCount + " = " + (EventSymbolPool.instance.storedCount + EventSymbolPool.instance.loanedCount);

			particlePoolCellBirthCount.text = "Cell Birth: " + ParticlePool.instance.GetStoredParticlesCount(ParticleTypeEnum.cellBirth) + " + " + ParticlePool.instance.GetLoanedParticlesCount(ParticleTypeEnum.cellBirth) + " = " + (ParticlePool.instance.GetStoredParticlesCount(ParticleTypeEnum.cellBirth) + ParticlePool.instance.GetLoanedParticlesCount(ParticleTypeEnum.cellBirth));
			particlePoolCellBleedCount.text = "Cell Bleed: " + ParticlePool.instance.GetStoredParticlesCount(ParticleTypeEnum.cellBleed) + " + " + ParticlePool.instance.GetLoanedParticlesCount(ParticleTypeEnum.cellBleed) + " = " + (ParticlePool.instance.GetStoredParticlesCount(ParticleTypeEnum.cellBleed) + ParticlePool.instance.GetLoanedParticlesCount(ParticleTypeEnum.cellBleed));
			particlePoolCellScatterCount.text = "Cell Scatter: " + ParticlePool.instance.GetStoredParticlesCount(ParticleTypeEnum.cellScatter) + " + " + ParticlePool.instance.GetLoanedParticlesCount(ParticleTypeEnum.cellScatter) + " = " + (ParticlePool.instance.GetStoredParticlesCount(ParticleTypeEnum.cellScatter) + ParticlePool.instance.GetLoanedParticlesCount(ParticleTypeEnum.cellScatter));
			particlePoolCellTeleportCount.text = "Cell Teleport: " + ParticlePool.instance.GetStoredParticlesCount(ParticleTypeEnum.cellTeleport) + " + " + ParticlePool.instance.GetLoanedParticlesCount(ParticleTypeEnum.cellTeleport) + " = " + (ParticlePool.instance.GetStoredParticlesCount(ParticleTypeEnum.cellTeleport) + ParticlePool.instance.GetLoanedParticlesCount(ParticleTypeEnum.cellTeleport));

			PhenotypePhysicsPanel.instance.UpdateFpsSliderText();
		}
	}

	private void FixedUpdate() {
		physicsUpdateCount++;
	}

	public void OnRestartClicked() {
		if (Morphosis.isInterferredByOtheActions()) { return; }
		MouseAction.instance.actionState = MouseActionStateEnum.restartingWorld;

		ProgressBar.instance.gameObject.SetActive(true);
		ProgressBar.instance.heading.text = "Restarting";
		ProgressBar.instance.ResetForRestart(Freezer.instance.creatureCount, World.instance.life.creatureAliveCount);

		SelectPausePhysics();
		Freezer.instance.Save();
		Morphosis.instance.Restart(() => {
			Debug.Log("Morfosis restarted");
			ProgressBar.instance.gameObject.SetActive(false);
			MouseAction.instance.actionState = MouseActionStateEnum.free;
		});
	}

	public void OnLoadClicked() {
		if (Morphosis.isInterferredByOtheActions()) { return; }
		MouseAction.instance.actionState = MouseActionStateEnum.loadingWorld;

		ProgressBar.instance.gameObject.SetActive(true);
		ProgressBar.instance.heading.text = "Loading World";
		WorldData worldData = World.instance.LoadWorldData("save.txt");
		ProgressBar.instance.ResetForLoad(Freezer.instance.creatureCount, World.instance.life.creatureAliveCount, worldData.metaCreatureCount);

		Morphosis.instance.LoadWorld(worldData,
		//onDone
		() => {

			ProgressBar.instance.gameObject.SetActive(false);
			Debug.Log("World loaded");
			MouseAction.instance.actionState = MouseActionStateEnum.free;
		});
	}

	public void OnSaveClicked() {
		if (Morphosis.isInterferredByOtheActions()) { return; }
		MouseAction.instance.actionState = MouseActionStateEnum.savingWorld;
		World.instance.Save();
		MouseAction.instance.actionState = MouseActionStateEnum.free;
	}

	public void OnPausePhysicsClicked() {
		if (Morphosis.isInterferredByOtheActions()) { return; }

		SelectPausePhysics();
	}

	public void SelectPausePhysics() {
		if (!isRunPhysicsGrayOut) {
			pausePhysicsImage.color = ColorScheme.instance.selectedButton;
			runPhysicsImage.color = ColorScheme.instance.notSelectedButton;
			
		}
		isRunPhysics = false;
	}

	public void OnRunPhysicsClicked() {
		if (MouseAction.instance.actionState != MouseActionStateEnum.free) { return; }

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
		if (MouseAction.instance.actionState != MouseActionStateEnum.free) { return; }

		World.instance.AddHistoryEvent(new HistoryEvent(historyGraphNote.text, false, new Color(0.5f, 0.5f, 0f)));
	}

	//Called on all MonoBehaviours on quitting application
	void OnApplicationQuit() {
		Freezer.instance.Save();
	}

	public bool isWritingHistoryNote {
		get {
			return historyGraphNote.isFocused;
		}
	}
}