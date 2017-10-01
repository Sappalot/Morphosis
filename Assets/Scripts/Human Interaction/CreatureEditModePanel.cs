using UnityEngine;
using UnityEngine.UI;

public class CreatureEditModePanel : MonoSingleton<CreatureEditModePanel> {
	public Image phenotypeImage;
	public Image genotypeImage;
	public Image historyImage;

	private CreatureEditModeEnum m_editMode;
	public CreatureEditModeEnum editMode {
		get {
			return m_editMode;
		}
	}

	public void Start() {
		m_editMode = CreatureEditModeEnum.phenotype;
		UpdateHUD();
		World.instance.ShowPhenotypes();
	}

	public void Restart() {
		m_editMode = CreatureEditModeEnum.phenotype;
		UpdateHUD();
		World.instance.ShowPhenotypes();
	}

	public void OnClickedPhenotypeEditMode() {
		World.instance.ShowPhenotypes();
		m_editMode = CreatureEditModeEnum.phenotype;
		UpdateHUD();
		SelectDefaultCell();
	}

	public void OnClickedGenotypeEditMode() {
		World.instance.ShowGenotypes();
		m_editMode = CreatureEditModeEnum.genotype;
		UpdateHUD();
		SelectDefaultGeneCell();
	}

	private void SelectDefaultCell() {
		Creature creature = CreatureSelectionPanel.instance.soloSelected;
		if (creature != null) {
			PhenotypePanel.instance.selectedCell = creature.phenotype.rootCell;
			PhenotypePanel.instance.UpdateRepresentation();
			creature.ShowCellsAndGeneCellsSelected(false);
			creature.ShowCellSelected(creature.phenotype.rootCell, true);
		}
	}

	private void SelectDefaultGeneCell() {
		Creature creature = CreatureSelectionPanel.instance.soloSelected;
		if (creature != null) {
			GenePanel.instance.selectedGene = creature.genotype.geneCellList[0].gene;
			GenotypePanel.instance.genotype = creature.genotype;
		}
	}

	private void UpdateHUD() {
		phenotypeImage.color = (editMode == CreatureEditModeEnum.phenotype) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		genotypeImage.color = (editMode == CreatureEditModeEnum.genotype) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		historyImage.color = (editMode == CreatureEditModeEnum.history) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;

		PhenotypePanel.instance.gameObject.SetActive(editMode == CreatureEditModeEnum.phenotype);
		GenotypePanel.instance.gameObject.SetActive(editMode == CreatureEditModeEnum.genotype);
	}   
}
