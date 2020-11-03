using UnityEngine;
using UnityEngine.UI;

public class SurroundingSensorCreatureCellFovCovPanel : SurroundingSensorChannelSensorPanel {

	public Text currentValueLabel;
	public Text thresholdSliderLabel;
	public Slider thresholdSlider;

	public Toggle seeEggToggle;
	public Toggle seeFungalToggle;
	public Toggle seeJawThreatToggle;
	public Toggle seeJawHarmlessToggle;
	public Toggle seeLeafToggle;
	public Toggle seeMuscleToggle;
	public Toggle seeRootToggle;
	public Toggle seeShellToggle;
	public Toggle seeVeinToggle;

	override public void Initialize(PhenoGenoEnum mode, CellAndGenePanel cellAndGenePanel, SurroundingSensorPanel motherPanel) {
		base.Initialize(mode, cellAndGenePanel, motherPanel);
		MakeDirty();
	}

	public void OnThresholdSliderMoved() {
		if (ignoreHumanInput || mode == PhenoGenoEnum.Phenotype) {
			return;
		}

		geneSurroundingSensorChannel.threshold = thresholdSlider.value;
		OnGenomeChanged();
	}

	public void OnToggleSeeEggChanged() {
		if (ignoreHumanInput) {
			return;
		}
		geneSurroundingSensorChannel.seeEgg = seeEggToggle.isOn;
		OnGenomeChanged();
	}

	public void OnToggleSeeFungalChanged() {
		if (ignoreHumanInput) {
			return;
		}
		geneSurroundingSensorChannel.seeFungal = seeFungalToggle.isOn;
		OnGenomeChanged();
	}

	public void OnToggleSeeJawThreatChanged() {
		if (ignoreHumanInput) {
			return;
		}
		geneSurroundingSensorChannel.seeJawThreat = seeJawThreatToggle.isOn;
		OnGenomeChanged();
	}

	public void OnToggleSeeJawHarmlassChanged() {
		if (ignoreHumanInput) {
			return;
		}
		geneSurroundingSensorChannel.seeJawHarmless = seeJawHarmlessToggle.isOn;
		OnGenomeChanged();
	}

	public void OnToggleSeeLeafChanged() {
		if (ignoreHumanInput) {
			return;
		}
		geneSurroundingSensorChannel.seeLeaf = seeLeafToggle.isOn;
		OnGenomeChanged();
	}

	public void OnToggleSeeMuscleChanged() {
		if (ignoreHumanInput) {
			return;
		}
		geneSurroundingSensorChannel.seeMuscle = seeMuscleToggle.isOn;
		OnGenomeChanged();
	}

	public void OnToggleSeeRootChanged() {
		if (ignoreHumanInput) {
			return;
		}
		geneSurroundingSensorChannel.seeRoot = seeRootToggle.isOn;
		OnGenomeChanged();
	}

	public void OnToggleSeeShellChanged() {
		if (ignoreHumanInput) {
			return;
		}
		geneSurroundingSensorChannel.seeShell = seeShellToggle.isOn;
		OnGenomeChanged();
	}

	public void OnToggleSeeVeinChanged() {
		if (ignoreHumanInput) {
			return;
		}
		geneSurroundingSensorChannel.seeVein = seeVeinToggle.isOn;
		OnGenomeChanged();
	}

	private void Update() {
		if (isDirty) {

			if (!CreatureSelectionPanel.instance.hasSoloSelected) {
				return;
			}

			ignoreHumanInput = true;
			if (mode == PhenoGenoEnum.Phenotype) {
				if (!motherPanel.isGhost && motherPanel.selectedCell != null && motherPanel.selectedCell.surroundingSensor != null && motherPanel.selectedCell.surroundingSensor.rootnessEnum == RootnessEnum.Rooted) {
					currentValueLabel.text = string.Format("Coverage: {0:F1} % ", motherPanel.selectedCell.surroundingSensor.CellsByTypeFovCov(motherPanel.viewedChannel) * 100f);
				} else {
					currentValueLabel.text = string.Format("Coverage: -");
				}
			} else /* Genotype */ {
				currentValueLabel.text = string.Format("Coverage: -");
			}

			float threshold = ((GeneSurroundingSensorChannelCreatureCellFovCov)cellAndGenePanel.gene.surroundingSensor.GeneSensorAtChannelByType(motherPanel.viewedChannel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov)).threshold;
			thresholdSliderLabel.text = string.Format("On when coverage > {0:F0} %", threshold * 100f);
			thresholdSlider.value = threshold;
			thresholdSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

			seeEggToggle.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;
			seeFungalToggle.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;
			seeJawThreatToggle.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;
			seeJawHarmlessToggle.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;
			seeLeafToggle.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;
			seeMuscleToggle.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;
			seeRootToggle.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;
			seeShellToggle.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;
			seeVeinToggle.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

			seeEggToggle.isOn = geneSurroundingSensorChannel.seeEgg;
			seeFungalToggle.isOn = geneSurroundingSensorChannel.seeFungal;
			seeJawThreatToggle.isOn = geneSurroundingSensorChannel.seeJawThreat;
			seeJawHarmlessToggle.isOn = geneSurroundingSensorChannel.seeJawHarmless;
			seeLeafToggle.isOn = geneSurroundingSensorChannel.seeLeaf;
			seeMuscleToggle.isOn = geneSurroundingSensorChannel.seeMuscle;
			seeRootToggle.isOn = geneSurroundingSensorChannel.seeRoot;
			seeShellToggle.isOn = geneSurroundingSensorChannel.seeShell;
			seeVeinToggle.isOn = geneSurroundingSensorChannel.seeVein;

			ignoreHumanInput = false;

			isDirty = false;
		}
	}
	private GeneSurroundingSensorChannelCreatureCellFovCov geneSurroundingSensorChannel {
		get {
			return (GeneSurroundingSensorChannelCreatureCellFovCov)cellAndGenePanel.gene.surroundingSensor.GeneSensorAtChannelByType(motherPanel.viewedChannel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov);
		}
	}
}