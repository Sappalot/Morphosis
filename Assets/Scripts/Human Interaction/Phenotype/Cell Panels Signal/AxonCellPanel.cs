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
			if (mode == PhenoGenoEnum.Phenotype || (mode == PhenoGenoEnum.Genotype && GenePanel.instance.selectedGene == null)) {
				enabledToggle.interactable = false;
				enabledText.color = Color.gray;

				fromOriginOffsetText.color = Color.gray;
				fromOriginDeg0Text.color = Color.gray;
				fromOriginDeg360Text.color = Color.gray;
				fromOriginOffsetSlider.interactable = false;
				fromOriginPlus180Toggle.interactable = false;
				fromOriginPlus180Text.color = Color.gray;

				fromMeOffsetText.color = Color.gray;
				fromMeDeg0Text.color = Color.gray;
				fromMeDeg360Text.color = Color.gray;
				fromMeOffsetSlider.interactable = false;

				relaxContractRelaxContractText.color = Color.gray;
				relaxContractRelaxText.color = Color.gray;
				relaxContractContractText.color = Color.gray;
				relaxContractSlider.interactable = false;

				reverseToggle.interactable = false;
				reverseText.color = Color.gray;
			} else if (mode == PhenoGenoEnum.Genotype) {
				enabledToggle.interactable = isUnlocked();
				enabledText.color = Color.black;

				fromOriginOffsetText.color = Color.black;
				fromOriginDeg0Text.color = Color.black;
				fromOriginDeg360Text.color = Color.black;
				fromOriginOffsetSlider.interactable = isUnlocked();
				fromOriginPlus180Toggle.interactable = isUnlocked();
				fromOriginPlus180Text.color = Color.black;

				fromMeOffsetText.color = Color.black;
				fromMeDeg0Text.color = Color.black;
				fromMeDeg360Text.color = Color.black;
				fromMeOffsetSlider.interactable = isUnlocked();

				relaxContractRelaxContractText.color = Color.black;
				relaxContractRelaxText.color = Color.black;
				relaxContractContractText.color = Color.black;
				relaxContractSlider.interactable = isUnlocked();

				reverseToggle.interactable = isUnlocked();
				reverseText.color = Color.black;
			}

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