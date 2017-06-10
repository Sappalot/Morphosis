using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArangementPanel : MonoBehaviour {
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

    public bool isEnabled {
        get {
            return !grayOut.enabled;
        }
    }

    public void UpdateRepresentation() {
        FlipSideEnum viewedFlipSide = GenotypePanel.instance.viewedFlipSide;

        //Center
        if (arrangement == null) {
            return;
        }
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

            arrangement.SnapToLegalSide();

            //Main Arrow

            arrowTransform.gameObject.SetActive(true);
            arrowTransform.transform.eulerAngles = new Vector3(0, 0, arrangement.GetFlipableMathAngle(GenotypePanel.instance.viewedFlipSide));

            //Flip Buttons
            UpdateFlipButtonColors();
        } else if (arrangement.type == ArrangementTypeEnum.Mirror) {
            angleButtons.SetActive(true);
            referenceCountButtons.SetActive(true);
            pairsToggle.SetActive(true);
            flipOppositeSameButtons.SetActive(false);
            flipWhiteBlackToArrowButtons.SetActive(true);
            gapSizeButtons.SetActive(true);

            arrangement.SnapToLegalMirror();

            arrowTransform.gameObject.SetActive(true);

            //Adjust Reference Count if nessesary
        } else if (arrangement.type == ArrangementTypeEnum.Star) {
            angleButtons.SetActive(arrangement.referenceCount < 6 || arrangement.flipPairsEnabled);
            referenceCountButtons.SetActive(true);
            pairsToggle.SetActive(arrangement.referenceCount == 6);
            flipOppositeSameButtons.SetActive(arrangement.referenceCount < 6 || !arrangement.flipPairsEnabled);
            flipWhiteBlackToArrowButtons.SetActive(arrangement.referenceCount == 6 && arrangement.flipPairsEnabled);
            gapSizeButtons.SetActive(false);

            arrangement.SnapToLegalStar();

            arrowTransform.gameObject.SetActive(arrangement.referenceCount < 6 || arrangement.flipPairsEnabled);
            arrowTransform.transform.eulerAngles = new Vector3(0, 0, arrangement.GetFlipableMathAngle(GenotypePanel.instance.viewedFlipSide));
            
            //Flip Buttons
            UpdateFlipButtonColors();

            //Checkmark
            UpdatePairCheckmark();
        }

        //Perifier
        for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
            referenceGraphics[cardinalIndex].reference = arrangement.GetFlippableReference(cardinalIndex, GenotypePanel.instance.viewedFlipSide);
        }
    }

    public void OnClickEnabledToggle(bool value) {
        grayOut.enabled = !value;
        arrangementButtons.SetActive(isEnabled);
    }

    public void OnClickedCenterCircle() {
        arrangement.CycleArrangementType();
    }

    public void OnClickedIncreasRefCount() {
        arrangement.IncreasRefCount();
    }

    public void OnClickedDecreasseRefCount() {
        arrangement.DecreasseRefCount();
    }

    public void OnClickedAngleCounterClowkwise() {
        arrangement.TurnArrowCounterClowkwise();
    }

    public void OnClickedAngleClowkwise() {
        arrangement.TurnArrowClowkwise();
    }

    public void OnClickedFlipSame() {
        arrangement.SetFlipSame();
    }

    public void OnClickedFlipOpposite() {
        arrangement.SetFlipOpposite();
    }

    public void OnClickedFlipBlackToArrow() {
        arrangement.SetFlipBlackToArrow();
    }

    public void OnClickedFlipWhiteToArrow() {
        arrangement.SetFlipWhiteToArrow();
    }

    public void OnTogglePairs(bool value) {
        arrangement.SetEnablePairs(value);
        UpdateFlipButtonColors();
    }

    public void OnClickedPerifierCircle() {
        Debug.Log("TODO: pick reference gene");
    }

    public void OnPointerEnterArea() {
        arrangementButtons.SetActive(isEnabled);
    }

    public void OnPointerExitArea() {
        arrangementButtons.SetActive(false);
    }

    private void UpdateFlipButtonColors() {
        flipSameButtonImage.color = (arrangement.flipTypeSameOpposite == ArrangementFlipTypeEnum.Same) ? GenotypePanel.instance.chosenColor : GenotypePanel.instance.unchosenColor;
        flipOppositeButtonImage.color = (arrangement.flipTypeSameOpposite == ArrangementFlipTypeEnum.Opposite) ? GenotypePanel.instance.chosenColor : GenotypePanel.instance.unchosenColor;

        flipBlackToArrowButtonImage.color = (arrangement.flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow) ? GenotypePanel.instance.chosenColor : GenotypePanel.instance.unchosenColor;
        flipWhiteToArrowButtonImage.color = (arrangement.flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.WhiteToArrow) ? GenotypePanel.instance.chosenColor : GenotypePanel.instance.unchosenColor;
    }

    private void UpdatePairCheckmark() {
        togglePair.isOn = arrangement.flipPairsEnabled;
    }
}
