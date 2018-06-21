using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour {

	//private float spacing = 50f; // pixels between each point, each point is 1 second
	public LineRenderer line;
	public Canvas textCanvas;
	public Text text;

	public RecordEnum type;
	public CellTypeEnum cellType; // just for the color
	//public short level;
	public string textPrefix;
	public string textPostfix;
	public float maxValue = 200;
	public short decimals;

	public void UpdateCanvas(Rect graphArea) {
		textCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(graphArea.width, graphArea.height);
		textCanvas.GetComponent<RectTransform>().position = new Vector2(graphArea.center.x, graphArea.center.y);
	}

	public void Start() {
		if (type == RecordEnum.cellCountEgg ||
			type == RecordEnum.cellCountFungal ||
			type == RecordEnum.cellCountJaw ||
			type == RecordEnum.cellCountLeaf ||
			type == RecordEnum.cellCountMuscle ||
			type == RecordEnum.cellCountRoot ||
			type == RecordEnum.cellCountShell ||
			type == RecordEnum.cellCountVein ) {
			text.color = line.material.color = ColorScheme.instance.ToColor(cellType);
		} else {
			text.color = line.material.color;
		}
		
	}

	private int oldPositionCount = 0;
	public void DrawGraph(Rect graphArea, float scale, short level, History history) {
		float levelScale = scale * Mathf.Pow(2f, level);

		int positionCount = Mathf.CeilToInt(graphArea.width / levelScale) + 1;
		if (positionCount != oldPositionCount) {
			line.positionCount = positionCount;
			oldPositionCount = positionCount;
		}

		for (int i = 0; i < positionCount; i++) {
			int stepsAgo = (positionCount - 1) - i;
			line.SetPosition(i, new Vector3(graphArea.xMax - levelScale * stepsAgo , graphArea.yMin + graphArea.height * (history.GetRecord(level, stepsAgo).Get(type) / maxValue), -1f));
		}

		text.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, Mathf.Clamp(graphArea.height * (history.GetRecord(0, 0).Get(type) / maxValue), - 5f, graphArea.height + 5f));
		if (decimals == 1) {
			text.text = string.Format("{0} {1:F1} {2}", textPrefix, history.GetRecord(0, 0).Get(type), textPostfix);
		} else {
			text.text = string.Format("{0} {1:F0} {2}", textPrefix, history.GetRecord(0, 0).Get(type), textPostfix);
		}
		
	}

}
