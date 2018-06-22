using UnityEngine;

public struct HistoryEvent {
	public string text;
	public bool showLine;
	public Color color;

	public HistoryEvent(string text, bool showLine, Color color) {
		this.text = text;
		this.showLine = showLine;
		this.color = color;
	}
	
}
