using UnityEngine;
using UnityEngine.UI;

public abstract class MetabolismCellPanel : MonoBehaviour {
	public PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

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
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome ? Color.black : Color.gray;
	}

	public void OnChanged() {
		CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
		CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
		CreatureSelectionPanel.instance.soloSelected.generation = 1;
	}
}