using UnityEngine;
using UnityEngine.UI;

public class PerformanceHUD : MonoBehaviour {
	public Text fpsText;
	public Text ppsText;
	public Text uptimeText;

	private int fpsCount;
	private int ppsCount;
	private float updateCooldown;
	private float timeSinceLastUpdate;

	private int debugLogPrintCounter;

	// Update is called once per frame
	private void Update() {
		fpsCount++;
		timeSinceLastUpdate += Time.unscaledDeltaTime;

		if (updateCooldown < 0f) {

			float fps = (float)fpsCount / timeSinceLastUpdate;
			float pps = (float)ppsCount / timeSinceLastUpdate;
			float uptime = Time.realtimeSinceStartup / 3600f;

			fpsText.text = "FPS: " + fps + " Hz";
			ppsText.text = "PPS: " + pps + " Hz";
			uptimeText.text = string.Format("Uptime: {0:0.00} ", uptime);

			fpsCount = 0;
			ppsCount = 0;

			updateCooldown = 1800f; // 1/2 hour
			timeSinceLastUpdate = 0f;

			Debug.Log("No: " + debugLogPrintCounter + " | Time since Startup: " + uptime + " h | FPS: " + fps + " | PPS: " + pps);
			debugLogPrintCounter++;
		}
		updateCooldown -= Time.unscaledDeltaTime;

	}

	private void FixedUpdate() {
		ppsCount++;
	}
}
