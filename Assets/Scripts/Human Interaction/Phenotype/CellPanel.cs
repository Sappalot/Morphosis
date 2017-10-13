using UnityEngine;
using UnityEngine.UI;

public class CellPanel : MonoSingleton<CellPanel> {
	public Text cellType;
	public Text cellEnergy;

	public EggPanel eggPanel;


	private bool isDirty = true;
	private Cell m_selectedCell;
	public Cell selectedCell {
		get {
			return m_selectedCell != null ? m_selectedCell : (CreatureSelectionPanel.instance.hasSoloSelected ? CreatureSelectionPanel.instance.soloSelected.phenotype.rootCell : null);
		}
		set {
			m_selectedCell = value;
			isDirty = true;
		}
	}

	
	private void Update() {
		if (isDirty) {
			Debug.Log("Update");
			eggPanel.gameObject.SetActive(false);

			//Nothing to represent
			if (selectedCell == null || !CreatureSelectionPanel.instance.hasSoloSelected) {
				cellType.text = "Type:";
				cellEnergy.text = "Energy:";

				eggPanel.gameObject.SetActive(false);
				isDirty = false;
				return;
			}

			cellType.text = "Type: " + selectedCell.gene.type.ToString();
			cellEnergy.text = "Energy: 100%";

			if (selectedCell is EggCell) {
				eggPanel.gameObject.SetActive(true);
			}

			isDirty = false;
		}
	}
}