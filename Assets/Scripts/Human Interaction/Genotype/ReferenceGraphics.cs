using UnityEngine;
using UnityEngine.UI;

public class ReferenceGraphics : MonoBehaviour {
    public Image geneReferenceImage;
    public Image flipBlackWhite;
    public Image flipWhiteBlack;
    public Text geneReferenceText;

    public GeneReference reference {
        set {
            if (value != null) {
                geneReferenceImage.enabled = true;
                geneReferenceImage.color = CellTypeUtil.ToColor(value.gene.type);

                flipBlackWhite.enabled = value.flip == FlipSideEnum.BlackWhite;

                flipWhiteBlack.enabled = value.flip == FlipSideEnum.WhiteBlack;

                geneReferenceText.enabled = true;
                geneReferenceText.text = value.gene.index.ToString();
            } else {
                geneReferenceImage.enabled = false;
                flipBlackWhite.enabled = false;
                flipWhiteBlack.enabled = false;
                geneReferenceText.enabled = false;
            }
        }
    }

}
