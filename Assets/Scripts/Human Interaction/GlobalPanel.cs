using System;
using UnityEngine;
using UnityEngine.UI;

public class GlobalPanel : MonoSingleton<GlobalPanel> {
	public Text fps;
	public Text worldNameAndTimeText;

	private int frameCount;
	private float timeCount;
	private float updateTimeCount;
	private float updatePeriod = 1f;

	public void UpdateWorldNameAndTime(string worldName, float fixedTime) {
		TimeSpan t = TimeSpan.FromSeconds(fixedTime);
		int d = t.Days;
		int h = t.Hours;
		int m = t.Minutes;
		int s = t.Seconds;

		if (d > 0) {
			worldNameAndTimeText.text = string.Format("{0} {1:F0}d {2:F0}h {3:F0}m {4:F0}s", worldName, d, h, m, s);
		} else if (h > 0) {
			worldNameAndTimeText.text = string.Format("{0} {1:F0}h {2:F0}m {3:F0}s", worldName, h, m, s);
		} else if (m > 0) {
			worldNameAndTimeText.text = string.Format("{0} {1:F0}m {2:F0}s", worldName, m, s);
		} else {
			worldNameAndTimeText.text = string.Format("{0} {1:F0}s", worldName, s);
		}
	}

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

	public void OnRestartClicked() {
		World.instance.Restart();
	}

	public void OnLoadClicked() {
		World.instance.Load();
	}

	public void OnSaveClicked() {
		World.instance.Save();
	}
}