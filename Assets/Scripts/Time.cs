public class Time {
	public long hours;
	public float seconds;

	public Time() {
		seconds = 0f;
		hours = 0;
	}

	public Time(long hours, float seconds) {
		this.hours = hours;
		this.seconds = seconds;
	}

	public void Tick(float deltaTime) {
		seconds += deltaTime;
		if (seconds > 3600f) {
			hours++;
			seconds -= 3600f; 
		}
	}

	public static Time operator +(Time a, Time b) {
		Time sum = new Time(a.hours + b.hours, a.seconds + b.seconds);
		return sum;
	}

	public static Time operator -(Time b, Time c) {
		Time sum = new Time();
		return sum;
	}

	public static bool operator ==(Time a, Time b) {
		return a.hours == b.hours && a.seconds == b.seconds; 
	}

	public static bool operator !=(Time a, Time b) {
		return !(a == b);
	}

	public static bool operator >(Time a, Time b) {
		if (a.hours > b.hours) {
			return true;
		} else if (a.hours < b.hours) {
			return false;
		}

		if (a.seconds > b.seconds) {
			return true;
		} else if (a.seconds < b.seconds) {
			return false;
		}

		return false;
	}

	public static bool operator >=(Time a, Time b) {
		return a > b || a == b;
	}

	public static bool operator <(Time a, Time b) {
		if (a.hours < b.hours) {
			return true;
		} else if (a.hours > b.hours) {
			return false;
		}

		if (a.seconds < b.seconds) {
			return true;
		} else if (a.seconds > b.seconds) {
			return false;
		}

		return false;
	}

	public static bool operator <=(Time a, Time b) {
		return a < b || a == b;
	}

	public bool Equals(Time other) {
		if (ReferenceEquals(null, other)) {
			return false;
		}
		if (ReferenceEquals(this, other)) {
			return true;
		}

		return (hours == other.hours && seconds == other.seconds);
	}

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj)) {
			return false;
		}
		if (ReferenceEquals(this, obj)) {
			return true;
		}

		return obj.GetType() == GetType() && Equals((Time)obj);
	}

	public override int GetHashCode() {
		unchecked {
			int hashCode = hours.GetHashCode();
			hashCode = (hashCode * 397) ^ seconds.GetHashCode();
			return hashCode;
		}
	}
}