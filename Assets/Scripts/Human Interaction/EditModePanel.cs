using UnityEngine;
using UnityEngine.UI;

public class EditModePanel : MonoSingleton<EditModePanel> {
	public Image lifeImage; // for highliting
	public Image terrainImage;

	public GameObject lifeHudRoot;
	// We will not turn the terrain off... not for now anyway
	public GameObject terrainHudRoot;

	private bool isDirty = true;

	private LifeTerrainEnum m_mode = LifeTerrainEnum.Life;
	public LifeTerrainEnum mode {
		get {
			return m_mode;
		}
		set {
			if (value == LifeTerrainEnum.Life) {
				OnClickedLifeEditMode();
			} else if (value == LifeTerrainEnum.Terrain) {
				OnClickedTerrainEditMode();
			}
		}
	}

	public void Start() {
		m_mode = LifeTerrainEnum.Life;
		isDirty = true;
	}

	public void Restart() {
		m_mode = LifeTerrainEnum.Life;
		GlobalPanel.instance.isRunPhysicsGrayOut = false;
		isDirty = true;
	}

	public void OnClickedLifeEditMode() {
		if (Morphosis.isInterferredByOtheActions()) {
			return;
		}

		m_mode = LifeTerrainEnum.Life;
		GlobalPanel.instance.isRunPhysicsGrayOut = CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype;
		UpdateAllAccordingToEditMode();
	}

	public void OnClickedTerrainEditMode() {
		if (Morphosis.isInterferredByOtheActions()) {
			return;
		}

		World.instance.cameraController.TryUnlockCamera();

		m_mode = LifeTerrainEnum.Terrain;
		GlobalPanel.instance.isRunPhysicsGrayOut = true;
		UpdateAllAccordingToEditMode();
		// TODO: make stuff dirty
	}

	public void UpdateAllAccordingToEditMode() {
		if (m_mode == LifeTerrainEnum.Life) {
			
		} else if (m_mode == LifeTerrainEnum.Terrain) {
			TerrainGlobalSettingsPanel.instance.MakeDirty();
		}
		isDirty = true;
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
				Debug.Log("Update EditModePanel");
			}

			lifeImage.color = (mode == LifeTerrainEnum.Life) ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;
			terrainImage.color = (mode == LifeTerrainEnum.Terrain) ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;

			World.instance.life.show = (mode == LifeTerrainEnum.Life);
			lifeHudRoot.SetActive(mode == LifeTerrainEnum.Life);
			terrainHudRoot.SetActive(mode == LifeTerrainEnum.Terrain);

			isDirty = false;
		}
	}
}
