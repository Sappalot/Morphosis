using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphMeasuringTool : MonoBehaviour {
	public LineRenderer line;
	public Canvas textCanvas;
	public Text text;

	public void UpdateCanvas(Rect graphArea) {
		textCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(graphArea.width, graphArea.height);
		textCanvas.GetComponent<RectTransform>().position = new Vector3(graphArea.center.x, graphArea.center.y, -2f);
	}

	public void SetPosition(Rect graphArea, float position, bool drawLine) { //x position counted fron right side "now" => all negative
		line.SetPosition(0, new Vector3(graphArea.xMax + position, graphArea.yMin, -1f));
		if (drawLine) {
			line.SetPosition(1, new Vector3(graphArea.xMax + position, graphArea.yMax, -1f));
		} else {
			line.SetPosition(1, new Vector3(graphArea.xMax + position, graphArea.yMin, -1f));
		}

		text.GetComponent<RectTransform>().anchoredPosition = new Vector2(position, text.GetComponent<RectTransform>().anchoredPosition.y);
	}

	private int oldPositionCount = 0;
	public void UpdateGraphics(Rect graphArea, float scale, ulong secoundsAgo) {
		float position = -(secoundsAgo * scale);
		SetPosition(graphArea, position, true);
		text.text = TimeUtil.GetTimeString(secoundsAgo) + " Ago";
	}
}