using UnityEngine;
using UnityEngine.UI;

public class CreatureEditModePanel : MonoSingleton<CreatureEditModePanel> {
	public Image phenotypeImage;
	public Image genotypeImage;
	public Image historyImage;

	private CretureEditMode m_editMode;
	public CretureEditMode editMode {
		get {
			return m_editMode;
		}
	}

	public void Start() {
		m_editMode = CretureEditMode.phenotype;
		UpdateHUD();
		World.instance.ShowPhenotypes();
	}

	public void Restart() {
		m_editMode = CretureEditMode.phenotype;
		UpdateHUD();
		World.instance.ShowPhenotypes();
	}

	public enum CretureEditMode {
		phenotype,
		genotype,
		history,
	}

	public void OnClickedPhenotypeEditMode() {
		World.instance.ShowPhenotypes();
		m_editMode = CretureEditMode.phenotype;
		UpdateHUD();
		SelectDefaultCell();
	}

	public void OnClickedGenotypeEditMode() {
		World.instance.ShowGenotypes();
		m_editMode = CretureEditMode.genotype;
		UpdateHUD();
		SelectDefaultGeneCell();
	}

	private void SelectDefaultCell() {
		Creature creature = CreatureSelectionPanel.instance.soloSelected;
		if (creature != null) {
			PhenotypePanel.instance.cell = creature.phenotype.rootCell;
			PhenotypePanel.instance.UpdateRepresentation();
			creature.ShowCellsAndGeneCellsSelected(false);
			creature.ShowCellSelected(creature.phenotype.rootCell, true);
		}
	}

	private void SelectDefaultGeneCell() {
		Creature creature = CreatureSelectionPanel.instance.soloSelected;
		if (creature != null) {
			GenePanel.instance.gene = creature.genotype.geneCellList[0].gene;
			GenotypePanel.instance.genotype = creature.genotype;
		}
	}

	private void UpdateHUD() {
		phenotypeImage.color = (editMode == CretureEditMode.phenotype) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		genotypeImage.color = (editMode == CretureEditMode.genotype) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		historyImage.color = (editMode == CretureEditMode.history) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;

		PhenotypePanel.instance.gameObject.SetActive(editMode == CretureEditMode.phenotype);
		GenotypePanel.instance.gameObject.SetActive(editMode == CretureEditMode.genotype);
	}   
}
