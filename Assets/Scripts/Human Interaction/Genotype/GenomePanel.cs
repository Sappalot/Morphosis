using UnityEngine;

public class GenomePanel : MonoSingleton<GenomePanel> {
	public GenomeGene genomeGeneTemplate;
	public Transform geneParent;

	private GenomeGene[] genomeGenes = new GenomeGene[Genotype.genomeLength];

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
	}

	public Genotype genotype {
		get {
			return GenotypePanel.instance.selectedGenotype;
		}
	}

	override public void Init() {
		RectTransform originalTransform = genomeGeneTemplate.GetComponent<RectTransform>();

		for (int index = 0; index < genomeGenes.Length; index++) {
			genomeGenes[index] = Instantiate(genomeGeneTemplate, geneParent);
			genomeGenes[index].gameObject.SetActive(true);
			genomeGenes[index].name = "Gene " + index;
			RectTransform spawnTransform = genomeGenes[index].GetComponent<RectTransform>();
			spawnTransform.position = originalTransform.position + Vector3.right * index * (originalTransform.rect.width + 2);
		}
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update GenomePanel");
			if (genotype == null) {
				for (int index = 0; index < genomeGenes.Length; index++) {
					genomeGenes[index].gene = null;
				}
				isDirty = false;
				return;
			}

			for (int index = 0; index < genomeGenes.Length; index++) {
				genomeGenes[index].gene = genotype.GetGeneAt(index);
			}
			isDirty = false;
		}
	}
}
