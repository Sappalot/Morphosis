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

    public Text arrangementTypeText;

    public Image centerCircleFlipBlackWhiteImage;
    public Image centerCircleFlipWhiteBlack;

    public Image flipSameButtonImage;
    public Image flipOppositeButtonImage;
    public Image flipBlackToArrowButtonImage;
    public Image flipWhiteToArrowButtonImage;

    public Toggle togglePair;

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
       
            //Main Arrow
            arrowTransform.gameObject.SetActive(true);
            arrowTransform.transform.eulerAngles = new Vector3(0, 0, arrangement.GetFlipableArrowAngle(GenotypePanel.instance.viewedFlipSide));

            //Flip Buttons
            UpdateFlipButtonColors();
        } else if (arrangement.type == ArrangementTypeEnum.Mirror) {
            angleButtons.SetActive(true);
            referenceCountButtons.SetActive(true);
            pairsToggle.SetActive(arrangement.referenceCount == 4);
            flipOppositeSameButtons.SetActive(false);
            flipWhiteBlackToArrowButtons.SetActive(true);
            gapSizeButtons.SetActive(arrangement.referenceCount <= 4);

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
        arrangement.isEnabled = value;
        //arrangementButtons.SetActive(value);

        UpdateRepresentation();
        genePanel.UpdateRepresentation();
        GenomePanel.instance.UpdateRepresentation();
    }

    public void OnClickedCenterCircle() {
        arrangement.CycleArrangementType();
        UpdateRepresentation();
        genePanel.UpdateRepresentation();
        GenomePanel.instance.UpdateRepresentation();
    }

    public void OnClickIncreaseGap() {
        arrangement.IncreaseGap();
        UpdateRepresentation();
        genePanel.UpdateRepresentation();
        GenomePanel.instance.UpdateRepresentation();
    }

    public void OnClickDecreseGap() {
        arrangement.DecreseGap();
        UpdateRepresentation();
        genePanel.UpdateRepresentation();
        GenomePanel.instance.UpdateRepresentation();
    }

    public void OnClickedIncreasRefCount() {
        arrangement.IncreasRefCount();
        UpdateRepresentation();
        genePanel.UpdateRepresentation();
        GenomePanel.instance.UpdateRepresentation();
    }

    public void OnClickedDecreasseRefCount() {
        arrangement.DecreasseRefCount();
        UpdateRepresentation();
        genePanel.UpdateRepresentation();
        GenomePanel.instance.UpdateRepresentation();
    }

    public void OnClickedAngleCounterClowkwise() {
        arrangement.TurnArrowCounterClowkwise();
        UpdateRepresentation();
        genePanel.UpdateRepresentation();
        GenomePanel.instance.UpdateRepresentation();
    }

    public void OnClickedAngleClowkwise() {
        arrangement.TurnArrowClowkwise();
        UpdateRepresentation();
        genePanel.UpdateRepresentation();
        GenomePanel.instance.UpdateRepresentation();
    }

    public void OnClickedFlipSame() {
        arrangement.flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Same;
        UpdateRepresentation();
        genePanel.UpdateRepresentation();
        GenomePanel.instance.UpdateRepresentation();
    }

    public void OnClickedFlipOpposite() {
        arrangement.flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Opposite;
        UpdateRepresentation();
        genePanel.UpdateRepresentation();
        GenomePanel.instance.UpdateRepresentation();
    }

    public void OnClickedFlipBlackToArrow() {
        arrangement.flipTypeBlackWhiteToArrow = ArrangementFlipBtaWtaTypeEnum.BlackToArrow;
        UpdateRepresentation();
        genePanel.UpdateRepresentation();
        GenomePanel.instance.UpdateRepresentation();
    }

    public void OnClickedFlipWhiteToArrow() {
        arrangement.flipTypeBlackWhiteToArrow = ArrangementFlipBtaWtaTypeEnum.WhiteToArrow;
        UpdateRepresentation();
        genePanel.UpdateRepresentation();
        GenomePanel.instance.UpdateRepresentation();
    }

    public void OnTogglePairs(bool value) {
        arrangement.flipPairsEnabled = value;
        UpdateRepresentation();
        genePanel.UpdateRepresentation();
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
        genePanel.UpdateRepresentation();
    }

    private void UpdateFlipButtonColors() {
        flipSameButtonImage.color = (arrangement.flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same) ? GenotypePanel.instance.chosenColor : GenotypePanel.instance.unchosenColor;
        flipOppositeButtonImage.color = (arrangement.flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Opposite) ? GenotypePanel.instance.chosenColor : GenotypePanel.instance.unchosenColor;

        flipBlackToArrowButtonImage.color = (arrangement.flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow) ? GenotypePanel.instance.chosenColor : GenotypePanel.instance.unchosenColor;
        flipWhiteToArrowButtonImage.color = (arrangement.flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.WhiteToArrow) ? GenotypePanel.instance.chosenColor : GenotypePanel.instance.unchosenColor;
    }

    private void UpdatePairCheckmark() {
        togglePair.isOn = arrangement.flipPairsEnabled;
    }

    private void UpdateIsUsed() {
        grayOut.enabled = !isEnabled;
        enableToggle.GetComponent<Toggle>().isOn = isEnabled;
    }    
}
