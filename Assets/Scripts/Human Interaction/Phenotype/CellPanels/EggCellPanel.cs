using UnityEngine;
using UnityEngine.UI;

public class EggCellPanel : MetabolismCellPanel {
	public Text fertilizeButtonText;

	public void OnClickFertilize() {
		if (CreatureSelectionPanel.instance.hasSoloSelected && mode == PhenoGenoEnum.Phenotype) {
			World.instance.life.FertilizeCreature(CellPanel.instance.selectedCell, true, World.instance.worldTicks, true);
		}
	}

	private void Update() {
		if (isDirty) {
			if (mode == PhenoGenoEnum.Phenotype) {
				fertilizeButtonText.color = Color.black;
			} else if (mode == PhenoGenoEnum.Genotype)  {
				fertilizeButtonText.color = Color.gray;
			}

			isDirty = false;
		}
	}
}