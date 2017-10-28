using UnityEngine;
using UnityEngine.UI;

public class CellPanel : MonoSingleton<CellPanel> {
	public Text cellType;
	public Text cellEnergy;
	public Text cellNeighbours;

	public EggPanel eggPanel;

	private bool isDirty = true;
	private Cell m_selectedCell;

	public void MakeDirty() {
		isDirty = true;
	}

	public Cell selectedCell {
		get {
			return m_selectedCell != null ? m_selectedCell : (CreatureSelectionPanel.instance.hasSoloSelected ? CreatureSelectionPanel.instance.soloSelected.phenotype.rootCell : null);
		}
		set {
			m_selectedCell = value;
			isDirty = true;
		}
	}

	public void OnClickDelete() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			Life.instance.DeleteCell(selectedCell);

			CreatureSelectionPanel.instance.MakeDirty();
			PhenotypePanel.instance.MakeDirty();
			isDirty = true;
		}
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update CellPanel");
			eggPanel.gameObject.SetActive(false);

			//Nothing to represent
			if (selectedCell == null || !CreatureSelectionPanel.instance.hasSoloSelected || !selectedCell.creature.phenotype.isAlive) {
				cellType.text = "Type:";
				cellEnergy.text = "Energy:";
				cellNeighbours.text = "Neighbours:";

				eggPanel.gameObject.SetActive(false);
				isDirty = false;
				return;
			}

			cellType.text = "Type: " + selectedCell.gene.type.ToString() + (selectedCell.isRoot ? " (Root)" : "");
			cellEnergy.text = "Energy: 100%";
			cellNeighbours.text = "Neighbours: " + (selectedCell.neighbourCountAll - selectedCell.neighbourCountConnected) + " + ("  + selectedCell.neighbourCountConnected + ")";

			if (selectedCell is EggCell) {
				eggPanel.gameObject.SetActive(true);
			}

			isDirty = false;
		}
	}
}