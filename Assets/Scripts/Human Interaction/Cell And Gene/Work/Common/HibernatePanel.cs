using UnityEngine;
using UnityEngine.UI;

public class HibernatePanel : MonoBehaviour {
	public CellTypeEnum cellType;

	public Image hibernatePanelBackground;
	public Image motherPanelBackground;
	public Image childPanelBackground;

	public Image motherBlockBackground;
	public Image motherPassBackground;
	public Image motherBlockSymbol;
	public Image motherPassSymbol;

	public Image childBlockBackground;
	public Image childPassBackground;
	public Image childBlockSymbol;
	public Image childPassSymbol;

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

	public void OnMotherPassClicked() {
		if (mode == PhenoGenoEnum.Phenotype) {
			return;
		}
		SetHibernateWhenAttachedToMother(true);
		MakeDirty();
	}

	public void OnMotherBlockClicked() {
		if (mode == PhenoGenoEnum.Phenotype) {
			return;
		}
		SetHibernateWhenAttachedToMother(false);
		MakeDirty();
	}

	public void OnChildPassClicked() {
		if (mode == PhenoGenoEnum.Phenotype) {
			return;
		}
		SetHibernateWhenAttachedToChild(true);
		MakeDirty();
	}

	public void OnChildBlockClicked() {
		if (mode == PhenoGenoEnum.Phenotype) {
			return;
		}
		SetHibernateWhenAttachedToChild(false);
		MakeDirty();
	}

	private void SetHibernateWhenAttachedToMother(bool on) {
		if (cellType == CellTypeEnum.Egg) {
			selectedGene.eggCellHibernateWhenAttachedToMother = on;
		} else if (cellType == CellTypeEnum.Jaw) {
			selectedGene.jawCellHibernateWhenAttachedToMother = on;
		} else if (cellType == CellTypeEnum.Leaf) {
			selectedGene.leafCellHibernateWhenAttachedToMother = on;
		} else if (cellType == CellTypeEnum.Muscle) {
			selectedGene.muscleCellHibernateWhenAttachedToMother = on;
		}
	}

	private void SetHibernateWhenAttachedToChild(bool on) {
		if (cellType == CellTypeEnum.Egg) {
			selectedGene.eggCellHibernateWhenAttachedToChild = on;
		} else if (cellType == CellTypeEnum.Jaw) {
			selectedGene.jawCellHibernateWhenAttachedToChild = on;
		} else if (cellType == CellTypeEnum.Leaf) {
			selectedGene.leafCellHibernateWhenAttachedToChild = on;
		} else if (cellType == CellTypeEnum.Muscle) {
			selectedGene.muscleCellHibernateWhenAttachedToChild = on;
		}
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Hibernate Panel");
			}
			ignoreSliderMoved = true;

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					hibernatePanelBackground.color = selectedCell.IsHibernating() ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
					motherPanelBackground.color = selectedCell.creature.IsAttachedToMotherAlive() ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
					childPanelBackground.color = selectedCell.creature.IsAttachedToChildAlive() ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				hibernatePanelBackground.color = ColorScheme.instance.signalOff;
				motherPanelBackground.color = ColorScheme.instance.signalOff;
				childPanelBackground.color = ColorScheme.instance.signalOff;
			}

			if (selectedGene != null) { 
				if (cellType == CellTypeEnum.Egg) {
					motherBlockBackground.color = !selectedGene.eggCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					motherPassBackground.color =   selectedGene.eggCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					motherBlockSymbol.color = !selectedGene.eggCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;
					motherPassSymbol.color = selectedGene.eggCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;

					childBlockBackground.color =  !selectedGene.eggCellHibernateWhenAttachedToChild   ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					childPassBackground.color =    selectedGene.eggCellHibernateWhenAttachedToChild   ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					childBlockSymbol.color = !selectedGene.eggCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;
					childPassSymbol.color = selectedGene.eggCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;
				} else if (cellType == CellTypeEnum.Jaw) {
					motherBlockBackground.color = !selectedGene.jawCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					motherPassBackground.color =   selectedGene.jawCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					motherBlockSymbol.color = !selectedGene.jawCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;
					motherPassSymbol.color = selectedGene.jawCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;

					childBlockBackground.color =  !selectedGene.jawCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					childPassBackground.color =    selectedGene.jawCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					childBlockSymbol.color = !selectedGene.jawCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;
					childPassSymbol.color = selectedGene.jawCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;
				} else if (cellType == CellTypeEnum.Leaf) {
					motherBlockBackground.color = !selectedGene.leafCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					motherPassBackground.color =   selectedGene.leafCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					motherBlockSymbol.color = !selectedGene.leafCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;
					motherPassSymbol.color = selectedGene.leafCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;

					childBlockBackground.color =  !selectedGene.leafCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					childPassBackground.color =    selectedGene.leafCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					childBlockSymbol.color = !selectedGene.leafCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;
					childPassSymbol.color = selectedGene.leafCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;
				} else if (cellType == CellTypeEnum.Muscle) {
					motherBlockBackground.color = !selectedGene.muscleCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					motherPassBackground.color =   selectedGene.muscleCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					motherBlockSymbol.color = !selectedGene.muscleCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;
					motherPassSymbol.color = selectedGene.muscleCellHibernateWhenAttachedToMother ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;

					childBlockBackground.color =  !selectedGene.muscleCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					childPassBackground.color =    selectedGene.muscleCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
					childBlockSymbol.color = !selectedGene.muscleCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;
					childPassSymbol.color = selectedGene.muscleCellHibernateWhenAttachedToChild ? ColorScheme.instance.selectedButtonSymbol : ColorScheme.instance.notSelectedButtonSymbol;
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
				return GenePanel.instance.selectedGene;
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
}