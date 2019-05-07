using UnityEngine;
using UnityEngine.UI;

// just graphical display of a bar with a text
public class ProgressBar : MonoSingleton<ProgressBar> {
	public Text heading;
	public Image background;
	public Image bar;

	public static float killCreatureTime = 0.5f;
	public static float spawnCreatureTime = 1f;
	public static float loadTextFileTime = 10f;
	public static float parseTextFileTime = 20f;

	private float finishedTime = 20f;
	private float elapsedTime = 0f;

	private float m_fullness = 1f;
	public float fullness {
		get {
			return m_fullness;
		}
		set {
			m_fullness = Mathf.Clamp01(value);
			float backgroundWidth = background.rectTransform.rect.width;
			bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backgroundWidth * m_fullness);
		}
	}

	public void ResetForStartup(int freezerCreatureCount) {
		finishedTime = freezerCreatureCount * spawnCreatureTime;
		elapsedTime = 0;
		UpdateGraphics();
	}

	public void ResetForRestart(int freezerCreatureCount, int worldCreatureRemoveCount) {
		finishedTime =  Freezer.instance.creatureCount * (killCreatureTime + spawnCreatureTime) + worldCreatureRemoveCount * killCreatureTime;
		elapsedTime = 0;
		UpdateGraphics();
	}

	public void ResetForLoad(int freezerCreatureCount, int worldCreatureRemoveCount, int worldCreatureSpawnCount) {
		finishedTime = Freezer.instance.creatureCount * (killCreatureTime + spawnCreatureTime) + worldCreatureRemoveCount * killCreatureTime + worldCreatureSpawnCount + spawnCreatureTime;
		elapsedTime = 0;
		UpdateGraphics();
	}

	public void SpawnCreature() {
		elapsedTime += spawnCreatureTime;
		UpdateGraphics();
	}

	public void KillCreature() {
		elapsedTime += killCreatureTime;
		UpdateGraphics();
	}

	private void UpdateGraphics() {
		fullness = elapsedTime / finishedTime;
	}
}