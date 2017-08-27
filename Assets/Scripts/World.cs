using UnityEngine;
using SerializerFree;
using SerializerFree.Serializers;
using System.IO;
using System;

public class World : MonoSingleton<World> {
    private string worldName = "Gaia";
    private float fixedTime;
    public Life life;
 
    //public Savable savable;

    //public SavableSimple savableSimple = new SavableSimple();



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

    public void Restart() {
        KillAllCreatures();
        for (int y = 1; y <= 2; y++) {
            for (int x = 1; x <= 2; x++) {
                life.SpawnCreatureJellyfish(new Vector3(x * 15f, 100f + y * 15, 0f));
            }
        }
        CreatureEditModePanel.instance.Restart();
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
            fixedTime += Time.fixedDeltaTime;
            life.EvoFixedUpdate(fixedTime);
        }
    }

    //Sava load

    public void Load() {
        // Open the file to read from.
        string serializedString = File.ReadAllText(path + "save.txt");

        WorldData loadedWorld = Serializer.Deserialize<WorldData>(serializedString, new UnityJsonSerializer());
        ApplyData(loadedWorld);

        CreatureSelectionPanel.instance.ClearSelection();
        CreatureEditModePanel.instance.Restart();
    }

    private string path = "F:/Morfosis/";
    public void Save() {
        //lifeData.StoreLifeData(null);
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
        worldData.fixedTime = fixedTime;

        worldData.lifeData = life.UpdateData();
    }

    private void ApplyData(WorldData worldData) {
        worldName = worldData.worldName;
        fixedTime = worldData.fixedTime;
        
        life.ApplyData(worldData.lifeData);

    }



}
