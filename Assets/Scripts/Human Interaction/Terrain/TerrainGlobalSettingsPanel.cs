using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TerrainGlobalSettingsPanel : MonoSingleton<TerrainGlobalSettingsPanel> {

	public Slider terrainWidthSlider;
	public Text terrainWidthSliderLabel;

	public Slider terrainHeightSlider;
	public Text terrainHeightSliderLabel;

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
	}


	private bool ignoreHumanInput = false;

	public void OnTerrainWidthSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		World.instance.terrain.sizeDepartureExclusive = new Vector2i((int)terrainWidthSlider.value, World.instance.terrain.sizeDepartureExclusive.y);
		MakeDirty();
	}

	public void OnTerrainHeightSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		World.instance.terrain.sizeDepartureExclusive = new Vector2i(World.instance.terrain.sizeDepartureExclusive.x, (int)terrainHeightSlider.value);
		MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.debug.debugLogMenuUpdate)
				Debug.Log("Update PhenotypePanel");

			ignoreHumanInput = true;

			terrainWidthSlider.value = World.instance.terrain.sizeDepartureExclusive.x;
			terrainWidthSliderLabel.text = string.Format("{0:F0} m", World.instance.terrain.sizeDepartureExclusive.x);

			terrainHeightSlider.value = World.instance.terrain.sizeDepartureExclusive.y;
			terrainHeightSliderLabel.text = string.Format("{0:F0} m", World.instance.terrain.sizeDepartureExclusive.y);

			ignoreHumanInput = false;

			isDirty = false;
		}
	}
}