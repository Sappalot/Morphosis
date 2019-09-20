﻿using UnityEngine;
using UnityEngine.UI;

public class OriginCellComponentPanel : MonoBehaviour {
	public Text detatchConditionsText;

	[HideInInspector]
	public PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	public Text pulseFrequenzySliderText;
	public Slider pulseFrequenzySlider;
	public Text pulseWaveCompletenessText;

	private bool ignoreSliderMoved = false;

	private void Awake() {
		ignoreSliderMoved = true;
		pulseFrequenzySlider.minValue = GlobalSettings.instance.phenotype.originPulseFrequenzyMin;
		pulseFrequenzySlider.maxValue = GlobalSettings.instance.phenotype.originPulseFrequenzyMax;
		ignoreSliderMoved = false;
	}

	public void OnPulseFrequenzySliderMoved() { 
		if (ignoreSliderMoved) {
			return;
		}

		GenePanel.instance.selectedGene.originPulsePeriodTicks = Mathf.CeilToInt(1f / (Time.fixedDeltaTime * pulseFrequenzySlider.value));
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		MakeDirty();
	}

	protected bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
	}

	private void Update() {
		if (isDirty) {
			ignoreSliderMoved = true;

			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			bool isOriginPhenotypeSelected = mode == PhenoGenoEnum.Phenotype && CellPanel.instance.selectedCell != null && CellPanel.instance.selectedCell.isOrigin;
			bool isOriginGenotypeSelected = mode == PhenoGenoEnum.Genotype && GenePanel.instance.selectedGene != null && GenePanel.instance.selectedGene.isOrigin;
			

			if (mode == PhenoGenoEnum.Phenotype) {
				Cell originCell = CellPanel.instance.selectedCell;

				if (!isOriginPhenotypeSelected) {
					detatchConditionsText.text = "Detatch when: -";
				} else if (originCell.creature.creation != CreatureCreationEnum.Born) {
					detatchConditionsText.text = "Detatch when: Creature wasn't born ==> No detatch conditions";
				} else if (originCell.originDetatchMode == ChildDetatchModeEnum.Size) {
					detatchConditionsText.text = string.Format("Detatch when: Body size ≥ {0:F1}% which is {1:F0} of {2:F0} cells", originCell.originDetatchSizeThreshold * 100f, (Mathf.Clamp(Mathf.RoundToInt(originCell.originDetatchSizeThreshold * originCell.creature.genotype.geneCellCount), 1, originCell.creature.genotype.geneCellCount)), originCell.creature.genotype.geneCellCount);
				} else {
					detatchConditionsText.text = string.Format("Detatch when: Can't grow more and cell energy ≥ {0:F1}%", originCell.originDetatchEnergyThreshold * 100f);
				}
				// pulse
				pulseFrequenzySlider.interactable = false;

				if (isOriginPhenotypeSelected) {
					pulseWaveCompletenessText.text = string.Format("Wave complete: {0:F1} of {1:F0} ticks, completeness {2:F2}", originCell.originPulseTick, originCell.gene.originPulsePeriodTicks, originCell.originPulseCompleteness);
				} else {
					pulseWaveCompletenessText.text = string.Format("Wave complete: -");
				}
			} else {
				detatchConditionsText.text = "Detatch when: -";
				//pulse
				pulseFrequenzySlider.interactable = isOriginGenotypeSelected && isUnlocked();

				pulseWaveCompletenessText.text = string.Format("Wave complete: -");
			}
			if (isOriginPhenotypeSelected || isOriginGenotypeSelected) {
				pulseFrequenzySlider.value = 1f / (GenePanel.instance.selectedGene.originPulsePeriodTicks * Time.fixedDeltaTime);
				pulseFrequenzySliderText.text = string.Format("Ferquenzy: {0:F2} Hz ==> Period: {1:F2} s = {2:F0} ticks", 1f / (GenePanel.instance.selectedGene.originPulsePeriodTicks * Time.fixedDeltaTime), GenePanel.instance.selectedGene.originPulsePeriodTicks * Time.fixedDeltaTime, GenePanel.instance.selectedGene.originPulsePeriodTicks);
			} else {
				pulseFrequenzySlider.value = 1f;
				pulseFrequenzySliderText.text = "Ferquenzy: -";
			}

			ignoreSliderMoved = false;
			isDirty = false;
		}
	}

	private bool isUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;
	}
}