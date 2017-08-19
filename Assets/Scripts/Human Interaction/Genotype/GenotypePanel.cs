using UnityEngine;
using UnityEngine.UI;

public class GenotypePanel : MonoSingleton<GenotypePanel> {
    public GenePanel genePanel;
    public Image blackWhiteImage;
    public Image whiteBlackImage;

    public Color unSelectedGeneColor;
    public Color selectedGeneColor;

    public FlipSideEnum viewedFlipSide { get; private set; }

    private Genotype m_genotype;
    public Genotype genotype {
        get {
            return m_genotype;
        }
        set {
            m_genotype = value;
            if (value != null) {
                genePanel.gene = m_genotype.GetGeneAt(0);
            }
            UpdateRepresentation();
        }
    }

    override public void Init() {
        viewedFlipSide = FlipSideEnum.BlackWhite;
        UpdateButtonImages();
    }

    private void Start() {
        UpdateRepresentation();
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
        blackWhiteImage.color = (viewedFlipSide == FlipSideEnum.BlackWhite) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
        whiteBlackImage.color = (viewedFlipSide == FlipSideEnum.WhiteBlack) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
    }

    public void UpdateRepresentation() {
        //Nothing to represent
        if (genotype == null) {
            genePanel.gene = null;
            GenomePanel.instance.genotype = null;
            return;
        }

        genePanel.UpdateRepresentation();
        GenomePanel.instance.genotype = genotype;
    }
}
