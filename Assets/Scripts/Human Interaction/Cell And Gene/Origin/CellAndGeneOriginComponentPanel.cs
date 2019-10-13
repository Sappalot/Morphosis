using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellAndGeneOriginComponentPanel : CellAndGeneComponentPanel {
	// Detatch
	public LogicBoxPanel detatchLogicBoxPanel;

	// Embryo max size
	public Image embryoMaxSizeLimitButtonImage;
	public Image embryoMaxSizeAsBigAsPossibleButtonImage;
	public Text embryoMaxSizeSliderLabel;
	public Slider embryoMaxSizeSlider;

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
	public void OnClickedEmbryoSizeLimitSize() {
		if (mode == PhenoGenoEnum.Genotype) {
			GenePanel.instance.selectedGene.embryoMaxSizeMode = EmbryoMaxSizeModeEnum.LimitSize;
			OnGenomeChanged(false);
		}
	}

	public void OnClickedEmbryoSizeAsBigAsPoissble() {
		if (mode == PhenoGenoEnum.Genotype) {
			GenePanel.instance.selectedGene.embryoMaxSizeMode = EmbryoMaxSizeModeEnum.AsBigAsPossible;
			OnGenomeChanged(false);
		}
	}

	public void OnEmbryoSizeSliderMoved() {
		if (ignoreSliderMoved || mode == PhenoGenoEnum.Phenotype) {
			return;
		}

		GenePanel.instance.selectedGene.embryoMaxSizeCompleteness = embryoMaxSizeSlider.value;
		OnGenomeChanged(false);
	}
	// ^embryo max size^



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
			if (!(isOriginPhenotypeSelected || isOriginGenotypeSelected)) {
				isDirty = false;
				return;
			}

			Cell originCell = CellPanel.instance.selectedCell;

			// embryo max size
			embryoMaxSizeLimitButtonImage.color = originCell.gene.embryoMaxSizeMode == EmbryoMaxSizeModeEnum.LimitSize ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
			embryoMaxSizeAsBigAsPossibleButtonImage.color = originCell.gene.embryoMaxSizeMode == EmbryoMaxSizeModeEnum.AsBigAsPossible ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
			embryoMaxSizeSliderLabel.text = pulseWaveCompletenessText.text = string.Format("Grow until size: {0:F0} % ==> {1} of {2} cells", originCell.gene.embryoMaxSizeCompleteness * 100f, Mathf.Max(1, Mathf.RoundToInt(originCell.gene.embryoMaxSizeCompleteness * CreatureSelectionPanel.instance.soloSelected.genotype.geneCellCount)), CreatureSelectionPanel.instance.soloSelected.genotype.geneCellCount);

			if (mode == PhenoGenoEnum.Phenotype) {
				// embryo max size
				embryoMaxSizeSlider.interactable = false;
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

			pulseFrequenzySlider.value = 1f / (GenePanel.instance.selectedGene.originPulsePeriodTicks * Time.fixedDeltaTime);
			pulseFrequenzySliderText.text = string.Format("Ferquenzy: {0:F2} Hz ==> Period: {1:F2} s = {2:F0} ticks", 1f / (GenePanel.instance.selectedGene.originPulsePeriodTicks * Time.fixedDeltaTime), GenePanel.instance.selectedGene.originPulsePeriodTicks * Time.fixedDeltaTime, GenePanel.instance.selectedGene.originPulsePeriodTicks);

			detatchLogicBoxPanel.outputText = "Detatch from mother";

			ignoreSliderMoved = false;
			isDirty = false;
		}
	}
}