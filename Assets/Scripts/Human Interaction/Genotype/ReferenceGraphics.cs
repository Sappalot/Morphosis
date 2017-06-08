using UnityEngine;
using UnityEngine.UI;

public class ReferenceGraphics : MonoBehaviour {
    public Image geneReferenceImage;
    public Image flipBlackWhite;
    public Image flipWhiteBlack;
    public Text geneReferenceText;

    public GeneReference reference {
        set {
            if (value != null && value.isThere) {
                geneReferenceImage.enabled = true;
                flipBlackWhite.enabled = value.flip == FlipSide.BlackWhite;
                flipWhiteBlack.enabled = value.flip == FlipSide.WhiteBlack;
                geneReferenceText.enabled = true;
                geneReferenceText.text = value.geneIndex.ToString();
            } else {
                geneReferenceImage.enabled = false;
                flipBlackWhite.enabled = false;
                flipWhiteBlack.enabled = false;
                geneReferenceText.enabled = false;
            }
        }
    }

}
