using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MouseDrag {
	public float cameraMoveSpeed = 1f; //Screenwidths per second
	public float cameraZoomStep = 0.1f; //Screenwidths per second

	public new Camera camera;

	private Vector3 dragVector = new Vector3();
	private Vector3 downPositionMouse;
	private Vector3 downPositionCamera;
	private bool isDragging = false;

	public override void OnDraggingStart(int mouseButton) {
		// implement this for start of dragging
		if (mouseButton == 2 && !EventSystem.current.IsPointerOverGameObject()) {
			downPositionMouse = Input.mousePosition;
			downPositionCamera = camera.transform.position;
			isDragging = true;

			TryReleaseCameraLock();
			//Debug.Log("MouseButton" + mouseButton + " START Drag");
		}
	}

	public override void OnDragging(int mouseButton) {
		// implement this for dragging
		if (mouseButton == 2 && isDragging) {
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
		if (mouseButton == 2) {
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

		camera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		if (PhenotypePanel.instance.followToggle.isOn && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && !Input.GetMouseButton(0)) {
			if (CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.phenotype.isAlive) {
				Vector2 focus = CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.position;
				camera.transform.position = new Vector3(focus.x, focus.y, camera.transform.position.z);
				if (PhenotypePanel.instance.yawToggle.isOn) {
					camera.transform.localRotation = Quaternion.Euler(0f, 0f, CreatureSelectionPanel.instance.soloSelected.phenotype.originCell.heading - 90f);
				} 
			} else if (CreatureSelectionPanel.instance.selectionCount > 1) {
				List<Creature> selection = CreatureSelectionPanel.instance.selection;
				int count = 0;
				Vector2 sum = new Vector2();
				foreach (Creature c in selection) {
					if (c.phenotype.isAlive) {
						sum += c.phenotype.originCell.position;
						count++;
					}
				}
				if (count >  0) {
					sum /= count;
					camera.transform.position = new Vector3(sum.x, sum.y, camera.transform.position.z);
				}
			} 
		}
	}

	

	private void UpdateSize() {
		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			if (GraphPlotter.instance.IsMouseInside()) {
				GraphPlotter.instance.ZoomStepOut();
			} else {
				camera.orthographicSize *= 1 + cameraZoomStep;
			}
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			if (GraphPlotter.instance.IsMouseInside()) {
				GraphPlotter.instance.ZoomStepIn();
			} else {
				camera.orthographicSize *= 1 / (1 + cameraZoomStep);
			}
		}

		camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 1f, 300f);
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
			horizontalMove * cameraMoveSpeed  * 2f * camera.orthographicSize * Time.deltaTime,
			verticalMove * cameraMoveSpeed * 2f * camera.orthographicSize * Time.deltaTime,
			0f);
	}

	private void TryReleaseCameraLock() {
		if (CreatureSelectionPanel.instance.hasSelection && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			PhenotypePanel.instance.followToggle.isOn = false;
		}
	}
}
