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
		foreach (NerveArrow nerveArrow in nerveArrowList) {
			nerveArrow.highlitedEnum = NerveArrow.HighliteEnum.notHighlited;
		}

		// highlite viewed
		List<Nerve> nervesToHighlite = HudSignalArrowHandler.GetNervesToHighliteGenotype(genotype, selectedGene);
		bool shouldHighlitAllXput = HudSignalArrowHandler.IsNervesHighliteAllModeGenotype();
		if (nervesToHighlite != null) {
			for (int index = 0; index < nerveArrowList.Count; index++) {
				NerveArrow nerveArrow = nerveArrowList[index];

				if (nervesToHighlite.Find(n => n == nerveArrow.nerve) != null) {
					//if (shouldHighlitAllXput) {
					//	nerveArrow.highlitedEnum = NerveArrow.HighliteEnum.highlitedArrow; // dont want circles when showing all, too messy
					//} else {
					nerveArrow.highlitedEnum = NerveArrow.HighliteEnum.highlitedArrowAndCircles;
					//}
				} 
			}
		}

		for (int index = 0; index < nerveArrowList.Count; index++) {
			nerveArrowList[index].UpdateGraphics();
		}
	}

	public void GenerateGenotype(Genotype genotype) {
		Clear();

		foreach (Nerve nerve in genotype.GetAllExternalNervesGenotype()) {
			NerveArrow nerveArrow = Morphosis.instance.nerveArrowPool.Borrow();
			nerveArrow.transform.parent = transform;
			nerveArrow.transform.position = transform.position;
			nerveArrow.Setup(nerve);
			nerveArrowList.Add(nerveArrow);
		}
	}

	public void OnRecycle() {
		Clear();
	}


}