using UnityEngine;

public class GraphPlotter : MonoBehaviour {

	public ResizeViewport viewport;
	public Graph fpsGraph;
	private Vector2i res;

	// Use this for initialization
	void Start() {
		res = new Vector2i();
	}

	// Update is called once per frame
	void Update() {

		if (res.x != viewport.width || res.y != viewport.height) {
			fpsGraph.UpdateResolution(viewport);
			//Debug.Log("w: " + viewport.width + ", h: " + viewport.height);

			res = new Vector2i(viewport.width, viewport.height);
		}
	}
}
