using UnityEngine;
using SerializerFree;
using SerializerFree.Serializers;
using System.IO;
using System.Collections.Generic;

public class World : MonoSingleton<World> {
	public Life life;
	public Terrain terrain;
	public Camera worldCamera;
	public History history = new History();
	[HideInInspector]
	public ulong worldTicks = 0;

	private string worldName = "Gaia";
	private bool doSave = false;
	private List<HistoryEvent> historyEvents = new List<HistoryEvent>();

	public void KillAllCreatures() {
		life.KillAllCreatures();
		CreatureSelectionPanel.instance.ClearSelection();
	}

	private void Start () {
		//for (int y = 0; y < 32; y++) {
		//	for (int x = 0; x < 32; x++) {
		//		GameObject.Instantiate(cellPrefab, new Vector3(10f + x * 2f, 10f + y * 2f, 0f), Quaternion.identity, this.transform);
		//	}
		//}

		//test, OK with 24 * 24 (18 cells per creature) ~ 27 FPS :)
		//including: turn hinged neighbours to correct angle, just one test string creature
		//excluding: turn cell graphics to correct angle, scale mussle cells
		//World.instance.life.EvoFixedUpdate(fixedTime);

		history.Init();
		terrain.Init();
		GraphPlotter.instance.history = history;
	}

	private void Update() {
		//Handle time from here to not get locked out
		if ((!GlobalPanel.instance.isRunPhysics || CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) && !doSave) {
			Time.timeScale = 0f;
			life.UpdateStructure();
		} else if (GlobalPanel.instance.physicsUpdatesPerSecond == 0f) {
			//kick start physics
			Time.timeScale = 1f;
		}

		if (PhenotypeGraphicsPanel.instance.graphicsCreatures.isOn) {
			life.UpdateGraphics();
		}
	}

	public void AddHistoryEvent(HistoryEvent historyEvent) {
		historyEvents.Add(historyEvent);
	}

	private void FixedUpdate() {
		life.UpdateStructure();
		
		life.UpdatePhysics(worldTicks);
		if (GlobalPanel.instance.physicsTeleport.isOn) {
			Portals.instance.UpdatePhysics(World.instance.life.creatures, worldTicks);
		}
		if (GlobalPanel.instance.physicsKillFugitive.isOn) {
			PrisonWall.instance.UpdatePhysics(World.instance.life.creatures, worldTicks);
		}
		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, worldTicks);
		if (worldTicks % 20 == 0) {

			if (worldTicks == 0) {
				Record record = new Record();
				record.SetTagText("Big Bang", Color.white, true);
				record.Set(RecordEnum.fps,             0);
				record.Set(RecordEnum.pps,             0);
				record.Set(RecordEnum.cellCountTotal,  0);
				record.Set(RecordEnum.cellCountEgg,    0);
				record.Set(RecordEnum.cellCountFungal, 0);
				record.Set(RecordEnum.cellCountJaw,    0);
				record.Set(RecordEnum.cellCountLeaf,   0);
				record.Set(RecordEnum.cellCountMuscle, 0);
				record.Set(RecordEnum.cellCountRoot,   0);
				record.Set(RecordEnum.cellCountShell , 0);
				record.Set(RecordEnum.cellCountVein,   0);
				record.Set(RecordEnum.creatureCount,   0);
				record.Set(RecordEnum.creatureBirthsPerSecond, 0);
				record.Set(RecordEnum.creatureDeathsPerSecond, 0);

				history.AddRecord(record);
				GraphPlotter.instance.MakeDirty();
			} else {
				if(doSave) {
					AddHistoryEvent(new HistoryEvent("Saved", true, Color.white));
					CreateRecord();
					DoSave("save.txt");
					doSave = false;
				} else {
					CreateRecord();
				}
			}
		}

		terrain.UpdatePhysics();

