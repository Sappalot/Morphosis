using System.Collections.Generic;
using UnityEngine;


// a panel that can change genotype
// Me <== (SignalUnitPanel), EggCellPanel, JawCellPanel
public abstract class ComponentPanel : MonoBehaviour {
	[HideInInspector]
	protected PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	protected bool ignoreHumanInput = false;
	protected bool isDirty = false;
	public ComponentFooterPanel componentFooterPanel;
	

	[HideInInspector]
	public CellAndGenePanel cellAndGenePanel;

	protected PhenoGenoEnum GetMode() {
		return mode;
	}

	public virtual void Initialize(PhenoGenoEnum mode, CellAndGenePanel cellAndGenePanel) {
		this.mode = mode;
		this.cellAndGenePanel = cellAndGenePanel;
	}

	public virtual ViewXput? viewXput {
		get {
			return null;
		}
	}

	// new



	// old
	// not pretty :/ try to merge into one input
	public virtual List<IGeneInput> GetAllGeneInputs() {
		return null;
	}
	// ^ merge ^

	public virtual void MakeDirty() {
		isDirty = true;
	}

	public bool IsUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome && !cellAndGenePanel.isAuxiliary;
	}

	public void OnGenomeChanged() {
		// updates as gene is changes
		//if (CreatureSelectionPanel.instance.hasSoloSelected) {
		//	CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
		//	CreatureSelectionPanel.instance.soloSelected.generation = 1;
		//}

		CreatureSelectionPanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		MakeDirty();
	}

	public Gene selectedGene {
		get {
			return cellAndGenePanel.gene;
		}
	}

	public Cell selectedCell {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return cellAndGenePanel.cell;
			} else {
				return null; // there could be many cells selected for the same gene
			}
		}
	}
}