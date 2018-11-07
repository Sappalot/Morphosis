using UnityEngine;
using UnityEngine.UI;

public class AxonCellPanel : MetabolismCellPanel {
	public Toggle enabledToggle;
	public Text enabledText;

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

	// TODO: Graph

	private void Awake() {
		ignoreSliderMoved = true;
		fromOriginOffsetSlider.minValue = 0f;
		fromOriginOffsetSlider.maxValue = 360f;

		fromMeOffsetSlider.minValue = 0f;
		fromMeOffsetSlider.maxValue = 360f;

		relaxContractSlider.minValue = -1f;
		relaxContractSlider.maxValue = 1f;

		ignoreSliderMoved = false;
	}

	public void OnToggleEnabledChanged() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.axonIsEnabled = enabledToggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	public void OnFromOriginOffsetSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.axonFromOriginOffset = fromOriginOffsetSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	public void OnToggleFromOriginPlus180TextChanged() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.axonIsFromOriginPlus180 = fromOriginPlus180Toggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	public void OnFromMeOffsetSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.axonFromMeOffset = fromMeOffsetSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	public void OnRelaxContractSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.axonRelaxContract = relaxContractSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	public void OnToggleReverseChanged() {
		if (ignoreSliderMoved) {
			return;
		}
		GenePanel.instance.selectedGene.axonIsReverse = reverseToggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			OnChanged();
		}
		MakeDirty();
	}

	private void Update() {
		if (isDirty) {

			Color textColor = mode == PhenoGenoEnum.Phenotype ? ColorScheme.instance.grayedOutGenotype : Color.black;
			bool interractable = mode == PhenoGenoEnum.Genotype && isUnlocked();

			enabledToggle.interactable = interractable;
			enabledText.color = textColor;

			fromOriginOffsetText.color = textColor;
			fromOriginDeg0Text.color = textColor;
			fromOriginDeg360Text.color = textColor;
			fromOriginOffsetSlider.interactable = interractable;
			fromOriginPlus180Toggle.interactable = interractable;
			fromOriginPlus180Text.color = textColor;

			fromMeOffsetText.color = textColor;
			fromMeDeg0Text.color = textColor;
			fromMeDeg360Text.color = textColor;
			fromMeOffsetSlider.interactable = interractable;

			relaxContractRelaxContractText.color = textColor;
			relaxContractRelaxText.color = textColor;
			relaxContractContractText.color = textColor;
			relaxContractSlider.interactable = interractable;

			reverseToggle.interactable = interractable;
			reverseText.color = textColor;

			if (GenePanel.instance.selectedGene != null) {
				ignoreSliderMoved = true;

				enabledToggle.isOn = GenePanel.instance.selectedGene.axonIsEnabled;
				if (mode == PhenoGenoEnum.Genotype) {
					fromOriginOffsetText.text = string.Format("Offset origin -> me: {0:F1}°", GenePanel.instance.selectedGene.axonFromOriginOffset);
				} else if (mode == PhenoGenoEnum.Phenotype) {
					if (GenePanel.instance.selectedGene.axonIsFromOriginPlus180 && CellPanel.instance.selectedCell.flipSide == FlipSideEnum.WhiteBlack) {
						fromOriginOffsetText.text = string.Format("Offset origin -> me: {0:F1} + 180°", GenePanel.instance.selectedGene.axonFromOriginOffset);
					} else {
						fromOriginOffsetText.text = string.Format("Offset origin -> me: {0:F1}°", GenePanel.instance.selectedGene.axonFromOriginOffset);
					}
				}
				fromOriginOffsetSlider.value = GenePanel.instance.selectedGene.axonFromOriginOffset;

				fromOriginPlus180Toggle.isOn = GenePanel.instance.selectedGene.axonIsFromOriginPlus180;

				fromMeOffsetSlider.value = GenePanel.instance.selectedGene.axonFromMeOffset;
				fromMeOffsetText.text = string.Format("Offset me -> muscle: {0:F1}°/cell distance", GenePanel.instance.selectedGene.axonFromMeOffset);

				relaxContractSlider.value = GenePanel.instance.selectedGene.axonRelaxContract;
				relaxContractRelaxContractText.text = string.Format("Relax/Contract offset: {0:F2}", GenePanel.instance.selectedGene.axonRelaxContract);

				reverseToggle.isOn = GenePanel.instance.selectedGene.axonIsReverse;

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}