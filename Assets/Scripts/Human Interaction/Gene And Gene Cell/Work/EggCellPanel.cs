using UnityEngine;
using UnityEngine.UI;

public class EggCellPanel : CellComponentPanel {
	public Text productionEffectText;

	public HibernatePanel hibernatePanel;

	public Text fertilizeHeadingText;
	public Text fertilizeSliderText;
	public Slider fertilizeSlider;

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
	public EffectSensorPanel effectSensorPanel;

	public override void Initialize(PhenoGenoEnum mode) {
		hibernatePanel.SetMode(mode);
		
		fertilizeLogicBoxPanel.Initialize(mode);
		fertilizeEnergySensorPanel.Initialize(mode, SignalUnitEnum.WorkSensorA);
		effectSensorPanel.Initialize(mode, SignalUnitEnum.EffectSensor);

		ignoreSliderMoved = true;
		fertilizeSlider.minValue = GlobalSettings.instance.phenotype.eggCellFertilizeThresholdMin;
		fertilizeSlider.maxValue = GlobalSettings.instance.phenotype.eggCellFertilizeThresholdMax;

		detatchSizeSlider.minValue = GlobalSettings.instance.phenotype.eggCellDetatchSizeThresholdMin;
		detatchSizeSlider.maxValue = GlobalSettings.instance.phenotype.eggCellDetatchSizeThresholdMax;

		detatchEnergySlider.minValue = GlobalSettings.instance.phenotype.eggCellDetatchEnergyThresholdMin;
		detatchEnergySlider.maxValue = GlobalSettings.instance.phenotype.eggCellDetatchEnergyThresholdMax;

		ignoreSliderMoved = false;

		MakeDirty();

		base.Initialize(mode);
	}

	public void OnClickFertilize() {
		if (CreatureSelectionPanel.instance.hasSoloSelected && GetMode() == PhenoGenoEnum.Phenotype) {
			World.instance.life.FertilizeCreature(CellPanel.instance.selectedCell, true, World.instance.worldTicks, true);
		}
	}

	public void OnFertilizeSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.eggCellFertilizeThreshold = fertilizeSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		MakeDirty();
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
					productionEffectText.text = productionEffectPhenotypeString;

					fertilizeSlider.interactable = false;
					detatchSizeToggle.interactable = false;
					detatchEnergyToggle.interactable = false;
					detatchSizeSlider.interactable = false;

					detatchEnergySlider.interactable = false;

					fertilizeButton.gameObject.SetActive(true);
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				productionEffectText.text = string.Format("Production Effect: 0.00 - {0:F2} W", GlobalSettings.instance.phenotype.eggCellEffectCost);

				fertilizeSlider.interactable = IsUnlocked();
				detatchSizeToggle.interactable = IsUnlocked();
				detatchEnergyToggle.interactable = IsUnlocked();
				detatchSizeSlider.interactable = IsUnlocked();
				detatchEnergySlider.interactable = IsUnlocked();

				fertilizeButton.gameObject.SetActive(false);
			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;

				fertilizeSlider.value = selectedGene.eggCellFertilizeThreshold;
				fertilizeSliderText.text = string.Format("Egg Energy ≥ {0:F1}%", selectedGene.eggCellFertilizeThreshold * 100f);
				
				detatchSizeToggle.isOn = selectedGene.eggCellDetatchMode == ChildDetatchModeEnum.Size;
				detatchEnergyToggle.isOn = selectedGene.eggCellDetatchMode == ChildDetatchModeEnum.Energy;

				detatchSizeSlider.value = selectedGene.eggCellDetatchSizeThreshold;
				detatchSizeSliderTextPercentage.text = string.Format("Body size ≥ {0:F1}%", selectedGene.eggCellDetatchSizeThreshold * 100f);
				int cellCount = CreatureSelectionPanel.instance.soloSelected.genotype.geneCellCount;
				detatchSizeSliderTextCellCount.text = string.Format("{0:F0} of {1:F0} cells", Mathf.Clamp(Mathf.RoundToInt(selectedGene.eggCellDetatchSizeThreshold * cellCount), 1, cellCount), cellCount);

				detatchEnergySlider.value = selectedGene.eggCellDetatchEnergyThreshold;
				detatchEnergySliderText.text = string.Format("Can't grow more and cell energy ≥ {0:F1}%", selectedGene.eggCellDetatchEnergyThreshold * 100f);

				fertilizeLogicBoxPanel.ConnectToGeneLogic(selectedGene.eggCellFertilizeLogic); // We need the connection human interface ==> gene logic (in order to update gene from interface (input) and interface from gene (output))
				fertilizeEnergySensorPanel.ConnectToGeneLogic(selectedGene.eggCellFertilizeEnergySensor);
				effectSensorPanel.ConnectToGeneLogic(selectedGene.effectSensor);

				ignoreSliderMoved = false;
			}

			// Subpanels
			hibernatePanel.MakeDirty(); // to be replaced with a logic panel (also make it so that we have an wakeup from hibernation time)

			
			fertilizeLogicBoxPanel.outputText = "Fertilize";
			fertilizeLogicBoxPanel.MakeDirty();
			fertilizeEnergySensorPanel.MakeDirty();
			effectSensorPanel.MakeDirty();

			isDirty = false;
		}
	}
}