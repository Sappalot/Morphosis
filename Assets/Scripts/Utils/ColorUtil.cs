using System.Collections.Generic;
using UnityEngine;

public static class ColorUtil {
	public static Color SetAlpha(Color color, float alpha) {
		return new Color(color.r, color.g, color.b, alpha);
	}
}