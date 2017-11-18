using UnityEngine;

static class GeometryUtils {
	
	//angle in degrees
	public static Vector2 GetVector(float angle, float length) {
		return new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle) * length, Mathf.Sin(Mathf.Deg2Rad * angle) * length);
	}

	public static bool AreCirclesIntersecting(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB) {
		return Vector2.Distance(centerA, centerB) < radiusA + radiusB;
	}
}
