using UnityEngine;
using UnityEngine.UI;

public class JawCellPanel : MetabolismCellPanel {

	public Text prayCellCount;

	public Text cannibalizeText;

	public Toggle cannibalizeKinToggle;
	public Text cannibalizeKinText;

	public Toggle cannibalizeMotherToggle;
	public Text cannibalizeMotherText;

	public Toggle cannibalizeFatherToggle;
	public Text cannibalizeFatherText;

	public Toggle cannibalizeSiblingsToggle;
	public Text cannibalizeSiblingsText;

	public Toggle cannibalizeChildrenToggle;
	public Text cannibalizeChildrenText;

	public Text idleWhenAttachedText;
	public Toggle idleWhenAttachedToggle;

	public void OnChangedCannibalizeKin() {
		if (ignoreSliderMoved) {
			return;
		}
		selectedGene.jawCellCannibalizeKin = cannibalizeKinToggle.isOn;
		ApplyChange();
	}

	public void OnChangedCannibalizeMother() {
		if (ignoreSliderMoved) {
			return;
		}
		selectedGene.jawCellCannibalizeMother = cannibalizeMotherToggle.isOn;
		ApplyChange();
	}

	public void OnChangedCannibalizeFather() {
		if (ignoreSliderMoved) {
			return;
		}
		selectedGene.jawCellCannibalizeFather = cannibalizeFatherToggle.isOn;
		ApplyChange();
	}

	public void OnChangedCannibalizeSiblings() {
		if (ignoreSliderMoved) {
			return;
		}
		selectedGene.jawCellCannibalizeSiblings = cannibalizeSiblingsToggle.isOn;
		ApplyChange();
	}

	public void OnChangedCannibalizeChildren() {
		if (ignoreSliderMoved) {
			return;
		}
		selectedGene.jawCellCannibalizeChildren = cannibalizeChildrenToggle.isOn;
		ApplyChange();
	}

	public void OnIdleWhenAttachedToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.jawCellIdleWhenAttached = idleWhenAttachedToggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (mode == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					prayCellCount.text = "Eating on cells: " + (CellPanel.instance.selectedCell as JawCell).prayCount;
					prayCellCount.color = Color.black;

					cannibalizeText.color = ColorScheme.instance.grayedOutGenotype;

					cannibalizeKinToggle.interactable = false;
					cannibalizeKinText.color = ColorScheme.instance.grayedOutGenotype;

					cannibalizeMotherToggle.interactable = false;
					cannibalizeMotherText.color = ColorScheme.instance.grayedOutGenotype;

					cannibalizeFatherToggle.interactable = false;
					cannibalizeFatherText.color = ColorScheme.instance.grayedOutGenotype;

					cannibalizeSiblingsToggle.interactable = false;
					cannibalizeSiblingsText.color = ColorScheme.instance.grayedOutGenotype;

					cannibalizeChildrenToggle.interactable = false;
					cannibalizeChildrenText.color = ColorScheme.instance.grayedOutGenotype;

					idleWhenAttachedText.color = ColorScheme.instance.grayedOutGenotype;
					idleWhenAttachedToggle.interactable = false;
				}
			} else if (mode == PhenoGenoEnum.Genotype) {
				prayCellCount.text = "Eating on cells: -";
				prayCellCount.color = ColorScheme.instance.grayedOutPhenotype;

				cannibalizeText.color = isUnlockedColor();

				cannibalizeKinToggle.interactable = isUnlocked();
				cannibalizeKinText.color = isUnlockedColor();

				cannibalizeMotherToggle.interactable = isUnlocked();
				cannibalizeMotherText.color = isUnlockedColor();

				cannibalizeFatherToggle.interactable = isUnlocked();
				cannibalizeFatherText.color = isUnlockedColor();

				cannibalizeSiblingsToggle.interactable = isUnlocked();
				cannibalizeSiblingsText.color = isUnlockedColor();

				cannibalizeChildrenToggle.interactable = isUnlocked();
				cannibalizeChildrenText.color = isUnlockedColor();

				idleWhenAttachedText.color = Color.black;
				idleWhenAttachedToggle.interactable = isUnlocked();
			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;

				cannibalizeKinToggle.isOn = selectedGene.jawCellCannibalizeKin;
				cannibalizeMotherToggle.isOn = selectedGene.jawCellCannibalizeMother;
				cannibalizeFatherToggle.isOn = selectedGene.jawCellCannibalizeFather;
				cannibalizeSiblingsToggle.isOn = selectedGene.jawCellCannibalizeSiblings;
				cannibalizeChildrenToggle.isOn = selectedGene.jawCellCannibalizeChildren;

				idleWhenAttachedToggle.isOn = selectedGene.jawCellIdleWhenAttached;

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}
