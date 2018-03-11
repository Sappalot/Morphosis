using UnityEngine;
using UnityEngine.UI;

public class GenotypePanel : MonoSingleton<GenotypePanel> {
	public LockedUnlockedPanel lockedUnlockedPanel;

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

	public void OnClickedClear() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.Clear();
		}
		GenePanel.instance.selectedGene = null;
		GenePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	public void OnClickedMutateAbsolute() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.MutateAbsolute(GlobalSettings.instance.mutation.masterMutationStrength * 10f);
		}
		GenePanel.instance.selectedGene = null;
		GenePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	public void OnClickedMutateCummulative() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.MutateCummulative(GlobalSettings.instance.mutation.masterMutationStrength * 10f);
		}
		GenePanel.instance.selectedGene = null;
		GenePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	public void OnClickedScramble() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.Scramble();
		}
		GenePanel.instance.selectedGene = null;
		GenePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	public void OnClickedRevert() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.RestoreState();
		}
		GenePanel.instance.selectedGene = null;
		GenePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	public void OnClickedBlackWhite() {
		viewedFlipSide = FlipSideEnum.BlackWhite;
		isDirty = true;
		GenePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	public void OnClickedWhiteBlack() {
		viewedFlipSide = FlipSideEnum.WhiteBlack;
		isDirty = true;
		GenePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update GenotypePanel");
			}

			blackWhiteImage.color = (viewedFlipSide == FlipSideEnum.BlackWhite) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
			whiteBlackImage.color = (viewedFlipSide == FlipSideEnum.WhiteBlack) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
			isDirty = false;
		}
	}
}
