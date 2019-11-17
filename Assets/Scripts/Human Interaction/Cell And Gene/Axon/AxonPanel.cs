using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AxonPanel : SensorPanel {
	public Toggle enabledToggle;

	// ...Pulse...
	public Text fromOriginOffsetText;
	public Text fromOriginDeg0Text;
	public Text fromOriginDeg360Text; 
	public Slider fromOriginOffsetSlider;
	public Toggle fromOriginPlus180Toggle;
	public Text fromOriginPlus180Text;

	public Text fromMeOffsetText;
	public Text fromMeDeg0Text;
	public Text fromMeDeg360Text;
	public Slider fromMeOffsetSlider;

	public Text relaxContractRelaxContractText;
	public Text relaxContractRelaxText;
	public Text relaxContractContractText;
	public Slider relaxContractSlider;

	public Toggle reverseToggle;
	public Text reverseText;
	// ^ Pulse ^

	// ...Switch...
	public Dropdown dropdown3; // 11
	public Dropdown dropdown2; // 10
	public Dropdown dropdown1; // 01
	public Dropdown dropdown0; // 00

	public Image dropdownBackgroundShow3;
	public Image dropdownBackgroundList3;
	public Image dropdownBackgroundShow2;
	public Image dropdownBackgroundList2;
	public Image dropdownBackgroundShow1;
	public Image dropdownBackgroundList1;
	public Image dropdownBackgroundShow0;
	public Image dropdownBackgroundList0;

	public Image combinationImage3; // 11
	public Image combinationImage2; // 10
	public Image combinationImage1; // 01
	public Image combinationImage0; // 00

	public AxonInputPanel inputLeftPanel;
	public AxonInputPanel inputRightPanel;
	public Image postInputBoxLeft;
	public Image postInputBoxRight;

	//  ^ Switch ^

	public override void Initialize(PhenoGenoEnum mode) {
		base.Initialize(mode, SignalUnitEnum.Axon);

		ignoreSliderMoved = true;
		fromOriginOffsetSlider.minValue = 0f;
		fromOriginOffsetSlider.maxValue = 360f;

		fromMeOffsetSlider.minValue = 0f;
		fromMeOffsetSlider.maxValue = 360f;

		relaxContractSlider.minValue = -1f;
		relaxContractSlider.maxValue = 1f;

		inputLeftPanel.Initialize(mode, 0, this); // left
		inputRightPanel.Initialize(mode, 1, this); // right

		dropdownBackgroundShow3.color = ColorScheme.instance.selectedChanged;
		dropdownBackgroundList3.color = ColorScheme.instance.selectedChanged;
		dropdownBackgroundShow2.color = ColorScheme.instance.selectedChanged;
		dropdownBackgroundList2.color = ColorScheme.instance.selectedChanged;
		dropdownBackgroundShow1.color = ColorScheme.instance.selectedChanged;
		dropdownBackgroundList1.color = ColorScheme.instance.selectedChanged;
		dropdownBackgroundShow0.color = ColorScheme.instance.selectedChanged;
		dropdownBackgroundList0.color = ColorScheme.instance.selectedChanged;

		ignoreSliderMoved = false;
	}

	public override List<IGeneInput> GetAllGeneInputs() {
		List<IGeneInput> arrows = new List<IGeneInput>();
		arrows.Add(inputLeftPanel.affectedGeneAxonInput);
		arrows.Add(inputRightPanel.affectedGeneAxonInput);
		return arrows;
	}

	public void OnToggleEnabledChanged() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.axon.axonIsEnabled = enabledToggle.isOn;
		OnGenomeChanged(true);
	}

	public void OnFromOriginOffsetSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.axon.axonFromOriginOffset = fromOriginOffsetSlider.value;
		OnGenomeChanged(true);
	}

	public void OnToggleFromOriginPlus180TextChanged() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.axon.axonIsFromOriginPlus180 = fromOriginPlus180Toggle.isOn;
		OnGenomeChanged(true);
	}

	public void OnFromMeOffsetSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.axon.axonFromMeOffset = fromMeOffsetSlider.value;
		OnGenomeChanged(true);
	}

	public void OnRelaxContractSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.axon.axonRelaxContract = relaxContractSlider.value;
		OnGenomeChanged(true);
	}

	public void OnToggleReverseChanged() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.axon.axonIsReverse = reverseToggle.isOn;
		OnGenomeChanged(true);
	}

	public void OnDropdownCombination3Changed() {
		if (ignoreSliderMoved) {
			return;
		}
		selectedGene.axon.pulseProgram3 = dropdown3.value; // 0 = relax, 1 = A ....
		OnGenomeChanged(false);
	}

	public void OnDropdownCombination2Changed() {
		if (ignoreSliderMoved) {
			return;
		}
		selectedGene.axon.pulseProgram2 = dropdown2.value; // 0 = relax, 1 = A ....
		OnGenomeChanged(false);
	}

	public void OnDropdownCombination1Changed() {
		if (ignoreSliderMoved) {
			return;
		}
		selectedGene.axon.pulseProgram1 = dropdown1.value; // 0 = relax, 1 = A ....
		OnGenomeChanged(false);
	}

	public void OnDropdownCombination0Changed() {
		if (ignoreSliderMoved) {
			return;
		}
		selectedGene.axon.pulseProgram0 = dropdown0.value; // 0 = relax, 1 = A ....
		OnGenomeChanged(false);
	}

	public void MarkAsNewForge() {
		CreatureSelectionPanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
	}

	public void UpdateConnections() {
		selectedGene.axon.UpdateConnections();
	}

	public override void Update() {
		if (isDirty) {
			base.Update();
			
			if (selectedGene == null) {
				isDirty = false;
				return;
			}

			bool interractable = GetMode() == PhenoGenoEnum.Genotype && IsUnlocked();

			enabledToggle.interactable = interractable;

			fromOriginOffsetSlider.interactable = interractable;
			fromOriginPlus180Toggle.interactable = interractable;

			fromMeOffsetSlider.interactable = interractable;

			relaxContractSlider.interactable = interractable;

			reverseToggle.interactable = interractable;

			dropdown3.interactable = (mode == PhenoGenoEnum.Genotype);
			dropdown2.interactable = (mode == PhenoGenoEnum.Genotype);
			dropdown1.interactable = (mode == PhenoGenoEnum.Genotype);
			dropdown0.interactable = (mode == PhenoGenoEnum.Genotype);

			dropdown3.value = selectedGene.axon.pulseProgram3;
			dropdown2.value = selectedGene.axon.pulseProgram2;
			dropdown1.value = selectedGene.axon.pulseProgram1;
			dropdown0.value = selectedGene.axon.pulseProgram0;

			if (GenePanel.instance.selectedGene != null && CreatureSelectionPanel.instance.hasSoloSelected) {
				ignoreSliderMoved = true;

				enabledToggle.isOn = GenePanel.instance.selectedGene.axon.axonIsEnabled;
				if (GetMode() == PhenoGenoEnum.Genotype) {
					fromOriginOffsetText.text = string.Format("Offset origin -> me: {0:F1}°", GenePanel.instance.selectedGene.axon.axonFromOriginOffset);
					
					postInputBoxLeft.color = ColorScheme.instance.signalOff;
					postInputBoxRight.color = ColorScheme.instance.signalOff;

					combinationImage3.color = ColorScheme.instance.signalOff; // 11
					combinationImage2.color = ColorScheme.instance.signalOff; // 10
					combinationImage1.color = ColorScheme.instance.signalOff; // 01
					combinationImage0.color = ColorScheme.instance.signalOff; // 00

				} else if (GetMode() == PhenoGenoEnum.Phenotype) {

					if (GenePanel.instance.selectedGene.axon.axonIsFromOriginPlus180 && CellPanel.instance.selectedCell.flipSide == FlipSideEnum.WhiteBlack) {
						fromOriginOffsetText.text = string.Format("Offset origin -> me: {0:F1} + 180°", GenePanel.instance.selectedGene.axon.axonFromOriginOffset);
					} else {
						fromOriginOffsetText.text = string.Format("Offset origin -> me: {0:F1}°", GenePanel.instance.selectedGene.axon.axonFromOriginOffset);
					}

					combinationImage3.color = selectedCell.axon.HasSignalAtCombination(3) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
					combinationImage2.color = selectedCell.axon.HasSignalAtCombination(2) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
					combinationImage1.color = selectedCell.axon.HasSignalAtCombination(1) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
					combinationImage0.color = selectedCell.axon.HasSignalAtCombination(0) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;

					postInputBoxLeft.color = selectedCell.axon.HasSignalPostInputValve(inputLeftPanel.affectedGeneAxonInput) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
					postInputBoxRight.color = selectedCell.axon.HasSignalPostInputValve(inputRightPanel.affectedGeneAxonInput) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				}
				fromOriginOffsetSlider.value = GenePanel.instance.selectedGene.axon.axonFromOriginOffset;

				fromOriginPlus180Toggle.isOn = GenePanel.instance.selectedGene.axon.axonIsFromOriginPlus180;

				fromMeOffsetSlider.value = GenePanel.instance.selectedGene.axon.axonFromMeOffset;
				fromMeOffsetText.text = string.Format("Offset me -> muscle: {0:F1}°/cell distance", GenePanel.instance.selectedGene.axon.axonFromMeOffset);

				relaxContractSlider.value = GenePanel.instance.selectedGene.axon.axonRelaxContract;
				relaxContractRelaxContractText.text = string.Format("Relax/Contract offset: {0:F2}", GenePanel.instance.selectedGene.axon.axonRelaxContract);

				reverseToggle.isOn = GenePanel.instance.selectedGene.axon.axonIsReverse;

				// Output

				// Switcher
				inputLeftPanel.MakeDirty();
				inputRightPanel.MakeDirty();

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}