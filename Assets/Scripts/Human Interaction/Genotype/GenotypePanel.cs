using UnityEngine;
using UnityEngine.UI;

public class GenotypePanel : MonoSingleton<GenotypePanel> {
    public GenePanel genePanel;
    public Image blackWhiteImage;
    public Image whiteBlackImage;

    public Color chosenColor;
    public Color unchosenColor;

    public FlipSideEnum viewedFlipSide {get; private set;}

    public void Start() {
        viewedFlipSide = FlipSideEnum.BlackWhite;
        UpdateButtonImages();
    }

    public void OnClickedBlackWhite() {
        viewedFlipSide = FlipSideEnum.BlackWhite;
        UpdateButtonImages();
        genePanel.UpdateRepresentation();
    }

    public void OnClickedWhiteBlack() {
        viewedFlipSide = FlipSideEnum.WhiteBlack;
        UpdateButtonImages();
        genePanel.UpdateRepresentation();
    }

    private void UpdateButtonImages() {
        blackWhiteImage.color = (viewedFlipSide == FlipSideEnum.BlackWhite) ? chosenColor : unchosenColor;
        whiteBlackImage.color = (viewedFlipSide == FlipSideEnum.WhiteBlack) ? chosenColor : unchosenColor;
    }
}
