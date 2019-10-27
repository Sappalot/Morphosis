using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellAndGeneOriginComponentPanel : CellAndGeneComponentPanel {
	// Detatch
	public LogicBoxPanel detatchLogicBoxPanel;

	// Creature size sensor
	public SizeSensorPanel sizeSensorPanel;

	// Embryo max size
	public Text embryoMaxSizeSliderLabel;
	public Slider embryoMaxSizeSlider;

	// Cell frowth persistance
	public Text growPriorityCellPersistanceLabel;
	public Slider growPriorityCellPersistanceSlider;

	// Pulse
	public Text pulseFrequenzySliderText;
	public Slider pulseFrequenzySlider;
	public Text pulseWaveCompletenessText;

	public override void Initialize(PhenoGenoEnum mode) {
		base.Initialize(mode);

		detatchLogicBoxPanel.Initialize(mode, SignalUnitEnum.OriginDetatchLogicBox);

		sizeSensorPanel.Initialize(mode, SignalUnitEnum.OriginSizeSensor);

		ignoreSliderMoved = true;
		pulseFrequenzySlider.minValue = GlobalSettings.instance.phenotype.originPulseFrequenzyMin;
		pulseFrequenzySlider.maxValue = GlobalSettings.instance.phenotype.originPulseFrequenzyMax;
		ignoreSliderMoved = false;
	}

	// ...pulse...
	public void OnPulseFrequenzySliderMoved() { 
		if (ignoreSliderMoved || mode == PhenoGenoEnum.Phenotype) {
			return;
		}

		GenePanel.instance.selectedGene.originPulsePeriodTicks = Mathf.CeilToInt(1f / (Time.fixedDeltaTime * pulseFrequenzySlider.value));
		OnGenomeChanged(false);
	}
	// ^pulse^

	// ...embryo max size...
	public void OnEmbryoSizeSliderMoved() {
		if (ignoreSliderMoved || mode == PhenoGenoEnum.Phenotype) {
			return;
		}

		GenePanel.instance.selectedGene.embryoMaxSizeCompleteness = embryoMaxSizeSlider.value;
		OnGenomeChanged(false);
	}
	// ^embryo max size^

	public void OnGrowPriorityCellPersistanceSliderMoved() {
		if (ignoreSliderMoved || mode == PhenoGenoEnum.Phenotype) {
			return;
		}

		GenePanel.instance.selectedGene.growPriorityCellPersistance = (int)growPriorityCellPersistanceSlider.value;
		OnGenomeChanged(false);
	}

	public override List<GeneLogicBoxInput> GetAllGeneGeneLogicBoxInputs() {
		return detatchLogicBoxPanel.GetAllGeneGeneLogicBoxInputs();
	}

	public override void MakeDirty() {
		base.MakeDirty();
		detatchLogicBoxPanel.MakeDirty();
		sizeSensorPanel.MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			ignoreSliderMoved = true;

			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			bool isOriginPhenotypeSelected = mode == PhenoGenoEnum.Phenotype && CellPanel.instance.selectedCell != null && CellPanel.instance.selectedCell.isOrigin;
			bool isOriginGenotypeSelected = mode == PhenoGenoEnum.Genotype && GenePanel.instance.selectedGene != null && GenePanel.instance.selectedGene.isOrigin;
			if (!(isOriginPhenotypeSelected || isOriginGenotypeSelected) || !CreatureSelectionPanel.instance.hasSoloSelected) {
				isDirty = false;
				return;
			}

			Cell originCell = CellPanel.instance.selectedCell;

			embryoMaxSizeSliderLabel.text = string.Format("Embryo max size: {0:F0} % ==> {1} of {2} cells", originCell.gene.embryoMaxSizeCompleteness * 100f, CreatureSelectionPanel.instance.soloSelected.CellCountAtCompleteness(originCell.gene.embryoMaxSizeCompleteness), CreatureSelectionPanel.instance.soloSelected.genotype.geneCellCount);
			growPriorityCellPersistanceLabel.text = string.Format("Persist to grow blocked priority cell for up to: {0:F0} s", originCell.gene.growPriorityCellPersistance);
			
			if (mode == PhenoGenoEnum.Phenotype) {
				embryoMaxSizeSlider.interactable = false;
				growPriorityCellPersistanceSlider.interactable = false;

				// pulse
				pulseFrequenzySlider.interactable = false;
				pulseWaveCompletenessText.text = string.Format("Wave complete: {0:F1} of {1:F0} ticks, completeness {2:F2}", originCell.originPulseTick, originCell.gene.originPulsePeriodTicks, originCell.originPulseCompleteness);

			} else {
				embryoMaxSizeSlider.interactable = isOriginGenotypeSelected && IsUnlocked();

				//pulse
				pulseFrequenzySlider.interactable = isOriginGenotypeSelected && IsUnlocked();
				pulseWaveCompletenessText.text = string.Format("Wave complete: -");
			}

			// embryo max size
			embryoMaxSizeSlider.value = originCell.gene.embryoMaxSizeCompleteness;

			growPriorityCellPersistanceSlider.value = originCell.gene.growPriorityCellPersistance;

			pulseFrequenzySlider.value = 1f / (GenePanel.instance.selectedGene.originPulsePeriodTicks * Time.fixedDeltaTime);
			pulseFrequenzySliderText.text = string.Format("Ferquenzy: {0:F2} Hz ==> Period: {1:F2} s = {2:F0} ticks", 1f / (GenePanel.instance.selectedGene.originPulsePeriodTicks * Time.fixedDeltaTime), GenePanel.instance.selectedGene.originPulsePeriodTicks * Time.fixedDeltaTime, GenePanel.instance.selectedGene.originPulsePeriodTicks);

			detatchLogicBoxPanel.outputText = "Detatch from mother";

			ignoreSliderMoved = false;
			isDirty = false;
		}
	}
}