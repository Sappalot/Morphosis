using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PhenotypePanel : MonoSingleton<PhenotypePanel> {

	//public Text creatureSize;
	public CellPanel cellPanel;
	public GameObject bodyPanel;
	public SizeBar sizeBar;
	public Text sizeText;
	public EnergyBar energyBar;
	public AgeBar ageBar;
	public Text creatureSpeed;
	public Text creatureEffect;
	public Text creatureEffectAverage;
	public Text creatureAgeText;

	public Toggle followToggle;
	public Toggle yawToggle;

	private bool isDirty = true;

	public void OnFollowToggleChanged() {
		if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && !followToggle.isOn && CreatureSelectionPanel.instance.hasSoloSelected) {
			World.instance.cameraController.TurnCameraStraightAtCameraUnlock();
		}
	}

	public void OnGrowClicked() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.TryGrow(false, 1, true);
		}
		CellPanel.instance.MakeDirty(); // leaf cell size factor must be updated as size change (maybee other stuff too)
	}

	public void OnShrinkClicked() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.TryShrink();
			
		}
		MakeDirty();
		CellPanel.instance.MakeDirty(); // same as above
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

	// To work with button presses... dont want it to appear into a click
	private IEnumerator UpdateIsVisible() {
		yield return 0;
		bodyPanel.SetActive(CreatureSelectionPanel.instance.hasSelection && MouseAction.instance.actionState == MouseActionStateEnum.free && !AlternativeToolModePanel.instance.isOn);
		bool cellPanelActive = CreatureSelectionPanel.instance.hasSoloSelected && MouseAction.instance.actionState == MouseActionStateEnum.free && !AlternativeToolModePanel.instance.isOn;
		cellPanel.gameObject.SetActive(cellPanelActive);
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				DebugUtil.Log("Update PhenotypePanel");

			StartCoroutine(UpdateIsVisible());

			Creature solo = CreatureSelectionPanel.instance.soloSelected;

			energyBar.effectMeasure = EffectTempEnum.None;
			if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.effect) {
				if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureTotal) {
					energyBar.effectMeasure = EffectTempEnum.Total;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureProduction) {
					energyBar.effectMeasure = EffectTempEnum.Production;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureExternal) {
					energyBar.effectMeasure = EffectTempEnum.External;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureFlux) {
					energyBar.effectMeasure = EffectTempEnum.Flux;
				}
			}

			if (solo == null || !solo.phenotype.isAlive) {
				sizeBar.isOn = false;
				energyBar.isOn = false;
				ageBar.isOn = false;
				if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellTotal || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureTotal) {
					creatureEffect.text = "Total Effect/Cell: ";
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellProduction  || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureProduction) {
					creatureEffect.text = "Production Effect/Cell: ";
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellExternal || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureExternal) {
					creatureEffect.text = "External Effect/Cell: ";
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellFlux  || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureFlux) {
					creatureEffect.text = "Flux Effect/Cell: ";
				}

				creatureAgeText.text = "";
				creatureSpeed.text = "Speed:";

				isDirty = false;
				return;
			}

			sizeBar.isOn = true;
			sizeBar.UpdateBar(solo.genotype.geneCellCount, solo.phenotype.cellCount, solo.genotype.GetGeneCellOfTypeCount(CellTypeEnum.Egg), solo.phenotype.GetCellOfTypeCount(CellTypeEnum.Egg), solo.GetAttachedChildrenAliveCount());

			sizeText.text = "Size: " + solo.phenotype.cellCount + " / " + solo.genotype.geneCellCount;

			energyBar.isOn = true;
			energyBar.fullness = solo.phenotype.energyFullness;
			energyBar.effectTotal = solo.phenotype.EffectPerCell(true, true, true);
			energyBar.effectProd = solo.phenotype.EffectPerCell(true, false, false);
			energyBar.effectExternal = solo.phenotype.EffectPerCell(false, true, false);
			energyBar.effectFlux = solo.phenotype.EffectPerCell(false, false, true);

			//creatureEnergy.text = string.Format("Energy: {0:F2}%", solo.phenotype.energyFullness * 100f);

			if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellTotal || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureTotal) {
				creatureEffect.text = string.Format("Total Effect/Cell: {0:F2} - {1:F2} = {2:F2}W", solo.phenotype.EffectUpPerCell(true, true), solo.phenotype.EffectDownPerCell(true, true, true), solo.phenotype.EffectPerCell(true, true, true));
			} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellProduction || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureProduction) {
				creatureEffect.text = string.Format("Production Effect/Cell: {0:F2} - {1:F2} = {2:F2}W", solo.phenotype.EffectUpPerCell(true, false), solo.phenotype.EffectDownPerCell(true, false, false), solo.phenotype.EffectPerCell(true, false, false));
			} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellExternal || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureExternal) {
				creatureEffect.text = string.Format("External Effect/Cell: {0:F2} - {1:F2} = {2:F2}W", 0f, solo.phenotype.EffectDownPerCell(false, true, false), solo.phenotype.EffectPerCell(false, true, false));
			} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellFlux || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureFlux) {
				creatureEffect.text = string.Format("Flux Effect/Cell: {0:F2} - {1:F2} = {2:F2}W", solo.phenotype.EffectUpPerCell(false, true), solo.phenotype.EffectDownPerCell(false, false, true), solo.phenotype.EffectPerCell(false, false, true));
			}

			if (solo.creation != CreatureCreationEnum.Frozen) {
				ulong ageInSeconds = (ulong)(solo.GetAgeTicks(World.instance.worldTicks) * Time.fixedDeltaTime);
				if (ageInSeconds < 3600) {
					creatureAgeText.text = "Age: " + TimeUtil.GetTimeString(ageInSeconds);
				} else {
					creatureAgeText.text = "Age: Ancient";
				}

				ageBar.isOn = true;
				ageBar.SetAge(ageInSeconds, GlobalSettings.instance.phenotype.maxAge);
			}


			creatureSpeed.text = string.Format("Speed: {0:F2} m/s", solo.phenotype.speed);

			isDirty = false;
		}
	}
}