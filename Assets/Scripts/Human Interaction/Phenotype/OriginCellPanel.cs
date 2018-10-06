using UnityEngine;
using UnityEngine.UI;

public class OriginCellPanel : MonoBehaviour {
	public Text detatchConditionsText;

	[HideInInspector]
	public PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	public Text pulseHeadingText;
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

			detatchConditionsText.text = "Detatch when: -";
			detatchConditionsText.color = Color.gray;

			pulseHeadingText.text = "Pulse";
			pulseHeadingText.color = Color.gray;

			pulseFrequenzySliderText.text = "Ferquenzy: -";
			pulseFrequenzySliderText.color = Color.gray;
			pulseFrequenzySlider.interactable = false;

			pulseWaveCompletenessText.text = "Wave complete: -";
			pulseWaveCompletenessText.color = Color.gray;

			if ((mode == PhenoGenoEnum.Phenotype && CellPanel.instance.selectedCell != null && CellPanel.instance.selectedCell.isOrigin) ||
				(mode == PhenoGenoEnum.Genotype && GenePanel.instance.selectedGene != null && GenePanel.instance.selectedGene.isOrigin)) {
				Cell originCell = CellPanel.instance.selectedCell;

				if (mode == PhenoGenoEnum.Phenotype) {
					detatchConditionsText.text = "Detatch when:";
					
					if (originCell.creature.creation != CreatureCreationEnum.Born) {
						detatchConditionsText.text = "Detatch when: Creature wasn't born ==> No detatch conditions";
					} else if (originCell.originDetatchMode == ChildDetatchModeEnum.Size) {
						detatchConditionsText.text = string.Format("Detatch when: Body size ≥ {0:F1}% which is {1:F0} of {2:F0} cells", originCell.originDetatchSizeThreshold * 100f, (Mathf.Clamp(Mathf.RoundToInt(originCell.originDetatchSizeThreshold * originCell.creature.genotype.geneCellCount), 1, originCell.creature.genotype.geneCellCount)), originCell.creature.genotype.geneCellCount);
					} else {
						detatchConditionsText.text = string.Format("Detatch when: Can't grow more and cell energy ≥ {0:F1}%", originCell.originDetatchEnergyThreshold * 100f);
					}
					detatchConditionsText.color = Color.black;

					pulseFrequenzySliderText.color = Color.gray;
					pulseFrequenzySlider.interactable = false;

					pulseWaveCompletenessText.text = string.Format("Wave complete: {0:F1} of {1:F0} ticks, completeness {2:F2}", originCell.originPulseTick, originCell.gene.originPulsePeriodTicks, originCell.originPulseCompleteness);
					pulseWaveCompletenessText.color = Color.black;
				} else if (mode == PhenoGenoEnum.Genotype) {
					pulseWaveCompletenessText.text = "Wave complete: -";
					pulseWaveCompletenessText.color = Color.gray;

					pulseFrequenzySliderText.color = isUnlockedColor();
					pulseFrequenzySlider.interactable = isUnlocked();
				}

				pulseFrequenzySlider.value = 1f / (GenePanel.instance.selectedGene.originPulsePeriodTicks * Time.fixedDeltaTime);
				pulseFrequenzySliderText.text = string.Format("Ferquenzy: {0:F2} Hz ==> Period: {1:F2} s = {2:F0} ticks", 1f / (GenePanel.instance.selectedGene.originPulsePeriodTicks * Time.fixedDeltaTime), GenePanel.instance.selectedGene.originPulsePeriodTicks * Time.fixedDeltaTime, GenePanel.instance.selectedGene.originPulsePeriodTicks);

			}

			ignoreSliderMoved = false;

			isDirty = false;
		}
	}

	private bool isUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;
	}

	private Color isUnlockedColor() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome ? Color.black : Color.gray;
	}
}