using UnityEngine;
using UnityEngine.UI;

public class MouseAction : MonoSingleton<MouseAction> {

    public Text text;
    public Canvas myCanvas;
    private MouseActionStateEnum m_actionState = MouseActionStateEnum.free;
    public MouseActionStateEnum actionState {
        get {
            return m_actionState;
        }
        set {
            m_actionState = value;
            UpdateText();
        }
    }

    public override void Init() {
        UpdateText();
    }

    private void Update() {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
        transform.position = myCanvas.transform.TransformPoint(pos);

        if (Input.GetKey(KeyCode.Escape)) {
            actionState = MouseActionStateEnum.free;
            UpdateText();
        }
    }

    private void UpdateText() {
        if (m_actionState == MouseActionStateEnum.free) {
            text.text = string.Empty;
        } else if (m_actionState == MouseActionStateEnum.selectGene) {
            text.text = "Select gene reference!";
        }
    }

}
