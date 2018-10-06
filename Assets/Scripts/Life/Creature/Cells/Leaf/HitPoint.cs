using System.Collections.Generic;
using UnityEngine;

public class HitPoint {
	public HitType hitType;
	public float distance;

	public HitPoint(HitType hitType, float distance) {
		this.hitType = hitType;
		this.distance = distance;
	}
}