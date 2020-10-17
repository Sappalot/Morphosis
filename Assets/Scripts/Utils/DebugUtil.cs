using System.Collections.Generic;
using UnityEngine;

public static class DebugUtil {
	public static void Log(string text) {
#if UNITY_EDITOR
		if (GlobalSettings.instance.debug.debugLogViaEditor) {
			Debug.Log(text);
			//Debug.LogWarning("Warning: Graph knows no GraphSettings");
		}
#else
		if (GlobalSettings.instance.debug.debugLogViaBuild) {
			Debug.Log(text);
			//Debug.LogWarning("Warning: Graph knows no GraphSettings");
		}
#endif
	}
}