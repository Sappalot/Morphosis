﻿using UnityEngine;
using UnityEngine.UI;

public class CellPanel : MonoSingleton<CellPanel> {
	public Text cellType;
	public Text cellEnergy;
	public Text cellEffect;
	public Text cellNeighbours;
	public Text connectionGroupCount;
	public Text predators; //number of Jaw cells eating on me

	public EggCellPanel eggCellPanel;
	public JawCellPanel jawCellPanel;

	private bool isDirty = true;
	private Cell m_selectedCell;

	public void MakeDirty() {
		isDirty = true;

		JawCellPanel.instance.MakeDirty();
	}

	public Cell selectedCell {
		get {
			return m_selectedCell != null ? m_selectedCell : (CreatureSelectionPanel.instance.hasSoloSelected ? CreatureSelectionPanel.instance.soloSelected.phenotype.originCell : null);
		}
		set {
			m_selectedCell = value;
			MakeDirty();
		}
	}

	public void OnClickDelete() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			Life.instance.KillCellSafe(selectedCell, World.instance.worldTicks);

			CreatureSelectionPanel.instance.MakeDirty();
			PhenotypePanel.instance.MakeDirty();
			MakeDirty();
		}
	}

	public void OnClickHeal() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			selectedCell.energy = Mathf.Min(selectedCell.energy + 5f, Cell.maxEnergy);

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

			//Nothing to represent
			if (selectedCell == null || !CreatureSelectionPanel.instance.hasSoloSelected) {
				cellType.text = "Type:";
				cellEnergy.text = "Energy:";
				cellEffect.text = "Effect:";
				cellNeighbours.text = "Neighbours:";
				connectionGroupCount.text = "Con. Groups: ";
				predators.text = "Eating on me:";

				isDirty = false;
				return;
			}

			cellType.text = "Type: " + selectedCell.gene.type.ToString() + (selectedCell.isOrigin ? " (O)" : "") + (selectedCell.isPlacenta ? " (P)" : "");
			cellEnergy.text = string.Format("Energy: {0:F2}J", selectedCell.energy);
			if (GlobalPanel.instance.physicsUpdateMetabolism.isOn) {
				cellEffect.text = string.Format("Effect: {0:F2} - {1:F2} = {2:F2}W", selectedCell.effectProduction, selectedCell.effectConsumption, selectedCell.effect);
			} else {
				cellEffect.text = "Effect: -";
			}
			
			cellNeighbours.text = "Neighbours: " + (selectedCell.neighbourCountAll - selectedCell.neighbourCountConnectedRelatives) + " + ("  + selectedCell.neighbourCountConnectedRelatives + ")";
			connectionGroupCount.text = "Con. Groups: " + selectedCell.groups;
			predators.text = "Eating on me: " + selectedCell.predatorCount;
			if (selectedCell is EggCell) {
				eggCellPanel.gameObject.SetActive(true);
			} else if (selectedCell is JawCell) {
				jawCellPanel.gameObject.SetActive(true);
			}

			isDirty = false;
		}
	}
}