using UnityEngine;

public class GraphPlotter : MonoSingleton<GraphPlotter> {

	public ResizeViewport viewport;
	public Graph fpsGraph;
	public Graph cellCountGraph;

	[HideInInspector]
	public History history;

	private Vector2i res;
	

	void Start() {
		
		res = new Vector2i();
	}

	private bool isDirty;

	public void MakeDirty() {  
		isDirty = true; 
	}

	private void Update() {
		if (res.x != viewport.width || res.y != viewport.height) {
			fpsGraph.UpdateResolution(viewport);
			cellCountGraph.UpdateResolution(viewport);
			res = new Vector2i(viewport.width, viewport.height); 
		}

		if (isDirty && history != null) {
			fpsGraph.UpdateDataFPS(viewport, history);
			cellCountGraph.UpdateDataCellCount(viewport, history);
			isDirty = false;
		}
	}
}
