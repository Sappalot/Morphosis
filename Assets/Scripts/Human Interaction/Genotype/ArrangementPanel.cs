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
		bool trueChange = arrangement.isEnabled != value;
		arrangement.isEnabled = value;
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedCenterCircle() {
		arrangement.CycleArrangementType();
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickIncreaseGap() {
		arrangement.IncreaseGap();
		//isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickDecreseGap() {
		arrangement.DecreseGap();
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedIncreasRefCount() {
		arrangement.IncreasRefCount();
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedDecreasseRefCount() {
		arrangement.DecreaseRefCount();
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedAngleCounterClowkwise() {
		arrangement.TurnArrowCounterClowkwise();
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedAngleClowkwise() {
		arrangement.TurnArrowClowkwise();
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedFlipSame() {
		arrangement.flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Same;
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedFlipOpposite() {
		arrangement.flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Opposite;
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedFlipBlackToArrow() {
		arrangement.flipTypeBlackWhiteToArrow = ArrangementFlipBtaWtaTypeEnum.BlackToArrow;
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedFlipWhiteToArrow() {
		arrangement.flipTypeBlackWhiteToArrow = ArrangementFlipBtaWtaTypeEnum.WhiteToArrow;
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnTogglePairs(bool value) {
		bool trueChange = arrangement.isFlipPairsEnabled != value;
		arrangement.isFlipPairsEnabled = value;
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedBuildSideBlack() {
		arrangement.referenceSide = ArrangementReferenceSideEnum.Black;
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
	}

	public void OnClickedBuildSideWhite() {
		arrangement.referenceSide = ArrangementReferenceSideEnum.White;
		isDirty = true;
		GenePanel.instance.isDirty = true;
		GenomePanel.instance.isDirty = true;
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
		isDirty = true;
		GenePanel.instance.isDirty = true;
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

				//referenceGraphics[cardinalIndex].SetGeneReference(arrangement.GetFlippableReference(cardinalIndex, GenotypePanel.instance.viewedFlipSide), genePanel.testCreature.genotype.genome);
				referenceGraphics[cardinalIndex].reference = arrangement.GetFlippableReference(cardinalIndex, GenotypePanel.instance.viewedFlipSide);
			}

			isDirty = true;
		}
	}
}
