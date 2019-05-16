using UnityEngine;
using UnityEngine.UI;

public class GenomeGenePanel : MonoBehaviour {
	public GameObject circles;

	public Image backgroundImage;
	public Image geneReferenceImage;
	public Image flipBlackWhite;
	public Image flipWhiteBlack;
	public Image grayOut;
	public Text geneReferenceText;
	public ReferenceGraphics[] referenceGraphics;

	private bool isDirty = true;

	private Gene m_gene;
	public Gene gene {
		get {
			return m_gene;
		}
		set {
			m_gene = value;
			isDirty = true;
		}
	}

	public void OnClicked() {
		//Debug.Log("Clicked " + index);
		if (MouseAction.instance.actionState == MouseActionStateEnum.free) {
			GeneCellPanel.instance.selectedGene = gene;
			GenomePanel.instance.MakeDirty();
			if (CreatureSelectionPanel.instance.hasSoloSelected) {
				CreatureSelectionPanel.instance.soloSelected.MakeDirtyGraphics();
			}
		} else if (MouseAction.instance.actionState == MouseActionStateEnum.selectGene) {
			GeneCellPanel.instance.geneNeighbourPanel.GiveAnswerGeneReference(gene);
			MouseAction.instance.actionState = MouseActionStateEnum.free;
		}
	}
	
	private void Update() {
		if (isDirty) {
			//if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				//Debug.Log("Update GenomeGene");
			//Nothing to represent
			if (GenomePanel.instance.genotype == null || gene == null) {
				//text.text = "-";
				circles.SetActive(false);

				isDirty = false;
				return;
			}

			circles.SetActive(true);

			//perifier
			for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
				referenceGraphics[cardinalIndex].reference = gene.GetFlippableReference(cardinalIndex, GenotypePanel.instance.viewedFlipSide);
			}

			if (gene.type == CellTypeEnum.Shell) {
				geneReferenceImage.color = ShellCell.GetColor(gene.shellCellArmorClass, gene.shellCellTransparancyClass);
			} else {
				geneReferenceImage.color = ColorScheme.instance.ToColor(gene.type);
			}
			flipBlackWhite.enabled = GenotypePanel.instance.viewedFlipSide == FlipSideEnum.BlackWhite;
			flipWhiteBlack.enabled = GenotypePanel.instance.viewedFlipSide == FlipSideEnum.WhiteBlack;
			geneReferenceText.text = gene.index.ToString();

			if (gene == GeneCellPanel.instance.selectedGene) {
				backgroundImage.color = GenotypePanel.instance.selectedGeneColor;
			} else {
				backgroundImage.color = GenotypePanel.instance.unSelectedGeneColor;
			}

			grayOut.enabled = !GenomePanel.instance.genotype.IsGeneReferencedTo(gene);

			isDirty = false;
		}
	}
}
