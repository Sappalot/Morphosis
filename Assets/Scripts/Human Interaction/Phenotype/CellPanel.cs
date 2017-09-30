using UnityEngine;
using UnityEngine.UI;

public class CellPanel : MonoSingleton<CellPanel> {
	public Text cellType;
	public Text cellEnergy;

	public EggPanel eggPanel;

	private Cell m_cell;
	public Cell cell {
		get {
			return m_cell;
		}
		set {
			m_cell = value;
			UpdateRepresentation();
		}
	}

	public void UpdateRepresentation() {
		eggPanel.gameObject.SetActive(false);

		//Nothing to represent
		if (cell == null) {
			cellType.text = "Type:";
			cellEnergy.text = "Energy:";
			return;
		}

		cellType.text = "Type: " + cell.gene.type.ToString();
		cellEnergy.text = "Energy: 100%";

		if (cell is EggCell) {
			eggPanel.gameObject.SetActive(true);
		}  
	}
}