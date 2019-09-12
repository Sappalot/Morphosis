using UnityEngine;
using UnityEngine.UI;

public class MouseAction : MonoSingleton<MouseAction> {

	public Text mouseText;
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
	}

	private void UpdateText() {
		if (m_actionState == MouseActionStateEnum.free) {
			mouseText.text = string.Empty;
		} else if (m_actionState == MouseActionStateEnum.selectGene) {
			mouseText.text = "Select Gene Reference";
			mouseText.color = ColorScheme.instance.mouseTextAction;
		} else if (m_actionState == MouseActionStateEnum.selectSignalOutput) {
			mouseText.text = "Attach to a signal output";
			mouseText.color = ColorScheme.instance.mouseTextAction;
		} else if (m_actionState == MouseActionStateEnum.moveCreatures) {
			mouseText.text = "Move";
			mouseText.color = ColorScheme.instance.mouseTextAction;
		} else if (m_actionState == MouseActionStateEnum.rotateCreatures) {
			mouseText.text = "Rotate";
			mouseText.color = ColorScheme.instance.mouseTextAction;
		} else if (m_actionState == MouseActionStateEnum.copyMoveCreatures) {
			mouseText.text = CreatureSelectionPanel.instance.hasSoloSelected ? "Place Copy" : "Place Copies";
			mouseText.color = ColorScheme.instance.mouseTextAction;
		} else if (m_actionState == MouseActionStateEnum.combineMoveCreatures) {
			mouseText.text = "Place Mergeling";
			mouseText.color = ColorScheme.instance.mouseTextAction;
		} else if (m_actionState == MouseActionStateEnum.savingWorld) {
			mouseText.text = "Zzzz";
			mouseText.color = ColorScheme.instance.mouseTextBussy;
		} else if (m_actionState == MouseActionStateEnum.loadingWorld) {
			mouseText.text = "Zzzz";
			mouseText.color = ColorScheme.instance.mouseTextBussy;
		} else if (m_actionState == MouseActionStateEnum.restartingWorld) {
			mouseText.text = "Zzzz";
			mouseText.color = ColorScheme.instance.mouseTextBussy;
		} else if (m_actionState == MouseActionStateEnum.loadingFreezer) {
			mouseText.text = "Zzzz";
			mouseText.color = ColorScheme.instance.mouseTextBussy;
		}
	}
}
