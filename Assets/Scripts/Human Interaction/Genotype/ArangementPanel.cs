using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArangementPanel : MonoBehaviour {
    public Image grayOut;
    public GameObject arrangementButtons;
    public Text arrangementTypeText;
    public Image centerCircleFlipBlackWhiteImage;
    public Image centerCircleFlipWhiteBlack;
    public Image flipSameButtonImage;
    public Image flipOppositeButtonImage;
    public RectTransform arrowTransform;
    public ReferenceGraphics[] referenceGraphics = new ReferenceGraphics[6];

    private static Dictionary<int, int> cardinalToArrangement = new Dictionary<int, int>();

    public void Awake() {
        cardinalToArrangement.Add(0, -2);
        cardinalToArrangement.Add(1, 0);
        cardinalToArrangement.Add(2, 2);
        cardinalToArrangement.Add(3, 4);
        cardinalToArrangement.Add(4, 6);
        cardinalToArrangement.Add(5, -4);
    }

    private Arrangement m_arrangement;
    public Arrangement arangement {
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

    public void OnClickEnabledToggle(bool value) {
        grayOut.enabled = !value;
        arrangementButtons.SetActive(isEnabled);
    }

    public void OnClickedCenterCircle() {
        if (m_arrangement.type == ArrangementTypeEnum.Side) {
            m_arrangement.type = ArrangementTypeEnum.Mirror;
        } else if (m_arrangement.type == ArrangementTypeEnum.Mirror) {
            m_arrangement.type = ArrangementTypeEnum.Star;
        } else if (m_arrangement.type == ArrangementTypeEnum.Star) {
            m_arrangement.type = ArrangementTypeEnum.Side;
        }
    }

    public void OnClickedIncreasRefCount() {
        Debug.Log("+");
        m_arrangement.referenceCount++;
        if (m_arrangement.referenceCount == -1) {
            m_arrangement.referenceCount = 1;
        }
        if (m_arrangement.referenceCount > 5) {
            m_arrangement.referenceCount = 5;
        }
    }

    public void OnClickedDecreasseRefCount() {
        Debug.Log("-");
        m_arrangement.referenceCount--;
        if (m_arrangement.referenceCount == 1) {
            m_arrangement.referenceCount = -1;
        }
        if (m_arrangement.referenceCount < -5) {
            m_arrangement.referenceCount = -5;
        }
    }

    public void OnClickedFlipSame() {
        m_arrangement.flipType = FlipTypeEnum.Same;
    }

    public void OnClickedFlipOpposite() {
        m_arrangement.flipType = FlipTypeEnum.Opposite;
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

    public void UpdateRepresentation() {
        FlipSideEnum viewedFlipSide = GenotypePanel.instance.viewedFlipSide;
        
        //Center
        if (m_arrangement == null) {
            return;
        }
        arrangementTypeText.text = m_arrangement.type.ToString();
        centerCircleFlipBlackWhiteImage.enabled = viewedFlipSide == FlipSideEnum.BlackWhite;
        centerCircleFlipWhiteBlack.enabled = viewedFlipSide == FlipSideEnum.WhiteBlack;

        if (m_arrangement.type == ArrangementTypeEnum.Side) {
            arrowTransform.transform.eulerAngles = new Vector3(0, 0, m_arrangement.angle * 30f + 90f);
        }

        //Flip Buttons
        flipSameButtonImage.color = (m_arrangement.flipType == FlipTypeEnum.Same) ? GenotypePanel.instance.chosenColor : GenotypePanel.instance.unchosenColor;
        flipOppositeButtonImage.color = (m_arrangement.flipType == FlipTypeEnum.Opposite) ? GenotypePanel.instance.chosenColor : GenotypePanel.instance.unchosenColor;

        //Perifier
        for (int i = 0; i < 6; i++) {
           referenceGraphics[i].reference = m_arrangement == null ? null : GetReference(i);
        }
    }

    private GeneReference GetReference(int cardinalIndex) {
        if (m_arrangement.type == ArrangementTypeEnum.Side) {

            for (int index = 0; index < Mathf.Abs(m_arrangement.referenceCount); index++) {
                if (CardinalIndexToArrangementAngle(cardinalIndex) == WarpArrAngle(m_arrangement.angle + ( (m_arrangement.referenceCount > 0) ? index * 2 : -index * 2))) {
                    return new GeneReference(m_arrangement.referenceGene, m_arrangement.flipType == FlipTypeEnum.Same ? GenotypePanel.instance.viewedFlipSide : Opposite(GenotypePanel.instance.viewedFlipSide));
                }
            }

        }
        return null;
    }

    private static int WarpArrAngle(int angle) {
        if (angle < -5) {
            return angle + 12;
        }
        if (angle > 6) {
            return angle - 12;
        }
        return angle;
    }

    private static int CardinalIndexToArrangementAngle(int cardinalIndex) {
        return cardinalToArrangement[cardinalIndex];
    }

    private static FlipSideEnum Opposite(FlipSideEnum flipSide) {
        if (flipSide == FlipSideEnum.BlackWhite) {
            return FlipSideEnum.WhiteBlack;
        }
        return FlipSideEnum.BlackWhite;
    }

    public void OnClickedAngleCounterClowkwise() {
        if (m_arrangement == null) {
            return;
        }
        if (m_arrangement.type == ArrangementTypeEnum.Side) {
            m_arrangement.angle += 2;
        }
        m_arrangement.angle = WarpArrAngle(m_arrangement.angle);
    }

    public void OnClickedAngleClowkwise() {
        if (m_arrangement == null) {
            return;
        }
        if (m_arrangement.type == ArrangementTypeEnum.Side) {
            m_arrangement.angle -= 2;
        }
        m_arrangement.angle = WarpArrAngle(m_arrangement.angle);
    }
}
