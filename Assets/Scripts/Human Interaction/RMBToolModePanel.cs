using UnityEngine.UI;

public class RMBToolModePanel : MonoSingleton<RMBToolModePanel> {
	public Image springImage;
	public Image attractImage;
	public Image repellImage;
	public Image spawnImage;

	public enum RMBToolMode {
		spring,
		attract,
		repell,
		spawn,
	}

	private RMBToolMode m_toolMode;
	public RMBToolMode toolMode	{
		get {
			return m_toolMode;
		}
	}

	public override void Init() {
		m_toolMode = RMBToolMode.spawn;
		UpdateHUD();
	}

	public void Restart() {
		m_toolMode = RMBToolMode.spawn;
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

	public void OnClickedSpawn() {
		m_toolMode = RMBToolMode.spawn;
		UpdateHUD();
	}

	private void UpdateHUD() {
		springImage.color = (toolMode == RMBToolMode.spring) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		attractImage.color = (toolMode == RMBToolMode.attract) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		repellImage.color = (toolMode == RMBToolMode.repell) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		spawnImage.color = (toolMode == RMBToolMode.spawn) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
	}
}
