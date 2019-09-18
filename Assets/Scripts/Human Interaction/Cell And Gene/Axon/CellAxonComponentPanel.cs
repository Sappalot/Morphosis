using UnityEngine;
using UnityEngine.UI;

public class CellAxonComponentPanel : CellComponentPanel {
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
		GeneCellPanel.instance.selectedGene.axonIsEnabled = enabledToggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			MakeCreatureChanged();
		}
		MakeDirty();
	}

	public void OnFromOriginOffsetSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}
		GeneCellPanel.instance.selectedGene.axonFromOriginOffset = fromOriginOffsetSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			MakeCreatureChanged();
		}
		MakeDirty();
	}

	public void OnToggleFromOriginPlus180TextChanged() {
		if (ignoreSliderMoved) {
			return;
		}
		GeneCellPanel.instance.selectedGene.axonIsFromOriginPlus180 = fromOriginPlus180Toggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			MakeCreatureChanged();
		}
		MakeDirty();
	}

	public void OnFromMeOffsetSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}
		GeneCellPanel.instance.selectedGene.axonFromMeOffset = fromMeOffsetSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			MakeCreatureChanged();
		}
		MakeDirty();
	}

	public void OnRelaxContractSliderMoved() {
		if (ignoreSliderMoved) {
			return;
		}
		GeneCellPanel.instance.selectedGene.axonRelaxContract = relaxContractSlider.value;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			MakeCreatureChanged();
		}
		MakeDirty();
	}

	public void OnToggleReverseChanged() {
		if (ignoreSliderMoved) {
			return;
		}
		GeneCellPanel.instance.selectedGene.axonIsReverse = reverseToggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			MakeCreatureChanged();
		}
		MakeDirty();
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

			if (GeneCellPanel.instance.selectedGene != null && CreatureSelectionPanel.instance.hasSoloSelected) {
				ignoreSliderMoved = true;

				enabledToggle.isOn = GeneCellPanel.instance.selectedGene.axonIsEnabled;
				if (GetMode() == PhenoGenoEnum.Genotype) {
					fromOriginOffsetText.text = string.Format("Offset origin -> me: {0:F1}°", GeneCellPanel.instance.selectedGene.axonFromOriginOffset);
				} else if (GetMode() == PhenoGenoEnum.Phenotype) {

					if (GeneCellPanel.instance.selectedGene.axonIsFromOriginPlus180 && CellPanel.instance.selectedCell.flipSide == FlipSideEnum.WhiteBlack) {
						fromOriginOffsetText.text = string.Format("Offset origin -> me: {0:F1} + 180°", GeneCellPanel.instance.selectedGene.axonFromOriginOffset);
					} else {
						fromOriginOffsetText.text = string.Format("Offset origin -> me: {0:F1}°", GeneCellPanel.instance.selectedGene.axonFromOriginOffset);
					}
				}
				fromOriginOffsetSlider.value = GeneCellPanel.instance.selectedGene.axonFromOriginOffset;

				fromOriginPlus180Toggle.isOn = GeneCellPanel.instance.selectedGene.axonIsFromOriginPlus180;

				fromMeOffsetSlider.value = GeneCellPanel.instance.selectedGene.axonFromMeOffset;
				fromMeOffsetText.text = string.Format("Offset me -> muscle: {0:F1}°/cell distance", GeneCellPanel.instance.selectedGene.axonFromMeOffset);

				relaxContractSlider.value = GeneCellPanel.instance.selectedGene.axonRelaxContract;
				relaxContractRelaxContractText.text = string.Format("Relax/Contract offset: {0:F2}", GeneCellPanel.instance.selectedGene.axonRelaxContract);

				reverseToggle.isOn = GeneCellPanel.instance.selectedGene.axonIsReverse; 

				ignoreSliderMoved = false;
			}

			isDirty = false;
		}
	}
}