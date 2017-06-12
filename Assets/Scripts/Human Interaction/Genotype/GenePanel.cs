using UnityEngine;
using UnityEngine.UI;

public class GenePanel : MonoSingleton<GenePanel> {

    public GameObject circles;

    public Image geneReferenceImage;
    public Image flipBlackWhite;
    public Image flipWhiteBlack;
    public Text geneReferenceText;

    private Gene m_gene;
    public Gene gene {
        get {
            return m_gene;
        }
        set {
            m_gene = value;
            UpdateRepresentation();
        }
    }
    public ReferenceGraphics[] referenceGraphics;

    public ArrangementPanel arrangementPanelTemplate;
    private ArrangementPanel[] arrangementPanels = new ArrangementPanel[3];
    
    override public void Init() {
        RectTransform originalTransform = arrangementPanelTemplate.GetComponent<RectTransform>();

        for (int index = 0; index < arrangementPanels.Length; index++) {
            arrangementPanels[index] = Instantiate(arrangementPanelTemplate, transform);
            arrangementPanels[index].gameObject.SetActive(true);
            arrangementPanels[index].name = "Arrangement Panel " + index;
            RectTransform spawnTransform = arrangementPanels[index].GetComponent<RectTransform>();
            spawnTransform.position = originalTransform.position + Vector3.down * index * (originalTransform.rect.height + 2);

            arrangementPanels[index].genePanel = this;
        }

        arrangementPanelTemplate.gameObject.SetActive(false);
        arrangementPanelTemplate.arrangementButtons.SetActive(false);
    }

    public void UpdateRepresentation() {
        //Nothing to represent
        if (gene == null) {
            for (int index = 0; index < arrangementPanels.Length; index++) {
                if (arrangementPanels[index] != null) {
                    arrangementPanels[index].arrangement = null;
                }
            }

            circles.SetActive(false);
            return;
        }

        circles.SetActive(true);

        for (int index = 0; index < arrangementPanels.Length; index++) {
            if (arrangementPanels[index] != null) {
                arrangementPanels[index].arrangement = gene.arrangements[index];
            }
        }

        //perifier
        for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
            referenceGraphics[cardinalIndex].reference = gene.GetFlippableReference(cardinalIndex, GenotypePanel.instance.viewedFlipSide);
        }

        geneReferenceImage.color = CellTypeUtil.ToColor(gene.type);
        flipBlackWhite.enabled = GenotypePanel.instance.viewedFlipSide == FlipSideEnum.BlackWhite;
        flipWhiteBlack.enabled = GenotypePanel.instance.viewedFlipSide == FlipSideEnum.WhiteBlack;
        geneReferenceText.text = gene.index.ToString();


    }

    //----
    private ArrangementPanel arrangementPanelAskingForReference;
    public void SetAskingForGeneReference(ArrangementPanel arrangementPanel) {
        arrangementPanelAskingForReference = arrangementPanel;
    }

    public void GiveAnswerGeneReference(Gene gene) {
        arrangementPanelAskingForReference.SetGeneReference(gene);
    }
}
