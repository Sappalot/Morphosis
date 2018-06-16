public struct HistoryEvent {
	public string text;
	public bool showLine;

	public HistoryEvent(string text, bool showLine) {
		this.text = text;
		this.showLine = showLine;
	}
	//TODO: color;
}
