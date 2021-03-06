﻿using UnityEngine;

public class MouseDrag : MonoBehaviour {

	// array for storing if the 3 mouse buttons are dragging
	bool[] isDragActive;

	// for remembering if a button was down in previous frame
	bool[] downInPreviousFrame;
	private void Start() {
		EvoStart();
	}

	public void EvoStart() {
		isDragActive = new bool[] { false, false, false };
		downInPreviousFrame = new bool[] { false, false, false };
	}

	private void Update() {
		EvoUpdate();
	}

	public void EvoUpdate() {
		for (int i = 0; i < isDragActive.Length; i++) {
			if (Input.GetMouseButton(i)) {
				if (downInPreviousFrame[i]) {
					if (isDragActive[i]) {
						OnDragging(i);
					}
					else {
						isDragActive[i] = true;
						OnDraggingStart(i);
					}
				}
				downInPreviousFrame[i] = true;
			}
			else {
				if (isDragActive[i]) {
					isDragActive[i] = false;
					OnDraggingEnd(i);
				}
				downInPreviousFrame[i] = false;
			}
		}
	}

	public virtual void OnDraggingStart(int mouseButton) {
		// implement this for start of dragging
		Debug.Log("MouseButton" + mouseButton + " START Drag");
	}

	public virtual void OnDragging(int mouseButton) {
		// implement this for dragging
		Debug.Log("MouseButton" + mouseButton + "DRAGGING");
	}

	public virtual void OnDraggingEnd(int mouseButton) {
		// implement this for end of dragging
		Debug.Log("MouseButton" + mouseButton + " END Drag");
	}
}