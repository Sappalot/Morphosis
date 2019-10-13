﻿using System.Collections.Generic;
using UnityEngine;


// a panel that can change genotype
// Me <== (CellAndGeneSignalUnitPanel), EggCellPanel, JawCellPanel
public abstract class CellAndGeneComponentPanel : MonoBehaviour {
	[HideInInspector]
	protected PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	protected bool ignoreSliderMoved = false;
	protected bool isDirty = false;
	public CellAndGeneFooterPanel footerPanel;

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

	public bool IsUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;
	}

	public void OnGenomeChanged(bool geneCellsDiffersFromGenome) {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = geneCellsDiffersFromGenome;
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}

		CreatureSelectionPanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		MakeDirty();

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