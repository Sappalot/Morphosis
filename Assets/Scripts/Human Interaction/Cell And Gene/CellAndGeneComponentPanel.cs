﻿using System.Collections.Generic;
using UnityEngine;


// a panel that can change genotype
// Me <== (CellAndGeneSignalUnitPanel), EggCellPanel, JawCellPanel
public abstract class CellAndGeneComponentPanel : MonoBehaviour {
	[HideInInspector]
	protected PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	protected bool ignoreSliderMoved = false;
	protected bool isDirty = false;

	protected PhenoGenoEnum GetMode() {
		return mode;
	}

	public virtual void Initialize(PhenoGenoEnum mode) {
		this.mode = mode;
	}

	public virtual List<GeneLogicBoxInput> GetAllGeneGeneLogicBoxInputs() {
		return null;
	}

	
	public virtual void MakeDirty() {
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

	public string productionEffectPhenotypeString {
		get {
			return string.Format("Production Effect: {0:F2} - {1:F2} = {2:F2} W", selectedCell.effectProductionInternalUp, selectedCell.effectProductionInternalDown, selectedCell.effectProductionInternalUp - selectedCell.effectProductionInternalDown)
			 + (selectedCell.IsHibernating() ? " (hibernating)" : "");
		}
	}

	public Gene selectedGene {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell != null ? CellPanel.instance.selectedCell.gene : null;
			} else {
				return GenePanel.instance.selectedGene;
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