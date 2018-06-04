using UnityEngine;

public class ResizeViewport : MonoBehaviour {
	public RectTransform windowSize;
	public Camera viewportToResize;

	public int rightMargin;
	public int topMargin;
	public int height;

	private Vector2i res;

	// Use this for initialization
	void Start() {
		res = new Vector2i();
	}

	// Update is called once per frame
	void Update() {

		if (res.x != (int)windowSize.rect.width || res.y != (int)windowSize.rect.height) {

			float viewportHeight = height / windowSize.rect.height;
			float viewportWidth = (windowSize.rect.width - rightMargin) / windowSize.rect.width;
			float viewportY = (windowSize.rect.height - (height + topMargin)) / windowSize.rect.height;

			viewportToResize.rect = new Rect(0, viewportY, viewportWidth, viewportHeight);
			//Debug.Log("w: " + viewportWidth + ", h: " + viewportHeight + ", y: " + viewportY);

			res = new Vector2i((int)windowSize.rect.width, (int)windowSize.rect.height);

		}

	}
}
