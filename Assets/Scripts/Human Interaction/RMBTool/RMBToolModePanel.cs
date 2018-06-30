using UnityEngine.UI;

public class RMBToolModePanel : MonoSingleton<RMBToolModePanel> {
	public Image springImage;
	public Image attractImage;
	public Image repellImage;
	public Image spawnEmbryoImage;
	public Image spawnFreakImage;

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
		springImage.color = (toolMode == RMBToolMode.spring) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		attractImage.color = (toolMode == RMBToolMode.attract) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		repellImage.color = (toolMode == RMBToolMode.repell) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		spawnEmbryoImage.color = (toolMode == RMBToolMode.spawnEmbryo) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		spawnFreakImage.color = (toolMode == RMBToolMode.spawnFreak) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
	}
}
