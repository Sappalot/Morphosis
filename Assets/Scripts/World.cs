using UnityEngine;
using SerializerFree;
using SerializerFree.Serializers;
using System.IO;
using System.Collections.Generic;

public class World : MonoSingleton<World> {
	[HideInInspector]
	public Life life;
	public Life lifePrefab;

	public GameObject cellPrefab;

	public Camera worldCamera;
	private string worldName = "Gaia";
	[HideInInspector]
	public ulong worldTicks = 0;

	public void KillAllCreatures() {
		World.instance.life.KillAllCreatures();
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

		CreateLife();

		history.Init();
		GraphPlotter.instance.history = history;
	}

	private void Update() {
		if (life == null) {
			return;
		}
		//Handle time from here to not get locked out
		if ((!GlobalPanel.instance.isRunPhysics || CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) && !doSave) {
			Time.timeScale = 0f;
			life.UpdateStructure();
		} else if (GlobalPanel.instance.physicsUpdatesPerSecond == 0f) {
			//kick start physics
			Time.timeScale = 1f;
		}

		if (GlobalPanel.instance.graphicsCreatures.isOn) {
			life.UpdateGraphics();
		}
	}

	public History history = new History();

	private List<HistoryEvent> historyEvents = new List<HistoryEvent>();
	public void AddHistoryEvent(HistoryEvent historyEvent) {
		historyEvents.Add(historyEvent);
	}

	private bool doSave = false;
	private bool stopAfterOneSecond = false;

	public void Save() {
		doSave = true;
		Time.timeScale = 1f; // if paused we need to tick one more tick
		GlobalPanel.instance.SelectPausePhysics();
	}

	private void FixedUpdate() {
		if (life == null) {
			return;
		}

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
				record.SetTagText("Big Bang", true);
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
				history.AddRecord(record);
				GraphPlotter.instance.MakeDirty();
			} else {
				if(doSave) {
					AddHistoryEvent(new HistoryEvent("Saved", true));
					CreateRecord();
					DoSave("save.txt");
					doSave = false;
				} else {
					CreateRecord();
				}
			}

			if (stopAfterOneSecond) {
				GlobalPanel.instance.SelectPausePhysics();
				stopAfterOneSecond = false;
			}
		}
		worldTicks++; //The only place where time is increased



		//gravityAngle += Time.fixedDeltaTime * 5f;
		//if (gravityAngle > 360f) {
		//	gravityAngle -= 360f;
		//}
		//float gravityFactor = 200f;
		//Physics2D.gravity = new Vector2(Mathf.Cos(gravityAngle * Mathf.Deg2Rad) * gravityFactor, Mathf.Sin(gravityAngle * Mathf.Deg2Rad) * gravityFactor);
	}

	private float gravityAngle;

	public void CreateRecord() {
		Record record = new Record();
		if (historyEvents.Count > 0) {
			string eventText = "";
			bool line = false;
			for (int i = 0; i < historyEvents.Count; i++) {
				eventText += historyEvents[i].text + " ";
				line |= historyEvents[i].showLine;
			}
			historyEvents.Clear();
			record.SetTagText(eventText, line);
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

		GlobalPanel.instance.SelectRunPhysics();
		stopAfterOneSecond = true;
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

		GlobalPanel.instance.SelectRunPhysics();
		stopAfterOneSecond = true;
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
	}

	private WorldData worldData = new WorldData();

	// Save
	private void UpdateData() {
		worldData.worldName = worldName;
		worldData.lifeData = life.UpdateData();
		worldData.worldTicks = worldTicks;
		worldData.historyData = history.UpdateData();

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
		
		PrisonWall.instance.runnersKilledCount = worldData.runnersKilledCount;
	}

	public string GetWorldData() {
		UpdateData();
		return Serializer.Serialize(worldData, new UnityJsonSerializer());
	}

	public void CreateWorldFromData(string data) {
		//CreateLife();

		WorldData loadedWorld = Serializer.Deserialize<WorldData>(data, new UnityJsonSerializer());
		ApplyData(loadedWorld);
		CreatureEditModePanel.instance.UpdateAllAccordingToEditMode();
		CreatureSelectionPanel.instance.ClearSelection();
		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, worldTicks);
	}

	public void CreateLife() {
		life = Instantiate(lifePrefab, transform);
		life.name = "Life";
	}
}