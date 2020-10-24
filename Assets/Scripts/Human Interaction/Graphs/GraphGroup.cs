using UnityEngine;

public class GraphGroup : MonoBehaviour {
	public GraphSettings graphSettings;
	public Graph[] graphs;

	public void UpdateIsActive() {
		foreach (Graph g in graphs) {
			if (graphSettings == null) {
				g.gameObject.SetActive(true);
			} else {
				g.gameObject.SetActive(graphSettings.isOn);
			}
		}
	}

	public void UpdateCanvases(Rect graphArea) {
		foreach (Graph g in graphs) {
			g.UpdateCanvas(graphArea);
		}
	}

	public void DrawGraphs(Rect graphArea, float scale, short level, History history, int textMeasureStepsAgo) {
		float maxValue = 1000;
		if (graphSettings != null) {
			maxValue = graphSettings.maxValue;
		} else {
			genotypeDirtyfy.ReforgeCellPatternAndForward();Warning("Warning: Graph knows no GraphSettings");
		}
		foreach (Graph g in graphs) {
			g.DrawGraph(graphArea, scale, level, history, maxValue, textMeasureStepsAgo);
		}
	}
}