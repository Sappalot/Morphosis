using UnityEngine;
using SerializerFree;
using SerializerFree.Serializers;
using System.IO;

public class World : MonoSingleton<World> {
	private string worldName = "Gaia";
	private float fixedTime;
	public Life life;

	public void ShowPhenotypes() {
		life.SwitchToPhenotypes();
	}

	public void ShowGenotypes() {
		life.SwitchToGenotypes();
	}

	public void KillAllCreatures() {
		life.KillAll();
		CreatureSelectionPanel.instance.ClearSelection();
	}

	private void Start () {
		//test, OK with 24 * 24 (18 cells per creature) ~ 27 FPS :)
		//including: turn hinged neighbours to correct angle, just one test string creature
		//excluding: turn cell graphics to correct angle, scale mussle cells
		life.EvoFixedUpdate(fixedTime);
	}

	private void Update() {
		//Handle time from here to not get locked out
		if (HUD.instance.timeControllValue == 0 || CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
			Time.timeScale = 0;
			life.EvoUpdate();
		} else if (HUD.instance.timeControllValue == 1) {
			Time.timeScale = 1;
			life.EvoUpdate();
		} else {
			Time.timeScale = 4;
			life.EvoUpdate();
		}
	}

	private void FixedUpdate() {
		if (HUD.instance.timeControllValue > 0) {
			fixedTime += Time.fixedDeltaTime * Time.timeScale;
			life.EvoFixedUpdate(fixedTime);
			GlobalPanel.instance.UpdateWorldNameAndTime(worldName, fixedTime);
		}
	}

	//Save load
	public void Restart() {
		Time.timeScale = 0;
		KillAllCreatures();
		fixedTime = 0f;
		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, fixedTime);
		for (int y = 1; y <= 10; y++) {
			for (int x = 1; x <= 10; x++) {
				life.SpawnCreatureFreak(new Vector3(x * 15f, 100f + y * 15, 0f), Random.Range(90f, 90f), PhenoGenoEnum.Phenotype);
			}
		}
		//life.SpawnCreatureEdgeFailure(new Vector3(100f, 200f, 0f)); //Fixed :)
		//life.SpawnCreatureJellyfish(new Vector3(100f, 100f, 0f));
		CreatureEditModePanel.instance.Restart();
		RMBToolModePanel.instance.Restart();

		Time.timeScale = HUD.instance.timeControllValue;
	}

	public void Load() {
		Time.timeScale = 0;
		// Open the file to read from.
		string serializedString = File.ReadAllText(path + "save.txt");

		WorldData loadedWorld = Serializer.Deserialize<WorldData>(serializedString, new UnityJsonSerializer());
		ApplyData(loadedWorld);

		CreatureSelectionPanel.instance.ClearSelection();
		GlobalPanel.instance.UpdateWorldNameAndTime(worldName, fixedTime);

		SwitchAllCreaturesToCurrentMode();

		Time.timeScale = HUD.instance.timeControllValue;
	}

	private void SwitchAllCreaturesToCurrentMode() {
		if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
			life.SwitchToGenotypes();
		} else if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype) {
			life.SwitchToPhenotypes();
		}
	}

	private string path = "F:/Morfosis/";
	public void Save() {
		life.GeneratePhenotypeCells(); // In case we are still in Genotype view, Phenotypes are not updated
		SwitchAllCreaturesToCurrentMode();
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

		worldData.lifeData = life.UpdateData();
		worldData.fixedTime = fixedTime;
	}

	private void ApplyData(WorldData worldData) {
		worldName = worldData.worldName;
        
        
		life.ApplyData(worldData.lifeData);

		fixedTime = worldData.fixedTime;
	}
}