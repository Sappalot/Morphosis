using UnityEngine;
using UnityEngine.UI;

public class GenotypePanel : MonoSingleton<GenotypePanel> {
    public GenePanel genePanel;
    public Image blackWhiteImage;
    public Image whiteBlackImage;

    public Color chosenColor;
    public Color unchosenColor;

    public FlipSideEnum viewedFlipSide { get; private set; }

    private Genotype m_genotype;
    public Genotype genotype {
        get {
            return m_genotype;
        }
        set {
            m_genotype = value;
            if (value != null) {
                Debug.Log("Sel");
                genePanel.gene = m_genotype.genome[0];
            } else {
                Debug.Log("xxx");
            }
            UpdateRepresentation();
        }
    }

    override public void Init() {
        viewedFlipSide = FlipSideEnum.BlackWhite;
        UpdateButtonImages();
    }

    public void OnClickedBlackWhite() {
        viewedFlipSide = FlipSideEnum.BlackWhite;
        UpdateButtonImages();
        UpdateRepresentation();
    }

    public void OnClickedWhiteBlack() {
        viewedFlipSide = FlipSideEnum.WhiteBlack;
        UpdateButtonImages();
        UpdateRepresentation();
    }

    private void UpdateButtonImages() {
        blackWhiteImage.color = (viewedFlipSide == FlipSideEnum.BlackWhite) ? chosenColor : unchosenColor;
        whiteBlackImage.color = (viewedFlipSide == FlipSideEnum.WhiteBlack) ? chosenColor : unchosenColor;
    }

    public void UpdateRepresentation() {
        //Nothing to represent
        if (genotype == null) {
            genePanel.gene = null;
            GenomePanel.instance.genotype = null;
            return;
        }

        //genePanel.gene = m_genotype.genome[0];
        genePanel.UpdateRepresentation();
        GenomePanel.instance.genotype = genotype;
    }
}
