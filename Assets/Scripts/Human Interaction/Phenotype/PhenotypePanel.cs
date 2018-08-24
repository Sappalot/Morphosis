using UnityEngine;
using UnityEngine.UI;

public class PhenotypePanel : MonoSingleton<PhenotypePanel> {
	public Text creatureAge;
	public Text creatureCellCount;
	public Text creatureEnergy;
	public Text creatureSpeed;
	public Text creatureEffect;
	public Text creatureEffectAverage;
	public CellPanel cellPanel;

	public Toggle followToggle;
	public Toggle yawToggle;

	private bool isDirty = true;

	public Dropdown effectMeasuredDropdown;
	public enum EffectMeasureEnum {
		CellEffectExclusiveFlux,
		CellEffectInclusiveFlux,
		CellEffectAverageExclusiveFlux,
		CellEffectAverageInclusiveFlux,
	}
	[HideInInspector]
	public EffectMeasureEnum effectMeasure {
		get {
			return (EffectMeasureEnum)effectMeasuredDropdown.value;
		}
	}

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
				if (effectMeasure == EffectMeasureEnum.CellEffectExclusiveFlux) {
					creatureEffect.text = "P body:";
				}
				else if (effectMeasure == EffectMeasureEnum.CellEffectInclusiveFlux) {
					creatureEffect.text = "P body:";
				}
				else if (effectMeasure == EffectMeasureEnum.CellEffectAverageExclusiveFlux) {
					creatureEffect.text = "P/cell avg.:";
				}
				else if (effectMeasure == EffectMeasureEnum.CellEffectAverageInclusiveFlux) {
					creatureEffect.text = "P/cell avg.:";
				}
				creatureSpeed.text = "Speed:";

				isDirty = false;
				return;
			}

			creatureAge.text = "Age: " + TimeUtil.GetTimeString((ulong)(solo.GetAgeTicks(World.instance.worldTicks) * Time.fixedDeltaTime));
			creatureCellCount.text = "Cells: " + solo.cellCount + " (" + solo.cellsCountFullyGrown + ")";
			creatureEnergy.text = string.Format("Energy: {0:F2}%", solo.phenotype.energyFullness * 100f);

			if (effectMeasure ==      EffectMeasureEnum.CellEffectExclusiveFlux) {
				//Total effect excluding energy inport/export to attached 
				creatureEffect.text = string.Format("P body: {0:F2} - {1:F2} = {2:F2}W", solo.phenotype.GetEffectUp(true, false, false), solo.phenotype.GetEffectDown(true, false, false), solo.phenotype.GetEffect(true, false, false));
			}
			else if (effectMeasure == EffectMeasureEnum.CellEffectInclusiveFlux) {
				//Total effect including energy inport/export to attached 
				creatureEffect.text = string.Format("P body: {0:F2} - {1:F2} = {2:F2}W", solo.phenotype.GetEffectUp(true, false, true), solo.phenotype.GetEffectDown(true, false, true), solo.phenotype.GetEffect(true, false, true));
			}
			else if (effectMeasure == EffectMeasureEnum.CellEffectAverageExclusiveFlux) {
				creatureEffect.text = string.Format("P/cell avg.: {0:F2} - {1:F2} = {2:F2}W", solo.phenotype.GetEffectUp(true, false, false) / solo.phenotype.cellCount, solo.phenotype.GetEffectDown(true, false, false) / solo.phenotype.cellCount, solo.phenotype.GetEffect(true, false, false) / solo.phenotype.cellCount);
			}
			else if (effectMeasure == EffectMeasureEnum.CellEffectAverageInclusiveFlux) {
				creatureEffect.text = string.Format("P/cell avg.: {0:F2} - {1:F2} = {2:F2}W", solo.phenotype.GetEffectUp(true, false, true) / solo.phenotype.cellCount, solo.phenotype.GetEffectDown(true, false, true) / solo.phenotype.cellCount, solo.phenotype.GetEffect(true, false, true) / solo.phenotype.cellCount);
			}

			creatureSpeed.text = string.Format("Speed: {0:F2} m/s", solo.phenotype.speed);

			isDirty = false;
		}
	}
}