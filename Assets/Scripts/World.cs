using UnityEngine;
using SerializerFree;
using SerializerFree.Serializers;
using System.IO;

public class World : MonoSingleton<World> {
	public Camera worldCamera;
	private string worldName = "Gaia";
	public ulong worldTicks = 0;

	public void KillAllCreaturesAndSouls() {
		Life.instance.KillAllCreaturesAndSouls();
		CreatureSelectionPanel.instance.ClearSelection();
	}

	private void Start () {
		//test, OK with 24 * 24 (18 cells per creature) ~ 27 FPS :)
		//including: turn hinged neighbours to correct angle, just one test string creature
		//excluding: turn cell graphics to correct angle, scale mussle cells
		//Life.instance.EvoFixedUpdate(fixedTime);
	}

	private void Update() {
		//Handle time from here to not get locked out
		if (GlobalPanel.instance.timeSpeedSilder.value < -10f || CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
			Time.timeScale = 0f;
			Life.instance.UpdateStructure();
			GlobalPanel.instance.physicsTimeSpeedText.text = "II";
		} else {
			if (GlobalPanel.instance.timeSpeedSilder.value >= -10f && GlobalPanel.instance.timeSpeedSilder.value <= 0f) {
				Time.timeScale = 1f;
				timeScaleAim = 1f;
			} else if (GlobalPanel.instance.physicsUpdatesPerSecond == 0f) {
				//kick start physics
				Time.timeScale = timeScaleAim;
			}
		}

		if (GlobalPanel.instance.graphicsCreatures.isOn) {
			Life.instance.UpdateGraphics();
		}
	}

	private int updates;

	private void FixedUpdate() {
		Life.instance.UpdateStructure();

		worldTicks++; //The only place where time is increased
		Life.instance.UpdatePhysics(worldTicks);

		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, worldTicks);

		float isValue = GlobalPanel.instance.physicsUpdatesPerSecond * Time.fixedDeltaTime;
		float sliderValue = GlobalPanel.instance.timeSpeedSilder.value / 5f;

		timeScaleAim = isValue + Mathf.Clamp((sliderValue - isValue) * 0.5f, -0.5f, 0.5f);
		timeScaleAim = Mathf.Clamp(timeScaleAim, 1f, Mathf.Max(0f, sliderValue));
		GlobalPanel.instance.physicsTimeSpeedText.text = string.Format("{0:F1} ({1:F1})", Mathf.Max(1f, sliderValue), isValue);
		//GlobalPanel.instance.physicsTimeSpeedText.text = string.Format("W: {0:F1}, A: {1:F1}, Is: {2:F2}", sliderValue, timeScaleAim, isValue);
		Time.timeScale = timeScaleAim;
	}

	private float timeScaleAim;

	//Save load
	public void Restart() {
		GlobalPanel.instance.timeSpeedSilder.value = -20f;
		Time.timeScale = 0;

		KillAllCreaturesAndSouls();
		worldTicks = 0;
		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, worldTicks);
		for (int y = 1; y <= 1; y++) {
			for (int x = 1; x <= 1; x++) {
				Life.instance.SpawnCreatureJellyfish(new Vector3(x * 15f, 100f + y * 15, 0f), Random.Range(90f, 90f));
			}
		}
		//Life.instance.SpawnCreatureEdgeFailure(new Vector3(100f, 200f, 0f)); //Fixed :)
		//Life.instance.SpawnCreatureJellyfish(new Vector3(100f, 100f, 0f));
		
		CreatureEditModePanel.instance.Restart();
		RMBToolModePanel.instance.Restart();
	}

	public void Load() {
		GlobalPanel.instance.timeSpeedSilder.value = -20f; //pause
		Time.timeScale = 0;

		Time.timeScale = 0;
		// Open the file to read from.
		string serializedString = File.ReadAllText(path + "save.txt");

		WorldData loadedWorld = Serializer.Deserialize<WorldData>(serializedString, new UnityJsonSerializer());
		ApplyData(loadedWorld);
		CreatureEditModePanel.instance.UpdateAllAccordingToEditMode();

		CreatureSelectionPanel.instance.ClearSelection();
		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, worldTicks);
	}

	private string path = "F:/Morfosis/";
	public void Save() {
		GlobalPanel.instance.timeSpeedSilder.value = -20f; //pause
		Time.timeScale = 0;

		UpdateData();

		string worldToSave = Serializer.Serialize(worldData, new UnityJsonSerializer());

		if (!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}
		File.WriteAllText(path + "save.txt", worldToSave);
	}

	private WorldData worldData = new WorldData();

	//data
	private void UpdateData() {
		worldData.worldName = worldName;
		worldData.lifeData = Life.instance.UpdateData();
		worldData.worldTicks = worldTicks;
	}

	private void ApplyData(WorldData worldData) {
		worldName = worldData.worldName;
		Life.instance.ApplyData(worldData.lifeData);
		worldTicks = worldData.worldTicks;
	}
}