using UnityEngine;
using UnityEngine.UI;

public class CreatureEditModePanel : MonoSingleton<CreatureEditModePanel> {
	public Image phenotypeImage;
	public Image genotypeImage;

	private bool isDirty = true;

	private PhenoGenoEnum m_mode;
	public PhenoGenoEnum mode {
		get {
			return m_mode;
		}
	}

	public void Start() {
		m_mode = PhenoGenoEnum.Phenotype;
		isDirty = true;
	}

	public void Restart() {
		m_mode = PhenoGenoEnum.Phenotype;
		GlobalPanel.instance.isRunPhysicsGrayOut = false;
		isDirty = true;
	}

	public void OnClickedPhenotypeEditMode() {
		if (Morphosis.isInterferredByOtheActions()) {
			return;
		}

		m_mode = PhenoGenoEnum.Phenotype;
		GlobalPanel.instance.isRunPhysicsGrayOut = false;
		UpdateAllAccordingToEditMode();
	}

	public void OnClickedGenotypeEditMode() {
		if (Morphosis.isInterferredByOtheActions()) {
			return;
		}

		World.instance.cameraController.TryUnlockCamera();

		m_mode = PhenoGenoEnum.Genotype;
		GlobalPanel.instance.isRunPhysicsGrayOut = true;
		UpdateAllAccordingToEditMode();
		GenePanel.instance.cellAndGenePanel.geneNeighboursPanel.MakeDirty();
		GenePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenomePanel.instance.MakeScrollDirty();
	}

	public void UpdateAllAccordingToEditMode() {
		CreatureSelectionPanel.instance.SetCellAndGeneSelectionToOrigin();
		foreach (Creature creature in World.instance.life.creatures) {
			creature.BringCurrentGenoPhenoPositionAndRotationToOther();
			creature.MakeDirtyGraphics();
		}
		foreach (Creature creature in Freezer.instance.creatures) {
			creature.BringCurrentGenoPhenoPositionAndRotationToOther();
			creature.MakeDirtyGraphics();
		}
		isDirty = true;
	}
	
	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CreatureEditModePanel");
			}

			phenotypeImage.color = (mode == PhenoGenoEnum.Phenotype) ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;
			genotypeImage.color = (mode == PhenoGenoEnum.Genotype) ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;

			PhenotypePanel.instance.gameObject.SetActive(mode == PhenoGenoEnum.Phenotype);
			GenotypePanel.instance.gameObject.SetActive(mode == PhenoGenoEnum.Genotype);
			isDirty = false;
		}
	}
}
