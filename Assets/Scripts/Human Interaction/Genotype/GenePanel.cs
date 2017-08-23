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
            UpdateRepresentation(false);
            if (CreatureSelectionPanel.instance.selectedCreature != null) {
                CreatureSelectionPanel.instance.selectedCreature.genotype.ShowGeneCellsSelected(false);
                CreatureSelectionPanel.instance.selectedCreature.genotype.ShowGeneCellsSelectedWithGene(m_gene, true);
            }
        }
    }
    public ReferenceGraphics[] referenceGraphics;

    public ArrangementPanel arrangementPanelTemplate;
    private ArrangementPanel[] arrangementPanels = new ArrangementPanel[3];

    //Gene Settings
    public Dropdown cellTypeDropdown;

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

    public void UpdateRepresentation(bool changeToGenomeMade) {

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

        if (GenomePanel.instance.genotype != null)
            GenomePanel.instance.genotype.isDirty |= changeToGenomeMade;

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

        //Gene Settings
        if (gene.type == CellTypeEnum.Jaw) {
            cellTypeDropdown.value = 0;
        } else if (gene.type == CellTypeEnum.Leaf) {
            cellTypeDropdown.value = 1;
        } else if (gene.type == CellTypeEnum.Muscle) {
            cellTypeDropdown.value = 2;
        } else if (gene.type == CellTypeEnum.Vein) {
            cellTypeDropdown.value = 3;
        }

        //Hack
        if (changeToGenomeMade && CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
            CreatureSelectionPanel.instance.selectedCreature.genotype.Generate(CreatureSelectionPanel.instance.selectedCreature);
            CreatureSelectionPanel.instance.selectedCreature.genotype.ShowGeneCellsSelectedWithGene(m_gene, true);
            CreatureSelectionPanel.instance.selectedCreature.genotype.UpdateTransformAndHighlite(CreatureSelectionPanel.instance.selectedCreature);

            CreatureSelectionPanel.instance.selectedCreature.phenotype.isDirty = true;
        } 
    }

    //----
    public void OnCellTypeDropdownChanged() {
        bool trueChange = (int)gene.type != cellTypeDropdown.value;

        if (cellTypeDropdown.value == 0) {
            gene.type = CellTypeEnum.Jaw;
        } else if (cellTypeDropdown.value == 1) {
            gene.type = CellTypeEnum.Leaf;
        } else if (cellTypeDropdown.value == 2) {
            gene.type = CellTypeEnum.Muscle;
        } else if (cellTypeDropdown.value == 3) {
            gene.type = CellTypeEnum.Vein;
        }
        
        if (trueChange && CreatureEditModePanel.instance.editMode == CreatureEditModePanel.CretureEditMode.genotype) {
            GenotypePanel.instance.UpdateRepresentation(trueChange);
        }
    }

    private ArrangementPanel arrangementPanelAskingForReference;
    public void SetAskingForGeneReference(ArrangementPanel arrangementPanel) {
        arrangementPanelAskingForReference = arrangementPanel;
    }

    public void GiveAnswerGeneReference(Gene gene) {
        arrangementPanelAskingForReference.SetGeneReference(gene);
    }
}
