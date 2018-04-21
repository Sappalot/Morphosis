using UnityEngine;
using SerializerFree;
using SerializerFree.Serializers;
using System.IO;

public class World : MonoSingleton<World> {
	public Life life;
	public Life lifePrefab;

	public Camera worldCamera;
	private string worldName = "Gaia";
	[HideInInspector]
	public ulong worldTicks = 0;

	public void KillAllCreatures() {
		World.instance.life.KillAllCreatures();
		CreatureSelectionPanel.instance.ClearSelection();
	}

	private void Start () {
		//test, OK with 24 * 24 (18 cells per creature) ~ 27 FPS :)
		//including: turn hinged neighbours to correct angle, just one test string creature
		//excluding: turn cell graphics to correct angle, scale mussle cells
		//World.instance.life.EvoFixedUpdate(fixedTime);

		CreateLife();
	}

	private void Update() {
		if (life == null) {
			return;
		}
		//Handle time from here to not get locked out
		if (GlobalPanel.instance.timeSpeedSilder.value < -10f || CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
			Time.timeScale = 0f;
			World.instance.life.UpdateStructure();
			GlobalPanel.instance.physicsTimeSpeedText.text = "II";
		} else {
			if (GlobalPanel.instance.timeSpeedSilder.value >= -10f && GlobalPanel.instance.timeSpeedSilder.value <= 0f) {
				Time.timeScale = 1f;
				//timeScaleAim = 1f;
			} else if (GlobalPanel.instance.physicsUpdatesPerSecond == 0f) {
				//kick start physics
				Time.timeScale = 1f; // timeScaleAim;
			}
		}

		if (GlobalPanel.instance.graphicsCreatures.isOn) {
			World.instance.life.UpdateGraphics();
		}
	}

	private void FixedUpdate() {
		if(life == null) {
			return;
		}

		life.UpdateStructure();

		worldTicks++; //The only place where time is increased
		life.UpdatePhysics(worldTicks);
		if (GlobalPanel.instance.physicsTeleport.isOn) {
			Portals.instance.UpdatePhysics(World.instance.life.creatures, worldTicks);
		}
		if (GlobalPanel.instance.physicsKillFugitive.isOn) {
			PrisonWall.instance.UpdatePhysics(World.instance.life.creatures, worldTicks);
		}
		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, worldTicks);

		float isValue = GlobalPanel.instance.physicsUpdatesPerSecond * Time.fixedDeltaTime;
		float sliderValue = GlobalPanel.instance.timeSpeedSilder.value / 5f;

		//timeScaleAim = isValue + Mathf.Clamp((sliderValue - isValue) * 0.5f, -0.5f, 0.5f);
		//timeScaleAim = Mathf.Clamp(timeScaleAim, 1f, Mathf.Max(0f, sliderValue));
		GlobalPanel.instance.physicsTimeSpeedText.text = string.Format("{0:F1} ({1:F1})", Mathf.Max(1f, sliderValue), isValue);
		//GlobalPanel.instance.physicsTimeSpeedText.text = string.Format("W: {0:F1}, A: {1:F1}, Is: {2:F2}", sliderValue, timeScaleAim, isValue);
		Time.timeScale = Mathf.Max(0f, sliderValue);
	}

	//private float timeScaleAim;

	//Save load
	public void Restart() {
		GlobalPanel.instance.timeSpeedSilder.value = -20f;
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
	}

	public void Load(string filename) {
		GlobalPanel.instance.timeSpeedSilder.value = -20f; //pause
		Time.timeScale = 0;

		Time.timeScale = 0;
		// Open the file to read from.
		if (filename == "") {
			filename = "save.txt";
		}
		string serializedString = File.ReadAllText(path + filename);

		WorldData loadedWorld = Serializer.Deserialize<WorldData>(serializedString, new UnityJsonSerializer());
		ApplyData(loadedWorld);
		CreatureEditModePanel.instance.UpdateAllAccordingToEditMode();

		CreatureSelectionPanel.instance.ClearSelection();
		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, worldTicks);
	}

	private string path = "F:/Morfosis/";
	public void Save(string filename) {
		GlobalPanel.instance.timeSpeedSilder.value = -20f; //pause
		Time.timeScale = 0;

		UpdateData();

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
	private void UpdateData() {
		worldData.worldName = worldName;
		worldData.lifeData = World.instance.life.UpdateData();
		worldData.worldTicks = worldTicks;

		worldData.runnersKilledCount = PrisonWall.instance.runnersKilledCount;
	}

	private void ApplyData(WorldData worldData) {
		worldName = worldData.worldName;
		World.instance.life.ApplyData(worldData.lifeData);
		worldTicks = worldData.worldTicks;

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