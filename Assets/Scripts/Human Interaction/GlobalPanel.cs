using System;
using UnityEngine;
using UnityEngine.UI;

public class GlobalPanel : MonoSingleton<GlobalPanel> {

	//Debug
	public Text worldNameAndTimeText;
	public Text fps;
	public Text pps;

	public Text CreatureCount;
	public Text soulsDirtyCount;
	public Text soulsCleanCount;
	public Text soulsDeadButUsedCount;
	public Text soulsLostCount;

	private int frameCount;
	private float timeCount;
	private float updateTimeCount;
	private float updatePeriod = 1f;

	private int physicsUpdateCount;

	//World
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

	//Physics
	public Toggle effectsUpdateMetabolism;
	public Toggle physicsApplyWingForce;
	public Slider timeSpeedSilder;
	public Text physicsTimeSpeedText;
	public float physicsUpdatesPerSecond;
	public RectTransform physicsSliderFillBar;


	//Graphics
	public Toggle graphicsCreatures;
	public Toggle graphicsRenderWings;
	public Dropdown graphicsCellDropdown;
	public enum CellGraphicsEnum {
		type,
		energy,
		effect,
		effectCreature,
		update,
	}
	[HideInInspector]
	public CellGraphicsEnum graphicsCell {
		get {
			return (CellGraphicsEnum)graphicsCellDropdown.value;
		}
	}

	// Effects
	public Toggle effectsPlaySound;
	public Toggle effectsShowParticles;

	private void Update () {
		frameCount++;
		timeCount += Time.unscaledDeltaTime;
		updateTimeCount += Time.unscaledDeltaTime;
		if (updateTimeCount > updatePeriod) {
			updateTimeCount = 0;

			//fps
			fps.text = timeCount > 0f ? string.Format("FPS: {0:F1}", frameCount / timeCount) : "FPS: ---";
			frameCount = 0;

			//pps
			physicsUpdatesPerSecond = physicsUpdateCount / timeCount;
			pps.text = CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype && timeCount > 0f ? string.Format("PPS: {0:F1}", physicsUpdateCount / timeCount) : "PPS: ---";
			physicsUpdateCount = 0;
			float width = Mathf.Clamp(physicsUpdatesPerSecond * Time.fixedDeltaTime * 9f, 0f, 180f);
			physicsSliderFillBar.sizeDelta = new Vector2(width, physicsSliderFillBar.sizeDelta.y);

			timeCount = 0;

			//creatures & souls
			CreatureCount.text =         "Creatures: "   + Life.instance.creatureCount;
			soulsDirtyCount.text =       "Alive & dirty: " + Life.instance.soulUnupdatedCount;
			soulsCleanCount.text =       "Alive & clean: " + (Life.instance.soulUpdatedCount - Life.instance.soulsDeadButUsedCount);
			soulsDeadButUsedCount.text = "Dead & used: "   + Life.instance.soulsDeadButUsedCount;
			soulsLostCount.text =        "Dead & lost: "   + Life.instance.soulsLostCount;
		}
	}

	private void FixedUpdate() {
		physicsUpdateCount++;
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