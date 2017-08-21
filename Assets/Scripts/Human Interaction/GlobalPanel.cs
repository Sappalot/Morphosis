using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalPanel : MonoBehaviour {

    public Text fps;

    private int frameCount;
    private float timeCount;
    private float updateTimeCount;
    private float updatePeriod = 1f;

	private void Update () {
        frameCount++;
        timeCount += Time.deltaTime;
        updateTimeCount += Time.unscaledDeltaTime;
        if (updateTimeCount > updatePeriod) {
            updateTimeCount = 0;
            fps.text = timeCount > 0 ? string.Format("FPS: {0:F1}", frameCount / timeCount) : "FPS: ---";
            frameCount = 0;
            timeCount = 0;
        }        
	}

    public void OnLoadWorldClicked() {
        World.instance.Restart();
    }
}
