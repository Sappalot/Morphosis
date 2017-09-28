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
			UpdateRepresentation();
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

	public void UpdateRepresentation() {
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
			angleButtons.SetActive(arrangement.referenceCount < 6 || arrangement.flipPairsEnabled);
			referenceCountButtons.SetActive(true);
			pairsToggle.SetActive(arrangement.referenceCount == 6);
			flipOppositeSameButtons.SetActive(arrangement.referenceCount < 6 || !arrangement.flipPairsEnabled);
			flipWhiteBlackToArrowButtons.SetActive(arrangement.referenceCount == 6 && arrangement.flipPairsEnabled);
			gapSizeButtons.SetActive(false);
			referenceSideButtions.SetActive(false);

			//Arrow
			arrowTransform.gameObject.SetActive(arrangement.referenceCount < 6 || arrangement.flipPairsEnabled);
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
	}

	public void OnClickEnabledToggle(bool value) {
		bool trueChange = arrangement.isEnabled != value;
		arrangement.isEnabled = value;
		//arrangementButtons.SetActive(value);

		UpdateRepresentation();
		genePanel.UpdateRepresentation(trueChange);

		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickedCenterCircle() {
		arrangement.CycleArrangementType();
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickIncreaseGap() {
		arrangement.IncreaseGap();
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickDecreseGap() {
		arrangement.DecreseGap();
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickedIncreasRefCount() {
		arrangement.IncreasRefCount();
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickedDecreasseRefCount() {
		arrangement.DecreasseRefCount();
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickedAngleCounterClowkwise() {
		arrangement.TurnArrowCounterClowkwise();
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickedAngleClowkwise() {
		arrangement.TurnArrowClowkwise();
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickedFlipSame() {
		arrangement.flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Same;
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickedFlipOpposite() {
		arrangement.flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Opposite;
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickedFlipBlackToArrow() {
		arrangement.flipTypeBlackWhiteToArrow = ArrangementFlipBtaWtaTypeEnum.BlackToArrow;
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickedFlipWhiteToArrow() {
		arrangement.flipTypeBlackWhiteToArrow = ArrangementFlipBtaWtaTypeEnum.WhiteToArrow;
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnTogglePairs(bool value) {
		bool trueChange = arrangement.flipPairsEnabled != value;
		arrangement.flipPairsEnabled = value;
		UpdateRepresentation();
		genePanel.UpdateRepresentation(trueChange);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickedBuildSideBlack() {
		arrangement.referenceSide = ArrangementReferenceSideEnum.Black;
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickedBuildSideWhite() {
		arrangement.referenceSide = ArrangementReferenceSideEnum.White;
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
		GenomePanel.instance.UpdateRepresentation();
	}

	public void OnClickedPerifierCircle() {
		Debug.Log("TODO: select the gene this reference is pointing to?");
	}

	public void OnPointerEnterArea() {
		if (isValid) {
			isMouseHoverng = true;
			UpdateRepresentation();
		}
	}

	public void OnPointerExitArea() {
		if (isValid) {
			isMouseHoverng = false;
			UpdateRepresentation();
		}
	}

	public void OnClickedSetReference() {
		GenePanel.instance.SetAskingForGeneReference(this);
		MouseAction.instance.actionState = MouseActionStateEnum.selectGene;
	}

	public void SetGeneReference(Gene gene) {
		arrangement.referenceGene = gene;
		UpdateRepresentation();
		genePanel.UpdateRepresentation(true);
	}

	private void UpdateFlipButtonColors() {
		flipSameButtonImage.color = (arrangement.flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		flipOppositeButtonImage.color = (arrangement.flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Opposite) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;

		flipBlackToArrowButtonImage.color = (arrangement.flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		flipWhiteToArrowButtonImage.color = (arrangement.flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.WhiteToArrow) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
	}

	private void UpdatePairCheckmark() {
		togglePair.isOn = arrangement.flipPairsEnabled;
	}

	private void UpdateReferenceSideButtonColors() {
		referenceSideBlack.color = (arrangement.referenceSide == ArrangementReferenceSideEnum.Black) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
		referenceSideWhite.color = (arrangement.referenceSide == ArrangementReferenceSideEnum.White) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
	}

	private void UpdateIsUsed() {
		grayOut.enabled = !isEnabled;
		enableToggle.GetComponent<Toggle>().isOn = isEnabled;
	}
}
