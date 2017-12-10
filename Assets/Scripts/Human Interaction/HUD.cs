public class HUD : MonoSingleton<HUD> {

	private bool m_isPlaySoundFX = true;
	public bool isPlaySoundFX {
		get {
			return m_isPlaySoundFX;
		}
	}

	private bool m_isShowVisualFX = true;
	public bool isShowVisualFX {
		get {
			return m_isShowVisualFX;
		}
	}

	private bool isUpdatingMetabolism = true;
	public bool shouldUpdateMetabolism {
		get {
			return isUpdatingMetabolism;
		}
	}

	public void OnClickIsUpdatingMetabolism() {
		isUpdatingMetabolism = !isUpdatingMetabolism;
	}

	public void OnClickIsPlayingSoundFX() {
		m_isPlaySoundFX = !m_isPlaySoundFX;
	}

	public void OnClickIsShowVisualFX() {
		m_isShowVisualFX = !m_isShowVisualFX;
	}
}