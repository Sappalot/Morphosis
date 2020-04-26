using UnityEngine;
using UnityEngine.UI;

public class BuildPriorityPanel : MonoBehaviour {
	public Text buildIndexText;
	public Text buildPriorityBiasText;

	public Text buildPriorityText;

	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	public Slider biasSlider;

	private bool ignoreSliderMoved = false;

	private CellAndGenePanel cellAndGenePanel;

	public void Initialize(PhenoGenoEnum mode, CellAndGenePanel cellAndGenePanel) {
		this.mode = mode;
		this.cellAndGenePanel = cellAndGenePanel;

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

		cellAndGenePanel.gene.buildPriorityBias = ToClosestTenth(biasSlider.value);
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

			if (mode == PhenoGenoEnum.Phenotype && cellAndGenePanel.cell != null) {
				buildIndexText.text = string.Format("Build index: {0:F0}", cellAndGenePanel.cell.buildIndex);
				buildPriorityBiasText.text = string.Format("Build priority bias: {0:F1}", cellAndGenePanel.cell.gene.buildPriorityBias);
				buildPriorityText.text = string.Format("Build priority: {0:F1}", cellAndGenePanel.cell.buildPriority);

				biasSlider.interactable = false;

				biasSlider.value = cellAndGenePanel.cell.gene.buildPriorityBias;

			} else if (mode == PhenoGenoEnum.Genotype && cellAndGenePanel.gene != null && CreatureSelectionPanel.instance.hasSoloSelected) {
				bool agreedBuildOrder = CreatureSelectionPanel.instance.soloSelected.genotype.HasAllOccurancesOfThisGeneSameBuildIndex(cellAndGenePanel.gene);
				if (agreedBuildOrder) {
					buildIndexText.text = string.Format("Build index: " + cellAndGenePanel.cell.buildIndex);
					buildPriorityBiasText.text = string.Format("Build priority bias: {0:F1}", cellAndGenePanel.gene.buildPriorityBias);
					buildPriorityText.text = string.Format("Build priority: {0:F1}", cellAndGenePanel.cell.buildIndex + cellAndGenePanel.gene.buildPriorityBias);
				} else {
					buildIndexText.text = string.Format("Build index: X");
					buildPriorityBiasText.text = string.Format("Build priority bias: {0:F1}", cellAndGenePanel.gene.buildPriorityBias);
					buildPriorityText.text = cellAndGenePanel.cell.gene.buildPriorityBias >= 0 ? string.Format("Build priority: X + {0:F1}", cellAndGenePanel.cell.gene.buildPriorityBias) : string.Format("Build priority: X {0:F1}", cellAndGenePanel.cell.gene.buildPriorityBias);
				}
				biasSlider.interactable = CreatureSelectionPanel.instance.hasSoloSelectedThatCanChangeGenome;

				biasSlider.value = cellAndGenePanel.gene.buildPriorityBias;
			}

			ignoreSliderMoved = false;
			isDirty = false;
		}
	}
}