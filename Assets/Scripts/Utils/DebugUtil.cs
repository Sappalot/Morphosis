using System.Collections.Generic;
using UnityEngine;

public static class DebugUtil {

	// Using this makes the log on file (when built) not mention the class logging the message, but rather this class :(
	// IF we are not spamming shit in the log anyway, what is the point of using this approach?
	public static void Log(string text) {
#if UNITY_EDITOR
		if (GlobalSettings.instance.debug.debugLogViaEditor) {
			Debug.Log(text);
		}
#else
		if (GlobalSettings.instance.debug.debugLogViaBuild) {
			Debug.Log(text);
		}
#endif
	}
}