using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellAndGeneOriginComponentPanel : CellAndGeneComponentPanel {
	// Detatch
	public LogicBoxPanel detatchLogicBoxPanel;

	// Embryo max size

	// Pulse
	public Text pulseFrequenzySliderText;
	public Slider pulseFrequenzySlider;
	public Text pulseWaveCompletenessText;

	public override void Initialize(PhenoGenoEnum mode) {
		base.Initialize(mode);

		detatchLogicBoxPanel.Initialize(mode, SignalUnitEnum.OriginDetatchLogicBox);

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

	public override List<GeneLogicBoxInput> GetAllGeneGeneLogicBoxInputs() {
		return detatchLogicBoxPanel.GetAllGeneGeneLogicBoxInputs();
	}

	public override void MakeDirty() {
		base.MakeDirty();
		detatchLogicBoxPanel.MakeDirty();
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

				// pulse
				pulseFrequenzySlider.interactable = false;

				if (isOriginPhenotypeSelected) {
					pulseWaveCompletenessText.text = string.Format("Wave complete: {0:F1} of {1:F0} ticks, completeness {2:F2}", originCell.originPulseTick, originCell.gene.originPulsePeriodTicks, originCell.originPulseCompleteness);
				} else {
					pulseWaveCompletenessText.text = string.Format("Wave complete: -");
				}
			} else {
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

			detatchLogicBoxPanel.outputText = "Detatch from mother";

			ignoreSliderMoved = false;
			isDirty = false;
		}
	}
}