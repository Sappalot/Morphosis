using UnityEngine;
using SerializerFree;
using SerializerFree.Serializers;
using System.IO;

public class World : MonoSingleton<World> {
	public Camera worldCamera;
	private string worldName = "Gaia";
	private float fixedTime;

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
		if (GlobalPanel.instance.timeSpeedSilder.value == 0 || CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
			Time.timeScale = 0f;
			Life.instance.UpdateStructure();
		} else if (GlobalPanel.instance.timeSpeedSilder.value == 1) {
			Time.timeScale = 1f;
		} else {
			Time.timeScale = 4f;
		}

		Life.instance.UpdateGraphics();
	}


	private int updates;
	private float lastUpdate = -1;
	private void FixedUpdate() {
		Life.instance.UpdateStructure();

		fixedTime += Time.fixedDeltaTime; // * Time.timeScale;
		Life.instance.UpdatePhysics(fixedTime);

		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, fixedTime);

		if (lastUpdate < 0) {
			lastUpdate = Time.fixedUnscaledTime;
		}
		if (Time.fixedUnscaledTime > lastUpdate + 1f) {
			lastUpdate = Time.fixedUnscaledTime;
			//Debug.Log("W: Time unscaled: " + Time.fixedUnscaledTime + ", FixedTime: " + fixedTime + ", Updates: " + updates + ", FixedDeltaTime: " + Time.fixedDeltaTime);
			updates = 0;
		}
		updates++;
	}

	//Save load
	public void Restart() {
		GlobalPanel.instance.timeSpeedSilder.value = 0;
		Time.timeScale = 0;

		KillAllCreaturesAndSouls();
		fixedTime = 0f;
		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, fixedTime);
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
		GlobalPanel.instance.timeSpeedSilder.value = 0;
		Time.timeScale = 0;

		Time.timeScale = 0;
		// Open the file to read from.
		string serializedString = File.ReadAllText(path + "save.txt");

		WorldData loadedWorld = Serializer.Deserialize<WorldData>(serializedString, new UnityJsonSerializer());
		ApplyData(loadedWorld);
		CreatureEditModePanel.instance.UpdateAllAccordingToEditMode();

		CreatureSelectionPanel.instance.ClearSelection();
		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, fixedTime);
	}

	private string path = "F:/Morfosis/";
	public void Save() {
		GlobalPanel.instance.timeSpeedSilder.value = 0;
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
		worldData.fixedTime = fixedTime;
	}

	private void ApplyData(WorldData worldData) {
		worldName = worldData.worldName;

		Life.instance.ApplyData(worldData.lifeData);

		fixedTime = worldData.fixedTime;
	}
}