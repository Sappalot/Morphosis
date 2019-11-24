using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MouseDrag  {
	public float cameraMoveSpeed = 1f; //Screenwidths per second
	public float cameraZoomStep = 0.1f;

	public new Camera camera;
	public CameraMovement cameraMovement;

	private Vector3 dragVector = new Vector3();
	private Vector3 downPositionMouse;
	private Vector3 downPositionCamera;
	private bool isDragging = false;

	private float followMargin = 10f;

	private bool followToggle = false;

	public override void OnDraggingStart(int mouseButton) {
		// implement this for start of dragging
		if ((mouseButton == 1 || mouseButton == 2) && !EventSystem.current.IsPointerOverGameObject()) {
			downPositionMouse = Input.mousePosition;
			downPositionCamera = camera.transform.position;
			isDragging = true;

			TryReleaseCameraLock();
			//Debug.Log("MouseButton" + mouseButton + " START Drag");
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
			//Debug.Log("MouseButton" + mouseButton + "DRAGGING: " + dragVector);
		}
	}

	public override void OnDraggingEnd(int mouseButton) {
		// implement this for end of dragging
		if (mouseButton == 1 || mouseButton == 2) {
			isDragging = false;
			//Debug.Log("MouseButton" + mouseButton + " END Drag");
		}
	}

	private void Start() {
		base.EvoStart();
	}

	private void Update() {
		base.EvoUpdate();

		UpdatePositionViaKeys();
		UpdateSize();

		UpdateMouseCursor();

		camera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		if (PhenotypePanel.instance.followToggle.isOn && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.phenotype.isAlive) { //&& !Input.GetMouseButton(0)
			if (!followToggle) {
				followMargin = camera.orthographicSize * (HUD.instance.WorldViewportBounds(HUD.instance.worldViewportPanel.bottomAndRightPanelsBlocking).height / HUD.instance.hudSize.y);
				followToggle = true;
			}
			Bounds AABB = new Bounds(	CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.x - followMargin,
										CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.x + followMargin,
										CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.y - followMargin,
										CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.y + followMargin);

			cameraMovement.MoveToBounds(AABB, HUD.instance.hudSize, HUD.instance.WorldViewportBounds(HUD.instance.worldViewportPanel.bottomAndRightPanelsBlocking), true);

			if (PhenotypePanel.instance.yawToggle.isOn) {
				//camera.transform.localRotation = Quaternion.Euler(0f, 0f, CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.heading - 90f);
				camera.transform.RotateAround(CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position, Vector3.forward, CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.heading - 90f);
			}
		} else {
			followToggle = false;
		}
	}

	private void UpdateSize() {
		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			if (GraphPlotter.instance.IsMouseInside()) {
				GraphPlotter.instance.ZoomStepOut();
			} else if (camera.orthographicSize < 300f) {
				Vector2 targetPostition = camera.ScreenToWorldPoint(Input.mousePosition);
				Vector2 cameraToTarget = targetPostition - (Vector2)camera.transform.position;
				float factor = 1f + cameraZoomStep;
				camera.orthographicSize *= factor;
				camera.transform.position += (Vector3)(cameraToTarget * (1f - factor));

				if (PhenotypePanel.instance.followToggle.isOn && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.phenotype.isAlive) {
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

				if (PhenotypePanel.instance.followToggle.isOn && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.phenotype.isAlive) {
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
		float horizontalMove = 0f;
		float verticalMove = 0f;

		if (Input.GetKey(KeyCode.A) && !GlobalPanel.instance.isWritingHistoryNote) {
			horizontalMove = -1;
			TryReleaseCameraLock();
		}
		if (Input.GetKey(KeyCode.D) && !GlobalPanel.instance.isWritingHistoryNote) {
			horizontalMove = 1;
			TryReleaseCameraLock();
		}
		if (Input.GetKey(KeyCode.S) && !GlobalPanel.instance.isWritingHistoryNote) {
			verticalMove = -1;
			TryReleaseCameraLock();
		}
		if (Input.GetKey(KeyCode.W) && !GlobalPanel.instance.isWritingHistoryNote) {
			verticalMove = 1;
			TryReleaseCameraLock();
		}

		camera.transform.position += new Vector3(
			horizontalMove * cameraMoveSpeed  * 2f * camera.orthographicSize * Time.unscaledDeltaTime,
			verticalMove * cameraMoveSpeed * 2f * camera.orthographicSize * Time.unscaledDeltaTime,
			0f);

	}

	private void TryReleaseCameraLock() {
		if (CreatureSelectionPanel.instance.hasSelection && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			PhenotypePanel.instance.followToggle.isOn = false;
		}
	}
}
