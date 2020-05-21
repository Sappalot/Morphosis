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
		if (!CreatureSelectionPanel.instance.hasSoloSelected) {
			return;
		}

		Genotype genotype = CreatureSelectionPanel.instance.soloSelected.genotype;
		Gene selectedGene = GenePanel.instance.selectedGene;

		// unhighlite all
		for (int index = 0; index < nerveArrowList.Count; index++) {
			nerveArrowList[index].isHighlited = false;
		}

		// highlite viewed
		List<Nerve> nervesToHighlite = HudSignalArrowHandler.GetNervesToHighliteGenotype(genotype, selectedGene);
		if (nervesToHighlite != null) {
			for (int index = 0; index < nerveArrowList.Count; index++) {
				NerveArrow nerveArrow = nerveArrowList[index];
				if (nervesToHighlite.Find(n => Nerve.AreTwinNerves(nerveArrow.nerve, n, false)) != null) {
					nerveArrow.isHighlited = true;
				} 
			}
		}

		for (int index = 0; index < nerveArrowList.Count; index++) {
			nerveArrowList[index].UpdateGraphics();
		}
	}

	public void GenerateGenotype(Genotype genotype) {
		Clear();

		foreach (Cell geneCell in genotype.geneCellListIndexSorted) {
			List<Nerve> nerves = geneCell.GetAllNervesGenotype();
			if (nerves.Count > 0) {
				

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