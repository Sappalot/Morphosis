using UnityEngine;
using UnityEngine.UI;

public class JawCellPanel : CellAndGeneComponentPanel {
	public Text productionEffectText;

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

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					footerPanel.SetProductionEffectText(selectedCell.effectProductionInternalUp, GlobalSettings.instance.phenotype.jawCellEffectCost);
					prayCellCount.text = "Pray count: " + (CellPanel.instance.selectedCell as JawCell).prayCount;
					cannibalizeKinToggle.interactable = false;
					cannibalizeMotherToggle.interactable = false;
					cannibalizeFatherToggle.interactable = false;
					cannibalizeSiblingsToggle.interactable = false;
					cannibalizeChildrenToggle.interactable = false;
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				footerPanel.SetProductionEffectText(string.Format("Production Effect: [pray count (0...6)] * {0:F2} - {1:F2} W <color=#808080ff>(@ normal pray armor)</color>", GlobalSettings.instance.phenotype.jawCellEatEffectAtSpeed.Evaluate(20f) * GlobalSettings.instance.phenotype.jawCellEatEarnFactor, GlobalSettings.instance.phenotype.jawCellEffectCost));
				prayCellCount.text = "Pray Count: -";
				cannibalizeKinToggle.interactable = IsUnlocked();
				cannibalizeMotherToggle.interactable = IsUnlocked();
				cannibalizeFatherToggle.interactable = IsUnlocked();
				cannibalizeSiblingsToggle.interactable = IsUnlocked();
				cannibalizeChildrenToggle.interactable = IsUnlocked();
			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;

				cannibalizeKinToggle.isOn = selectedGene.jawCellCannibalizeKin;
				cannibalizeMotherToggle.isOn = selectedGene.jawCellCannibalizeMother;
				cannibalizeFatherToggle.isOn = selectedGene.jawCellCannibalizeFather;
				cannibalizeSiblingsToggle.isOn = selectedGene.jawCellCannibalizeSiblings;
				cannibalizeChildrenToggle.isOn = selectedGene.jawCellCannibalizeChildren;

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}
