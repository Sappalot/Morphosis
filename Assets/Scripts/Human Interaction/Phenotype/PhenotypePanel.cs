using UnityEngine.UI;

public class PhenotypePanel : MonoSingleton<PhenotypePanel> {
	public Text creatureAge;
	public Text creatureCellCount;
	public Text creatureEnergy;
	public CellPanel cellPanel;

	public Cell cell {
		get {
			return cellPanel.cell;
		}
		set {
			cellPanel.cell = value;
		}
	}

	public void OnGrowClicked() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.TryGrow();
		}
	}

	public void OnShrinkClicked() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.TryShrink();
		}
	}

	public void UpdateRepresentation() {
		//Nothing to represent
		if (CreatureSelectionPanel.instance.soloSelected == null) {
			creatureAge.text = "Age:";
			creatureCellCount.text = "Cells:";
			creatureEnergy.text = "Energy:";

			cellPanel.cell = null;
			return;
		}

		creatureAge.text = "Age: 100 days";
		creatureCellCount.text = "Cells: 10 (20)";
		creatureEnergy.text = "Energy: 100%";

		if (cell == null) {
			cell = CreatureSelectionPanel.instance.soloSelected.phenotype.rootCell;
		}
	}
}