using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MouseDrag {
	public float follownessAutomatic = 8f;
	public float follownessManual = 30f;
	private float followness;

	public float cameraMoveSpeed = 1f; //Screenwidths per second
	public float cameraZoomStep = 0.4f;

	public Camera cameraVirtual;
	public Camera cameraView;

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
			downPositionCamera = cameraVirtual.transform.position;
			isDragging = true;
			followness = follownessManual;
		}
	}

	public override void OnDragging(int mouseButton) {
		// implement this for dragging
		if ((mouseButton == 1 || mouseButton == 2) && isDragging) {
			float pixels = cameraVirtual.pixelHeight;
			float units = cameraVirtual.orthographicSize * 2f;
			float unitsPerPixel = units / pixels;

			dragVector = (Input.mousePosition - downPositionMouse) * unitsPerPixel;
			cameraVirtual.transform.position = downPositionCamera - dragVector;
		}
	}

	public override void OnDraggingEnd(int mouseButton) {
		// implement this for end of dragging
		if (mouseButton == 1 || mouseButton == 2) {
			isDragging = false;
		}
		followness = follownessAutomatic;
	}

	private void Start() {
		base.EvoStart();
	}



	private void UpdateSize() {
		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			if (GraphPlotter.instance.IsMouseInside()) {
				GraphPlotter.instance.ZoomStepOut();
			} else if (cameraVirtual.orthographicSize < 300f) {
				Vector2 targetPostition = cameraVirtual.ScreenToWorldPoint(Input.mousePosition);
				Vector2 cameraToTarget = targetPostition - (Vector2)cameraVirtual.transform.position;
				float factor = 1f + cameraZoomStep;
				cameraVirtual.orthographicSize *= factor;
				cameraVirtual.transform.position += (Vector3)(cameraToTarget * (1f - factor));

				if (isFollowingCreature) {
					followMargin *= factor;
				}
			}
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			if (GraphPlotter.instance.IsMouseInside()) {
				GraphPlotter.instance.ZoomStepIn();
			} else if (cameraVirtual.orthographicSize > 1f) {
				Vector2 targetPostition = cameraVirtual.ScreenToWorldPoint(Input.mousePosition);
				Vector2 cameraToTarget = targetPostition - (Vector2)cameraVirtual.transform.position;
				float factor = 1f / (1f + cameraZoomStep);
				cameraVirtual.orthographicSize *= factor;
				cameraVirtual.transform.position += (Vector3)(cameraToTarget * (1f - factor));

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
			//followSpeed = followManualSpeed;
		} else if (Input.GetKey(KeyCode.D) && !GlobalPanel.instance.isWritingHistoryNote) {
			horizontalMove = 1;
			//followSpeed = followManualSpeed;
		} else if (Input.GetKey(KeyCode.S) && !GlobalPanel.instance.isWritingHistoryNote) {
			verticalMove = -1;
			//followSpeed = followManualSpeed;
		} else if (Input.GetKey(KeyCode.W) && !GlobalPanel.instance.isWritingHistoryNote) {
			verticalMove = 1;
			//followSpeed = followManualSpeed;
		} else {
			//followSpeed = followAutomaticSpeed;
		}

		cameraVirtual.transform.position += new Vector3(
			horizontalMove * cameraMoveSpeed * 2f * cameraVirtual.orthographicSize * Time.unscaledDeltaTime,
			verticalMove * cameraMoveSpeed * 2f * cameraVirtual.orthographicSize * Time.unscaledDeltaTime,
			0f);

	}


	// Messy to turn camera right after lock
	public void TryUnlockCamera() {
		if (PhenotypePanel.instance.followToggle.isOn && CreatureSelectionPanel.instance.hasSelection) {
			PhenotypePanel.instance.followToggle.isOn = false;
			TurnCameraStraightAtCameraUnlock();
		}
	}

	public void TurnCameraStraightAtCameraUnlock() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			cameraVirtual.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			//AABB = new Bounds(CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.x - followMargin,
			//							CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.x + followMargin,
			//							CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.y - followMargin,
			//							CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.y + followMargin);
			MoveToBounds(AABB, HUD.instance.hudSize, HUD.instance.WorldViewportBounds(HUD.instance.worldViewportPanel.bottomAndRightPanelsBlocking));
		}
	}

	public void MoveToBounds(Bounds worldRect, Vector2i screentBoundsHUD, Bounds viewportBoundsHUD) {

		float worldRectAspect = worldRect.width / worldRect.height;
		float worldViewportBoundsAspect = viewportBoundsHUD.width / viewportBoundsHUD.height;

		float enclosingWidth = 0f;
		float enclosingHeight = 0f;

		if (worldRectAspect < worldViewportBoundsAspect) {
			// world rect is touching floor and ceiling of viewport
			enclosingHeight = worldRect.height;
			enclosingWidth = worldRect.height * worldViewportBoundsAspect;
		} else {
			// world rect is touching left and rigth wall of viewport
			enclosingWidth = worldRect.width;
			enclosingHeight = worldRect.width / worldViewportBoundsAspect;
		}

		float enclosingWidthHalf = enclosingWidth / 2f;
		float enclosingHeightHalf = enclosingHeight / 2f;

		Bounds viewportBoundsWorld = new Bounds(worldRect.center.x - enclosingWidthHalf,
											worldRect.center.x + enclosingWidthHalf,
											worldRect.center.y - enclosingHeightHalf,
											worldRect.center.y + enclosingHeightHalf);

		Bounds screenBoundsWorld = new Bounds(viewportBoundsWorld.xMin - viewportBoundsHUD.xMin * (viewportBoundsWorld.width / viewportBoundsHUD.width),
												viewportBoundsWorld.xMax + (screentBoundsHUD.x - viewportBoundsHUD.xMax) * (viewportBoundsWorld.width / viewportBoundsHUD.width),
												viewportBoundsWorld.yMin - viewportBoundsHUD.yMin * (viewportBoundsWorld.height / viewportBoundsHUD.height),
												viewportBoundsWorld.yMax + (screentBoundsHUD.y - viewportBoundsHUD.yMax) * (viewportBoundsWorld.height / viewportBoundsHUD.height));

		Vector2 center = screenBoundsWorld.center;
		cameraVirtual.transform.position = new Vector3(center.x, center.y, cameraVirtual.transform.position.z);

		cameraVirtual.orthographicSize = screenBoundsWorld.height * 0.5f;
	}

	private void Update() {
		base.EvoUpdate();

		UpdatePositionViaKeys();
		UpdateSize();

		UpdateMouseCursor();

		cameraVirtual.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		if (PhenotypePanel.instance.followToggle.isOn && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && CreatureSelectionPanel.instance.hasSelection) { //&& !Input.GetMouseButton(0)
			if (!followToggle) {
				followMargin = cameraVirtual.orthographicSize * (HUD.instance.WorldViewportBounds(HUD.instance.worldViewportPanel.bottomAndRightPanelsBlocking).height / HUD.instance.hudSize.y);
				followToggle = true;
			}


			if (CreatureSelectionPanel.instance.hasSoloSelected) {
				AABB = new Bounds(CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.x - followMargin,
									CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.x + followMargin,
									CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.y - followMargin,
									CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position.y + followMargin);
				MoveToBounds(AABB, HUD.instance.hudSize, HUD.instance.WorldViewportBounds(HUD.instance.worldViewportPanel.bottomAndRightPanelsBlocking));
			} else {

				AABB = ViewSelectedCreaturePanel.BoundsOfCreatures(CreatureSelectionPanel.instance.selection); // HUD.instance.worldViewportPanel.bottomPanelBlocking
				Vector2 pos = ViewSelectedCreaturePanel.CenterOfBounds(AABB, HUD.instance.hudSize, HUD.instance.WorldViewportBounds(HUD.instance.worldViewportPanel.bottomPanelBlocking));
				cameraVirtual.transform.position = new Vector3(pos.x, pos.y, cameraVirtual.transform.position.z);
			}

			if (CreatureSelectionPanel.instance.hasSoloSelected && PhenotypePanel.instance.yawToggle.isOn) {
				// YEY ROTATE AROUND SAVES THE DAY!
				cameraVirtual.transform.RotateAround(CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position, Vector3.forward, CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.heading - 90f);
			}
		} else {
			followToggle = false;
		}

		// let cameraView follow cameraVirtual
		//CameraData cameraViewData = new CameraData(cameraView.transform.position, cameraView.transform.rotation, cameraView.orthographicSize);
		//CameraData cameraVirtualData = new CameraData(cameraVirtual.transform.position, cameraVirtual.transform.rotation, cameraVirtual.orthographicSize);

		

		Vector2 lerpPosition = Vector2.Lerp(cameraView.transform.position, cameraVirtual.transform.position, Time.unscaledDeltaTime * followness);
		cameraView.transform.position = new Vector3(lerpPosition.x, lerpPosition.y, cameraVirtual.transform.position.z);

		cameraView.transform.rotation = Quaternion.Lerp(cameraView.transform.rotation, cameraVirtual.transform.rotation, Time.unscaledDeltaTime * follownessAutomatic);
		
		cameraView.orthographicSize = Mathf.Lerp(cameraView.orthographicSize, cameraVirtual.orthographicSize, Time.unscaledDeltaTime * follownessAutomatic);
	}

	private struct CameraData {
		Vector2 position;
		Quaternion rotation;
		float orthographicSize;

		public CameraData(Vector2 position, Quaternion rotation, float orthographicSize) {
			this.position = position;
			this.rotation = rotation;
			this.orthographicSize = orthographicSize;
		}

		public CameraData Lerp(CameraData from, CameraData to, float t) {
			CameraData mix = new CameraData();
			
			return mix;
		}

	}
}
