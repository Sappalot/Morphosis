using UnityEngine;
using UnityEngine.UI;

public class CellBuildPriorityComponentPanel : MonoBehaviour {
	public Text buildIndexText;
	public Text buildPriorityBiasText;

	public Text buildPriorityText;

	[HideInInspector]
	public PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	public Slider biasSlider;

	private bool ignoreSliderMoved = false;

	private void Awake() {
		ignoreSliderMoved = true;
		biasSlider.minValue = GlobalSettings.instance.phenotype.buildPriorityBiasMin;
		biasSlider.maxValue = GlobalSettings.instance.phenotype.buildPriorityBiasMax;
		ignoreSliderMoved = false;
	}

	public float ToClosestTenth(float value) {
		return Mathf.Round(value * 10f) / 10f;
	}

	public void OnSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		GeneCellPanel.instance.selectedGene.buildPriorityBias = ToClosestTenth(biasSlider.value);
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
			CreatureSelectionPanel.instance.MakeDirty();
		}
		MakeDirty();
	}

	private bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
	}

	private void Update() {
		if (isDirty) {
			ignoreSliderMoved = true;

			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellBuildPriorityPanel");
			}

			bool isOriginPhenotypeSelected = mode == PhenoGenoEnum.Phenotype && CellPanel.instance.selectedCell != null && CellPanel.instance.selectedCell.isOrigin;
			bool isOriginGenotypeSelected = mode == PhenoGenoEnum.Genotype && GeneCellPanel.instance.selectedGene != null && GeneCellPanel.instance.selectedGene.isOrigin;


			if (mode == PhenoGenoEnum.Phenotype && CellPanel.instance.selectedCell != null) {
				buildIndexText.text = string.Format("Build index: {0:F0}", CellPanel.instance.selectedCell.buildIndex);
				buildPriorityBiasText.text = string.Format("Build priority bias: {0:F1}", CellPanel.instance.selectedCell.gene.buildPriorityBias);
				buildPriorityText.text = string.Format("Build priority: {0:F1}", CellPanel.instance.selectedCell.buildPriority);

				biasSlider.interactable = false;

				biasSlider.value = CellPanel.instance.selectedCell.gene.buildPriorityBias;

			} else if (mode == PhenoGenoEnum.Genotype && GeneCellPanel.instance.selectedGene != null && CreatureSelectionPanel.instance.hasSoloSelected) {
				bool agreedBuildOrder = CreatureSelectionPanel.instance.soloSelected.genotype.HasAllOccurancesOfThisGeneSameBuildIndex(GeneCellPanel.instance.selectedGene);
				if (agreedBuildOrder) {
					buildIndexText.text = string.Format("Build index: " + CellPanel.instance.selectedCell.buildIndex);
					buildPriorityBiasText.text = string.Format("Build priority bias: {0:F1}", GeneCellPanel.instance.selectedGene.buildPriorityBias);
					buildPriorityText.text = string.Format("Build priority: {0:F1}", CellPanel.instance.selectedCell.buildIndex + GeneCellPanel.instance.selectedGene.buildPriorityBias);
				} else {
					buildIndexText.text = string.Format("Build index: X");
					buildPriorityBiasText.text = string.Format("Build priority bias: {0:F1}", GeneCellPanel.instance.selectedGene.buildPriorityBias);
					buildPriorityText.text = CellPanel.instance.selectedCell.gene.buildPriorityBias >= 0 ? string.Format("Build priority: X + {0:F1}", CellPanel.instance.selectedCell.gene.buildPriorityBias) : string.Format("Build priority: X {0:F1}", CellPanel.instance.selectedCell.gene.buildPriorityBias);
				}
				biasSlider.interactable = CreatureSelectionPanel.instance.hasSoloSelectedThatCanChangeGenome;

				biasSlider.value = GeneCellPanel.instance.selectedGene.buildPriorityBias;
			}

			ignoreSliderMoved = false;
			isDirty = false;
		}
	}
}