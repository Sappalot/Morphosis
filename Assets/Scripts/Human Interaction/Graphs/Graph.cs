using UnityEngine;

public class Graph : MonoBehaviour {

	private float spacing = 10f; // pixels between each point, each point is 1 second

	public LineRenderer line;



	public void UpdateResolution(ResizeViewport viewport) {
		int positionCount = Mathf.CeilToInt(viewport.width / spacing) + 1;
		line.positionCount = positionCount;

		float strideX = viewport.width / (float)(positionCount - 1);
		float strideY = viewport.height / (float)(positionCount - 1);
		for (int i = 0; i < positionCount; i++) {
			float offset = i % 2 == 0 ? 20f : -20f;
			line.SetPosition(i, new Vector3(viewport.left + strideX * i, viewport.bottom + strideY * i + offset, -1f));
		}
	}

	public void UpdateDataFPS(ResizeViewport viewport, History history) {
		int positionCount = Mathf.CeilToInt(viewport.width / spacing) + 1;
		float strideX = viewport.width / (float)(positionCount - 1);

		for (int i = 0; i < positionCount; i++) {
			int secondsAgo = (positionCount - 1) - i;

			line.SetPosition(i, new Vector3(viewport.right - strideX * (secondsAgo + ((float)tick / 20f)), viewport.bottom + viewport.height * (history.GetRecord(secondsAgo).fps / 200f), -1f));
		}
	}

	public void UpdateDataCellCount(ResizeViewport viewport, History history) {
		int positionCount = Mathf.CeilToInt(viewport.width / spacing) + 1;
		float strideX = viewport.width / (float)(positionCount - 1);

		for (int i = 0; i < positionCount; i++) {
			int secondsAgo = (positionCount - 1) - i;

			line.SetPosition(i, new Vector3(viewport.right - strideX * secondsAgo, viewport.bottom + viewport.height * (history.GetRecord(secondsAgo).cellCount / 2500f), -1f));
		}
	}

	private int tick = 0;

	public void UpdateTick(ResizeViewport viewport) {
		int positionCount = Mathf.CeilToInt(viewport.width / spacing) + 1;
		float strideX = viewport.width / (float)(positionCount - 1);

		if (tick >= 20) {
			tick = 0;
		} else {
			tick++;
		}
	}
}
