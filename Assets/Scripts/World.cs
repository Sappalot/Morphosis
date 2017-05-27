using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {

    public Life life;

    private float time;

	void Start () {

        //test, OK with 24 * 24 (18 cells per creature) ~ 10 FPS :) 
        for (int y = 1; y <= 24; y++) {
            for (int x = 1; x <= 24; x++) {
                life.SpawnCreatureEmbryo(new Vector3(x*10f, y*10, 0f));
            }
         }
        //life.SpawnCreatureEmbryo(new Vector3(50f, 50f, 0f));

        //life.SpawnCreatureEmbryo(new Vector3(10f, 20f, 0f));
        //life.SpawnCreatureEmbryo(new Vector3(10f, 30f, 0f));

    }

	void Update () {
        //life.UpdateGraphics();
	}

    void FixedUpdate() {
        life.UpdatePhyscis();
    }
}
