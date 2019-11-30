using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MouseDrag  {
	public Transform testRectangle;

	public float cameraMoveSpeed = 1f; //Screenwidths per second
	public float cameraZoomStep = 0.1f;

	public new Camera camera;

	private Vector3 dragVector = new Vector3();
	private Vector3 downPositionMouse;
	private Vector3 downPositionCamera;
	private bool isDragging = false;

	private float followMargin = 10f;

	private bool followToggle = false;
	private Bounds AABB;

	

	public bool isFollowingCreature {
		get {
			return PhenotypePanel.instance.followToggle.isOn && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.phenotype.isAlive;
		}
	}

	public override void OnDraggingStart(int mouseButton) {
		// implement this for start of dragging
		if (!followToggle && (mouseButton == 1 || mouseButton == 2) && !EventSystem.current.IsPointerOverGameObject()) {
			downPositionMouse = Input.mousePosition;
			downPositionCamera = camera.transform.position;
			isDragging = true;
		}
	}

	public override void OnDragging(int mouseButton) {
		// implement this for dragging
		if ((mouseButton == 1 || mouseButton == 2) && isDragging) {
			float pixels = camera.pixelHeight;
			float units = camera.orthographicSize * 2f;
			float unitsPerPixel = units / pixels;

			dragVector = (Input.mousePosition - downPositionMouse) * unitsPerPixel;
			camera.transform.position = downPositionCamera - dragVector;
		}
	}

	public override void OnDraggingEnd(int mouseButton) {
		// implement this for end of dragging
		if (mouseButton == 1 || mouseButton == 2) {
			isDragging = false;
		}
	}

	private void Start() {
		base.EvoStart();
	}

	private void UpdateZoom() {
		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			if (GraphPlotter.instance.IsMouseInside()) {
				GraphPlotter.instance.ZoomStepOut();
			} else if (camera.orthographicSize < 300f) {
				Vector2 targetPostition = camera.ScreenToWorldPoint(Input.mousePosition);
				Vector2 cameraToTarget = targetPostition - (Vector2)camera.transform.position;
				float factor = 1f + cameraZoomStep;
				camera.orthographicSize *= factor;
				camera.transform.position += (Vector3)(cameraToTarget * (1f - factor));

				if (isFollowingCreature) {
					followMargin *= factor;
				}
			}
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			if (GraphPlotter.instance.IsMouseInside()) {
				GraphPlotter.instance.ZoomStepIn();
			} else if (camera.orthographicSize > 1f) {
				Vector2 targetPostition = camera.ScreenToWorldPoint(Input.mousePosition);
				Vector2 cameraToTarget = targetPostition - (Vector2)camera.transform.position;
				float factor = 1f / (1f + cameraZoomStep);
				camera.orthographicSize *= factor;
				camera.transform.position += (Vector3)(cameraToTarget * (1f - factor));

				if (isFollowingCreature) {
					followMargin *= factor;
				}
			}
		}
	}
	

	public Texture2D cursorTextureCameraPan;

	private Vector2 offsetPan = new Vector2(18f, 20f);

	private void UpdateMouseCursor() {
		if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse2)) {
			Cursor.SetCursor(cursorTextureCameraPan, offsetPan, CursorMode.ForceSoftware);
		} else {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}

	private void UpdatePositionViaKeys() {
		if (PhenotypePanel.instance.followToggle.isOn) {
			return;
		}

		float horizontalMove = 0f;
		float verticalMove = 0f;

		if (Input.GetKey(KeyCode.A) && !GlobalPanel.instance.isWritingHistoryNote) {
			horizontalMove = -1;
		}
		if (Input.GetKey(KeyCode.D) && !GlobalPanel.instance.isWritingHistoryNote) {
			horizontalMove = 1;
		}
		if (Input.GetKey(KeyCode.S) && !GlobalPanel.instance.isWritingHistoryNote) {
			verticalMove = -1;
		}
		if (Input.GetKey(KeyCode.W) && !GlobalPanel.instance.isWritingHistoryNote) {
			verticalMove = 1;
		}

		camera.transform.position += new Vector3(
			horizontalMove * cameraMoveSpeed  * 2f * camera.orthographicSize * Time.unscaledDeltaTime,
			verticalMove * cameraMoveSpeed * 2f * camera.orthographicSize * Time.unscaledDeltaTime,
			0f);

	}


	// Messy to turn camera right after lock
	public void TryUnlockCamera() {
		if (PhenotypePanel.instance.followToggle.isOn && CreatureSelectionPanel.instance.hasSoloSelected) {
			PhenotypePanel.instance.followToggle.isOn = false;
			TurnCameraStraightAtCameraUnlock();
		}
	}

	public void TurnCameraStraightAtCameraUnlock() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {

			Bounds viewportBoundsInWorld = GetViewportBoundsInWorld(camera.transform.position, cameraSize, HUD.instance.hudSize, HUD.instance.WorldViewportBoundsHUD(HUD.instance.worldViewportPanel.bottomPanelBlocking));

			FilmAnimateWorldRect(viewportBoundsInWorld.center, viewportBoundsInWorld.size, 0f, HUD.instance.hudSize, HUD.instance.WorldViewportBoundsHUD(HUD.instance.worldViewportPanel.bottomPanelBlocking));
		}
	}

	private Vector2 cameraSize {
		get {
			return new Vector2(camera.orthographicSize * 2f * camera.aspect, camera.orthographicSize * 2f);
		}
	}

	// from cameras position to viewport in world
	// cameraRectSize = (width of 'frustum' , height of 'frustum')
	private Bounds GetViewportBoundsInWorld(Vector2 cameraPosition, Vector2 cameraRectSize, Vector2i screentBoundsHUD, Bounds viewportBoundsHUD) {
		float viewportHeightInWorld = cameraRectSize.y * (viewportBoundsHUD.height / screentBoundsHUD.y);
		float viewportWidthInWorld = cameraRectSize.x * (viewportBoundsHUD.width / screentBoundsHUD.x);

		float viewportInWorldXMin = cameraPosition.x - cameraRectSize.x / 2f + (viewportBoundsHUD.xMin / screentBoundsHUD.x) * cameraRectSize.x;
		float viewportInWorldXMax = cameraPosition.x - cameraRectSize.x / 2f + (viewportBoundsHUD.xMax / screentBoundsHUD.x) * cameraRectSize.x;

		float viewportInWorldYMin = cameraPosition.y - cameraRectSize.y / 2f + (viewportBoundsHUD.yMin / screentBoundsHUD.y) * cameraRectSize.y;
		float viewportInWorldYMax = cameraPosition.y - cameraRectSize.y / 2f + (viewportBoundsHUD.yMax / screentBoundsHUD.y) * cameraRectSize.y;

		return new Bounds(viewportInWorldXMin, viewportInWorldXMax, viewportInWorldYMin, viewportInWorldYMax);
	}

	private Bounds GetViewportBoundsInWorld(Bounds worldRect, Bounds viewportBoundsHUD) {
		float worldRectAspect = worldRect.width / worldRect.height;
		float worldViewportBoundsAspect = viewportBoundsHUD.width / viewportBoundsHUD.height;

		float viewportWidthWorld;
		float viewportHeightWorld;

		if (worldRectAspect < worldViewportBoundsAspect) {
			// world rect is touching floor and ceiling of viewport
			viewportHeightWorld = worldRect.height;
			viewportWidthWorld = worldRect.height * worldViewportBoundsAspect;
		} else {
			// world rect is touching left and rigth wall of viewport
			viewportWidthWorld = worldRect.width;
			viewportHeightWorld = worldRect.width / worldViewportBoundsAspect;
		}

		float viewportWidthWorldHalf = viewportWidthWorld / 2f;
		float viewportHeightWorldHalf = viewportHeightWorld / 2f;

		Bounds viewportBoundsWorld = new Bounds(worldRect.center.x - viewportWidthWorldHalf,
											worldRect.center.x + viewportWidthWorldHalf,
											worldRect.center.y - viewportHeightWorldHalf,
											worldRect.center.y + viewportHeightWorldHalf);

		return viewportBoundsWorld;
	}

	private Bounds GetScreenBoundsInWorld(Bounds worldRect, Vector2i screentSize, Bounds viewportBoundsHUD) {
		Bounds viewportBoundsWorld = GetViewportBoundsInWorld(worldRect, viewportBoundsHUD);

		Bounds screenBoundsWorld = new Bounds(viewportBoundsWorld.xMin - viewportBoundsHUD.xMin * (viewportBoundsWorld.width / viewportBoundsHUD.width),
												viewportBoundsWorld.xMax + (screentSize.x - viewportBoundsHUD.xMax) * (viewportBoundsWorld.width / viewportBoundsHUD.width),
												viewportBoundsWorld.yMin - viewportBoundsHUD.yMin * (viewportBoundsWorld.height / viewportBoundsHUD.height),
												viewportBoundsWorld.yMax + (screentSize.y - viewportBoundsHUD.yMax) * (viewportBoundsWorld.height / viewportBoundsHUD.height));
		return screenBoundsWorld;
	}

	public void MoveToRect(Bounds worldRect, Vector2i screentBoundsHUD, Bounds viewportBoundsHUD) {
		Bounds screenBoundsWorld = GetScreenBoundsInWorld(worldRect, screentBoundsHUD, viewportBoundsHUD);

		// position
		Vector2 center = screenBoundsWorld.center;
		camera.transform.position = new Vector3(center.x, center.y, camera.transform.position.z);

		// rotation
		camera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

		//zoom
		camera.orthographicSize = screenBoundsWorld.height * 0.5f;
	}

	// Bring camera to a position where worldPosition is viewed in center of viewportBoundsHUD. The view should be at an angle of worldAngle
	public void FilmWorldRect(Vector2 worldPosition, Vector2 worldRectSize, float worldAngle, Vector2i screentBoundsHUD, Bounds viewportBoundsHUD) {
		Bounds worldRectAxisAligned = new Bounds(worldPosition.x - worldRectSize.x / 2f, worldPosition.x + worldRectSize.x / 2f, worldPosition.y - worldRectSize.y / 2f, worldPosition.y + worldRectSize.y / 2f);
		Bounds screenBoundsWorldAxisAligned = GetScreenBoundsInWorld(worldRectAxisAligned, screentBoundsHUD, viewportBoundsHUD);

		// position
		Vector2 center = screenBoundsWorldAxisAligned.center;
		camera.transform.position = new Vector3(center.x, center.y, camera.transform.position.z);

		// rotation
		//camera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		camera.transform.RotateAround(worldRectAxisAligned.center, Vector3.forward, worldAngle);

		//zoom
		camera.orthographicSize = screenBoundsWorldAxisAligned.height * 0.5f;
	}



	// worldRect: at the end of the animation
	// screentBoundsHUD
	// viewportBoundsHUD: ...at the end of the animation
	public void FilmAnimateWorldRect(Vector2 worldPosition, Vector2 worldRectSize, float worldAngle, Vector2i screentBoundsHUD, Bounds viewportBoundsHUD) {
		// Start
		Bounds startWorldViewportBoundsHUD = HUD.instance.worldViewportBoundsHUD;
		float cameraHeight = camera.orthographicSize * 2;
		float cameraWidth = cameraHeight * camera.aspect;

		Bounds viewportBoundsInWorld = GetViewportBoundsInWorld(camera.transform.position, new Vector2(cameraWidth, cameraHeight), screentBoundsHUD, viewportBoundsHUD);
		// ...with creature in center

		//test
		//testRectangle.transform.position = viewportBoundsInWorld.center;
		//testRectangle.transform.localScale = viewportBoundsInWorld.size;

		Vector2 viewportCenterToCameraPosition = (Vector2)camera.transform.position - viewportBoundsInWorld.center;

		animateStart.position = viewportBoundsInWorld.center; // ...with creature in center
		animateStart.rotationArm = viewportCenterToCameraPosition;
		animateStart.rotation = camera.transform.rotation.eulerAngles.z; //todo
		animateStart.orthoSize = camera.orthographicSize;
		animateTime = 0f;

		// Goal
		Bounds worldRectGoal = new Bounds(worldPosition.x - worldRectSize.x / 2f, worldPosition.x + worldRectSize.x / 2f, worldPosition.y - worldRectSize.y / 2f, worldPosition.y + worldRectSize.y / 2f);
		Bounds screenBoundsWorldGoal = GetScreenBoundsInWorld(worldRectGoal, screentBoundsHUD, viewportBoundsHUD);

		animateGoal.position = worldPosition;
		animateGoal.rotationArm = screenBoundsWorldGoal.center - worldPosition;
		animateGoal.rotation = worldAngle;
		animateGoal.orthoSize = worldRectSize.y;
	}

	private CameraState animateStart;
	private CameraState animateGoal;
	private float animateTime = 100f; // 0 => 1
	private float animateTimespsan = 1f;

	private bool isAnimating {
		get {
			return animateTime < animateTimespsan;
		}
	}

	private float animatingProgression {
		get {
			return animateTime / animateTimespsan;
		}
	}

	///------ animate camera
	private void Update() {
		base.EvoUpdate();

		if (isAnimating) {
			animateTime += Time.unscaledDeltaTime;
			animateTime = Mathf.Min(animateTime, animateTimespsan);

			CameraState mix = CameraState.Lerp(animateStart, animateGoal, animatingProgression);

			camera.transform.position = new Vector3(mix.position.x, mix.position.y, camera.transform.position.z) + (Vector3)mix.rotationArm;

			camera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			camera.transform.RotateAround(mix.position, Vector3.forward, mix.rotation);

			camera.orthographicSize = mix.orthoSize;

			Debug.Log("Anim: " + animatingProgression + " ,arm: " + mix.rotationArm);
		} else {

			UpdatePositionViaKeys();
			UpdateZoom();
			UpdateMouseCursor();

			if (isFollowingCreature) {
				if (!followToggle) {
					followMargin = camera.orthographicSize * (HUD.instance.WorldViewportBoundsHUD(HUD.instance.worldViewportPanel.bottomAndRightPanelsBlocking).height / HUD.instance.hudSize.y);
					followToggle = true;
				}

				
				AABB = new Bounds(	CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.x - followMargin,
									CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.x + followMargin,
									CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.y - followMargin,
									CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.y + followMargin);

				return;

				MoveToRect(AABB, HUD.instance.hudSize, HUD.instance.WorldViewportBoundsHUD(HUD.instance.worldViewportPanel.bottomAndRightPanelsBlocking));

				if (PhenotypePanel.instance.yawToggle.isOn) {
					// YEY ROTATE AROUND SAVES THE DAY! =D
					camera.transform.RotateAround(CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position, Vector3.forward, CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.heading - 90f);
				}
			} else {
				followToggle = false;
				camera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
	}

	private struct CameraState {
		public Vector2 position;
		public Vector2 rotationArm; // the vector in worldspace: from viewportInWorcldCenter to cameraCenter
		public float rotation;
		public float orthoSize;

		public static CameraState Lerp(CameraState a, CameraState b, float t) {
			CameraState mix = new CameraState();
			mix.position = Vector2.Lerp(a.position, b.position, t);
			mix.rotationArm = Vector2.Lerp(a.rotationArm, b.rotationArm, t);
			mix.rotation = Mathf.LerpAngle(a.rotation, b.rotation, t);
			mix.orthoSize = Mathf.Lerp(a.orthoSize, b.orthoSize, t);
			return mix;
		}
	}
}
