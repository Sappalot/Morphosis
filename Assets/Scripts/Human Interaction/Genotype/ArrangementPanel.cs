using UnityEngine;
using UnityEngine.UI;

public class ArrangementPanel : MonoBehaviour {
	public GameObject enableToggle;
	public GameObject circles;
	[HideInInspector]
	public GenePanel genePanel;

	public Image grayOut;
	public GameObject arrangementButtons;

	public GameObject angleButtons;
	public GameObject referenceCountButtons;
	public GameObject pairsToggle;
	public GameObject flipOppositeSameButtons;
	public GameObject flipWhiteBlackToArrowButtons;
	public GameObject gapSizeButtons;
	public GameObject referenceSideButtions;

	public Text arrangementTypeText;

	public Image centerCircleFlipBlackWhiteImage;
	public Image centerCircleFlipWhiteBlack;

	public Image flipSameButtonImage;
	public Image flipOppositeButtonImage;
	public Image flipBlackToArrowButtonImage;
	public Image flipWhiteToArrowButtonImage;

	public Toggle togglePair;

	public Image referenceSideBlack;
	public Image referenceSideWhite;

	public RectTransform arrowTransform;

	public ReferenceGraphics[] referenceGraphics = new ReferenceGraphics[6];

	private bool isMouseHoverng;

	private Arrangement m_arrangement;
	public Arrangement arrangement {
		get {
			return m_arrangement;
		}
		set {
			m_arrangement = value;
			isDirty = true;
		}
	}

	public bool isValid {
		get {
			return m_arrangement != null;
		}
	}

	private void Awake() {
		arrangementButtons.SetActive(false);
	}

	public bool isEnabled {
		get {
			return arrangement.isEnabled;
		}
	}

	public void OnClickEnabledToggle(bool value) {
		bool wasChanged = arrangement.isEnabled != value; //To prevent dirtymarking when just setting startup value
		arrangement.isEnabled = value;
		if (wasChanged) {
			MakeAllGenomeStuffDirty();
		}
	}

	public void OnClickedCenterCircle() {
		arrangement.CycleArrangementType();
		MakeAllGenomeStuffDirty();
	}

	public void OnClickIncreaseGap() {
		arrangement.IncreaseGap();
		MakeAllGenomeStuffDirty();
	}

	public void OnClickDecreseGap() {
		arrangement.DecreseGap();
		MakeAllGenomeStuffDirty();
	}

	public void OnClickedIncreasRefCount() {
		arrangement.IncreasRefCount();
		MakeAllGenomeStuffDirty();
	}

	public void OnClickedDecreasseRefCount() {
		arrangement.DecreaseRefCount();
		MakeAllGenomeStuffDirty();
	}

	public void OnClickedAngleCounterClowkwise() {
		arrangement.TurnArrowCounterClowkwise();
		MakeAllGenomeStuffDirty();
	}

	public void OnClickedAngleClowkwise() {
		arrangement.TurnArrowClowkwise();
		MakeAllGenomeStuffDirty();
	}

	public void OnClickedFlipSame() {
		arrangement.flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Same;
		MakeAllGenomeStuffDirty();
	}

	public void OnClickedFlipOpposite() {
		arrangement.flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Opposite;
		MakeAllGenomeStuffDirty();
	}

	public void OnClickedFlipBlackToArrow() {
		arrangement.flipTypeBlackWhiteToArrow = ArrangementFlipBtaWtaTypeEnum.BlackToArrow;
		MakeAllGenomeStuffDirty();
	}

	public void OnClickedFlipWhiteToArrow() {
		arrangement.flipTypeBlackWhiteToArrow = ArrangementFlipBtaWtaTypeEnum.WhiteToArrow;
		MakeAllGenomeStuffDirty();
	}

	public void OnTogglePairs(bool value) {
		bool wasChanged = arrangement.isFlipPairsEnabled != value; //To prevent dirtymarking when just setting startup value
		arrangement.isFlipPairsEnabled = value;
		if (wasChanged) {
			MakeAllGenomeStuffDirty();
		}
	}

	public void OnClickedBuildSideBlack() {
		arrangement.referenceSide = ArrangementReferenceSideEnum.Black;
		MakeAllGenomeStuffDirty();
	}

	public void OnClickedBuildSideWhite() {
		arrangement.referenceSide = ArrangementReferenceSideEnum.White;
		MakeAllGenomeStuffDirty();
	}

	public void OnClickedPerifierCircle() {
		Debug.Log("TODO: select the gene this reference is pointing to?");
	}

	public void OnPointerEnterArea() {
		if (isValid) {
			isMouseHoverng = true;
			isDirty = true;
		}
	}

	public void OnPointerExitArea() {
		if (isValid) {
			isMouseHoverng = false;
			isDirty = true;
		}
	}

	public void OnClickedSetReference() {
		GenePanel.instance.SetAskingForGeneReference(this);
		MouseAction.instance.actionState = MouseActionStateEnum.selectGene;
	}

	public void SetGeneReference(Gene gene) {
		arrangement.referenceGene = gene;
		MakeAllGenomeStuffDirty();
	}

