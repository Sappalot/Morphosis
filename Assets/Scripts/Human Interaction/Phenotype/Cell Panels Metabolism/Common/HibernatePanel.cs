using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HibernatePanel : MonoBehaviour {
	public CellTypeEnum cellType;

	public Image hibernatePanelBackground;

	public Toggle motherToggle;
	public Image motherToggleBackground;
	public Image motherPanelBackground;

	public Toggle childToggle;
	public Image childToggleBackground;
	public Image childPanelBackground;


	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	private bool isDirty = false;
	private bool ignoreSliderMoved = false;

	public void SetMode(PhenoGenoEnum mode) {
		this.mode = mode;
	}

	private PhenoGenoEnum GetMode() {
		return mode;
	}

	public void MakeDirty() {
		isDirty = true;
	}

	public void OnMotherToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		if (cellType == CellTypeEnum.Egg) {
			selectedGene.eggCellHibernateWhenAttachedToMother = motherToggle.isOn;
		} else if (cellType == CellTypeEnum.Jaw) {
			selectedGene.jawCellHibernateWhenAttachedToMother = motherToggle.isOn;
		} else if (cellType == CellTypeEnum.Leaf) {
			selectedGene.leafCellHibernateWhenAttachedToMother = motherToggle.isOn;
		} else if (cellType == CellTypeEnum.Muscle) {
			selectedGene.muscleCellHibernateWhenAttachedToMother = motherToggle.isOn;
		}

		MakeCreatureForged();
	}



	public void OnChildToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		if (cellType == CellTypeEnum.Egg) {
			selectedGene.eggCellHibernateWhenAttachedToChild = childToggle.isOn;
		} else if (cellType == CellTypeEnum.Jaw) {
			selectedGene.jawCellHibernateWhenAttachedToChild = childToggle.isOn;
		} else if (cellType == CellTypeEnum.Leaf) {
			selectedGene.leafCellHibernateWhenAttachedToChild = childToggle.isOn;
		} else if (cellType == CellTypeEnum.Muscle) {
			selectedGene.muscleCellHibernateWhenAttachedToChild = childToggle.isOn;
		}

		MakeCreatureForged();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Hibernate Panel");
			}
			ignoreSliderMoved = true;

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					motherToggle.interactable = false;
					childToggle.interactable = false;

					motherToggleBackground.color = ColorScheme.instance.grayedOutGenotype;
					childToggleBackground.color = ColorScheme.instance.grayedOutGenotype;

					hibernatePanelBackground.color = selectedCell.IsHibernating() ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
					motherPanelBackground.color = selectedCell.creature.IsAttachedToMotherAlive() ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
					childPanelBackground.color = selectedCell.creature.IsAttachedToChildAlive() ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				motherToggle.interactable = IsUnlocked();
				childToggle.interactable = IsUnlocked();

				motherToggleBackground.color = Color.white;
				childToggleBackground.color = Color.white;

				hibernatePanelBackground.color = ColorScheme.instance.grayedOutPhenotype;
				motherPanelBackground.color = ColorScheme.instance.grayedOutPhenotype;
				childPanelBackground.color = ColorScheme.instance.grayedOutPhenotype;
			}

			if (selectedGene != null) { 
				if (cellType == CellTypeEnum.Egg) {
					motherToggle.isOn = selectedGene.eggCellHibernateWhenAttachedToMother;
					childToggle.isOn = selectedGene.eggCellHibernateWhenAttachedToChild;
				} else if (cellType == CellTypeEnum.Jaw) {
					motherToggle.isOn = selectedGene.jawCellHibernateWhenAttachedToMother;
					childToggle.isOn = selectedGene.jawCellHibernateWhenAttachedToChild;
				} else if (cellType == CellTypeEnum.Leaf) {
					motherToggle.isOn = selectedGene.leafCellHibernateWhenAttachedToMother;
					childToggle.isOn = selectedGene.leafCellHibernateWhenAttachedToChild;
				} else if (cellType == CellTypeEnum.Muscle) {
					motherToggle.isOn = selectedGene.muscleCellHibernateWhenAttachedToMother;
					childToggle.isOn = selectedGene.muscleCellHibernateWhenAttachedToChild;
				}
			}

			ignoreSliderMoved = false;

			isDirty = false;
		}
	}

	private void MakeCreatureForged() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		MakeDirty();
	}

	private Gene selectedGene {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell != null ? CellPanel.instance.selectedCell.gene : null;
			} else {
				return GeneCellPanel.instance.selectedGene;
			}
		}
	}

	private Cell selectedCell {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell;
			} else {
				return null; // there could be many cells selected for the same gene
			}
		}
	}

	public bool IsUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;
	}

	public Color IsUnlockedColor() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome ? Color.black : ColorScheme.instance.grayedOut;
	}
}
