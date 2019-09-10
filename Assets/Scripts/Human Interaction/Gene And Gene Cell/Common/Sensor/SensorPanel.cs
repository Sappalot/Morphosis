﻿using UnityEngine;

public abstract class CellSensorPanel : MonoBehaviour {
	[HideInInspector]
	protected bool ignoreSliderMoved = false;

	[HideInInspector]
	public PhenoGenoEnum mode { get; set; }
	protected SignalUnitEnum outputUnit;

	public void Initialize(PhenoGenoEnum mode, SignalUnitEnum outputUnit) {
		this.mode = mode;
		this.outputUnit = outputUnit;
	}

	protected bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
	}

	public void ApplyChange() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			MakeCreatureChanged();
		}
		MakeDirty();
	}

	public bool IsUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;
	}

	public void MakeCreatureChanged() {
		CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
		CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
		CreatureSelectionPanel.instance.soloSelected.generation = 1;
	}

	public Gene selectedGene {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell != null ? CellPanel.instance.selectedCell.gene : null;
			} else {
				return GeneCellPanel.instance.selectedGene;
			}
		}
	}

	public Cell selectedCell {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell;
			} else {
				return null; // there could be many cells selected for the same gene
			}
		}
	}
}
