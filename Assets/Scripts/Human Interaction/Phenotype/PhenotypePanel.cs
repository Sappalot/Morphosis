using UnityEngine.UI;

public class PhenotypePanel : MonoSingleton<PhenotypePanel> {
    public Text creatureAge;
    public Text creatureCellCount;
    public Text creatureEnergy;

    public Text cellType;
    public Text cellEnergy;

    private Cell m_cell;
    public Cell cell {
        get {
            return m_cell;
        }
        set {
            m_cell = value;
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
        if (CreatureSelectionPanel.instance.selectedCreature == null) {
            creatureAge.text = "Age:";
            creatureCellCount.text = "Cells:";
            creatureEnergy.text = "Energy:";

            cellType.text = "Type:";
            cellEnergy.text = "Energy:";
            return;
        }

        creatureAge.text = "Age: 100 days";
        creatureCellCount.text = "Cells: 10 (20)";
        creatureEnergy.text = "Energy: 100%";

        if (cell == null) {
			cell = CreatureSelectionPanel.instance.selectedCreature.phenotype.rootCell;
		}

        cellType.text = "Type: " + cell.gene.type.ToString();
        cellEnergy.text = "Energy: 100%";
    }
}
