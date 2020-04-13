using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollRectLinkScrolling : MonoBehaviour {

	public Transform self;
	public Transform other;

	public void OnScrolling() {
		other.localPosition = new Vector3(other.localPosition.x, self.localPosition.y, other.localPosition.z);
	}
	
}
