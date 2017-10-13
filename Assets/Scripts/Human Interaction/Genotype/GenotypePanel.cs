using UnityEngine;
using UnityEngine.UI;

public class GenotypePanel : MonoSingleton<GenotypePanel> {
	public Image blackWhiteImage;
	public Image whiteBlackImage;

	public Color unSelectedGeneColor;
	public Color selectedGeneColor;
	public Color unUsedGeneColor;

	public FlipSideEnum viewedFlipSide { get; private set; }

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
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;

	}

	public void OnClickedMutateAbsolute() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.MutateAbsolute(GlobalSettings.instance.mutationStrength);
		}
		GenePanel.instance.selectedGene = null;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedMutateCummulative() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.MutateCummulative(GlobalSettings.instance.mutationStrength);
		}
		GenePanel.instance.selectedGene = null;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedScramble() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.Scramble();
		}
		GenePanel.instance.selectedGene = null;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true; ;
	}

	public void OnClickedRevert() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.RestoreState();
		}
		GenePanel.instance.selectedGene = null;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedBlackWhite() {
		viewedFlipSide = FlipSideEnum.BlackWhite;
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedWhiteBlack() {
		viewedFlipSide = FlipSideEnum.WhiteBlack;
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public bool isDirty = true;
	private void Update() {
		if (isDirty) {
			Debug.Log("Update");
			blackWhiteImage.color = (viewedFlipSide == FlipSideEnum.BlackWhite) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
			whiteBlackImage.color = (viewedFlipSide == FlipSideEnum.WhiteBlack) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
			isDirty = false;
		}
	}
}