	private void MakeAllGenomeStuffDirty() {
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.genotype.differsFromGenome = true;
		}
		isDirty = true;
	}

	private void UpdateFlipButtonColors() {
		flipSameButtonImage.color = (arrangement.flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		flipOppositeButtonImage.color = (arrangement.flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Opposite) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;

		flipBlackToArrowButtonImage.color = (arrangement.flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		flipWhiteToArrowButtonImage.color = (arrangement.flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.WhiteToArrow) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
	}

	private void UpdatePairCheckmark() {
		togglePair.isOn = arrangement.isFlipPairsEnabled;
	}

	private void UpdateReferenceSideButtonColors() {
		referenceSideBlack.color = (arrangement.referenceSide == ArrangementReferenceSideEnum.Black) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		referenceSideWhite.color = (arrangement.referenceSide == ArrangementReferenceSideEnum.White) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
	}

	private void UpdateIsUsed() {
		grayOut.enabled = !isEnabled;
		enableToggle.GetComponent<Toggle>().isOn = isEnabled;
	}

	public bool isDirty = true;
	private void Update() {
		if (isDirty) {
			FlipSideEnum viewedFlipSide = GenotypePanel.instance.viewedFlipSide;

			//Nothing to represent
			if (arrangement == null) {
				for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
					referenceGraphics[cardinalIndex].reference = null;
				}
				arrangementButtons.SetActive(false);

				grayOut.gameObject.SetActive(false);
				circles.SetActive(false);
				arrowTransform.gameObject.SetActive(false);
				enableToggle.SetActive(false);
				return;
			}
			grayOut.gameObject.SetActive(true);
			circles.SetActive(true);
			arrowTransform.gameObject.SetActive(true);
			enableToggle.SetActive(true);

			arrangementButtons.SetActive(isEnabled && isMouseHoverng);

			//grey out
			UpdateIsUsed();

			//Center
			arrangementTypeText.text = arrangement.type.ToString();
			centerCircleFlipBlackWhiteImage.enabled = viewedFlipSide == FlipSideEnum.BlackWhite;
			centerCircleFlipWhiteBlack.enabled = viewedFlipSide == FlipSideEnum.WhiteBlack;

			if (arrangement.type == ArrangementTypeEnum.Side) {
				angleButtons.SetActive(true);
				referenceCountButtons.SetActive(true);
				pairsToggle.SetActive(false);
				flipOppositeSameButtons.SetActive(true);
				flipWhiteBlackToArrowButtons.SetActive(false);
				gapSizeButtons.SetActive(false);
				referenceSideButtions.SetActive(true);

				//Main Arrow
				arrowTransform.gameObject.SetActive(true);
				arrowTransform.transform.eulerAngles = new Vector3(0, 0, arrangement.GetFlipableArrowAngle(GenotypePanel.instance.viewedFlipSide));

				//Flip Buttons
				UpdateFlipButtonColors();

				//reference side
				UpdateReferenceSideButtonColors();

			} else if (arrangement.type == ArrangementTypeEnum.Mirror) {
				angleButtons.SetActive(true);
				referenceCountButtons.SetActive(true);
				pairsToggle.SetActive(arrangement.referenceCount == 4);
				flipOppositeSameButtons.SetActive(false);
				flipWhiteBlackToArrowButtons.SetActive(true);
				gapSizeButtons.SetActive(arrangement.referenceCount <= 4);
				referenceSideButtions.SetActive(false);

				arrowTransform.gameObject.SetActive(true);
				arrowTransform.transform.eulerAngles = new Vector3(0, 0, arrangement.GetFlipableArrowAngle(GenotypePanel.instance.viewedFlipSide));

				//Flip Buttons
				UpdateFlipButtonColors();

				//Checkmark
				UpdatePairCheckmark();
			} else if (arrangement.type == ArrangementTypeEnum.Star) {
				angleButtons.SetActive(arrangement.referenceCount < 6 || arrangement.isFlipPairsEnabled);
				referenceCountButtons.SetActive(true);
				pairsToggle.SetActive(arrangement.referenceCount == 6);
				flipOppositeSameButtons.SetActive(arrangement.referenceCount < 6 || !arrangement.isFlipPairsEnabled);
				flipWhiteBlackToArrowButtons.SetActive(arrangement.referenceCount == 6 && arrangement.isFlipPairsEnabled);
				gapSizeButtons.SetActive(false);
				referenceSideButtions.SetActive(false);

				//Arrow
				arrowTransform.gameObject.SetActive(arrangement.referenceCount < 6 || arrangement.isFlipPairsEnabled);
				arrowTransform.transform.eulerAngles = new Vector3(0, 0, arrangement.GetFlipableArrowAngle(GenotypePanel.instance.viewedFlipSide));

				//Flip Buttons
				UpdateFlipButtonColors();

				//Checkmark
				UpdatePairCheckmark();
			}

			//Perifier
			for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
				referenceGraphics[cardinalIndex].reference = arrangement.GetFlippableReference(cardinalIndex, GenotypePanel.instance.viewedFlipSide);
			}

			isDirty = false;
		}
	}
}
