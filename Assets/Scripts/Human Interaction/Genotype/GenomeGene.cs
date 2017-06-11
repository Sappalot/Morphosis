using UnityEngine;
using UnityEngine.UI;

public class GenomeGene : MonoBehaviour {

    public int index;

    public Text text;

    private Gene m_gene;
    public Gene gene
    {
        get {
            return m_gene;
        }
        set {
            m_gene = value;
            UpdateRepresentation();
        }
    }

    public void OnClicked() {
        Debug.Log("Clicked " + index);
        GenotypePanel.instance.genePanel.gene = gene;
    }

    public void UpdateRepresentation() {
        //Nothing to represent
        if (gene == null) {
            text.text = "-";
            return;
        }

        text.text = gene.index.ToString() + gene.type;
    }
}
