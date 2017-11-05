using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoSingleton<HUD> {
	[HideInInspector]

	private bool m_isShowEdges = false;
	public bool isShowEdges {
		get {
			return m_isShowEdges;
		}
	}

	private bool isShowEnergy = false;
	

	public int timeControllValue = 1;
	public bool shouldRenderEdges {
		get {
			return m_isShowEdges && CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype;
		}
	}

	public bool shouldRenderEnergy {
		get {
			return isShowEnergy && CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype;
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
}