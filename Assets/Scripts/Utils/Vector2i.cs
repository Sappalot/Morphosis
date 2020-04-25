using System;

[Serializable]
public class Vector2i {
	public int x;
	public int y;

	public Vector2i() {
		x = 0;
		y = 0;
	}

	public Vector2i(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public static Vector2i zero {
		get {
			return new Vector2i();
		}
	}

	public static Vector2i operator -(Vector2i a, Vector2i b) {
		return new Vector2i(a.x - b.x, a.y - b.y);
	}

	public static Vector2i operator +(Vector2i a, Vector2i b) {
		return new Vector2i(a.x + b.x, a.y + b.y);
	}

	public static bool operator ==(Vector2i obj1, Vector2i obj2) {
		if (ReferenceEquals(obj1, obj2)) {
			return true;
		}

		if (ReferenceEquals(obj1, null)) {
			return false;
		}
		if (ReferenceEquals(obj2, null)) {
			return false;
		}

		return (obj1.x == obj2.x && obj1.y == obj2.y);
	}

	public static bool operator !=(Vector2i obj1, Vector2i obj2) {
		return !(obj1 == obj2);
	}

	public bool Equals(Vector2i other) {
		if (ReferenceEquals(null, other)) {
			return false;
		}
		if (ReferenceEquals(this, other)) {
			return true;
		}

		return (x == other.x && y == other.y);
	}

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj)) {
			return false;
		}
		if (ReferenceEquals(this, obj)) {
			return true;
		}

		return obj.GetType() == GetType() && Equals((Vector2i)obj);
	}

	public override int GetHashCode() {
		unchecked {
			int hashCode = x.GetHashCode();
			hashCode = (hashCode * 397) ^ y.GetHashCode();
			return hashCode;
		}
	}

	public override string ToString() {
		return x + " : " + y;
	}
}
