using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenePanel : MonoSingleton<GenePanel> {

    public Image geneReferenceImage;
    public Image flipBlackWhite;
    public Image flipWhiteBlack;
    public Text geneReferenceText;

    public Gene gene;

    public ArrangementPanel arrangementPanelTemplate;
    private ArrangementPanel[] arrangementPanels = new ArrangementPanel[3];
    
    override public void Init() {


        RectTransform originalTransform = arrangementPanelTemplate.GetComponent<RectTransform>();

        for (int index = 0; index < arrangementPanels.Length; index++) {
            arrangementPanels[index] = Instantiate(arrangementPanelTemplate, transform);
            arrangementPanelTemplate.gameObject.SetActive(true);
            arrangementPanels[index].name = "Arrangement Panel " + index;
            RectTransform spawnTransform = arrangementPanels[index].GetComponent<RectTransform>();
            spawnTransform.position = originalTransform.position + Vector3.down * index * (originalTransform.rect.height + 2);

        }

        arrangementPanelTemplate.gameObject.SetActive(false);
        arrangementPanelTemplate.arrangementButtons.SetActive(false);
    }


    public void UpdateRepresentation() {



        //hack select
        List<Creature> selection = CreatureSelectionPanel.instance.selection;

        
        
        //perifier
        arrangementPanels[0].arrangement = selection.Count == 1 ? selection[0].genotype.genome[0].arrangements[0] : null;
        arrangementPanels[1].arrangement = selection.Count == 1 ? selection[0].genotype.genome[0].arrangements[1] : null;
        arrangementPanels[2].arrangement = selection.Count == 1 ? selection[0].genotype.genome[0].arrangements[2] : null;

        arrangementPanels[0].UpdateRepresentation();
        arrangementPanels[1].UpdateRepresentation();
        arrangementPanels[2].UpdateRepresentation();

        //core
        gene = selection[0].genotype.genome[0];

        geneReferenceImage.color = CellTypeUtil.ToColor(gene.type);
        flipBlackWhite.enabled = GenotypePanel.instance.viewedFlipSide == FlipSideEnum.BlackWhite;
        flipWhiteBlack.enabled = GenotypePanel.instance.viewedFlipSide == FlipSideEnum.WhiteBlack;
        geneReferenceText.text = gene.index.ToString();
    }
}
