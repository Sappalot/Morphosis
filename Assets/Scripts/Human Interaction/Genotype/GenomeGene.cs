using UnityEngine;
using UnityEngine.UI;

public class GenomeGene : MonoBehaviour {
	public GameObject circles;

	public Image backgroundImage;
	public Image geneReferenceImage;
	public Image flipBlackWhite;
	public Image flipWhiteBlack;
	public Image grayOut;
	public Text geneReferenceText;
	public ReferenceGraphics[] referenceGraphics;

	//public int index;
	//public Text text;

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
			GenePanel.instance.selectedGene = gene;

		} else if (MouseAction.instance.actionState == MouseActionStateEnum.selectGene) {
			GenePanel.instance.GiveAnswerGeneReference(gene);
			MouseAction.instance.actionState = MouseActionStateEnum.free;
		}
	}

	public bool isDirty = true;
	private void Update() {
		if (isDirty) {
			//Nothing to represent
			if (GenomePanel.instance.genotype == null || gene == null) {
				//text.text = "-";
				circles.SetActive(false);
				return;
			}

			//text.text = gene.index.ToString() + gene.type;

			circles.SetActive(true);

			//perifier
			for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
				referenceGraphics[cardinalIndex].reference = gene.GetFlippableReference(cardinalIndex, GenotypePanel.instance.viewedFlipSide);
			}

			geneReferenceImage.color = ColorScheme.instance.ToColor(gene.type);
			flipBlackWhite.enabled = GenotypePanel.instance.viewedFlipSide == FlipSideEnum.BlackWhite;
			flipWhiteBlack.enabled = GenotypePanel.instance.viewedFlipSide == FlipSideEnum.WhiteBlack;
			geneReferenceText.text = gene.index.ToString();

			if (gene == GenePanel.instance.selectedGene) {
				backgroundImage.color = GenotypePanel.instance.selectedGeneColor;
			} else {
				backgroundImage.color = GenotypePanel.instance.unSelectedGeneColor;
			}

			grayOut.enabled = !GenomePanel.instance.genotype.IsGeneReferencedTo(gene);
		}
	}
}
