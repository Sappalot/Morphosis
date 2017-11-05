using UnityEngine;
using UnityEngine.UI;

public class PhenotypePanel : MonoSingleton<PhenotypePanel> {
	public Text creatureAge;
	public Text creatureCellCount;
	public Text creatureEnergy;
	public Text creatureEffect;
	public CellPanel cellPanel;

	private bool isDirty = true;

	public void OnGrowClicked() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.TryGrow(false, 1, true);
		}
	}

	public void OnShrinkClicked() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.TryShrink();
			MakeDirty();
		}
	}

	public void OnClickDetatchFromMother() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.DetatchFromMother(true);
		}
	}

	public void OnClickHeal() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.ChangeEnergy(5f);
			CellPanel.instance.MakeDirty();
			MakeDirty();
		}
	}

	public void OnClickHurt() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.ChangeEnergy(-5f);
			CellPanel.instance.MakeDirty();
			MakeDirty();
		}
	}

	public void MakeDirty() {
		isDirty = true;
	}
	
	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update PhenotypePanel");
			//Nothing to represent
			Creature solo = CreatureSelectionPanel.instance.soloSelected;
			if (solo == null) {
				creatureAge.text = "Age:";
				creatureCellCount.text = "Cells: ";
				creatureEnergy.text = "Energy:";
				creatureEffect.text = "<Effect>:";

				isDirty = false;
				return;
			}

			creatureAge.text = "Age: 100 days";
			creatureCellCount.text = "Cells: " + solo.cellsCount + " (" + solo.cellsCountFullyGrown + ")";
			creatureEnergy.text = string.Format("Energy: {0:F1}%", solo.phenotype.energy / solo.phenotype.cellCount);
			creatureEffect.text = string.Format("<Effect>: {0:F1} - {1:F1} = {2:F1}W", solo.phenotype.effectProduction / solo.phenotype.cellCount, solo.phenotype.effectConsumption / solo.phenotype.cellCount, solo.phenotype.effect / solo.phenotype.cellCount);

			isDirty = false;
		}
	}
}