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
    }

    private void UpdateSize() {
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            camera.orthographicSize *= 1 + cameraZoomStep;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            camera.orthographicSize *= 1 / (1 + cameraZoomStep);
        }

        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 1f, 300f);
    }

    private void UpdatePositionViaKeys() {
        float horizontalMove = 0f;
        float verticalMove = 0f;

        if (Input.GetKey(KeyCode.A)) {
            horizontalMove = -1;
        }
        if (Input.GetKey(KeyCode.D)) {
            horizontalMove = 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            verticalMove = -1;
        }
        if (Input.GetKey(KeyCode.W)) {
            verticalMove = 1;
        }

        camera.transform.position += new Vector3(
            horizontalMove * cameraMoveSpeed  * 2f * camera.orthographicSize * Time.deltaTime,
            verticalMove * cameraMoveSpeed * 2f * camera.orthographicSize * Time.deltaTime,
            0f);
    }

    private void OnMouseDown() {
        if (Input.GetKey("mouse 0") && !EventSystem.current.IsPointerOverGameObject()) {
            CreatureSelectionPanel.instance.ClearSelection();
        }
    }
}
