using UnityEngine;

static class GeometryUtil {
	
	//angle in degrees
	public static Vector2 GetVector(float angle, float length) {
		return new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle) * length, Mathf.Sin(Mathf.Deg2Rad * angle) * length);
	}

	public static bool AreCirclesIntersecting(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB) {
		return Vector2.Distance(centerA, centerB) < radiusA + radiusB;
	}

	public static bool IsPointInsideCircle(Vector2 point, Vector2 center, float radius) {
		return Mathf.Pow((point.x - center.x), 2) + Mathf.Pow((point.y - center.y), 2) < Mathf.Pow(radius, 2);
	}
}
