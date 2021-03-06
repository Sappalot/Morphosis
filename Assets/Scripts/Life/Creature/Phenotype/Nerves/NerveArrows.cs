﻿using System.Collections.Generic;
using UnityEngine;


// These will work as a graphical representation of the external nerves in a creature
// Use same one but 2 instances per creature: Genotype & Phenotype ??
// Nerves in genotype phenotype should work just fine even without this graphical layer

public class NerveArrows : MonoBehaviour {
	public Transform arrowContainer;

	public PhenoGenoEnum phenoGeno; // set in inspector

	private List<NerveArrow> nerveArrowList = new List<NerveArrow>();
	private PhenoGenoEnum phenoGenoMode;



	public void Clear() {
		for (int index = 0; index < nerveArrowList.Count; index++) {
			Morphosis.instance.nerveArrowPool.Recycle(nerveArrowList[index]);
		}
		nerveArrowList.Clear();
	}

	// if !show just disable cheep :)
	public void UpdateGraphics(bool show, bool isGrabbed) { // is grabbed is used to be able as we use root position and rotation instread of cells position and rotation
		if (show) {
			Show(true);

			// unhighlite all
			foreach (NerveArrow nerveArrow in nerveArrowList) {
				nerveArrow.highlitedEnum = NerveArrow.HighliteEnum.notHighlited;
			}

			// highlite viewed
			List<Nerve> nervesToHighlite = null;
			if (phenoGeno == PhenoGenoEnum.Genotype) {
				if (CreatureSelectionPanel.instance.hasSoloSelected) {
					Gene selectedGene = GenePanel.instance.selectedGene;
					Genotype genotype = CreatureSelectionPanel.instance.soloSelected.genotype;
					nervesToHighlite = HudSignalArrowHandler.GetNervesToHighliteGenotype(genotype, selectedGene);
				}
			} else {
				Cell selectedCell = CellPanel.instance.selectedCell;
				nervesToHighlite = HudSignalArrowHandler.GetNervesToHighlitePhenotype(selectedCell);
			}

			if (nervesToHighlite != null) {
				for (int index = 0; index < nerveArrowList.Count; index++) {
					NerveArrow nerveArrow = nerveArrowList[index];

					if (nervesToHighlite.Find(n => n == nerveArrow.nerve) != null) {
						nerveArrow.highlitedEnum = NerveArrow.HighliteEnum.highlitedArrowAndCircles;
					}
				}
			}

			for (int index = 0; index < nerveArrowList.Count; index++) {
				nerveArrowList[index].UpdateGraphics(isGrabbed);
			}
		} else {
			Show(false);
		}
	}

	private void Show(bool show) {
		arrowContainer.gameObject.SetActive(show);
	}

	public void GenerateGenotype(Genotype genotype) {
		phenoGenoMode = PhenoGenoEnum.Genotype;

		Clear();

		foreach (Nerve nerve in genotype.GetAllExternalNerves()) {
			NerveArrow nerveArrow = Morphosis.instance.nerveArrowPool.Borrow();
			nerveArrow.transform.parent = arrowContainer;
			nerveArrow.transform.position = transform.position;
			nerveArrow.Setup(phenoGenoMode, nerve);
			nerveArrowList.Add(nerveArrow);
		}
	}

	public void GeneratePhenotype(Phenotype phenotype) {
		phenoGenoMode = PhenoGenoEnum.Phenotype;

		Clear();

		foreach (Nerve nerve in phenotype.GetAllExternalNerves()) {
			NerveArrow nerveArrow = Morphosis.instance.nerveArrowPool.Borrow();
			nerveArrow.transform.parent = arrowContainer;
			nerveArrow.transform.position = transform.position;
			nerveArrow.Setup(phenoGenoMode, nerve);
			nerveArrowList.Add(nerveArrow);
		}
	}

	public void OnRecycle() {
		Clear();
	}
}