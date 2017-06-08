using UnityEngine;
using UnityEngine.UI;

public class ArangementPanel : MonoBehaviour {
    public Image grayOut;
    public GameObject arrangementButtons;

    public ReferenceGraphics[] referenceGraphics = new ReferenceGraphics[6];

    public FlipSide flipSideView = FlipSide.BlackWhite;

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

    public void OnPointerEnterArea() {
        arrangementButtons.SetActive(isEnabled);
    }

    public void OnPointerExitArea() {
        arrangementButtons.SetActive(false);
    }

    private void Update() {
        //UpdateRepresentation(); 
    }

    private void UpdateRepresentation() {
        for (int i = 0; i < 6; i++) {
           referenceGraphics[i].reference = m_arrangement == null ? null : m_arrangement.GetReference(flipSideView, CardinalDirectionHelper.ToCardinalDirection(i));
        }
    }
}
