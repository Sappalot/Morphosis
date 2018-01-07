using System;

static class TimeUtil {
	public static string GetTimeString(ulong seconds) {
		string text = "...";
		TimeSpan t = TimeSpan.FromSeconds(seconds);
		int d = t.Days;
		int h = t.Hours;
		int m = t.Minutes;
		int s = t.Seconds;

		if (d > 0) {
			text = string.Format("{0:F0}d {1:F0}h {2:F0}m {3:F0}s", d, h, m, s);
		} else if (h > 0) {
			text = string.Format("{0:F0}h {1:F0}m {2:F0}s", h, m, s);
		} else if (m > 0) {
			text = string.Format("{0:F0}m {1:F0}s", m, s);
		} else {
			text = string.Format("{0:F0}s", s);
		}
		return text;
	}
}
