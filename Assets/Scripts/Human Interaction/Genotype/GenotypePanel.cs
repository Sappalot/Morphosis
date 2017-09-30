using UnityEngine;
using UnityEngine.UI;

public class GenotypePanel : MonoSingleton<GenotypePanel> {
	public GenePanel genePanel;
	public Image blackWhiteImage;
	public Image whiteBlackImage;

	public Color unSelectedGeneColor;
	public Color selectedGeneColor;
	public Color unUsedGeneColor;

	public FlipSideEnum viewedFlipSide { get; private set; }

	private Genotype m_genotype;
	public Genotype genotype {
		get {
			return m_genotype;
		}
		set {
			m_genotype = value;
			UpdateRepresentation(false);
		}
	}

	override public void Init() {
		viewedFlipSide = FlipSideEnum.BlackWhite;
		UpdateButtonImages();
	}

	private void Start() {
		UpdateRepresentation(false);
	}

	private void UpdateButtonImages() {
		blackWhiteImage.color = (viewedFlipSide == FlipSideEnum.BlackWhite) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		whiteBlackImage.color = (viewedFlipSide == FlipSideEnum.WhiteBlack) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
	}

	public void UpdateRepresentation(bool changeToGenomeMade) {
		//Nothing to represent
		if (genotype == null) {
			genePanel.gene = null;
			GenomePanel.instance.genotype = null;
			return;
		}

		genotype.hasDirtyGenes |= changeToGenomeMade;
		genePanel.UpdateRepresentation(changeToGenomeMade);
		GenomePanel.instance.genotype = genotype;
	}

	public void OnClickedClear() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.Clear();
		}
		genePanel.gene = genotype.genes[0];
		UpdateRepresentation(true);
	}

	public void OnClickedMutateAbsolute() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.MutateAbsolute(GlobalSettings.instance.mutationStrength);
		}
		genePanel.gene = genotype.genes[0];
		UpdateRepresentation(true);
	}

	public void OnClickedMutateCummulative() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.MutateCummulative(GlobalSettings.instance.mutationStrength);
		}
		genePanel.gene = genotype.genes[0];
		UpdateRepresentation(true);
	}

	public void OnClickedScramble() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.Scramble();
		}
		genePanel.gene = genotype.genes[0];
		UpdateRepresentation(true);
	}

	public void OnClickedRevert() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			creature.RestoreState();
		}
		genePanel.gene = genotype.genes[0];
		UpdateRepresentation(true);
	}

	public void OnClickedBlackWhite() {
		viewedFlipSide = FlipSideEnum.BlackWhite;
		UpdateButtonImages();
		UpdateRepresentation(false);
	}

	public void OnClickedWhiteBlack() {
		viewedFlipSide = FlipSideEnum.WhiteBlack;
		UpdateButtonImages();
		UpdateRepresentation(false);
	}
}
