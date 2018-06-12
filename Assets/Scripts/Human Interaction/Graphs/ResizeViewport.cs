using UnityEngine;

public class ResizeViewport : MonoBehaviour {
	public RectTransform windowSize;
	public Camera viewportToResize;

	public int rightMargin;
	public int topMargin;
	public int height; // height of the viewport set by user (camera is allways sime height as viewport)

	public Rect graphPlotterArea = new Rect();

	private Vector2i windowResolution;

	// Use this for initialization
	void Start() {
		windowResolution = new Vector2i();
	}

	// Update is called once per frame
	void Update() {
		if (windowResolution.x != (int)windowSize.rect.width || windowResolution.y != (int)windowSize.rect.height) {

			float viewportHeight = height / windowSize.rect.height;
			float viewportWidth = (windowSize.rect.width - rightMargin) / windowSize.rect.width;
			float viewportY = (windowSize.rect.height - (height + topMargin)) / windowSize.rect.height;

			viewportToResize.rect = new Rect(0, viewportY, viewportWidth, viewportHeight);
			//Debug.Log("w: " + viewportWidth + ", h: " + viewportHeight + ", y: " + viewportY);

			windowResolution = new Vector2i((int)windowSize.rect.width, (int)windowSize.rect.height);

			graphPlotterArea.width = viewportToResize.pixelWidth;
			graphPlotterArea.height = height;
			graphPlotterArea.center = gameObject.transform.position;


		}
	}
}
