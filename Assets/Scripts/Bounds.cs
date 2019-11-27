using UnityEngine;

public struct Bounds {
	public float xMin;
	public float xMax;
	public float yMin;
	public float yMax;

	public Bounds(float xMin, float xMax, float yMin, float yMax) {
		this.xMin = xMin;
		this.xMax = xMax;
		this.yMin = yMin;
		this.yMax = yMax;
	}

	public Vector2 center {
		get {
			return new Vector2((xMin + xMax) / 2f, (yMin + yMax) / 2f);
		}
	}

	public float width {
		get {
			return xMax - xMin;
		}
	}

	public float height {
		get {
			return yMax - yMin;
		}
	}

}
