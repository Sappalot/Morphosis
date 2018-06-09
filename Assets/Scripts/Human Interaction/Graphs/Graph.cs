using UnityEngine;

public class Graph : MonoBehaviour {

	private float spacing = 10f; // pixels between each point

	public LineRenderer line;

	public void UpdateResolution(ResizeViewport viewport) {
		int positionCount = Mathf.CeilToInt(viewport.width / spacing) + 1;
		line.positionCount = positionCount;

		//line.SetPosition(0, new Vector3(viewport.left,  viewport.bottom, -1f));
		//line.SetPosition(1, new Vector3(viewport.right, viewport.top,    -1f));
		float strideX = viewport.width / (float)(positionCount - 1);
		float strideY = viewport.height / (float)(positionCount - 1);
		for (int i = 0; i < positionCount; i++) {
			float offset = i % 2 == 0 ? 20f : -20f;
			line.SetPosition(i, new Vector3(viewport.left + strideX * i, viewport.bottom + strideY * i + offset, -1f));
		}
	}
}
