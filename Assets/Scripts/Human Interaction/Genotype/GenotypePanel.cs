﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GenotypePanel : MonoSingleton<GenotypePanel> {
	public GameObject lowerPanel;

	public Image blackWhiteImage;
	public Image whiteBlackImage;

	public Color unSelectedGeneColor;
	public Color selectedGeneColor;
	public Color unUsedGeneColor;

	public FlipSideEnum viewedFlipSide { get; private set; }

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
	}

	public Genotype selectedGenotype {
		get {
			return (CreatureSelectionPanel.instance.hasSoloSelected ? CreatureSelectionPanel.instance.soloSelected.genotype : null);
		}
	}

	override public void Init() {
		viewedFlipSide = FlipSideEnum.BlackWhite; 
		isDirty = true;
	}

	public void OnClickedClear() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			if (creature.allowedToChangeGenome) {
				creature.Clear();
				creature.creation = CreatureCreationEnum.Forged;
			}
		}
		CreatureSelectionPanel.instance.MakeDirty();
		GeneCellPanel.instance.selectedGene = null;
		GeneCellPanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	public void OnClickedMutate() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			if (creature.allowedToChangeGenome) {
				creature.MutateCummulative(GlobalSettings.instance.mutation.masterMutationStrength * 10f);
				creature.creation = CreatureCreationEnum.Forged;
			}
		}
		CreatureSelectionPanel.instance.MakeDirty();
		GeneCellPanel.instance.selectedGene = null;
		GeneCellPanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	public void OnClickedScramble() {
		foreach (Creature creature in CreatureSelectionPanel.instance.selection) {
			if (creature.allowedToChangeGenome) {
				creature.Scramble();
				creature.creation = CreatureCreationEnum.Forged;
			}
		}
		CreatureSelectionPanel.instance.MakeDirty();
		GeneCellPanel.instance.selectedGene = null;
		GeneCellPanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	public void OnClickedBlackWhite() {
		viewedFlipSide = FlipSideEnum.BlackWhite;
		isDirty = true;
		GeneCellPanel.instance.geneNeighbourPanel.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	public void OnClickedWhiteBlack() {
		viewedFlipSide = FlipSideEnum.WhiteBlack;
		isDirty = true;
		GeneCellPanel.instance.geneNeighbourPanel.MakeDirty();
		GenomePanel.instance.MakeDirty();
	}

	private IEnumerator UpdateIsVisible() {
		yield return 0;
		GeneCellPanel.instance.gameObject.SetActive((CreatureSelectionPanel.instance.hasSoloSelected && MouseAction.instance.actionState == MouseActionStateEnum.free && !AlternativeToolModePanel.instance.isOn) || MouseAction.instance.actionState == MouseActionStateEnum.selectGene);
		lowerPanel.gameObject.SetActive((CreatureSelectionPanel.instance.hasSelection && MouseAction.instance.actionState == MouseActionStateEnum.free && !AlternativeToolModePanel.instance.isOn) || MouseAction.instance.actionState == MouseActionStateEnum.selectGene);
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update GenotypePanel");
			}

			StartCoroutine(UpdateIsVisible());

			blackWhiteImage.color = (viewedFlipSide == FlipSideEnum.BlackWhite) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
			whiteBlackImage.color = (viewedFlipSide == FlipSideEnum.WhiteBlack) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
			isDirty = false;
		}
	}
}
