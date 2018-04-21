using UnityEngine;
using UnityEngine.UI;

public class PhenotypePanel : MonoSingleton<PhenotypePanel> {
	public Text creatureAge;
	public Text creatureCellCount;
	public Text creatureEnergy;
	public Text creatureSpeed;
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
			creature.DetatchFromMother(true, true);
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
				creatureEffect.text = "Effect:";

				isDirty = false;
				return;
			}

			creatureAge.text = "Age: " + TimeUtil.GetTimeString((ulong)(solo.GetAgeTicks(World.instance.worldTicks) * Time.fixedDeltaTime));
			creatureCellCount.text = "Cells: " + solo.cellsCount + " (" + solo.cellsCountFullyGrown + ")";
			creatureEnergy.text = string.Format("Energy: {0:F2}%", solo.phenotype.energy / solo.phenotype.cellCount);
			creatureSpeed.text = string.Format("Speed: {0:F2} m/s", solo.phenotype.speed);

			creatureEffect.text = string.Format("Effect: {0:F2} - {1:F2} = {2:F2}W", solo.phenotype.effectProduction, solo.phenotype.effectConsumption, solo.phenotype.effect);


			isDirty = false;
		}
	}
}