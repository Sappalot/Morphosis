using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OriginPanel : ComponentPanel {
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

	public override void Initialize(PhenoGenoEnum mode, CellAndGenePanel cellAndGenePanel) {
		base.Initialize(mode, cellAndGenePanel);

		detatchLogicBoxPanel.Initialize(mode, SignalUnitEnum.OriginDetatchLogicBox, cellAndGenePanel);

		sizeSensorPanel.Initialize(mode, SignalUnitEnum.OriginSizeSensor, cellAndGenePanel);

		ignoreHumanInput = true;
		pulseFrequenzySlider.minValue = GlobalSettings.instance.phenotype.originPulseFrequenzyMin;
		pulseFrequenzySlider.maxValue = GlobalSettings.instance.phenotype.originPulseFrequenzyMax;
		ignoreHumanInput = false;
	}

	// ...pulse...
	public void OnPulseFrequenzySliderMoved() { 
		if (ignoreHumanInput || mode == PhenoGenoEnum.Phenotype) {
			return;
		}

		gene.originPulseTickPeriod = Mathf.CeilToInt(1f / (Time.fixedDeltaTime * pulseFrequenzySlider.value));
		OnGenomeChanged();
	}
	// ^pulse^

	// ...embryo max size...
	public void OnEmbryoSizeSliderMoved() {
		if (ignoreHumanInput || mode == PhenoGenoEnum.Phenotype) {
			return;
		}

		gene.originEmbryoMaxSizeCompleteness = embryoMaxSizeSlider.value;
		OnGenomeChanged();
	}
	// ^embryo max size^

	public void OnGrowPriorityCellPersistanceSliderMoved() {
		if (ignoreHumanInput || mode == PhenoGenoEnum.Phenotype) {
			return;
		}

		cellAndGenePanel.gene.originGrowPriorityCellPersistance = (int)growPriorityCellPersistanceSlider.value;
		OnGenomeChanged();
	}

	public override List<IGeneInput> GetAllGeneInputs() {
		return detatchLogicBoxPanel.GetAllGeneInputs();
	}

	public override void MakeDirty() {
		base.MakeDirty();
		detatchLogicBoxPanel.MakeDirty();
		sizeSensorPanel.MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			ignoreHumanInput = true;

			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			bool isOriginPhenotypeSelected = mode == PhenoGenoEnum.Phenotype && cellAndGenePanel.cell != null && cellAndGenePanel.cell.isOrigin;
			bool isOriginGenotypeSelected = mode == PhenoGenoEnum.Genotype && cellAndGenePanel.gene != null && cellAndGenePanel.gene.isOrigin;
			if (!(isOriginPhenotypeSelected || isOriginGenotypeSelected) || !CreatureSelectionPanel.instance.hasSoloSelected) {
				isDirty = false;
				return;
			}

			Cell originCell = cellAndGenePanel.cell;

			embryoMaxSizeSliderLabel.text = string.Format("Embryo max size: {0:F0} % ==> {1} of {2} cells", originCell.gene.originEmbryoMaxSizeCompleteness * 100f, CreatureSelectionPanel.instance.soloSelected.CellCountAtCompleteness(originCell.gene.originEmbryoMaxSizeCompleteness), CreatureSelectionPanel.instance.soloSelected.genotype.geneCellCount);
			growPriorityCellPersistanceLabel.text = string.Format("Persist to grow blocked priority cell for up to: {0:F0} s", originCell.gene.originGrowPriorityCellPersistance);
			
			if (mode == PhenoGenoEnum.Phenotype) {
				embryoMaxSizeSlider.interactable = false;
				growPriorityCellPersistanceSlider.interactable = false;

				// pulse
				pulseFrequenzySlider.interactable = false;
				pulseWaveCompletenessText.text = string.Format("Wave complete: {0:F1} of {1:F0} ticks, completeness {2:F2}", originCell.originPulseTick, originCell.gene.originPulseTickPeriod, originCell.originPulseCompleteness);

			} else {
				embryoMaxSizeSlider.interactable = isOriginGenotypeSelected && IsUnlocked();
				growPriorityCellPersistanceSlider.interactable = isOriginGenotypeSelected && IsUnlocked();

				//pulse
				pulseFrequenzySlider.interactable = isOriginGenotypeSelected && IsUnlocked();
				pulseWaveCompletenessText.text = string.Format("Wave complete: -");
			}

			// embryo max size
			embryoMaxSizeSlider.value = originCell.gene.originEmbryoMaxSizeCompleteness;

			growPriorityCellPersistanceSlider.value = originCell.gene.originGrowPriorityCellPersistance;

			pulseFrequenzySlider.value = 1f / (cellAndGenePanel.gene.originPulseTickPeriod * Time.fixedDeltaTime);
			pulseFrequenzySliderText.text = string.Format("Ferquenzy: {0:F2} Hz ==> Period: {1:F2} s = {2:F0} ticks", 1f / (cellAndGenePanel.gene.originPulseTickPeriod * Time.fixedDeltaTime), cellAndGenePanel.gene.originPulseTickPeriod * Time.fixedDeltaTime, cellAndGenePanel.gene.originPulseTickPeriod);

			detatchLogicBoxPanel.outputText = "Detatch from mother";

			ignoreHumanInput = false;
			isDirty = false;
		}
	}
}