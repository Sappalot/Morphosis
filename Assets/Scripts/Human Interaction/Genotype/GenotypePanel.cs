using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GenotypePanel : MonoSingleton<GenotypePanel> {
	public GameObject lowerPanel;

	public Image blackWhiteImage;
	public Image whiteBlackImage;

	public Color unSelectedGeneColor;
	public Color selectedGeneColor;
	public Color unUsedGeneColor;

	public FlipSideEnum viewedFlipSide { get; private set; }

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
	}

	public Genotype selectedGenotype {
		get {
			return (CreatureSelectionPanel.instance.hasSoloSelected ? CreatureSelectionPanel.instance.soloSelected.genotype : null);
		}
	}

	override public void Init() {
		viewedFlipSide = FlipSideEnum.BlackWhite; 
		isDirty = true;
	}

	public void OnClickedDefaultify() {
		if (MouseAction.instance.actionState != MouseActionStateEnum.free) {
			return;
		}
		Defaultify(false);
	}

	public void OnClickedDefaultifyJunk() {
		if (MouseAction.instance.actionState != MouseActionStateEnum.free) {
			return;
		}
		Defaultify(true);
	}

	private void Defaultify(bool junkOnly) {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			if (creature.allowedToChangeGenome) {
				creature.generation = 1;
				creature.creation = CreatureCreationEnum.Forged;

				creature.genotype.Defaultify(junkOnly);
				GenePanel.instance.selectedGene = null;
			}
		}
		CreatureSelectionPanel.instance.MakeDirty();
		GenePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}
	public void OnClickedRandomize() {
		if (MouseAction.instance.actionState != MouseActionStateEnum.free) {
			return;
		}
		Randomize(false);
	}

	public void OnClickedRandomizeJunk() {
		if (MouseAction.instance.actionState != MouseActionStateEnum.free) {
			return;
		}
		Randomize(true);
	}


	private void Randomize(bool junkOnly) {
		if (MouseAction.instance.actionState != MouseActionStateEnum.free) {
			return;
		}

		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			if (creature.allowedToChangeGenome) {
				creature.generation = 1;
				creature.creation = CreatureCreationEnum.Forged;

				creature.genotype.Randomize(junkOnly);
				GenePanel.instance.selectedGene = null;
			}
		}
		CreatureSelectionPanel.instance.MakeDirty();
		GenePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	public void OnClickedMutate() {
		if (MouseAction.instance.actionState != MouseActionStateEnum.free) {
			return;
		}
		Mutate(false);
	}

	public void OnClickedMutateJunk() {
		if (MouseAction.instance.actionState != MouseActionStateEnum.free) {
			return;
		}
		Mutate(true);
	}

	private void Mutate(bool junkOnly) {
		if (MouseAction.instance.actionState != MouseActionStateEnum.free) {
			return;
		}

		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			if (creature.allowedToChangeGenome) {
				creature.generation = 1;
				creature.creation = CreatureCreationEnum.Forged;
				creature.genotype.Mutate(GlobalSettings.instance.mutation.masterMutationStrength, junkOnly);
				GenePanel.instance.selectedGene = null;
			}
		}
		CreatureSelectionPanel.instance.MakeDirty();
		GenePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	public void OnClickedBlackWhite() {
		viewedFlipSide = FlipSideEnum.BlackWhite;
		isDirty = true;
		GenePanel.instance.cellAndGenePanel.geneNeighboursPanel.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	public void OnClickedWhiteBlack() {
		viewedFlipSide = FlipSideEnum.WhiteBlack;
		isDirty = true;
		GenePanel.instance.cellAndGenePanel.geneNeighboursPanel.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	private IEnumerator UpdateIsVisible() {
		yield return 0;
		GenePanel.instance.gameObject.SetActive((CreatureSelectionPanel.instance.hasSoloSelected && MouseAction.instance.actionState == MouseActionStateEnum.free && !AlternativeToolModePanel.instance.isOn) || MouseAction.instance.actionState == MouseActionStateEnum.selectGene);
		lowerPanel.gameObject.SetActive((CreatureSelectionPanel.instance.hasSelection && MouseAction.instance.actionState == MouseActionStateEnum.free && !AlternativeToolModePanel.instance.isOn) || MouseAction.instance.actionState == MouseActionStateEnum.selectGene);
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update GenotypePanel");
			}

			StartCoroutine(UpdateIsVisible());

			blackWhiteImage.color = (viewedFlipSide == FlipSideEnum.BlackWhite) ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;
			whiteBlackImage.color = (viewedFlipSide == FlipSideEnum.WhiteBlack) ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;
			isDirty = false;
		}
	}
}
