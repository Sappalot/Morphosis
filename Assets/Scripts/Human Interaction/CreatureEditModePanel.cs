using UnityEngine;
using UnityEngine.UI;

public class CreatureEditModePanel : MonoSingleton<CreatureEditModePanel> {
	public Image phenotypeImage;
	public Image genotypeImage;
	public Image historyImage;

	private bool isDirty = true;

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
		m_mode = CreatureEditModeEnum.Phenotype;
		UpdateAllAccordingToEditMode();
	}

	public void OnClickedGenotypeEditMode() {
		m_mode = CreatureEditModeEnum.Genotype;
		UpdateAllAccordingToEditMode();
		GenePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenomePanel.instance.MakeScrollDirty();
	}

	public void UpdateAllAccordingToEditMode() {
		CreatureSelectionPanel.instance.SetCellAndGeneSelectionToRoot();
		foreach (Creature c in World.instance.life.creatures) {
			c.BringCurrentGenoPhenoPositionAndRotationToOther();
			c.MakeDirty();
		}
		isDirty = true;
	}
	
	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update CreatureEditModePanel");
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
