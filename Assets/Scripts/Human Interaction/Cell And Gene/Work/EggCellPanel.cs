using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggCellPanel : CellAndGeneComponentPanel {
	public Button fertilizeButton;

	public Text detatchHeadingText;

	public Toggle detatchSizeToggle;
	public Toggle detatchEnergyToggle;

	public Text detatchSizeSliderTextPercentage;
	public Text detatchSizeSliderTextCellCount;
	public Slider detatchSizeSlider;

	public Text detatchEnergySliderText;
	public Slider detatchEnergySlider;

	public LogicBoxPanel fertilizeLogicBoxPanel;
	public EnergySensorPanel fertilizeEnergySensorPanel;

	public override void Initialize(PhenoGenoEnum mode) {
		fertilizeLogicBoxPanel.Initialize(mode, SignalUnitEnum.WorkLogicBoxA, true);
		fertilizeEnergySensorPanel.Initialize(mode, SignalUnitEnum.WorkSensorA, true);

		ignoreSliderMoved = true; 
		detatchSizeSlider.minValue = GlobalSettings.instance.phenotype.eggCellDetatchSizeThresholdMin;
		detatchSizeSlider.maxValue = GlobalSettings.instance.phenotype.eggCellDetatchSizeThresholdMax;

		detatchEnergySlider.minValue = GlobalSettings.instance.phenotype.eggCellDetatchEnergyThresholdMin;
		detatchEnergySlider.maxValue = GlobalSettings.instance.phenotype.eggCellDetatchEnergyThresholdMax;

		ignoreSliderMoved = false;

		MakeDirty();

		base.Initialize(mode);
	}

	public override void MakeDirty() {
		base.MakeDirty();
		fertilizeLogicBoxPanel.MakeDirty();
		fertilizeEnergySensorPanel.MakeDirty();
	}


	public override List<GeneLogicBoxInput> GetAllGeneGeneLogicBoxInputs() {
		return fertilizeLogicBoxPanel.GetAllGeneGeneLogicBoxInputs();
	}

	public void OnClickFertilize() {
		if (CreatureSelectionPanel.instance.hasSoloSelected && GetMode() == PhenoGenoEnum.Phenotype) {
			World.instance.life.FertilizeCreature(CellPanel.instance.selectedCell, true, World.instance.worldTicks, true);
		}
	}

	public void OnDetatchModeToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.eggCellDetatchMode = detatchSizeToggle.isOn ? ChildDetatchModeEnum.Size : ChildDetatchModeEnum.Energy;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		MakeDirty();
	}

	public void OnDetatchSizeSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.eggCellDetatchSizeThreshold = detatchSizeSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		MakeDirty();
	}

	public void OnDetatchEnergySliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.eggCellDetatchEnergyThreshold = detatchEnergySlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					detatchSizeToggle.interactable = false;
					detatchEnergyToggle.interactable = false;
					detatchSizeSlider.interactable = false;

					detatchEnergySlider.interactable = false;

					fertilizeButton.gameObject.SetActive(true);
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				detatchSizeToggle.interactable = IsUnlocked();
				detatchEnergyToggle.interactable = IsUnlocked();
				detatchSizeSlider.interactable = IsUnlocked();
				detatchEnergySlider.interactable = IsUnlocked();

				fertilizeButton.gameObject.SetActive(false);
			}
			footerPanel.SetProductionEffectText(0f, GlobalSettings.instance.phenotype.eggCellEffectCost);

			if (selectedGene != null) {
				ignoreSliderMoved = true;

				detatchSizeToggle.isOn = selectedGene.eggCellDetatchMode == ChildDetatchModeEnum.Size;
				detatchEnergyToggle.isOn = selectedGene.eggCellDetatchMode == ChildDetatchModeEnum.Energy;

				detatchSizeSlider.value = selectedGene.eggCellDetatchSizeThreshold;
				detatchSizeSliderTextPercentage.text = string.Format("Body size ≥ {0:F1}%", selectedGene.eggCellDetatchSizeThreshold * 100f);
				int cellCount = CreatureSelectionPanel.instance.soloSelected.genotype.geneCellCount;
				detatchSizeSliderTextCellCount.text = string.Format("{0:F0} of {1:F0} cells", Mathf.Clamp(Mathf.RoundToInt(selectedGene.eggCellDetatchSizeThreshold * cellCount), 1, cellCount), cellCount);

				detatchEnergySlider.value = selectedGene.eggCellDetatchEnergyThreshold;
				detatchEnergySliderText.text = string.Format("Can't grow more and cell energy ≥ {0:F1}%", selectedGene.eggCellDetatchEnergyThreshold * 100f);

				ignoreSliderMoved = false;
			}

			fertilizeLogicBoxPanel.outputText = "Fertilize asexually";

			isDirty = false;
		}
	}
}