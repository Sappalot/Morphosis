public class HUD : MonoSingleton<HUD> {
	public ShowCellInformation showCellInformation = ShowCellInformation.type;
	public enum ShowCellInformation {
		type,
		energy,
		effect,
		creatureEffect,
	}

	private bool m_isShowEdges = false;
	public bool isShowEdges {
		get {
			return m_isShowEdges;
		}
	}

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

	public int timeControllValue = 1;
	public bool shouldRenderEdges {
		get {
			return m_isShowEdges && CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype;
		}
	}

	//Global
	public void OnTimeControllChanged(float value) {
		timeControllValue = (int)value;
	}

	public void OnClickShowCellType() {
		showCellInformation = ShowCellInformation.type;
	}

	public void OnClickShowEnergy() {
		showCellInformation = ShowCellInformation.energy;
	}

	public void OnClickShowEffect() {
		showCellInformation = ShowCellInformation.effect;
	}

	public void OnClickShowCreatureEffect() {
		showCellInformation = ShowCellInformation.creatureEffect;
	}

	public void OnClickIsUpdatingMetabolism() {
		isUpdatingMetabolism = !isUpdatingMetabolism;
	}

	public void OnClickToggleEdges() {
		m_isShowEdges = !m_isShowEdges;
	}

	public void OnClickIsPlayingSoundFX() {
		m_isPlaySoundFX = !m_isPlaySoundFX;
	}

	public void OnClickIsShowVisualFX() {
		m_isShowVisualFX = !m_isShowVisualFX;
	}
}