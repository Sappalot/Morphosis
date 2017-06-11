using UnityEngine;

public class GenomePanel : MonoSingleton<GenomePanel> {
    public GenomeGene genomeGeneTemplate;
    public Transform geneParent;

    private GenomeGene[] genomeGenes = new GenomeGene[Genotype.genomeLength];

    private Genotype m_genotype;
    public Genotype genotype {
        get {
            return m_genotype;
        }
        set {
            m_genotype = value;
            UpdateRepresentation();
        }
    }

    override public void Init() {
        RectTransform originalTransform = genomeGeneTemplate.GetComponent<RectTransform>();

        //float newHeight = genomeGenes.Length * (originalTransform.rect.height + 2);
        //float width = GetComponent<RectTransform>().rect.width;
        //Vector2 position = GetComponent<RectTransform>().rect.position;
        //geneParent.GetComponent<RectTransform>().rect.Set(0f, 0f, 0f, 5000f); //TODO: figgure out how to change the height runtime

        for (int index = 0; index < genomeGenes.Length; index++) {
            genomeGenes[index] = Instantiate(genomeGeneTemplate, geneParent);
            genomeGenes[index].gameObject.SetActive(true);
            genomeGenes[index].name = "Gene " + index;
            RectTransform spawnTransform = genomeGenes[index].GetComponent<RectTransform>();
            spawnTransform.position = originalTransform.position + Vector3.down * index * (originalTransform.rect.height + 2);

            genomeGenes[index].index = index;
            genomeGenes[index].text.text = "Gene " + index;
        }
    }

    public void UpdateRepresentation() {
        //Nothing to represent
        if (genotype == null) {
            for (int index = 0; index < genomeGenes.Length; index++) {
                genomeGenes[index].gene = null;
            }
            return;
        }

        for (int index = 0; index < genomeGenes.Length; index++) {
            genomeGenes[index].gene = genotype.genome[index];
        }
    }
}
