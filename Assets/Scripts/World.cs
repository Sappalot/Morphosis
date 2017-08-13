using UnityEngine;

public class World : MonoBehaviour {

    public Life life;

    private float fixedTime;

	void Start () {
        //test, OK with 24 * 24 (18 cells per creature) ~ 27 FPS :)
        //including: turn hinged neighbours to correct angle, just one test string creature
        //excluding: turn cell graphics to correct angle, scale mussle cells
        for (int y = 1; y <= 10; y++) {
            for (int x = 1; x <= 15; x++) {
                life.SpawnCreatureEmbryo(new Vector3(x * 10f, 200f + y * 10, 0f));
            }
        }
        life.EvoFixedUpdate(fixedTime);
        //life.EvoFixedUpdate(fixedTime);
        //life.SpawnCreatureEmbryo(new Vector3(50f, 50f, 0f));
        //life.SpawnCreatureEmbryo(new Vector3(10f, 20f, 0f));
        //life.SpawnCreatureEmbryo(new Vector3(10f, 30f, 0f));
    }

    void Update() {
        //Handle time from here to not get locked out
        if (HUD.instance.timeControllValue == 0) {
            Time.timeScale = 0;
            life.EvoUpdate();
        } else if (HUD.instance.timeControllValue == 1) {
            Time.timeScale = 1;
            //life.EvoUpdate();
        } else {
            Time.timeScale = 4;
        } 
    }

    void FixedUpdate() {
        if (HUD.instance.timeControllValue > 0) {
            fixedTime += Time.fixedDeltaTime;
            life.EvoFixedUpdate(fixedTime);
        }
    }
}
