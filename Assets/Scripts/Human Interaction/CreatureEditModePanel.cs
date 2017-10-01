using UnityEngine;
using UnityEngine.UI;

public class CreatureEditModePanel : MonoSingleton<CreatureEditModePanel> {
	public Image phenotypeImage;
	public Image genotypeImage;
	public Image historyImage;

	private CreatureEditModeEnum m_mode;
	public CreatureEditModeEnum mode {
		get {
			return m_mode;
		}
	}

	public void Start() {
		m_mode = CreatureEditModeEnum.Phenotype;
		isDirty = true;
	}

	public void Restart() {
		m_mode = CreatureEditModeEnum.Phenotype;
		isDirty = true;
	}

	public void OnClickedPhenotypeEditMode() {
		//World.instance.ShowPhenotypes();
		m_mode = CreatureEditModeEnum.Phenotype;
		World.instance.life.MakeAllCreaturesDirty();
		CreatureSelectionPanel.instance.SetCellAndGeneSelectionToRoot();
		isDirty = true;
	}

	public void OnClickedGenotypeEditMode() {
		//World.instance.ShowGenotypes();
		m_mode = CreatureEditModeEnum.Genotype;
		World.instance.life.MakeAllCreaturesDirty();
		CreatureSelectionPanel.instance.SetCellAndGeneSelectionToRoot();
		isDirty = true;
	}

	//private void SelectDefaultCell() {
	//	Creature creature = CreatureSelectionPanel.instance.soloSelected;
	//	if (creature != null) {
	//		PhenotypePanel.instance.selectedCell = creature.phenotype.rootCell;
	//		PhenotypePanel.instance.UpdateRepresentation();
	//		creature.ShowCellsAndGeneCellsSelected(false);
	//		creature.ShowCellSelected(creature.phenotype.rootCell, true);
	//	}
	//}

	//private void SelectDefaultGeneCell() {
	//	Creature creature = CreatureSelectionPanel.instance.soloSelected;
	//	if (creature != null) {
	//		GenePanel.instance.selectedGene = creature.genotype.geneCellList[0].gene;
	//		GenotypePanel.instance.genotype = creature.genotype;
	//	}
	//}

	public bool isDirty = true;
	private void Update() {
		if (isDirty) {
			phenotypeImage.color = (mode == CreatureEditModeEnum.Phenotype) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
			genotypeImage.color = (mode == CreatureEditModeEnum.Genotype) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
			historyImage.color = (mode == CreatureEditModeEnum.History) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;

			//TODO: set enabled true/false, then dirty mark
			PhenotypePanel.instance.gameObject.SetActive(mode == CreatureEditModeEnum.Phenotype);
			GenotypePanel.instance.gameObject.SetActive(mode == CreatureEditModeEnum.Genotype);

			isDirty = false;
		}
	}
}