		worldTicks++; //The only place where time is increased
	}

	public void CreateRecord() {
		Record record = new Record();
		if (historyEvents.Count > 0) {
			string eventText = "";
			bool line = false;
			Color averageColor = Color.black;
			for (int i = 0; i < historyEvents.Count; i++) {
				eventText += historyEvents[i].text + " ";
				line |= historyEvents[i].showLine;
				averageColor += historyEvents[i].color;
			}
			averageColor /= historyEvents.Count;
			historyEvents.Clear();
			record.SetTagText(eventText, averageColor, line);
		}

		record.Set(RecordEnum.fps, GlobalPanel.instance.frameRate);
		record.Set(RecordEnum.pps, GlobalPanel.instance.physicsUpdatesPerSecond);

		record.Set(RecordEnum.cellCountTotal,  life.cellAliveCount);
		record.Set(RecordEnum.cellCountEgg,    life.GetCellAliveCount(CellTypeEnum.Egg));
		record.Set(RecordEnum.cellCountFungal, life.GetCellAliveCount(CellTypeEnum.Fungal));
		record.Set(RecordEnum.cellCountJaw,    life.GetCellAliveCount(CellTypeEnum.Jaw));
		record.Set(RecordEnum.cellCountLeaf,   life.GetCellAliveCount(CellTypeEnum.Leaf));
		record.Set(RecordEnum.cellCountMuscle, life.GetCellAliveCount(CellTypeEnum.Muscle));
		record.Set(RecordEnum.cellCountRoot,   life.GetCellAliveCount(CellTypeEnum.Root));
		record.Set(RecordEnum.cellCountShell,  life.GetCellAliveCount(CellTypeEnum.Shell));
		record.Set(RecordEnum.cellCountVein,   life.GetCellAliveCount(CellTypeEnum.Vein));

		record.Set(RecordEnum.creatureCount, life.creatureAliveCount);
		record.Set(RecordEnum.creatureBirthsPerSecond, life.GetCreatureBirthsPerSecond());
		record.Set(RecordEnum.creatureDeathsPerSecond, life.GetCreatureDeathsPerSecond());

		history.AddRecord(record);
		GraphPlotter.instance.MakeDirty();
	}

	//Save load
	public void Restart() {
		Time.timeScale = 0;

		KillAllCreatures();
		worldTicks = 0;
		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, worldTicks);
		for (int y = 1; y <= 1; y++) {
			for (int x = 1; x <= 1; x++) {
				World.instance.life.SpawnCreatureJellyfish(new Vector3(100f + x * 15f, 100f + y * 15, 0f), Random.Range(90f, 90f), worldTicks);
			}
		}
		
		CreatureEditModePanel.instance.Restart();
		RMBToolModePanel.instance.Restart();
		PrisonWall.instance.Restart();

		history.Clear();

		terrain.Restart();

		GlobalPanel.instance.SelectPausePhysics();
		GraphPlotter.instance.MakeDirty();
	}

	public void Load(string filename) {
		// Open the file to read from.
		if (filename == "") {
			filename = "save.txt";
		}
		string path = GlobalPanel.instance.worldSaveDirerectory.text;
		if (path == "") {
			path = "F:/Morfosis/";
		}
		string serializedString = File.ReadAllText(path + filename);

		WorldData loadedWorld = Serializer.Deserialize<WorldData>(serializedString, new UnityJsonSerializer());
		ApplyData(loadedWorld);
		CreatureEditModePanel.instance.UpdateAllAccordingToEditMode();

		CreatureSelectionPanel.instance.ClearSelection();
		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, worldTicks);

		GlobalPanel.instance.SelectPausePhysics();
		GraphPlotter.instance.MakeDirty();
	}

	private void DoSave(string filename) {
		GlobalPanel.instance.SelectPausePhysics();

		UpdateData();

		string path = GlobalPanel.instance.worldSaveDirerectory.text;
		if (path == "") {
			path = "F:/Morfosis/";
		}
		string worldToSave = Serializer.Serialize(worldData, new UnityJsonSerializer());

		if (!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}
		if (filename == "") {
			filename = "save.txt";
		}
		File.WriteAllText(path + filename, worldToSave);

		GlobalPanel.instance.SelectPausePhysics();
		GraphPlotter.instance.MakeDirty();
	}

	private WorldData worldData = new WorldData();

	public void Save() {
		doSave = true;
		Time.timeScale = 1f; // if paused we need to tick one more tick
	}

	// Save
	private void UpdateData() {
		worldData.worldName = worldName;
		worldData.lifeData = life.UpdateData();
		worldData.worldTicks = worldTicks;
		worldData.historyData = history.UpdateData();
		worldData.terrainData = terrain.UpdateData();
		worldData.runnersKilledCount = PrisonWall.instance.runnersKilledCount;
	}

	// Load
	private void ApplyData(WorldData worldData) {
		worldName = worldData.worldName;
		life.ApplyData(worldData.lifeData);
		worldTicks = worldData.worldTicks;
		if (worldData.historyData != null) {
			history.ApplyData(worldData.historyData);
		} else {
			history.Clear();
		}
		terrain.ApplyData(worldData.terrainData);
		PrisonWall.instance.runnersKilledCount = worldData.runnersKilledCount;
	}
}