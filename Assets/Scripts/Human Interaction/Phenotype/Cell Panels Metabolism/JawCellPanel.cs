using UnityEngine;
using UnityEngine.UI;

public class JawCellPanel : MetabolismCellPanel {
	public Text productionEffectText;

	public HibernatePanel hibernatePanel;

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

	public override void SetMode(PhenoGenoEnum mode) {
		hibernatePanel.SetMode(mode);
		base.SetMode(mode);
	}

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
					productionEffectText.text = productionEffectPhenotypeString;

					prayCellCount.text = "Pray count: " + (CellPanel.instance.selectedCell as JawCell).prayCount;
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
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				productionEffectText.text = string.Format("Production Effect: [pray count (0...6)] * {0:F2} - {1:F2} W <color=#808080ff>(@ normal pray armor)</color>", GlobalSettings.instance.phenotype.jawCellEatEffect * GlobalSettings.instance.phenotype.jawCellEatEarnFactor, GlobalSettings.instance.phenotype.jawCellEffectCost);

				prayCellCount.text = "Pray Count: -";
				prayCellCount.color = ColorScheme.instance.grayedOutPhenotype;

				cannibalizeText.color = IsUnlockedColor();

				cannibalizeKinToggle.interactable = IsUnlocked();
				cannibalizeKinText.color = IsUnlockedColor();

				cannibalizeMotherToggle.interactable = IsUnlocked();
				cannibalizeMotherText.color = IsUnlockedColor();

				cannibalizeFatherToggle.interactable = IsUnlocked();
				cannibalizeFatherText.color = IsUnlockedColor();

				cannibalizeSiblingsToggle.interactable = IsUnlocked();
				cannibalizeSiblingsText.color = IsUnlockedColor();

				cannibalizeChildrenToggle.interactable = IsUnlocked();
				cannibalizeChildrenText.color = IsUnlockedColor();
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

			// Subpanels
			hibernatePanel.MakeDirty();

			isDirty = false;
		}
	}
}
