using UnityEngine;
using UnityEngine.UI;

public class CellPanel : MonoSingleton<CellPanel> {
	public EnergyBar energyBar;

	public Text cellType;
	public Text cellEffect;
	public Text isOrigo;
	public Text isPlacenta;
	public Text cellNeighbours;
	public Text connectionGroupCount;
	public Text detatchThreshold;
	public Text predators; //number of Jaw cells eating on me

	public EggCellPanel eggCellPanel;
	public JawCellPanel jawCellPanel;
	public LeafCellPanel leafCellPanel;

	private bool isDirty = true;
	private Cell m_selectedCell;

	public void MakeDirty() {
		isDirty = true;

		EggCellPanel.instance.MakeDirty();
		JawCellPanel.instance.MakeDirty();
		LeafCellPanel.instance.MakeDirty();
	}

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

	public void OnClickDelete() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			World.instance.life.KillCellSafe(selectedCell, World.instance.worldTicks);

			CreatureSelectionPanel.instance.MakeDirty();
			PhenotypePanel.instance.MakeDirty();
			MakeDirty();
		}
	}

	public void OnClickHeal() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			selectedCell.energy = Mathf.Min(selectedCell.energy + 5f, GlobalSettings.instance.phenotype.cellMaxEnergy);

			PhenotypePanel.instance.MakeDirty();
			MakeDirty();
		}
	}

	public void OnClickHurt() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			selectedCell.energy -= 5f;

			PhenotypePanel.instance.MakeDirty();
			MakeDirty();
		}
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			eggCellPanel.gameObject.SetActive(false);
			jawCellPanel.gameObject.SetActive(false);
			leafCellPanel.gameObject.SetActive(false);

			//Nothing to represent
			if (selectedCell == null || !CreatureSelectionPanel.instance.hasSoloSelected) {
				cellType.text = "Type:";
				energyBar.isOn = false;
				cellEffect.text = "P cell:";
				isOrigo.text = "-";
				isPlacenta.text = "-";
				cellNeighbours.text = "Neighbours:";
				connectionGroupCount.text = "Con. Groups: ";
				detatchThreshold.text = "Detatch: ";
				predators.text = "Eating on me:";

				isDirty = false;
				return;
			}

			cellType.text = "Type: " + selectedCell.gene.type.ToString();
			energyBar.isOn = true;
			energyBar.fullness = selectedCell.energyFullness;
			energyBar.effect = selectedCell.GetEffect(true, true, true);

			//cellEnergy.text = string.Format("Energy: {0:F2}% ({1:F2}/{2:F2}J)", selectedCell.energyFullness * 100f, selectedCell.energy, GlobalSettings.instance.phenotype.cellMaxEnergy);

			if (PhenotypePanel.instance.effectMeasure == PhenotypePanel.EffectMeasureEnum.CellEffectExclusiveFlux || PhenotypePanel.instance.effectMeasure == PhenotypePanel.EffectMeasureEnum.CellEffectAverageExclusiveFlux) {
				//Total effect excluding energy inport/export to attached 
				cellEffect.text = string.Format("P cell: {0:F2} - {1:F2} = {2:F2}W", selectedCell.GetEffectUp(true, false, false), selectedCell.GetEffectDown(true, false, false), selectedCell.GetEffect(true, false, false));
			} else {
				cellEffect.text = string.Format("P cell: {0:F2} - {1:F2} = {2:F2}W", selectedCell.GetEffectUp(true, true, true), selectedCell.GetEffectDown(true, true, true), selectedCell.GetEffect(true, true, true));
			}

			isOrigo.text = selectedCell.isOrigin ? "Origin" : "...";
			isPlacenta.text = selectedCell.isPlacenta ? "Placenta" : "...";
			cellNeighbours.text = "Neighbours: " + (selectedCell.neighbourCountAll - selectedCell.neighbourCountConnectedRelatives) + ((selectedCell.neighbourCountConnectedRelatives > 0) ? (" + "  + selectedCell.neighbourCountConnectedRelatives + " relatives") : "");
			connectionGroupCount.text = "Con. Groups: " + selectedCell.groups;
			predators.text = "Eating on me: " + selectedCell.predatorCount;
			detatchThreshold.text = "Detatch: " + selectedCell.originDetatchMode;

			if (selectedCell is EggCell) {
				eggCellPanel.gameObject.SetActive(true);
			} else if (selectedCell is JawCell) {
				jawCellPanel.gameObject.SetActive(true);
			} else if (selectedCell is LeafCell) {
				leafCellPanel.gameObject.SetActive(true);
			}

			isDirty = false;
		}
	}
}