using System.Collections.Generic;
using UnityEngine;


// These will work as a graphical representation of the external nerves in a creature
// Use same one but 2 instances per creature: Genotype & Phenotype ??
// Nerves in genotype phenotype should work just fine even without this graphical layer

public class NerveArrows : MonoBehaviour {
	public PhenoGenoEnum phenoGeno; // set in inspector

	private List<NerveArrow> nerveArrowList = new List<NerveArrow>();

	public void Clear() {
		for (int index = 0; index < nerveArrowList.Count; index++) {
			Morphosis.instance.nerveArrowPool.Recycle(nerveArrowList[index]);
		}
		nerveArrowList.Clear();
	}

	public void UpdateGraphics() {
		for (int index = 0; index < nerveArrowList.Count; index++) {
			nerveArrowList[index].UpdateGraphics();
		}
	}

	public void GenerateGenotype(Genotype genotype) {
		Clear();

		foreach (Cell geneCell in genotype.geneCellListIndexSorted) {
			List<Nerve> nerves = geneCell.GetAllNervesGenotype(false);
			if (nerves != null) {
				foreach (Nerve n in nerves) {
					if (n.nerveStatusEnum == NerveStatusEnum.Input_GenotypeExternalVoid || n.nerveStatusEnum == NerveStatusEnum.Input_GenotypeExternal) {
						NerveArrow nerveArrow = Morphosis.instance.nerveArrowPool.Borrow();
						nerveArrow.transform.parent = transform;
						nerveArrow.transform.position = transform.position;
						nerveArrow.Setup(geneCell, n);
						nerveArrowList.Add(nerveArrow);
					}

				}
			}
		}
	}

	public void OnRecycle() {
		Clear();
	}
}