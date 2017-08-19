using UnityEngine;

public class World : MonoSingleton<World> {

    public Life life;

    private float fixedTime;

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
        for (int y = 1; y <= 1; y++) {
            for (int x = 1; x <= 1; x++) {
                life.SpawnCreatureEmbryo(new Vector3(x * 15f, 100f + y * 15, 0f));
            }
        }
    }

    private void Start () {
        //test, OK with 24 * 24 (18 cells per creature) ~ 27 FPS :)
        //including: turn hinged neighbours to correct angle, just one test string creature
        //excluding: turn cell graphics to correct angle, scale mussle cells
        //Restart();
        life.EvoFixedUpdate(fixedTime);
        //life.EvoFixedUpdate(fixedTime);
        //life.SpawnCreatureEmbryo(new Vector3(50f, 50f, 0f));
        //life.SpawnCreatureEmbryo(new Vector3(10f, 20f, 0f));
        //life.SpawnCreatureEmbryo(new Vector3(10f, 30f, 0f));
    }

    private bool isShowingPhenotypes = true;
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
        }

        if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype && isShowingPhenotypes) {
            isShowingPhenotypes = false;
            ShowGenotypes();
        }
        if (CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.phenotype && !isShowingPhenotypes) {
            isShowingPhenotypes = true;
            ShowPhenotypes();
        }
    }

    private int frameCounter = 0;

    private void FixedUpdate() {
        //frameCounter++;
        //Debug.Log(frameCounter);
        //if (frameCounter == 80) {
        //    Restart();
        //}

        if (HUD.instance.timeControllValue > 0) {
            fixedTime += Time.fixedDeltaTime;
            life.EvoFixedUpdate(fixedTime);
        }
    }    
}
