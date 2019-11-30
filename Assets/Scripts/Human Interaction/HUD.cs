using UnityEngine;

public class HUD : MonoSingleton<HUD> {
	public WorldViewportPanel worldViewportPanel;
	
	public Vector2i hudSize { // in pixels
		get {
			return windowResolution;
		}
	}
	// the bounds of the view rectangle inside the entire hud window, in pixels 
	[HideInInspector]
	public Bounds worldViewportBoundsHUD {
		get {
			return WorldViewportBoundsHUD(worldViewportPanel.usedViewportPanel);
		}
	}

	public Bounds WorldViewportBoundsHUD(RectTransform panel) {
			return new Bounds(	panel.anchoredPosition.x,
								panel.anchoredPosition.x + panel.rect.width,
								windowResolution.y - panel.rect.height + panel.anchoredPosition.y,
								windowResolution.y + panel.anchoredPosition.y);
	}

	private Vector2i windowResolution;

	void Start() {
		windowResolution = new Vector2i();
	}

	// Update is called once per frame
	void Update() {
		if (windowResolution.x != Screen.width || windowResolution.y != Screen.height) {
			windowResolution = new Vector2i(Screen.width, Screen.height);

		}
	}
}
