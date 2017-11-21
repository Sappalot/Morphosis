public class HUD : MonoSingleton<HUD> {
	private bool isShowEnergy = false;
	public bool shouldRenderEnergy {
		get {
			return isShowEnergy && CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype;
		}
	}

	private bool m_isShowEdges = false;
	public bool isShowEdges {
		get {
			return m_isShowEdges;
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

	public void OnClickToggleEdges() {
		m_isShowEdges = !m_isShowEdges;
	}

	public void OnClickToggleShowEnergy() {
		isShowEnergy = !isShowEnergy;
	}

	public void OnClickIsUpdatingMetabolism() {
		isUpdatingMetabolism = !isUpdatingMetabolism;
	}
}