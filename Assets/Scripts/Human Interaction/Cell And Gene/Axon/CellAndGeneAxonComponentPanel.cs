using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellAndGeneAxonComponentPanel : CellAndGeneSignalUnitPanel {
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
	public Dropdown dropdown11;
	public Dropdown dropdown10;
	public Dropdown dropdown01;
	public Dropdown dropdown00;

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

	private void Update() {
		if (isDirty) {

			bool interractable = GetMode() == PhenoGenoEnum.Genotype && IsUnlocked();

			enabledToggle.interactable = interractable;

			fromOriginOffsetSlider.interactable = interractable;
			fromOriginPlus180Toggle.interactable = interractable;

			fromMeOffsetSlider.interactable = interractable;

			relaxContractSlider.interactable = interractable;

			reverseToggle.interactable = interractable;

			if (GenePanel.instance.selectedGene != null && CreatureSelectionPanel.instance.hasSoloSelected) {
				ignoreSliderMoved = true;

				enabledToggle.isOn = GenePanel.instance.selectedGene.axon.axonIsEnabled;
				if (GetMode() == PhenoGenoEnum.Genotype) {
					fromOriginOffsetText.text = string.Format("Offset origin -> me: {0:F1}°", GenePanel.instance.selectedGene.axon.axonFromOriginOffset);
					postInputBoxLeft.color = Color.yellow; ////ColorScheme.instance.signalOff;
					
				} else if (GetMode() == PhenoGenoEnum.Phenotype) {

					if (GenePanel.instance.selectedGene.axon.axonIsFromOriginPlus180 && CellPanel.instance.selectedCell.flipSide == FlipSideEnum.WhiteBlack) {
						fromOriginOffsetText.text = string.Format("Offset origin -> me: {0:F1} + 180°", GenePanel.instance.selectedGene.axon.axonFromOriginOffset);
					} else {
						fromOriginOffsetText.text = string.Format("Offset origin -> me: {0:F1}°", GenePanel.instance.selectedGene.axon.axonFromOriginOffset);
					}

					postInputBoxLeft.color = Axon.GetInputResult(inputLeftPanel.affectedGeneAxonInput, selectedCell) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				}
				fromOriginOffsetSlider.value = GenePanel.instance.selectedGene.axon.axonFromOriginOffset;

				fromOriginPlus180Toggle.isOn = GenePanel.instance.selectedGene.axon.axonIsFromOriginPlus180;

				fromMeOffsetSlider.value = GenePanel.instance.selectedGene.axon.axonFromMeOffset;
				fromMeOffsetText.text = string.Format("Offset me -> muscle: {0:F1}°/cell distance", GenePanel.instance.selectedGene.axon.axonFromMeOffset);

				relaxContractSlider.value = GenePanel.instance.selectedGene.axon.axonRelaxContract;
				relaxContractRelaxContractText.text = string.Format("Relax/Contract offset: {0:F2}", GenePanel.instance.selectedGene.axon.axonRelaxContract);

				reverseToggle.isOn = GenePanel.instance.selectedGene.axon.axonIsReverse;

				// Switcher
				inputLeftPanel.MakeDirty();
				inputRightPanel.MakeDirty();

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}