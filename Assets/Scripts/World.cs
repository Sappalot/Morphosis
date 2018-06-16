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
			//aimSpeedLowPass = 0f;
			World.instance.life.UpdateStructure();
			GlobalPanel.instance.physicsTimeScaleWantText.text = "II";
		} else if (GlobalPanel.instance.physicsUpdatesPerSecond == 0f) {
			//kick start physics
			Time.timeScale = 1f;
			//aimSpeedLowPass = 1f;
		}

		if (GlobalPanel.instance.graphicsCreatures.isOn) {
			World.instance.life.UpdateGraphics();
		}

		//if (redGCText > 0) {
		//	GlobalPanel.instance.garbageCollectText.color = Color.red;
		//} else {
		//	GlobalPanel.instance.garbageCollectText.color = Color.black;
		//}
	}

	public History history = new History();

	private List<HistoryEvent> historyEvents = new List<HistoryEvent>();
	public void AddHistoryEvent(HistoryEvent historyEvent) {
		historyEvents.Add(historyEvent);
	}

	private bool doSave = false;
	public void Save() {
		doSave = true;
		Time.timeScale = 1f; // if paused we need to tick one more tick
		GlobalPanel.instance.OnPausePhysicsClicked();
	}

	//private int redGCText = 0;
	//private int garbageFixed = 0;
	private void FixedUpdate() {
		if (life == null) {
			return;
		}


		//if (GlobalPanel.instance.garbageCollectPeriodSlider.value != 0) {
		//	garbageFixed++;
		//	if (garbageFixed >= GlobalPanel.instance.garbageCollectPeriodSlider.value * 10) {
		//		garbageFixed = 0;
		//	}
		//	if (garbageFixed == 0) {
		//		System.GC.Collect();
		//		redGCText = 2;
		//	} else {
		//		redGCText--;
		//	}
		//}

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
				record.Add(RecordEnum.fps,            0);
				record.Add(RecordEnum.cellCountTotal, 0);
				record.Add(RecordEnum.cellCountJaw,   0);
				record.Add(RecordEnum.cellCountLeaf,  0);
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

		}
		worldTicks++; //The only place where time is increased

		//float isSpeed = GlobalPanel.instance.physicsUpdatesPerSecond * Time.fixedDeltaTime;
		//float desiredSpeed = GlobalPanel.instance.physicsTimeScaleSilder.value / 5f;

		//if (GlobalPanel.instance.fpsGuardToggle.isOn) {
		//	float quickness = 0.2f;
		//	float newAimSpeed = 0f;
		//	if (GlobalPanel.instance.frameRate > GlobalPanel.instance.fpsGuardSlider.value) {
		//		newAimSpeed = aimSpeedLowPass + Mathf.Sign(desiredSpeed - aimSpeedLowPass) * Time.fixedUnscaledDeltaTime * 0.6f;
		//	} else {
		//		newAimSpeed = aimSpeedLowPass - Time.fixedUnscaledDeltaTime * 0.6f;
		//	}
		//	aimSpeedLowPass = aimSpeedLowPass * (1f - quickness) + newAimSpeed * quickness;
		//	if (desiredSpeed < aimSpeedLowPass) {
		//		aimSpeedLowPass = Mathf.Max(0f, desiredSpeed);
		//	}
		//} else {
		//	aimSpeedLowPass = Mathf.Max(0f, desiredSpeed);
		//}
		//Time.timeScale = Mathf.Max(0f, aimSpeedLowPass);
		//GlobalPanel.instance.physicsTimeScaleIsText.text = string.Format("{0:F1}", isSpeed);
		//GlobalPanel.instance.physicsTimeScaleWantText.text = string.Format("{0:F1}", Mathf.Max(1f, desiredSpeed));

		//gravityAngle += Time.fixedDeltaTime * 5f;
		//if (gravityAngle > 360f) {
		//	gravityAngle -= 360f;
		//}
		//float gravityFactor = 200f;
		//Physics2D.gravity = new Vector2(Mathf.Cos(gravityAngle * Mathf.Deg2Rad) * gravityFactor, Mathf.Sin(gravityAngle * Mathf.Deg2Rad) * gravityFactor);
	}
	//public float aimSpeedLowPass;

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

		record.Add(RecordEnum.fps, GlobalPanel.instance.frameRate);
		record.Add(RecordEnum.cellCountTotal, life.cellAliveCount);
		record.Add(RecordEnum.cellCountJaw, life.GetCellAliveCount(CellTypeEnum.Jaw));
		record.Add(RecordEnum.cellCountLeaf, life.GetCellAliveCount(CellTypeEnum.Leaf));

		history.AddRecord(record);
		GraphPlotter.instance.MakeDirty();
	}

	//Save load
	public void Restart() {
		GlobalPanel.instance.physicsTimeScaleSilder.value = -20f;
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
	}

	public void Load(string filename) {
		GlobalPanel.instance.physicsTimeScaleSilder.value = -20f; //pause
		Time.timeScale = 0;

		Time.timeScale = 0;
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
	}

	//private string path = "F:/Morfosis/";
	private void DoSave(string filename) {
		//AddHistoryEvent(new HistoryEvent("Saved", true));
		//CreateRecord();
		//--

		GlobalPanel.instance.physicsTimeScaleSilder.value = -20f; //pause
		Time.timeScale = 0;

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

	//data
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


//	private string allThereIs;
	public string GetWorldData() {
		UpdateData();
		return Serializer.Serialize(worldData, new UnityJsonSerializer());

		//Destroy(life.gameObject);
		//System.GC.Collect();
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