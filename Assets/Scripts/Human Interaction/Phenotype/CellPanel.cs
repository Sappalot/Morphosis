using UnityEngine;
using UnityEngine.UI;

public class CellPanel : MonoSingleton<CellPanel> {

	public Text typeHeadingLabel;
	public CellAndGenePanel cellAndGenePanel;

	private Cell m_selectedCell;

	public Cell selectedCell {
		get {
			if (m_selectedCell != null) {
				return m_selectedCell;
			}
			if (CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.phenotype.isAlive) {
				if (CreatureSelectionPanel.instance.soloSelected.phenotype.originCell != null) {
					return CreatureSelectionPanel.instance.soloSelected.phenotype.originCell;
				}
				return null;
			}
			return null;
		}
		set {
			m_selectedCell = value;
			MakeDirty();
		}
	}

	public void Initialize() {
		cellAndGenePanel.Initialize(PhenoGenoEnum.Phenotype, false);
		MakeDirty();
	}

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
		cellAndGenePanel.MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				DebugUtil.Log("Update CellPanel");
			}
			if (selectedCell == null || !CreatureSelectionPanel.instance.hasSoloSelected) {
				typeHeadingLabel.text = "Cell:";
			} else {
				typeHeadingLabel.text = "Cell: " + selectedCell.GetCellType().ToString();
			}

			isDirty = false;
		}
	}
}