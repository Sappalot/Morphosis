using UnityEngine;
using UnityEngine.UI;

public class GenomePanel : MonoSingleton<GenomePanel> {
	public GenomeGenePanel genomeGeneTemplate;
	public Transform geneParent;

	public ScrollRect scrollRect;

	private GenomeGenePanel[] genomeGenes = new GenomeGenePanel[Genotype.genomeLength];

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
	}

	private bool isScrollDirty = true;
	public void MakeScrollDirty() {
		isScrollDirty = true;
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

		//scrollRect.horizontalNormalizedPosition = 0f;
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.debug.debugLogMenuUpdate)
				DebugUtil.Log("Update GenomePanel");
			if (genotype == null) {
				for (int index = 0; index < genomeGenes.Length; index++) {
					genomeGenes[index].gene = null;
				}
				scrollRect.horizontalNormalizedPosition = 0f;
				isDirty = false;
				return;
			}

			for (int index = 0; index < genomeGenes.Length; index++) {
				genomeGenes[index].gene = genotype.GetGeneAt(index);
			}
			isDirty = false;
		} else {
			// Don't scroll at the same frame that craphics in scroll rect is updated!
			// Doing so will ocationally cause scrambled graphics
			if (isScrollDirty) {
				if (genotype == null) {
					isScrollDirty = false;
					return;
				}

				// horizontalNormalizedPosition causes faulty update. Fix it!! 
				for (int index = 0; index < genomeGenes.Length; index++) {
					if (genomeGenes[index].gene == GenePanel.instance.selectedGene) {
						scrollRect.horizontalNormalizedPosition = (float)index / ((float)genomeGenes.Length - 1f);
					}
				}

				isScrollDirty = false;
			}
		}
	}
}
