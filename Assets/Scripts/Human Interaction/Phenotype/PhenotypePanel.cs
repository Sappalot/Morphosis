using UnityEngine;
using UnityEngine.UI;

public class PhenotypePanel : MonoSingleton<PhenotypePanel> {
	public EnergyBar energyBar;

	public Text creatureAge;
	//public Text creatureSize;
	public SizeBar sizeBar;
	public Text creatureSpeed;
	public Text creatureEffect;
	public Text creatureEffectAverage;
	public CellPanel cellPanel;

	public Toggle followToggle;
	public Toggle yawToggle;

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

			energyBar.effectMeasure = EffectTempEnum.None;
			if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.effect) {
				if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureTotal) {
					energyBar.effectMeasure = EffectTempEnum.Total;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureProduction) {
					energyBar.effectMeasure = EffectTempEnum.Production;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureFlux) {
					energyBar.effectMeasure = EffectTempEnum.Flux;
				}
			}

			if (solo == null || !solo.phenotype.isAlive) {
				creatureAge.text = "Age:";
				//creatureSize.text = "Size: ";
				energyBar.isOn = false;
				sizeBar.isOn = false;
				if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellTotal || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureTotal) {
					creatureEffect.text = "Total Effect/Cell: ";
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellProduction  || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureProduction) {
					creatureEffect.text = "Production Effect/Cell: ";
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellFlux  || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureFlux) {
					creatureEffect.text = "Flux Effect/Cell: ";
				}

				//creatureEffect.text = "P/cell avg.:";
				creatureSpeed.text = "Speed:";

				isDirty = false;
				return;
			}

			creatureAge.text = "Age: " + TimeUtil.GetTimeString((ulong)(solo.GetAgeTicks(World.instance.worldTicks) * Time.fixedDeltaTime));
			//creatureSize.text = "Size: " + solo.cellCount + " (" + solo.cellsCountFullyGrown + ")";

			energyBar.isOn = true;
			energyBar.fullness = solo.phenotype.energyFullness;
			energyBar.effectTotal = solo.phenotype.GetEffectPerCell(true, true);
			energyBar.effectProd = solo.phenotype.GetEffectPerCell(true, false);
			energyBar.effectFlux = solo.phenotype.GetEffectPerCell(false, true);

			sizeBar.isOn = true;
			sizeBar.UpdateBar(solo.genotype.geneCellCount, solo.phenotype.cellCount, solo.genotype.GetGeneCellOfTypeCount(CellTypeEnum.Egg), solo.phenotype.GetCellOfTypeCount(CellTypeEnum.Egg), solo.GetAttachedChildrenAliveCount());
			//creatureEnergy.text = string.Format("Energy: {0:F2}%", solo.phenotype.energyFullness * 100f);

			if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellTotal || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureTotal) {
				creatureEffect.text = string.Format("Total Effect/Cell: {0:F2} - {1:F2} = {2:F2}W", solo.phenotype.GetEffectUpPerCell(true, true), solo.phenotype.GetEffectDownPerCell(true, true), solo.phenotype.GetEffectPerCell(true, true));
			} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellProduction || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureProduction) {
				creatureEffect.text = string.Format("Production Effect/Cell: {0:F2} - {1:F2} = {2:F2}W", solo.phenotype.GetEffectUpPerCell(true, false), solo.phenotype.GetEffectDownPerCell(true, false), solo.phenotype.GetEffectPerCell(true, false));
			} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellFlux || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureFlux) {
				creatureEffect.text = string.Format("Flux Effect/Cell: {0:F2} - {1:F2} = {2:F2}W", solo.phenotype.GetEffectUpPerCell(false, true), solo.phenotype.GetEffectDownPerCell(false, true), solo.phenotype.GetEffectPerCell(false, true));
			}

			creatureSpeed.text = string.Format("Speed: {0:F2} m/s", solo.phenotype.speed);

			isDirty = false;
		}
	}
}