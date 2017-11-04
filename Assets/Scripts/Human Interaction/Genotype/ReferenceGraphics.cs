using UnityEngine;
using UnityEngine.UI;

public class ReferenceGraphics : MonoBehaviour {
	public Image geneReferenceImage;
	public Image flipBlackWhite;
	public Image flipWhiteBlack;
	public Text geneReferenceText;

	Gene gene;
	public GeneReference reference {
		set {
			if (value == null) {
				geneReferenceImage.enabled = false;
				flipBlackWhite.enabled = false;
				flipWhiteBlack.enabled = false;
				geneReferenceText.enabled = false;
				return;
			} 

			geneReferenceImage.enabled = true;
			geneReferenceImage.color = ColorScheme.instance.ToColor(value.gene.type);

			flipBlackWhite.enabled = value.flipSide == FlipSideEnum.BlackWhite;

			flipWhiteBlack.enabled = value.flipSide == FlipSideEnum.WhiteBlack;

			geneReferenceText.enabled = true;
			geneReferenceText.text = value.gene.index.ToString();

			gene = value.gene;
		}
	}

	public void OnClicked() {
		GenePanel.instance.selectedGene = gene;
		GenePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		GenomePanel.instance.MakeScrollDirty();
		CreatureSelectionPanel.instance.soloSelected.MakeDirtyGraphics();
	}
}
