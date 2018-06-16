using UnityEngine;
using UnityEngine.UI;

public class Flag : MonoBehaviour {
	//private float spacing = 50f; // pixels between each point, each point is 1 second
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
		
		text.GetComponent<RectTransform>().anchoredPosition = new Vector2(position, graphArea.height - 5f);
	}
}
