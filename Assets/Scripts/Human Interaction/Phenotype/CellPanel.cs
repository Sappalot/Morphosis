using UnityEngine;
using UnityEngine.UI;

public class CellPanel : MonoSingleton<CellPanel> {
	public Text cellType;
	public Text cellEnergy;
	public Text cellEffect;
	public Text cellNeighbours;
	public Text connectionGroupCount;

	public EggCellPanel eggCellPanel;
	public JawCellPanel jawCellPanel;

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

	public void OnClickHeal() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			selectedCell.energy = Mathf.Min(selectedCell.energy + 5f, Cell.maxEnergy);

			PhenotypePanel.instance.MakeDirty();
			isDirty = true;
		}
	}

	public void OnClickHurt() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			selectedCell.energy -= 5f;

			PhenotypePanel.instance.MakeDirty();
			isDirty = true;
		}
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			eggCellPanel.gameObject.SetActive(false);
			jawCellPanel.gameObject.SetActive(false);

			//Nothing to represent
			if (selectedCell == null || !CreatureSelectionPanel.instance.hasSoloSelected || !selectedCell.creature.phenotype.isAlive) {
				cellType.text = "Type:";
				cellEnergy.text = "Energy:";
				cellEffect.text = "Effect:";
				cellNeighbours.text = "Neighbours:";
				connectionGroupCount.text = "Con. Groups: ";

				isDirty = false;
				return;
			}

			cellType.text = "Type: " + selectedCell.gene.type.ToString() + (selectedCell.isRoot ? " (Rt)" : "");
			cellEnergy.text = string.Format("Energy: {0:F1}J", selectedCell.energy);
			cellEffect.text = string.Format("Effect: {0:F1} - {1:F1} = {2:F1}W", selectedCell.effectProduction, selectedCell.effectConsumption, selectedCell.effect);
			cellNeighbours.text = "Neighbours: " + (selectedCell.neighbourCountAll - selectedCell.neighbourCountConnected) + " + ("  + selectedCell.neighbourCountConnected + ")";
			connectionGroupCount.text = "Con. Groups: " + selectedCell.groups;
			if (selectedCell is EggCell) {
				eggCellPanel.gameObject.SetActive(true);
			} else if (selectedCell is JawCell) {
				jawCellPanel.gameObject.SetActive(true);
			}

			isDirty = false;
		}
	}
}