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

	private bool ignoreSliderMoved;

	public void OnChangedCannibalizeKin() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.jawCellCannibalizeKin = cannibalizeKinToggle.isOn;
		ApplyChange();
	}

	public void OnChangedCannibalizeMother() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.jawCellCannibalizeMother = cannibalizeMotherToggle.isOn;
		ApplyChange();
	}

	public void OnChangedCannibalizeFather() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.jawCellCannibalizeFather = cannibalizeFatherToggle.isOn;
		ApplyChange();
	}

	public void OnChangedCannibalizeSiblings() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.jawCellCannibalizeSiblings = cannibalizeSiblingsToggle.isOn;
		ApplyChange();
	}

	public void OnChangedCannibalizeChildren() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.jawCellCannibalizeChildren = cannibalizeChildrenToggle.isOn;
		ApplyChange();
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

					cannibalizeText.color = Color.gray;

					cannibalizeKinToggle.interactable = false;
					cannibalizeKinText.color = Color.gray;

					cannibalizeMotherToggle.interactable = false;
					cannibalizeMotherText.color = Color.gray;

					cannibalizeFatherToggle.interactable = false;
					cannibalizeFatherText.color = Color.gray;

					cannibalizeSiblingsToggle.interactable = false;
					cannibalizeSiblingsText.color = Color.gray;

					cannibalizeChildrenToggle.interactable = false;
					cannibalizeChildrenText.color = Color.gray;
				}
			} else if (mode == PhenoGenoEnum.Genotype) {
				prayCellCount.text = "Eating on cells: -";
				prayCellCount.color = Color.gray;

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
			}

			if (GenePanel.instance.selectedGene != null) {
				ignoreSliderMoved = true;

				cannibalizeKinToggle.isOn = GenePanel.instance.selectedGene.jawCellCannibalizeKin;
				cannibalizeMotherToggle.isOn = GenePanel.instance.selectedGene.jawCellCannibalizeMother;
				cannibalizeFatherToggle.isOn = GenePanel.instance.selectedGene.jawCellCannibalizeFather;
				cannibalizeSiblingsToggle.isOn = GenePanel.instance.selectedGene.jawCellCannibalizeSiblings;
				cannibalizeChildrenToggle.isOn = GenePanel.instance.selectedGene.jawCellCannibalizeChildren;

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}
