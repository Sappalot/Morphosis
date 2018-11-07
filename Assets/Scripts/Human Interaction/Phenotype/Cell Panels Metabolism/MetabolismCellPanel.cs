using UnityEngine;

public abstract class MetabolismCellPanel : MonoBehaviour {
	[HideInInspector]
	public PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	protected bool ignoreSliderMoved = false;

	protected bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
	}

	public void ApplyChange() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	public bool isUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;
	}

	public Color isUnlockedColor() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome ? Color.black : ColorScheme.instance.grayedOut;
	}

	public void OnChanged() {
		CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
		CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
		CreatureSelectionPanel.instance.soloSelected.generation = 1;
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
}