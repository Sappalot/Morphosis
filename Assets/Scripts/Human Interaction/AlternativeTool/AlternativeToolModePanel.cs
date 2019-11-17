using UnityEngine;
using UnityEngine.UI;

public class AlternativeToolModePanel : MonoSingleton<AlternativeToolModePanel> {
	public Image springImage;
	public Image spawnEmbryoImage;
	public Image spawnFreakImage;

	public GameObject showHide;
	public bool isOn;

	public enum RMBToolMode {
		spring,
		attract,
		repell,
		spawnEmbryo,
		spawnFreak,
	}

	private RMBToolMode m_toolMode = RMBToolMode.spawnEmbryo;
	public RMBToolMode toolMode	{
		get {
			return m_toolMode;
		}
	}

	public override void Init() {
		Restart();
	}

	public void Restart() {
		m_toolMode = RMBToolMode.spawnEmbryo;
		UpdateHUD();
	}

	public void OnClickedSpring() {
		m_toolMode = RMBToolMode.spring;
		UpdateHUD();
	}

	public void OnClickedAttract() {
		m_toolMode = RMBToolMode.attract;
		UpdateHUD();
	}

	public void OnClickedRepell() {
		m_toolMode = RMBToolMode.repell;
		UpdateHUD();
	}

	public void OnClickedSpawnEmbryo() {
		m_toolMode = RMBToolMode.spawnEmbryo;
		UpdateHUD();
	}

	public void OnClickedSpawnFreak() {
		m_toolMode = RMBToolMode.spawnFreak;
		UpdateHUD();
	}

	private void UpdateHUD() {
		springImage.color = (toolMode == RMBToolMode.spring) ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;
		spawnEmbryoImage.color = (toolMode == RMBToolMode.spawnEmbryo) ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;
		spawnFreakImage.color = (toolMode == RMBToolMode.spawnFreak) ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;
	}

	private void OnGUI() {
		Event e = Event.current;
		if (e.alt) {
			if (!isOn) {
				showHide.SetActive(true);
				isOn = true;
				CreatureSelectionPanel.instance.MakeDirty();
				PhenotypePanel.instance.MakeDirty();
				GenotypePanel.instance.MakeDirty();
			}
		} else {
			if (isOn) {
				showHide.SetActive(false);
				isOn = false;
				CreatureSelectionPanel.instance.MakeDirty();
				PhenotypePanel.instance.MakeDirty();
				GenotypePanel.instance.MakeDirty();
			}
		}
		if (!isFreeToUse()) {
			if (isOn) {
				showHide.SetActive(false);
				isOn = false;
			}
		}
	}

	private bool isFreeToUse() {
		return MouseAction.instance.actionState == MouseActionStateEnum.free && World.instance.creatureSelectionController.IsIdle;
	}
}
